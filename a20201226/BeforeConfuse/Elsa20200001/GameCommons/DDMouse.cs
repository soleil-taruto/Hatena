using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDMouse
	{
		private static int _rot;

		public static int Rot
		{
			get
			{
				return 1 <= DDEngine.FreezeInputFrame ? 0 : _rot;
			}
		}

		public class Button
		{
			public int Status = 0;

			public int GetInput()
			{
				return 1 <= DDEngine.FreezeInputFrame ? 0 : this.Status;
			}

			public bool IsPound()
			{
				return DDUtils.IsPound(this.GetInput());
			}
		}

		public static Button L = new Button();
		public static Button R = new Button();
		public static Button M = new Button();

		public static void EachFrame()
		{
			uint status;

			if (DDEngine.WindowIsActive)
			{
				_rot = DX.GetMouseWheelRotVol();
				status = (uint)DX.GetMouseInput();
			}
			else
			{
				_rot = 0;
				status = 0u;
			}
			_rot = SCommon.ToRange(_rot, -SCommon.IMAX, SCommon.IMAX);

			DDUtils.UpdateInput(ref L.Status, (status & (uint)DX.MOUSE_INPUT_LEFT) != 0u);
			DDUtils.UpdateInput(ref R.Status, (status & (uint)DX.MOUSE_INPUT_RIGHT) != 0u);
			DDUtils.UpdateInput(ref M.Status, (status & (uint)DX.MOUSE_INPUT_MIDDLE) != 0u);

			UpdatePos_EF();
		}

		public static int X = (int)(DDConsts.Screen_W / 2.0);
		public static int Y = (int)(DDConsts.Screen_H / 2.0);

		private static void UpdatePos_EF()
		{
			if (DX.GetMousePoint(out X, out Y) != 0) // ? 失敗
				throw new DDError();

			if (DDGround.RealScreenDraw_W != -1)
			{
				X -= DDGround.RealScreenDraw_L;
				X *= DDConsts.Screen_W;
				X /= DDGround.RealScreenDraw_W;
				Y -= DDGround.RealScreenDraw_T;
				Y *= DDConsts.Screen_H;
				Y /= DDGround.RealScreenDraw_H;
			}
			else
			{
				X *= DDConsts.Screen_W;
				X /= DDGround.RealScreen_W;
				Y *= DDConsts.Screen_H;
				Y /= DDGround.RealScreen_H;
			}
		}

		private static bool PosChangedFlag = false;

		/// <summary>
		/// DDMouse.X, DDMouse.Y を更新したら必ず呼び出すこと。
		/// </summary>
		public static void PosChanged()
		{
			PosChangedFlag = true;
		}

		public static void PosChanged_Delay()
		{
			if (PosChangedFlag)
			{
				PosChangedFlag = false;
				PosChanged_Main();
			}
		}

		private static void PosChanged_Main()
		{
			int mx = X;
			int my = Y;

			if (DDGround.RealScreenDraw_W != -1)
			{
				mx *= DDGround.RealScreenDraw_W;
				mx /= DDConsts.Screen_W;
				mx += DDGround.RealScreenDraw_L;
				my *= DDGround.RealScreenDraw_H;
				my /= DDConsts.Screen_H;
				my += DDGround.RealScreenDraw_T;
			}
			else
			{
				mx *= DDGround.RealScreen_W;
				mx /= DDConsts.Screen_W;
				my *= DDGround.RealScreen_H;
				my /= DDConsts.Screen_H;
			}
			if (DX.SetMousePoint(mx, my) != 0) // ? 失敗
				throw new DDError();
		}

		public static int MoveX;
		public static int MoveY;

		private static int UM_LastFrame = -SCommon.IMAX;

		/// <summary>
		/// 前フレームからのマウス移動量を DDMouse.MoveX, DDMouse.MoveY にセットし、マウスカーソルをスクリーンの中央に強制移動する。
		/// </summary>
		public static void UpdateMove()
		{
			if (DDEngine.ProcFrame <= UM_LastFrame) // ? 1フレームで2回以上更新しようとした。
				return;

			const int centerX = (int)(DDConsts.Screen_W / 2.0);
			const int centerY = (int)(DDConsts.Screen_H / 2.0);

			MoveX = X - centerX;
			MoveY = Y - centerY;

			X = centerX;
			Y = centerY;

			PosChanged();

			if (UM_LastFrame + 1 < DDEngine.ProcFrame) // ? 直前のフレームは更新しなかった。
			{
				MoveX = 0;
				MoveY = 0;
			}
			UM_LastFrame = DDEngine.ProcFrame;
		}
	}
}
