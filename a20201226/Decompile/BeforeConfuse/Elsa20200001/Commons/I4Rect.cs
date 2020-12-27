using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A9 RID: 169
	public struct I4Rect
	{
		// Token: 0x06000306 RID: 774 RVA: 0x000118F1 File Offset: 0x0000FAF1
		public I4Rect(I2Point lt, I2Size size)
		{
			this = new I4Rect(lt.X, lt.Y, size.W, size.H);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00011911 File Offset: 0x0000FB11
		public I4Rect(int l, int t, int w, int h)
		{
			this.L = l;
			this.T = t;
			this.W = w;
			this.H = h;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00011930 File Offset: 0x0000FB30
		public static I4Rect LTRB(int l, int t, int r, int b)
		{
			return new I4Rect(l, t, r - l, b - t);
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0001193F File Offset: 0x0000FB3F
		public int R
		{
			get
			{
				return this.L + this.W;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0001194E File Offset: 0x0000FB4E
		public int B
		{
			get
			{
				return this.T + this.H;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0001195D File Offset: 0x0000FB5D
		public I2Point LT
		{
			get
			{
				return new I2Point(this.L, this.T);
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600030C RID: 780 RVA: 0x00011970 File Offset: 0x0000FB70
		public I2Point RT
		{
			get
			{
				return new I2Point(this.R, this.T);
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600030D RID: 781 RVA: 0x00011983 File Offset: 0x0000FB83
		public I2Point RB
		{
			get
			{
				return new I2Point(this.R, this.B);
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00011996 File Offset: 0x0000FB96
		public I2Point LB
		{
			get
			{
				return new I2Point(this.L, this.B);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600030F RID: 783 RVA: 0x000119A9 File Offset: 0x0000FBA9
		public I2Size Size
		{
			get
			{
				return new I2Size(this.W, this.H);
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000119BC File Offset: 0x0000FBBC
		public D4Rect ToD4Rect()
		{
			return new D4Rect((double)this.L, (double)this.T, (double)this.W, (double)this.H);
		}

		// Token: 0x0400023B RID: 571
		public int L;

		// Token: 0x0400023C RID: 572
		public int T;

		// Token: 0x0400023D RID: 573
		public int W;

		// Token: 0x0400023E RID: 574
		public int H;
	}
}
