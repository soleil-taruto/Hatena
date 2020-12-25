using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct D2Size
	{
		public double W;
		public double H;

		public D2Size(double w, double h)
		{
			this.W = w;
			this.H = h;
		}

		public I2Size ToI2Size()
		{
			return new I2Size(
				SCommon.ToInt(this.W),
				SCommon.ToInt(this.H)
				);
		}

		public static D2Size operator +(D2Size a, D2Size b)
		{
			return new D2Size(a.W + b.W, a.H + b.H);
		}

		public static D2Size operator -(D2Size a, D2Size b)
		{
			return new D2Size(a.W - b.W, a.H - b.H);
		}

		public static D2Size operator *(D2Size a, double b)
		{
			return new D2Size(a.W * b, a.H * b);
		}

		public static D2Size operator /(D2Size a, double b)
		{
			return new D2Size(a.W / b, a.H / b);
		}
	}
}
