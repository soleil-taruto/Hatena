using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Charlotte.Commons
{
	/// <summary>
	/// <para>アルファ値の無い色を表す。</para>
	/// <para>各色は 0 ～ 255 を想定する。</para>
	/// <para>R を -1 にすることによって無効な色を示す。</para>
	/// </summary>
	public struct I3Color
	{
		public int R; // -1 == 無効
		public int G;
		public int B;

		public I3Color(int r, int g, int b)
		{
			this.R = r;
			this.G = g;
			this.B = b;
		}

		public override string ToString()
		{
			return string.Format("{0:x2}{1:x2}{2:x2}", this.R, this.G, this.B);
		}

		public I4Color WithAlpha(int a = 255)
		{
			return new I4Color(this.R, this.G, this.B, a);
		}

		public D3Color ToD3Color()
		{
			return new D3Color(
				this.R / 255.0,
				this.G / 255.0,
				this.B / 255.0
				);
		}

		public Color ToColor()
		{
			return Color.FromArgb(this.R, this.G, this.B);
		}
	}
}
