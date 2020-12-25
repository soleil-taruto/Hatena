using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public struct P4Poly
	{
		public D2Point LT;
		public D2Point RT;
		public D2Point RB;
		public D2Point LB;

		public P4Poly(D2Point lt, D2Point rt, D2Point rb, D2Point lb)
		{
			this.LT = lt;
			this.RT = rt;
			this.RB = rb;
			this.LB = lb;
		}

		public P4Poly(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
		{
			this.LT = new D2Point(x1, y1);
			this.RT = new D2Point(x2, y2);
			this.RB = new D2Point(x3, y3);
			this.LB = new D2Point(x4, y4);
		}
	}
}
