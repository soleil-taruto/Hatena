using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	public static class DDCrashUtils
	{
		public enum Kind_e
		{
			NONE = 1,
			POINT,
			CIRCLE,
			RECT,
			MULTI,
		}

		public static DDCrash None()
		{
			return new DDCrash()
			{
				Kind = Kind_e.NONE,
			};
		}

		public static DDCrash Point(D2Point pt)
		{
			return new DDCrash()
			{
				Kind = Kind_e.POINT,
				Pt = pt,
			};
		}

		public static DDCrash Circle(D2Point pt, double r)
		{
			return new DDCrash()
			{
				Kind = Kind_e.CIRCLE,
				Pt = pt,
				R = r,
			};
		}

		public static DDCrash Rect_CenterSize(D2Point centerPt, D2Size size)
		{
			return Rect(new D4Rect(centerPt.X - size.W / 2.0, centerPt.Y - size.H / 2.0, size.W, size.H));
		}

		public static DDCrash Rect(D4Rect rect)
		{
			return new DDCrash()
			{
				Kind = Kind_e.RECT,
				Rect = rect,
			};
		}

		public static DDCrash Multi(params DDCrash[] crashes)
		{
			return new DDCrash()
			{
				Kind = Kind_e.MULTI,
				Crashes = crashes,
			};
		}

		public static bool IsCrashed(DDCrash a, DDCrash b)
		{
			if ((int)b.Kind < (int)a.Kind)
			{
				DDCrash tmp = a;
				a = b;
				b = tmp;
			}
			if (a.Kind == Kind_e.NONE)
				return false;

			if (b.Kind == Kind_e.MULTI)
				return IsCrashed_Any_Multi(a, b);

			if (a.Kind == Kind_e.POINT)
			{
				if (b.Kind == Kind_e.POINT)
					return false;

				if (b.Kind == Kind_e.CIRCLE)
					return DDUtils.IsCrashed_Circle_Point(b.Pt, b.R, a.Pt);

				if (b.Kind == Kind_e.RECT)
					return DDUtils.IsCrashed_Rect_Point(b.Rect, a.Pt);

				if (b.Kind == Kind_e.MULTI)

					throw new DDError();
			}
			if (a.Kind == Kind_e.CIRCLE)
			{
				if (b.Kind == Kind_e.CIRCLE)
					return DDUtils.IsCrashed_Circle_Circle(a.Pt, a.R, b.Pt, b.R);

				if (b.Kind == Kind_e.RECT)
					return DDUtils.IsCrashed_Circle_Rect(a.Pt, a.R, b.Rect);

				throw new DDError();
			}
			if (a.Kind == Kind_e.RECT)
			{
				if (b.Kind == Kind_e.RECT)
					return DDUtils.IsCrashed_Rect_Rect(a.Rect, b.Rect);

				throw new DDError();
			}
			throw new DDError();
		}

		private static bool IsCrashed_Any_Multi(DDCrash a, DDCrash b)
		{
			//if (b.Kind != Kind_e.MULTI) throw null; // never

			if (a.Kind == Kind_e.MULTI)
				return IsCrashed_Multi_Multi(a, b);

			foreach (DDCrash crash in b.Crashes)
				if (IsCrashed(a, crash))
					return true;

			return false;
		}

		private static bool IsCrashed_Multi_Multi(DDCrash a, DDCrash b)
		{
			//if (a.Kind != Kind_e.MULTI) throw null; // never
			//if (b.Kind != Kind_e.MULTI) throw null; // never

			foreach (DDCrash ac in a.Crashes)
				foreach (DDCrash bc in b.Crashes)
					if (IsCrashed(ac, bc))
						return true;

			return false;
		}
	}
}
