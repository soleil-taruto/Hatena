using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x0200009C RID: 156
	public static class DDSlideUtils
	{
		// Token: 0x060002BD RID: 701 RVA: 0x0001073A File Offset: 0x0000E93A
		public static double Slide(double start, double end, double rate)
		{
			return start + (end - start) * rate;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00010EC4 File Offset: 0x0000F0C4
		public static D2Point Slide(D2Point start, D2Point end, double rate)
		{
			return new D2Point(DDSlideUtils.Slide(start.X, end.X, rate), DDSlideUtils.Slide(start.Y, end.Y, rate));
		}

		// Token: 0x060002BF RID: 703 RVA: 0x00010EF0 File Offset: 0x0000F0F0
		public static D4Rect Slide(D4Rect start, D4Rect end, double rate)
		{
			return new D4Rect(DDSlideUtils.Slide(start.L, end.L, rate), DDSlideUtils.Slide(start.T, end.T, rate), DDSlideUtils.Slide(start.W, end.W, rate), DDSlideUtils.Slide(start.H, end.H, rate));
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00010F4C File Offset: 0x0000F14C
		public static P4Poly Slide(P4Poly start, P4Poly end, double rate)
		{
			return new P4Poly(DDSlideUtils.Slide(start.LT, end.LT, rate), DDSlideUtils.Slide(start.RT, end.RT, rate), DDSlideUtils.Slide(start.RB, end.RB, rate), DDSlideUtils.Slide(start.LB, end.LB, rate));
		}
	}
}
