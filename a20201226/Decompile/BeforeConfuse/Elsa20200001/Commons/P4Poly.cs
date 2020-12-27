using System;

namespace Charlotte.Commons
{
	// Token: 0x020000AA RID: 170
	public struct P4Poly
	{
		// Token: 0x06000311 RID: 785 RVA: 0x000119DF File Offset: 0x0000FBDF
		public P4Poly(D2Point lt, D2Point rt, D2Point rb, D2Point lb)
		{
			this.LT = lt;
			this.RT = rt;
			this.RB = rb;
			this.LB = lb;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x000119FE File Offset: 0x0000FBFE
		public P4Poly(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			this.LT = new D2Point(x1, y1);
			this.RT = new D2Point(x2, y2);
			this.RB = new D2Point(x3, y3);
			this.LB = new D2Point(x4, y4);
		}

		// Token: 0x0400023F RID: 575
		public D2Point LT;

		// Token: 0x04000240 RID: 576
		public D2Point RT;

		// Token: 0x04000241 RID: 577
		public D2Point RB;

		// Token: 0x04000242 RID: 578
		public D2Point LB;
	}
}
