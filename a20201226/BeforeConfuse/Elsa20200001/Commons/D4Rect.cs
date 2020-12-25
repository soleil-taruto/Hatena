using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct D4Rect
	{
		public double L;
		public double T;
		public double W;
		public double H;

		public D4Rect(D2Point lt, D2Size size)
			: this(lt.X, lt.Y, size.W, size.H)
		{ }

		public D4Rect(double l, double t, double w, double h)
		{
			this.L = l;
			this.T = t;
			this.W = w;
			this.H = h;
		}

		public static D4Rect LTRB(double l, double t, double r, double b)
		{
			return new D4Rect(l, t, r - l, b - t);
		}

		public double R
		{
			get
			{
				return this.L + this.W;
			}
		}

		public double B
		{
			get
			{
				return this.T + this.H;
			}
		}

		public D2Point LT
		{
			get
			{
				return new D2Point(this.L, this.T);
			}
		}

		public D2Point RT
		{
			get
			{
				return new D2Point(this.R, this.T);
			}
		}

		public D2Point RB
		{
			get
			{
				return new D2Point(this.R, this.B);
			}
		}

		public D2Point LB
		{
			get
			{
				return new D2Point(this.L, this.B);
			}
		}

		public P4Poly Poly
		{
			get
			{
				return new P4Poly(this.LT, this.RT, this.RB, this.LB);
			}
		}

		public D2Size Size
		{
			get
			{
				return new D2Size(this.W, this.H);
			}
		}

		public I4Rect ToI4Rect()
		{
			return new I4Rect(
				SCommon.ToInt(this.L),
				SCommon.ToInt(this.T),
				SCommon.ToInt(this.W),
				SCommon.ToInt(this.H)
				);
		}
	}
}
