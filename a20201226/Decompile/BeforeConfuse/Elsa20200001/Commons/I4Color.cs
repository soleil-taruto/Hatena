using System;
using System.Drawing;

namespace Charlotte.Commons
{
	// Token: 0x020000A8 RID: 168
	public struct I4Color
	{
		// Token: 0x06000301 RID: 769 RVA: 0x000117EA File Offset: 0x0000F9EA
		public I4Color(int r, int g, int b, int a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0001180C File Offset: 0x0000FA0C
		public override string ToString()
		{
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", new object[]
			{
				this.R,
				this.G,
				this.B,
				this.A
			});
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00011861 File Offset: 0x0000FA61
		public I3Color WithoutAlpha()
		{
			return new I3Color(this.R, this.G, this.B);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0001187C File Offset: 0x0000FA7C
		public D4Color ToD4Color()
		{
			return new D4Color((double)this.R / 255.0, (double)this.G / 255.0, (double)this.B / 255.0, (double)this.A / 255.0);
		}

		// Token: 0x06000305 RID: 773 RVA: 0x000118D2 File Offset: 0x0000FAD2
		public Color ToColor()
		{
			return Color.FromArgb(this.A, this.R, this.G, this.B);
		}

		// Token: 0x04000237 RID: 567
		public int R;

		// Token: 0x04000238 RID: 568
		public int G;

		// Token: 0x04000239 RID: 569
		public int B;

		// Token: 0x0400023A RID: 570
		public int A;
	}
}
