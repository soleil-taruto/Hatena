using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDMain
	{
		public static List<Action> Finalizers = new List<Action>();

		private static int LogCount = 0;

		public static void GameStart()
		{
			foreach (string dllFile in "DxLib.dll:DxLib_x64.dll:DxLibDotNet.dll".Split(':')) // DxLibDotNet.dll 等の存在確認 (1)
				if (!File.Exists(dllFile))
					throw new DDError();

			DX.GetColor(0, 0, 0); // DxLibDotNet.dll 等の存在確認 (2)

			DDConfig.Load(); // LogFile, LOG_ENABLED 等を含むので真っ先に

			// Log >

			File.WriteAllBytes(DDConfig.LogFile, SCommon.EMPTY_BYTES);

			ProcMain.WriteLog = message =>
			{
				if (LogCount < DDConfig.LogCountMax)
				{
					using (StreamWriter writer = new StreamWriter(DDConfig.LogFile, true, Encoding.UTF8))
					{
						writer.WriteLine("[" + DateTime.Now + "] " + message);
					}
					LogCount++;
				}
			};

			// < Log

			// *.INIT
			{
				DDGround.INIT();
				DDResource.INIT();
				DDDatStrings.INIT();
				DDUserDatStrings.INIT();
				DDFontRegister.INIT();
				DDKey.INIT();
			}

			DDSaveData.Load();

			// DxLib >

			if (DDConfig.LOG_ENABLED)
				DX.SetApplicationLogSaveDirectory(SCommon.MakeFullPath(DDConfig.ApplicationLogSaveDirectory));

			DX.SetOutApplicationLogValidFlag(DDConfig.LOG_ENABLED ? 1 : 0); // DxLib のログを出力 1: する 0: しない

			DX.SetAlwaysRunFlag(1); // ? 非アクティブ時に 1: 動く 0: 止まる

			SetMainWindowTitle();

			//DX.SetGraphMode(DDConsts.Screen_W, DDConsts.Screen_H, 32);
			DX.SetGraphMode(DDGround.RealScreen_W, DDGround.RealScreen_H, 32);
			DX.ChangeWindowMode(1); // 1: ウィンドウ 0: フルスクリーン

			//DX.SetFullSceneAntiAliasingMode(4, 2); // フルスクリーンを廃止したので不要

			DX.SetWindowIconHandle(GetAppIcon()); // ウィンドウ左上のアイコン

			if (DDConfig.DisplayIndex != -1)
				DX.SetUseDirectDrawDeviceIndex(DDConfig.DisplayIndex);

			if (DX.DxLib_Init() != 0) // ? 失敗
				throw new DDError();

			Finalizers.Add(() =>
			{
				if (DX.DxLib_End() != 0) // ? 失敗
					throw new DDError();
			});

			DDUtils.SetMouseDispMode(DDGround.RO_MouseDispMode); // ? マウスを表示する。
			DX.SetWindowSizeChangeEnableFlag(0); // ウィンドウの右下をドラッグで伸縮 1: する 0: しない

			//DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetDrawMode(DDConsts.DEFAULT_DX_DRAWMODE); // これをデフォルトとする。

			// < DxLib

			DDGround.MainScreen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
			DDGround.MainScreen.ChangeDrawScreen();
			DDGround.LastMainScreen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
			DDGround.KeptMainScreen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);

			{
				int l;
				int t;
				int w;
				int h;
				int p1;
				int p2;
				int p3;
				int p4;

				DX.GetDefaultState(out w, out h, out p1, out p2, out l, out t, out p3, out p4);

				if (
					w < 1 || SCommon.IMAX < w ||
					h < 1 || SCommon.IMAX < h ||
					l < -SCommon.IMAX || SCommon.IMAX < l ||
					t < -SCommon.IMAX || SCommon.IMAX < t
					)
					throw new DDError();

				DDGround.MonitorRect = new I4Rect(l, t, w, h);
			}

			PostSetScreenSize(DDGround.RealScreen_W, DDGround.RealScreen_H);

			DDGround.GeneralResource = new DDGeneralResource();

			// Font
			{
				//DDFontRegister.Add(@"e20200928_NovelAdv\Font\Genkai-Mincho-font\genkai-mincho.ttf");
				//DDFontRegister.Add(@"e20200928_NovelAdv\Font\riitf\RiiT_F.otf");
				//DDFontRegister.Add(@"e20200928_NovelAdv\Font\K Gothic\K Gothic.ttf");
			}

			Ground.I = new Ground();
			Ground.I.Picture2 = new ResourcePicture2(); // Ground.I を参照しているので Ground のコンストラクタには書けない。

			MainWin.I.PostGameStart_G3();

			DDSaveData.Load_Delay();

			Finalizers.Add(() =>
			{
				DDSaveData.Save();
			});
		}

		public static void GameEnd(List<Exception> errors)
		{
			while (1 <= Finalizers.Count)
			{
				try
				{
					SCommon.UnaddElement(Finalizers)();
				}
				catch (Exception e)
				{
					errors.Add(e);
				}
			}
		}

		public static void SetMainWindowTitle()
		{
			DX.SetMainWindowText(DDDatStrings.Title + " " + DDUserDatStrings.Version);
		}

		private static IntPtr GetAppIcon()
		{
			using (MemoryStream mem = new MemoryStream(DDResource.Load(@"e20200002_General\General\game_app.ico")))
			{
				return new Icon(mem).Handle;
			}
		}

		public static void SetScreenSize(int w, int h)
		{
			if (
				w < DDConsts.Screen_W_Min || DDConsts.Screen_W_Max < w ||
				h < DDConsts.Screen_H_Min || DDConsts.Screen_H_Max < h
				)
				throw new DDError();

			DDGround.RealScreenDraw_W = -1; // 無効化

			if (DDGround.RealScreen_W != w || DDGround.RealScreen_H != h)
			{
				DDGround.RealScreen_W = w;
				DDGround.RealScreen_H = h;

				ApplyScreenSize();

				PostSetScreenSize(w, h);
			}
		}

		public static void ApplyScreenSize()
		{
			ApplyScreenSize(DDGround.RealScreen_W, DDGround.RealScreen_H);
		}

		public static void ApplyScreenSize(int w, int h)
		{
			bool mdm = DDUtils.GetMouseDispMode();

			//DDDerivationUtils.UnloadAll(); // moved -> DDPictureUtils.UnloadAll
			DDPictureUtils.UnloadAll();
			DDSubScreenUtils.UnloadAll();
			DDFontUtils.UnloadAll();

			if (DX.SetGraphMode(w, h, 32) != DX.DX_CHANGESCREEN_OK)
				throw new DDError();

			DX.SetDrawScreen(DX.DX_SCREEN_BACK); // DDSubScreenUtils.CurrDrawScreenHandle にするべきだが、このフレームだけの問題なので、無難なところで DX_SCREEN_BACK にしておく。
			DX.SetDrawMode(DDConsts.DEFAULT_DX_DRAWMODE);

			DDUtils.SetMouseDispMode(mdm);
		}

		public static void PostSetScreenSize(int w, int h)
		{
			if (DDGround.MonitorRect.W == w && DDGround.MonitorRect.H == h)
			{
				SetScreenPosition(DDGround.MonitorRect.L, DDGround.MonitorRect.T);
			}
		}

		public static void SetScreenPosition(int l, int t)
		{
			DX.SetWindowPosition(l, t);

			DDWin32.POINT p;

			p.X = 0;
			p.Y = 0;

			DDWin32.ClientToScreen(DDWin32.GetMainWindowHandle(), out p);

			int pToTrgX = l - (int)p.X;
			int pToTrgY = t - (int)p.Y;

			DX.SetWindowPosition(l + pToTrgX, t + pToTrgY);
		}

		private static int LastKeepMainScreenFrame = -1;

		public static void KeepMainScreen()
		{
			// once per frame
			{
				if (DDEngine.ProcFrame <= LastKeepMainScreenFrame)
					return;

				LastKeepMainScreenFrame = DDEngine.ProcFrame;
			}

			DDSubScreen tmp = DDGround.LastMainScreen;
			DDGround.LastMainScreen = DDGround.KeptMainScreen;
			DDGround.KeptMainScreen = tmp;
		}
	}
}
