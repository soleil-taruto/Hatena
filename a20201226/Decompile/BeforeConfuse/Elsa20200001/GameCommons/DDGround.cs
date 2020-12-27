using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000074 RID: 116
	public static class DDGround
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0000B89C File Offset: 0x00009A9C
		public static void INIT()
		{
			DDInput.DIR_2.BtnId = 0;
			DDInput.DIR_4.BtnId = 1;
			DDInput.DIR_6.BtnId = 2;
			DDInput.DIR_8.BtnId = 3;
			DDInput.A.BtnId = 4;
			DDInput.B.BtnId = 7;
			DDInput.C.BtnId = 5;
			DDInput.D.BtnId = 8;
			DDInput.E.BtnId = 6;
			DDInput.F.BtnId = 9;
			DDInput.L.BtnId = 10;
			DDInput.R.BtnId = 11;
			DDInput.PAUSE.BtnId = 13;
			DDInput.START.BtnId = 12;
			DDInput.DIR_2.KeyId = 208;
			DDInput.DIR_4.KeyId = 203;
			DDInput.DIR_6.KeyId = 205;
			DDInput.DIR_8.KeyId = 200;
			DDInput.A.KeyId = 44;
			DDInput.B.KeyId = 45;
			DDInput.C.KeyId = 46;
			DDInput.D.KeyId = 47;
			DDInput.E.KeyId = 30;
			DDInput.F.KeyId = 31;
			DDInput.L.KeyId = 32;
			DDInput.R.KeyId = 33;
			DDInput.PAUSE.KeyId = 57;
			DDInput.START.KeyId = 28;
		}

		// Token: 0x04000196 RID: 406
		public static DDTaskList EL = new DDTaskList();

		// Token: 0x04000197 RID: 407
		public static int PrimaryPadId = -1;

		// Token: 0x04000198 RID: 408
		public static DDSubScreen MainScreen;

		// Token: 0x04000199 RID: 409
		public static DDSubScreen LastMainScreen;

		// Token: 0x0400019A RID: 410
		public static DDSubScreen KeptMainScreen;

		// Token: 0x0400019B RID: 411
		public static I4Rect MonitorRect;

		// Token: 0x0400019C RID: 412
		public static int RealScreen_W = 960;

		// Token: 0x0400019D RID: 413
		public static int RealScreen_H = 540;

		// Token: 0x0400019E RID: 414
		public static int RealScreenDraw_L;

		// Token: 0x0400019F RID: 415
		public static int RealScreenDraw_T;

		// Token: 0x040001A0 RID: 416
		public static int RealScreenDraw_W = -1;

		// Token: 0x040001A1 RID: 417
		public static int RealScreenDraw_H;

		// Token: 0x040001A2 RID: 418
		public static double MusicVolume = 0.45;

		// Token: 0x040001A3 RID: 419
		public static double SEVolume = 0.45;

		// Token: 0x040001A4 RID: 420
		public static bool RO_MouseDispMode = false;

		// Token: 0x040001A5 RID: 421
		public static D2Point Camera = default(D2Point);

		// Token: 0x040001A6 RID: 422
		public static I2Point ICamera = default(I2Point);

		// Token: 0x040001A7 RID: 423
		public static DDGeneralResource GeneralResource;
	}
}
