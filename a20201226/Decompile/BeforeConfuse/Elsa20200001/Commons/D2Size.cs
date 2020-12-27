using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A1 RID: 161
	public struct D2Size
	{
		// Token: 0x060002D8 RID: 728 RVA: 0x00011330 File Offset: 0x0000F530
		public D2Size(double w, double h)
		{
			this.W = w;
			this.H = h;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00011340 File Offset: 0x0000F540
		public I2Size ToI2Size()
		{
			return new I2Size(SCommon.ToInt(this.W), SCommon.ToInt(this.H));
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0001135D File Offset: 0x0000F55D
		public static D2Size operator +(D2Size a, D2Size b)
		{
			return new D2Size(a.W + b.W, a.H + b.H);
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001137E File Offset: 0x0000F57E
		public static D2Size operator -(D2Size a, D2Size b)
		{
			return new D2Size(a.W - b.W, a.H - b.H);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0001139F File Offset: 0x0000F59F
		public static D2Size operator *(D2Size a, double b)
		{
			return new D2Size(a.W * b, a.H * b);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x000113B6 File Offset: 0x0000F5B6
		public static D2Size operator /(D2Size a, double b)
		{
			return new D2Size(a.W / b, a.H / b);
		}

		// Token: 0x04000223 RID: 547
		public double W;

		// Token: 0x04000224 RID: 548
		public double H;
	}
}
