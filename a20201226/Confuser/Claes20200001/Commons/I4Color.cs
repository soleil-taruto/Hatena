using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Charlotte.Commons
{
	/// <summary>
	/// <para>アルファ値を含む色を表す。</para>
	/// <para>各色は 0 ～ 255 を想定する。</para>
	/// <para>R を -1 にすることによって無効な色を示す。</para>
	/// </summary>
	public struct I4Color
	{
		public int R; // -1 == 無効
		public int G;
		public int B;
		public int A;

		public I4Color(int r, int g, int b, int a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public override string ToString()
		{
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", this.R, this.G, this.B, this.A);
		}

		public I3Color WithoutAlpha()
		{
			return new I3Color(this.R, this.G, this.B);
		}

		public D4Color ToD4Color()
		{
			return new D4Color(
				this.R / 255.0,
				this.G / 255.0,
				this.B / 255.0,
				this.A / 255.0
				);
		}

		public Color ToColor()
		{
			return Color.FromArgb(this.A, this.R, this.G, this.B); // 引数の並びは ARGB なので注意すること。
		}
	}
}
