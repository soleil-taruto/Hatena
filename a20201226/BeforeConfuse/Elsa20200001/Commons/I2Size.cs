using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct I2Size
	{
		public int W;
		public int H;

		public I2Size(int w, int h)
		{
			this.W = w;
			this.H = h;
		}

		public D2Size ToD2Size()
		{
			return new D2Size(this.W, this.H);
		}

		public static I2Size operator +(I2Size a, I2Size b)
		{
			return new I2Size(a.W + b.W, a.H + b.H);
		}

		public static I2Size operator -(I2Size a, I2Size b)
		{
			return new I2Size(a.W - b.W, a.H - b.H);
		}

		public static I2Size operator *(I2Size a, int b)
		{
			return new I2Size(a.W * b, a.H * b);
		}

		public static I2Size operator /(I2Size a, int b)
		{
			return new I2Size(a.W / b, a.H / b);
		}
	}
}
