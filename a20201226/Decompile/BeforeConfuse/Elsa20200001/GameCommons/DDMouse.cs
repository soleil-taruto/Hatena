using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007B RID: 123
	public static class DDMouse
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000C4C4 File Offset: 0x0000A6C4
		public static int Rot
		{
			get
			{
				if (1 > DDEngine.FreezeInputFrame)
				{
					return DDMouse._rot;
				}
				return 0;
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000C4D8 File Offset: 0x0000A6D8
		public static void EachFrame()
		{
			uint status;
			if (DDEngine.WindowIsActive)
			{
				DDMouse._rot = DX.GetMouseWheelRotVol();
				status = (uint)DX.GetMouseInput();
			}
			else
			{
				DDMouse._rot = 0;
				status = 0u;
			}
			DDMouse._rot = SCommon.ToRange(DDMouse._rot, -1000000000, 1000000000);
			DDUtils.UpdateInput(ref DDMouse.L.Status, (status & 1u) > 0u);
			DDUtils.UpdateInput(ref DDMouse.R.Status, (status & 2u) > 0u);
			DDUtils.UpdateInput(ref DDMouse.M.Status, (status & 4u) > 0u);
			DDMouse.UpdatePos_EF();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000C564 File Offset: 0x0000A764
		private static void UpdatePos_EF()
		{
			if (DX.GetMousePoint(out DDMouse.X, out DDMouse.Y) != 0)
			{
				throw new DDError();
			}
			if (DDGround.RealScreenDraw_W != -1)
			{
				DDMouse.X -= DDGround.RealScreenDraw_L;
				DDMouse.X *= 960;
				DDMouse.X /= DDGround.RealScreenDraw_W;
				DDMouse.Y -= DDGround.RealScreenDraw_T;
				DDMouse.Y *= 540;
				DDMouse.Y /= DDGround.RealScreenDraw_H;
				return;
			}
			DDMouse.X *= 960;
			DDMouse.X /= DDGround.RealScreen_W;
			DDMouse.Y *= 540;
			DDMouse.Y /= DDGround.RealScreen_H;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000C631 File Offset: 0x0000A831
		public static void PosChanged()
		{
			DDMouse.PosChangedFlag = true;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000C639 File Offset: 0x0000A839
		public static void PosChanged_Delay()
		{
			if (DDMouse.PosChangedFlag)
			{
				DDMouse.PosChangedFlag = false;
				DDMouse.PosChanged_Main();
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000C650 File Offset: 0x0000A850
		private static void PosChanged_Main()
		{
			int mx = DDMouse.X;
			int my = DDMouse.Y;
			if (DDGround.RealScreenDraw_W != -1)
			{
				mx *= DDGround.RealScreenDraw_W;
				mx /= 960;
				mx += DDGround.RealScreenDraw_L;
				my *= DDGround.RealScreenDraw_H;
				my /= 540;
				my += DDGround.RealScreenDraw_T;
			}
			else
			{
				mx *= DDGround.RealScreen_W;
				mx /= 960;
				my *= DDGround.RealScreen_H;
				my /= 540;
			}
			if (DX.SetMousePoint(mx, my) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000C6D4 File Offset: 0x0000A8D4
		public static void UpdateMove()
		{
			if (DDEngine.ProcFrame <= DDMouse.UM_LastFrame)
			{
				return;
			}
			DDMouse.MoveX = DDMouse.X - 480;
			DDMouse.MoveY = DDMouse.Y - 270;
			DDMouse.X = 480;
			DDMouse.Y = 270;
			DDMouse.PosChanged();
			if (DDMouse.UM_LastFrame + 1 < DDEngine.ProcFrame)
			{
				DDMouse.MoveX = 0;
				DDMouse.MoveY = 0;
			}
			DDMouse.UM_LastFrame = DDEngine.ProcFrame;
		}

		// Token: 0x040001BE RID: 446
		private static int _rot;

		// Token: 0x040001BF RID: 447
		public static DDMouse.Button L = new DDMouse.Button();

		// Token: 0x040001C0 RID: 448
		public static DDMouse.Button R = new DDMouse.Button();

		// Token: 0x040001C1 RID: 449
		public static DDMouse.Button M = new DDMouse.Button();

		// Token: 0x040001C2 RID: 450
		public static int X = 480;

		// Token: 0x040001C3 RID: 451
		public static int Y = 270;

		// Token: 0x040001C4 RID: 452
		private static bool PosChangedFlag = false;

		// Token: 0x040001C5 RID: 453
		public static int MoveX;

		// Token: 0x040001C6 RID: 454
		public static int MoveY;

		// Token: 0x040001C7 RID: 455
		private static int UM_LastFrame = -1000000000;

		// Token: 0x02000127 RID: 295
		public class Button
		{
			// Token: 0x06000641 RID: 1601 RVA: 0x000216D6 File Offset: 0x0001F8D6
			public int GetInput()
			{
				if (1 > DDEngine.FreezeInputFrame)
				{
					return this.Status;
				}
				return 0;
			}

			// Token: 0x06000642 RID: 1602 RVA: 0x000216E8 File Offset: 0x0001F8E8
			public bool IsPound()
			{
				return DDUtils.IsPound(this.GetInput());
			}

			// Token: 0x040004E8 RID: 1256
			public int Status;
		}
	}
}
