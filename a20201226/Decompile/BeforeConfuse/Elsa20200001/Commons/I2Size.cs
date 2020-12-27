using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A6 RID: 166
	public struct I2Size
	{
		// Token: 0x060002F6 RID: 758 RVA: 0x000116A4 File Offset: 0x0000F8A4
		public I2Size(int w, int h)
		{
			this.W = w;
			this.H = h;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x000116B4 File Offset: 0x0000F8B4
		public D2Size ToD2Size()
		{
			return new D2Size((double)this.W, (double)this.H);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x000116C9 File Offset: 0x0000F8C9
		public static I2Size operator +(I2Size a, I2Size b)
		{
			return new I2Size(a.W + b.W, a.H + b.H);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x000116EA File Offset: 0x0000F8EA
		public static I2Size operator -(I2Size a, I2Size b)
		{
			return new I2Size(a.W - b.W, a.H - b.H);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0001170B File Offset: 0x0000F90B
		public static I2Size operator *(I2Size a, int b)
		{
			return new I2Size(a.W * b, a.H * b);
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00011722 File Offset: 0x0000F922
		public static I2Size operator /(I2Size a, int b)
		{
			return new I2Size(a.W / b, a.H / b);
		}

		// Token: 0x04000232 RID: 562
		public int W;

		// Token: 0x04000233 RID: 563
		public int H;
	}
}
