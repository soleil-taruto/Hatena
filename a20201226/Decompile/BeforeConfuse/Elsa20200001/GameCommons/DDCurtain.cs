using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000069 RID: 105
	public static class DDCurtain
	{
		// Token: 0x06000142 RID: 322 RVA: 0x00009FA4 File Offset: 0x000081A4
		public static void EachFrame()
		{
			if (DDEngine.ProcFrame <= DDCurtain.LastFrame)
			{
				return;
			}
			DDCurtain.LastFrame = DDEngine.ProcFrame;
			double wl;
			if (1 <= DDCurtain.WhiteLevels.Count)
			{
				wl = DDCurtain.WhiteLevels.Dequeue();
			}
			else
			{
				wl = DDCurtain.CurrWhiteLevel;
			}
			DDCurtain.DrawCurtain(wl);
			DDCurtain.CurrWhiteLevel = wl;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00009FF4 File Offset: 0x000081F4
		public static void SetCurtain(int frameMax = 30, double destWhiteLevel = 0.0)
		{
			DDCurtain.SetCurtain(frameMax, destWhiteLevel, DDCurtain.CurrWhiteLevel);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000A004 File Offset: 0x00008204
		public static void SetCurtain(int frameMax, double destWhiteLevel, double startWhiteLevel)
		{
			DDCurtain.WhiteLevels.Clear();
			if (frameMax == 0)
			{
				DDCurtain.CurrWhiteLevel = destWhiteLevel;
				return;
			}
			for (int frame = 0; frame <= frameMax; frame++)
			{
				double wl;
				if (frame == 0)
				{
					wl = startWhiteLevel;
				}
				else if (frame == frameMax)
				{
					wl = destWhiteLevel;
				}
				else
				{
					wl = startWhiteLevel + (destWhiteLevel - startWhiteLevel) * (double)frame / (double)frameMax;
				}
				DDCurtain.WhiteLevels.Enqueue(wl);
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000A058 File Offset: 0x00008258
		public static void DrawCurtain(double whiteLevel = -1.0)
		{
			if (whiteLevel == 0.0)
			{
				return;
			}
			whiteLevel = SCommon.ToRange(whiteLevel, -1.0, 1.0);
			if (whiteLevel < 0.0)
			{
				DDDraw.SetAlpha(-whiteLevel);
				DDDraw.SetBright(0.0, 0.0, 0.0);
			}
			else
			{
				DDDraw.SetAlpha(whiteLevel);
			}
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 0.0, 0.0, 960.0, 540.0);
			DDDraw.Reset();
		}

		// Token: 0x04000178 RID: 376
		private static Queue<double> WhiteLevels = new Queue<double>();

		// Token: 0x04000179 RID: 377
		public static double CurrWhiteLevel = 0.0;

		// Token: 0x0400017A RID: 378
		public static int LastFrame = -1;
	}
}
