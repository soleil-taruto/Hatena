using System;

namespace Charlotte.Commons
{
	// Token: 0x020000A4 RID: 164
	public struct D4Rect
	{
		// Token: 0x060002E4 RID: 740 RVA: 0x000114F2 File Offset: 0x0000F6F2
		public D4Rect(D2Point lt, D2Size size)
		{
			this = new D4Rect(lt.X, lt.Y, size.W, size.H);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00011512 File Offset: 0x0000F712
		public D4Rect(double l, double t, double w, double h)
		{
			this.L = l;
			this.T = t;
			this.W = w;
			this.H = h;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00011531 File Offset: 0x0000F731
		public static D4Rect LTRB(double l, double t, double r, double b)
		{
			return new D4Rect(l, t, r - l, b - t);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x00011540 File Offset: 0x0000F740
		public double R
		{
			get
			{
				return this.L + this.W;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x0001154F File Offset: 0x0000F74F
		public double B
		{
			get
			{
				return this.T + this.H;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0001155E File Offset: 0x0000F75E
		public D2Point LT
		{
			get
			{
				return new D2Point(this.L, this.T);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060002EA RID: 746 RVA: 0x00011571 File Offset: 0x0000F771
		public D2Point RT
		{
			get
			{
				return new D2Point(this.R, this.T);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060002EB RID: 747 RVA: 0x00011584 File Offset: 0x0000F784
		public D2Point RB
		{
			get
			{
				return new D2Point(this.R, this.B);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060002EC RID: 748 RVA: 0x00011597 File Offset: 0x0000F797
		public D2Point LB
		{
			get
			{
				return new D2Point(this.L, this.B);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060002ED RID: 749 RVA: 0x000115AA File Offset: 0x0000F7AA
		public P4Poly Poly
		{
			get
			{
				return new P4Poly(this.LT, this.RT, this.RB, this.LB);
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000115C9 File Offset: 0x0000F7C9
		public D2Size Size
		{
			get
			{
				return new D2Size(this.W, this.H);
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000115DC File Offset: 0x0000F7DC
		public I4Rect ToI4Rect()
		{
			return new I4Rect(SCommon.ToInt(this.L), SCommon.ToInt(this.T), SCommon.ToInt(this.W), SCommon.ToInt(this.H));
		}

		// Token: 0x0400022C RID: 556
		public double L;

		// Token: 0x0400022D RID: 557
		public double T;

		// Token: 0x0400022E RID: 558
		public double W;

		// Token: 0x0400022F RID: 559
		public double H;
	}
}
