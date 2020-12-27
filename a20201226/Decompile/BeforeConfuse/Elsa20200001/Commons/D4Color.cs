using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A3 RID: 163
	public struct D4Color
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x00011451 File Offset: 0x0000F651
		public D4Color(double r, double g, double b, double a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00011470 File Offset: 0x0000F670
		public D3Color WithoutAlpha()
		{
			return new D3Color(this.R, this.G, this.B);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0001148C File Offset: 0x0000F68C
		public I4Color ToI4Color()
		{
			return new I4Color(SCommon.ToInt(this.R * 255.0), SCommon.ToInt(this.G * 255.0), SCommon.ToInt(this.B * 255.0), SCommon.ToInt(this.A * 255.0));
		}

		// Token: 0x04000228 RID: 552
		public double R;

		// Token: 0x04000229 RID: 553
		public double G;

		// Token: 0x0400022A RID: 554
		public double B;

		// Token: 0x0400022B RID: 555
		public double A;
	}
}
