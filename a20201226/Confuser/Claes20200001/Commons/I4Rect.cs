using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct I4Rect
	{
		public int L;
		public int T;
		public int W;
		public int H;

		public I4Rect(I2Point lt, I2Size size)
			: this(lt.X, lt.Y, size.W, size.H)
		{ }

		public I4Rect(int l, int t, int w, int h)
		{
			this.L = l;
			this.T = t;
			this.W = w;
			this.H = h;
		}

		public static I4Rect LTRB(int l, int t, int r, int b)
		{
			return new I4Rect(l, t, r - l, b - t);
		}

		public int R
		{
			get
			{
				return this.L + this.W;
			}
		}

		public int B
		{
			get
			{
				return this.T + this.H;
			}
		}

		public I2Point LT
		{
			get
			{
				return new I2Point(this.L, this.T);
			}
		}

		public I2Point RT
		{
			get
			{
				return new I2Point(this.R, this.T);
			}
		}

		public I2Point RB
		{
			get
			{
				return new I2Point(this.R, this.B);
			}
		}

		public I2Point LB
		{
			get
			{
				return new I2Point(this.L, this.B);
			}
		}

		public I2Size Size
		{
			get
			{
				return new I2Size(this.W, this.H);
			}
		}

		public D4Rect ToD4Rect()
		{
			return new D4Rect(this.L, this.T, this.W, this.H);
		}
	}
}
