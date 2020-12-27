using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A5 RID: 165
	public struct I2Point
	{
		// Token: 0x060002F0 RID: 752 RVA: 0x0001160F File Offset: 0x0000F80F
		public I2Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001161F File Offset: 0x0000F81F
		public D2Point ToD2Point()
		{
			return new D2Point((double)this.X, (double)this.Y);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00011634 File Offset: 0x0000F834
		public static I2Point operator +(I2Point a, I2Point b)
		{
			return new I2Point(a.X + b.X, a.Y + b.Y);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00011655 File Offset: 0x0000F855
		public static I2Point operator -(I2Point a, I2Point b)
		{
			return new I2Point(a.X - b.X, a.Y - b.Y);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00011676 File Offset: 0x0000F876
		public static I2Point operator *(I2Point a, int b)
		{
			return new I2Point(a.X * b, a.Y * b);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0001168D File Offset: 0x0000F88D
		public static I2Point operator /(I2Point a, int b)
		{
			return new I2Point(a.X / b, a.Y / b);
		}

		// Token: 0x04000230 RID: 560
		public int X;

		// Token: 0x04000231 RID: 561
		public int Y;
	}
}
