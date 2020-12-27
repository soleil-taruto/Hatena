using System;
using System.Drawing;

namespace Charlotte.Commons
{
	// Token: 0x020000A7 RID: 167
	public struct I3Color
	{
		// Token: 0x060002FC RID: 764 RVA: 0x00011739 File Offset: 0x0000F939
		public I3Color(int r, int g, int b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00011750 File Offset: 0x0000F950
		public override string ToString()
		{
			return string.Format("{0:x2}{1:x2}{2:x2}", this.R, this.G, this.B);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001177D File Offset: 0x0000F97D
		public I4Color WithAlpha(int a = 255)
		{
			return new I4Color(this.R, this.G, this.B, a);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00011797 File Offset: 0x0000F997
		public D3Color ToD3Color()
		{
			return new D3Color((double)this.R / 255.0, (double)this.G / 255.0, (double)this.B / 255.0);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x000117D1 File Offset: 0x0000F9D1
		public Color ToColor()
		{
			return Color.FromArgb(this.R, this.G, this.B);
		}

		// Token: 0x04000234 RID: 564
		public int R;

		// Token: 0x04000235 RID: 565
		public int G;

		// Token: 0x04000236 RID: 566
		public int B;
	}
}
