using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x02000099 RID: 153
	public static class DDCrashUtils
	{
		// Token: 0x060002AA RID: 682 RVA: 0x00010918 File Offset: 0x0000EB18
		public static DDCrash None()
		{
			return new DDCrash
			{
				Kind = DDCrashUtils.Kind_e.NONE
			};
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00010938 File Offset: 0x0000EB38
		public static DDCrash Point(D2Point pt)
		{
			return new DDCrash
			{
				Kind = DDCrashUtils.Kind_e.POINT,
				Pt = pt
			};
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00010960 File Offset: 0x0000EB60
		public static DDCrash Circle(D2Point pt, double r)
		{
			return new DDCrash
			{
				Kind = DDCrashUtils.Kind_e.CIRCLE,
				Pt = pt,
				R = r
			};
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00010990 File Offset: 0x0000EB90
		public static DDCrash Rect_CenterSize(D2Point centerPt, D2Size size)
		{
			return DDCrashUtils.Rect(new D4Rect(centerPt.X - size.W / 2.0, centerPt.Y - size.H / 2.0, size.W, size.H));
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000109E4 File Offset: 0x0000EBE4
		public static DDCrash Rect(D4Rect rect)
		{
			return new DDCrash
			{
				Kind = DDCrashUtils.Kind_e.RECT,
				Rect = rect
			};
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00010A0C File Offset: 0x0000EC0C
		public static DDCrash Multi(params DDCrash[] crashes)
		{
			return new DDCrash
			{
				Kind = DDCrashUtils.Kind_e.MULTI,
				Crashes = crashes
			};
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00010A34 File Offset: 0x0000EC34
		public static bool IsCrashed(DDCrash a, DDCrash b)
		{
			if (b.Kind < a.Kind)
			{
				DDCrash ddcrash = a;
				a = b;
				b = ddcrash;
			}
			if (a.Kind == DDCrashUtils.Kind_e.NONE)
			{
				return false;
			}
			if (b.Kind == DDCrashUtils.Kind_e.MULTI)
			{
				return DDCrashUtils.IsCrashed_Any_Multi(a, b);
			}
			if (a.Kind == DDCrashUtils.Kind_e.POINT)
			{
				if (b.Kind == DDCrashUtils.Kind_e.POINT)
				{
					return false;
				}
				if (b.Kind == DDCrashUtils.Kind_e.CIRCLE)
				{
					return DDUtils.IsCrashed_Circle_Point(b.Pt, b.R, a.Pt);
				}
				if (b.Kind == DDCrashUtils.Kind_e.RECT)
				{
					return DDUtils.IsCrashed_Rect_Point(b.Rect, a.Pt);
				}
				if (b.Kind == DDCrashUtils.Kind_e.MULTI)
				{
					throw new DDError();
				}
			}
			if (a.Kind == DDCrashUtils.Kind_e.CIRCLE)
			{
				if (b.Kind == DDCrashUtils.Kind_e.CIRCLE)
				{
					return DDUtils.IsCrashed_Circle_Circle(a.Pt, a.R, b.Pt, b.R);
				}
				if (b.Kind == DDCrashUtils.Kind_e.RECT)
				{
					return DDUtils.IsCrashed_Circle_Rect(a.Pt, a.R, b.Rect);
				}
				throw new DDError();
			}
			else
			{
				if (a.Kind != DDCrashUtils.Kind_e.RECT)
				{
					throw new DDError();
				}
				if (b.Kind == DDCrashUtils.Kind_e.RECT)
				{
					return DDUtils.IsCrashed_Rect_Rect(a.Rect, b.Rect);
				}
				throw new DDError();
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00010B58 File Offset: 0x0000ED58
		private static bool IsCrashed_Any_Multi(DDCrash a, DDCrash b)
		{
			if (a.Kind == DDCrashUtils.Kind_e.MULTI)
			{
				return DDCrashUtils.IsCrashed_Multi_Multi(a, b);
			}
			foreach (DDCrash crash in b.Crashes)
			{
				if (DDCrashUtils.IsCrashed(a, crash))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00010BA0 File Offset: 0x0000EDA0
		private static bool IsCrashed_Multi_Multi(DDCrash a, DDCrash b)
		{
			foreach (DDCrash ac in a.Crashes)
			{
				foreach (DDCrash bc in b.Crashes)
				{
					if (DDCrashUtils.IsCrashed(ac, bc))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0200014E RID: 334
		public enum Kind_e
		{
			// Token: 0x04000547 RID: 1351
			NONE = 1,
			// Token: 0x04000548 RID: 1352
			POINT,
			// Token: 0x04000549 RID: 1353
			CIRCLE,
			// Token: 0x0400054A RID: 1354
			RECT,
			// Token: 0x0400054B RID: 1355
			MULTI
		}
	}
}
