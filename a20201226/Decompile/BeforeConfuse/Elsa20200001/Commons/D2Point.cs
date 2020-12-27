using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A0 RID: 160
	public struct D2Point
	{
		// Token: 0x060002D2 RID: 722 RVA: 0x00011293 File Offset: 0x0000F493
		public D2Point(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x000112A3 File Offset: 0x0000F4A3
		public I2Point ToI2Point()
		{
			return new I2Point(SCommon.ToInt(this.X), SCommon.ToInt(this.Y));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000112C0 File Offset: 0x0000F4C0
		public static D2Point operator +(D2Point a, D2Point b)
		{
			return new D2Point(a.X + b.X, a.Y + b.Y);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000112E1 File Offset: 0x0000F4E1
		public static D2Point operator -(D2Point a, D2Point b)
		{
			return new D2Point(a.X - b.X, a.Y - b.Y);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00011302 File Offset: 0x0000F502
		public static D2Point operator *(D2Point a, double b)
		{
			return new D2Point(a.X * b, a.Y * b);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00011319 File Offset: 0x0000F519
		public static D2Point operator /(D2Point a, double b)
		{
			return new D2Point(a.X / b, a.Y / b);
		}

		// Token: 0x04000221 RID: 545
		public double X;

		// Token: 0x04000222 RID: 546
		public double Y;
	}
}
