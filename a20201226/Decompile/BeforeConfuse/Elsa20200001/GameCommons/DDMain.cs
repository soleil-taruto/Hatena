using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000079 RID: 121
	public static class DDMain
	{
		// Token: 0x060001B1 RID: 433 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		public static void GameStart()
		{
			string[] array = "DxLib.dll:DxLib_x64.dll:DxLibDotNet.dll".Split(new char[]
			{
				':'
			});
			for (int j = 0; j < array.Length; j++)
			{
				if (!File.Exists(array[j]))
				{
					throw new DDError();
				}
			}
			DX.GetColor(0, 0, 0);
			DDConfig.Load();
			File.WriteAllBytes(DDConfig.LogFile, SCommon.EMPTY_BYTES);
			ProcMain.WriteLog = delegate(object message)
			{
				if (DDMain.LogCount < DDConfig.LogCountMax)
				{
					using (StreamWriter writer = new StreamWriter(DDConfig.LogFile, true, Encoding.UTF8))
					{
						writer.WriteLine("[" + DateTime.Now.ToString() + "] " + ((message != null) ? message.ToString() : null));
					}
					DDMain.LogCount++;
				}
			};
			DDGround.INIT();
			DDResource.INIT();
			DDDatStrings.INIT();
			DDUserDatStrings.INIT();
			DDFontRegister.INIT();
			DDKey.INIT();
			DDSaveData.Load();
			if (DDConfig.LOG_ENABLED)
			{
				DX.SetApplicationLogSaveDirectory(SCommon.MakeFullPath(DDConfig.ApplicationLogSaveDirectory));
			}
			DX.SetOutApplicationLogValidFlag(DDConfig.LOG_ENABLED ? 1 : 0);
			DX.SetAlwaysRunFlag(1);
			DDMain.SetMainWindowTitle();
			DX.SetGraphMode(DDGround.RealScreen_W, DDGround.RealScreen_H, 32);
			DX.ChangeWindowMode(1);
			DX.SetWindowIconHandle(DDMain.GetAppIcon());
			if (DDConfig.DisplayIndex != -1)
			{
				DX.SetUseDirectDrawDeviceIndex(DDConfig.DisplayIndex);
			}
			if (DX.DxLib_Init() != 0)
			{
				throw new DDError();
			}
			DDMain.Finalizers.Add(delegate
			{
				if (DX.DxLib_End() != 0)
				{
					throw new DDError();
				}
			});
			DDUtils.SetMouseDispMode(DDGround.RO_MouseDispMode);
			DX.SetWindowSizeChangeEnableFlag(0);
			DX.SetDrawMode(2);
			DDGround.MainScreen = new DDSubScreen(960, 540, false);
			DDGround.MainScreen.ChangeDrawScreen();
			DDGround.LastMainScreen = new DDSubScreen(960, 540, false);
			DDGround.KeptMainScreen = new DDSubScreen(960, 540, false);
			int w;
			int h;
			int p;
			int p2;
			int i;
			int t;
			int p3;
			int p4;
			DX.GetDefaultState(out w, out h, out p, out p2, out i, out t, out p3, out p4);
			if (w < 1 || 1000000000 < w || h < 1 || 1000000000 < h || i < -1000000000 || 1000000000 < i || t < -1000000000 || 1000000000 < t)
			{
				throw new DDError();
			}
			DDGround.MonitorRect = new I4Rect(i, t, w, h);
			DDMain.PostSetScreenSize(DDGround.RealScreen_W, DDGround.RealScreen_H);
			DDGround.GeneralResource = new DDGeneralResource();
			Ground.I = new Ground();
			Ground.I.Picture2 = new ResourcePicture2();
			MainWin.I.PostGameStart_G3();
			DDSaveData.Load_Delay();
			DDMain.Finalizers.Add(delegate
			{
				DDSaveData.Save();
			});
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C228 File Offset: 0x0000A428
		public static void GameEnd(List<Exception> errors)
		{
			while (1 <= DDMain.Finalizers.Count)
			{
				try
				{
					SCommon.UnaddElement<Action>(DDMain.Finalizers)();
				}
				catch (Exception e)
				{
					errors.Add(e);
				}
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000C270 File Offset: 0x0000A470
		public static void SetMainWindowTitle()
		{
			DX.SetMainWindowText(DDDatStrings.Title + " " + DDUserDatStrings.Version);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000C28C File Offset: 0x0000A48C
		private static IntPtr GetAppIcon()
		{
			IntPtr handle;
			using (MemoryStream mem = new MemoryStream(DDResource.Load("e20200002_General\\General\\game_app.ico")))
			{
				handle = new Icon(mem).Handle;
			}
			return handle;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000C2D4 File Offset: 0x0000A4D4
		public static void SetScreenSize(int w, int h)
		{
			if (w < 100 || 1000000000 < w || h < 100 || 1000000000 < h)
			{
				throw new DDError();
			}
			DDGround.RealScreenDraw_W = -1;
			if (DDGround.RealScreen_W != w || DDGround.RealScreen_H != h)
			{
				DDGround.RealScreen_W = w;
				DDGround.RealScreen_H = h;
				DDMain.ApplyScreenSize();
				DDMain.PostSetScreenSize(w, h);
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000C32F File Offset: 0x0000A52F
		public static void ApplyScreenSize()
		{
			DDMain.ApplyScreenSize(DDGround.RealScreen_W, DDGround.RealScreen_H);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000C340 File Offset: 0x0000A540
		public static void ApplyScreenSize(int w, int h)
		{
			bool mouseDispMode = DDUtils.GetMouseDispMode();
			DDPictureUtils.UnloadAll();
			DDSubScreenUtils.UnloadAll();
			DDFontUtils.UnloadAll();
			if (DX.SetGraphMode(w, h, 32) != 0)
			{
				throw new DDError();
			}
			DX.SetDrawScreen(-2);
			DX.SetDrawMode(2);
			DDUtils.SetMouseDispMode(mouseDispMode);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000C37B File Offset: 0x0000A57B
		public static void PostSetScreenSize(int w, int h)
		{
			if (DDGround.MonitorRect.W == w && DDGround.MonitorRect.H == h)
			{
				DDMain.SetScreenPosition(DDGround.MonitorRect.L, DDGround.MonitorRect.T);
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000C3B0 File Offset: 0x0000A5B0
		public static void SetScreenPosition(int l, int t)
		{
			DX.SetWindowPosition(l, t);
			DDWin32.POINT p;
			p.X = 0;
			p.Y = 0;
			DDWin32.ClientToScreen(DDWin32.GetMainWindowHandle(), out p);
			int pToTrgX = l - p.X;
			int pToTrgY = t - p.Y;
			DX.SetWindowPosition(l + pToTrgX, t + pToTrgY);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000C400 File Offset: 0x0000A600
		public static void KeepMainScreen()
		{
			if (DDEngine.ProcFrame <= DDMain.LastKeepMainScreenFrame)
			{
				return;
			}
			DDMain.LastKeepMainScreenFrame = DDEngine.ProcFrame;
			DDSubScreen lastMainScreen = DDGround.LastMainScreen;
			DDGround.LastMainScreen = DDGround.KeptMainScreen;
			DDGround.KeptMainScreen = lastMainScreen;
		}

		// Token: 0x040001BB RID: 443
		public static List<Action> Finalizers = new List<Action>();

		// Token: 0x040001BC RID: 444
		private static int LogCount = 0;

		// Token: 0x040001BD RID: 445
		private static int LastKeepMainScreenFrame = -1;
	}
}
