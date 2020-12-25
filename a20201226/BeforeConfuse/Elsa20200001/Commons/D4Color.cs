using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	/// <summary>
	/// <para>アルファ値を含む色または色の比率を表す。</para>
	/// <para>各色は 0.0 ～ 1.0 を想定する。</para>
	/// </summary>
	public struct D4Color
	{
		public double R;
		public double G;
		public double B;
		public double A;

		public D4Color(double r, double g, double b, double a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public D3Color WithoutAlpha()
		{
			return new D3Color(this.R, this.G, this.B);
		}

		public I4Color ToI4Color()
		{
			return new I4Color(
				SCommon.ToInt(this.R * 255.0),
				SCommon.ToInt(this.G * 255.0),
				SCommon.ToInt(this.B * 255.0),
				SCommon.ToInt(this.A * 255.0)
				);
		}
	}
}
