using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A2 RID: 162
	public struct D3Color
	{
		// Token: 0x060002DE RID: 734 RVA: 0x000113CD File Offset: 0x0000F5CD
		public D3Color(double r, double g, double b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000113E4 File Offset: 0x0000F5E4
		public D4Color WithAlpha(double a = 1.0)
		{
			return new D4Color(this.R, this.G, this.B, a);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00011400 File Offset: 0x0000F600
		public I3Color ToI3Color()
		{
			return new I3Color(SCommon.ToInt(this.R * 255.0), SCommon.ToInt(this.G * 255.0), SCommon.ToInt(this.B * 255.0));
		}

		// Token: 0x04000225 RID: 549
		public double R;

		// Token: 0x04000226 RID: 550
		public double G;

		// Token: 0x04000227 RID: 551
		public double B;
	}
}
