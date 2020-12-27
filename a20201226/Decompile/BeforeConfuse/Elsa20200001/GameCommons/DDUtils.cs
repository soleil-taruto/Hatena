using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000095 RID: 149
	public static class DDUtils
	{
		// Token: 0x06000270 RID: 624 RVA: 0x00010001 File Offset: 0x0000E201
		public static bool IsWindowActive()
		{
			return DX.GetActiveFlag() != 0;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0001000B File Offset: 0x0000E20B
		public static long GetCurrTime()
		{
			return DX.GetNowHiPerformanceCount() / 1000L;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00010019 File Offset: 0x0000E219
		public static bool GetMouseDispMode()
		{
			return DX.GetMouseDispFlag() != 0;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00010023 File Offset: 0x0000E223
		public static void SetMouseDispMode(bool mode)
		{
			DX.SetMouseDispFlag(mode ? 1 : 0);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00010032 File Offset: 0x0000E232
		public static uint GetColor(I3Color color)
		{
			return DX.GetColor(color.R, color.G, color.B);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0001004B File Offset: 0x0000E24B
		public static byte[] SplitableJoin(string[] lines)
		{
			return SCommon.SplittableJoin((from line in lines
			select Encoding.UTF8.GetBytes(line)).ToArray<byte[]>());
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0001007C File Offset: 0x0000E27C
		public static string[] Split(byte[] data)
		{
			return (from bLine in SCommon.Split(data)
			select Encoding.UTF8.GetString(bLine)).ToArray<string>();
		}

		// Token: 0x06000277 RID: 631 RVA: 0x000020C9 File Offset: 0x000002C9
		public static void Noop(params object[] dummyPrms)
		{
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000100B0 File Offset: 0x0000E2B0
		public static T FastDesertElement<T>(List<T> list, Predicate<T> match, T defval = default(T))
		{
			for (int index = 0; index < list.Count; index++)
			{
				if (match(list[index]))
				{
					return SCommon.FastDesertElement<T>(list, index);
				}
			}
			return defval;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000100E6 File Offset: 0x0000E2E6
		public static bool CountDown(ref int count)
		{
			if (count < 0)
			{
				count++;
			}
			else
			{
				if (0 >= count)
				{
					return false;
				}
				count--;
			}
			return true;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00010105 File Offset: 0x0000E305
		public static void Approach(ref double value, double target, double rate)
		{
			value -= target;
			value *= rate;
			value += target;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00010119 File Offset: 0x0000E319
		public static void ToRange(ref double value, double minval, double maxval)
		{
			value = SCommon.ToRange(value, minval, maxval);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00010126 File Offset: 0x0000E326
		public static void ToRange(ref int value, int minval, int maxval)
		{
			value = SCommon.ToRange(value, minval, maxval);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00010133 File Offset: 0x0000E333
		public static void Minim(ref double value, double minval)
		{
			value = Math.Min(value, minval);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0001013F File Offset: 0x0000E33F
		public static void Minim(ref int value, int minval)
		{
			value = Math.Min(value, minval);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0001014B File Offset: 0x0000E34B
		public static void Maxim(ref double value, double minval)
		{
			value = Math.Max(value, minval);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00010157 File Offset: 0x0000E357
		public static void Maxim(ref int value, int minval)
		{
			value = Math.Max(value, minval);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00010163 File Offset: 0x0000E363
		public static void Maxim(ref long value, long minval)
		{
			value = Math.Max(value, minval);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00010170 File Offset: 0x0000E370
		public static void Rotate(ref double x, ref double y, double rot)
		{
			double w = x * Math.Cos(rot) - y * Math.Sin(rot);
			y = x * Math.Sin(rot) + y * Math.Cos(rot);
			x = w;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000101A9 File Offset: 0x0000E3A9
		public static double GetDistance(double x, double y)
		{
			return Math.Sqrt(x * x + y * y);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x000101B7 File Offset: 0x0000E3B7
		public static double GetDistance(D2Point pt)
		{
			return DDUtils.GetDistance(pt.X, pt.Y);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000101CA File Offset: 0x0000E3CA
		public static double GetDistance(D2Point pt, D2Point origin)
		{
			return DDUtils.GetDistance(pt.X - origin.X, pt.Y - origin.Y);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000101EC File Offset: 0x0000E3EC
		public static double GetAngle(double x, double y)
		{
			if (y < 0.0)
			{
				return 6.2831853071795862 - DDUtils.GetAngle(x, -y);
			}
			if (x < 0.0)
			{
				return 3.1415926535897931 - DDUtils.GetAngle(-x, y);
			}
			if (x <= 0.0)
			{
				return 1.5707963267948966;
			}
			if (y <= 0.0)
			{
				return 0.0;
			}
			double r = 0.0;
			double r2 = 1.5707963267948966;
			double t = y / x;
			int c = 1;
			double rm;
			for (;;)
			{
				rm = (r + r2) / 2.0;
				if (10 <= c)
				{
					break;
				}
				double rmt = Math.Tan(rm);
				if (t < rmt)
				{
					r2 = rm;
				}
				else
				{
					r = rm;
				}
				c++;
			}
			return rm;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000102AE File Offset: 0x0000E4AE
		public static double GetAngle(D2Point pt)
		{
			return DDUtils.GetAngle(pt.X, pt.Y);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000102C1 File Offset: 0x0000E4C1
		public static D2Point AngleToPoint(double angle, double distance)
		{
			return new D2Point(distance * Math.Cos(angle), distance * Math.Sin(angle));
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000102D8 File Offset: 0x0000E4D8
		public static void MakeXYSpeed(double x, double y, double targetX, double targetY, double speed, out double xa, out double ya)
		{
			D2Point pt = new D2Point(targetX - x, targetY - y);
			pt = DDUtils.AngleToPoint(DDUtils.GetAngle(pt), speed);
			xa = pt.X;
			ya = pt.Y;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00010312 File Offset: 0x0000E512
		public static bool IsCrashed_Circle_Circle(D2Point pt1, double r1, D2Point pt2, double r2)
		{
			return DDUtils.GetDistance(pt1 - pt2) < r1 + r2;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00010325 File Offset: 0x0000E525
		public static bool IsCrashed_Circle_Point(D2Point pt1, double r1, D2Point pt2)
		{
			return DDUtils.GetDistance(pt1 - pt2) < r1;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00010338 File Offset: 0x0000E538
		public static bool IsCrashed_Circle_Rect(D2Point pt1, double r1, D4Rect rect2)
		{
			if (pt1.X < rect2.L)
			{
				if (pt1.Y < rect2.T)
				{
					return DDUtils.IsCrashed_Circle_Point(pt1, r1, new D2Point(rect2.L, rect2.T));
				}
				if (rect2.B < pt1.Y)
				{
					return DDUtils.IsCrashed_Circle_Point(pt1, r1, new D2Point(rect2.L, rect2.B));
				}
				return rect2.L < pt1.X + r1;
			}
			else
			{
				if (rect2.R >= pt1.X)
				{
					return rect2.T - r1 < pt1.Y && pt1.Y < rect2.B + r1;
				}
				if (pt1.Y < rect2.T)
				{
					return DDUtils.IsCrashed_Circle_Point(pt1, r1, new D2Point(rect2.R, rect2.T));
				}
				if (rect2.B < pt1.Y)
				{
					return DDUtils.IsCrashed_Circle_Point(pt1, r1, new D2Point(rect2.R, rect2.B));
				}
				return pt1.X - r1 < rect2.R;
			}
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0001044A File Offset: 0x0000E64A
		public static bool IsCrashed_Rect_Point(D4Rect rect1, D2Point pt2)
		{
			return rect1.L < pt2.X && pt2.X < rect1.R && rect1.T < pt2.Y && pt2.Y < rect1.B;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00010488 File Offset: 0x0000E688
		public static bool IsCrashed_Rect_Rect(D4Rect rect1, D4Rect rect2)
		{
			return rect1.L < rect2.R && rect2.L < rect1.R && rect1.T < rect2.B && rect2.T < rect1.B;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x000104C8 File Offset: 0x0000E6C8
		public static bool IsOut(D2Point pt, D4Rect rect, double margin = 0.0)
		{
			return pt.X < rect.L - margin || rect.R + margin < pt.X || pt.Y < rect.T - margin || rect.B + margin < pt.Y;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00010519 File Offset: 0x0000E719
		public static bool IsOutOfScreen(D2Point pt, double margin = 0.0)
		{
			return DDUtils.IsOut(pt, new D4Rect(0.0, 0.0, 960.0, 540.0), margin);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0001054B File Offset: 0x0000E74B
		public static bool IsOutOfCamera(D2Point pt, double margin = 0.0)
		{
			return DDUtils.IsOut(pt, new D4Rect((double)DDGround.ICamera.X, (double)DDGround.ICamera.Y, 960.0, 540.0), margin);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00010581 File Offset: 0x0000E781
		public static void UpdateInput(ref int counter, bool status)
		{
			if (!status)
			{
				counter = ((0 < counter) ? -1 : 0);
				return;
			}
			if (counter < 0)
			{
				counter = 1;
				return;
			}
			counter++;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000105A1 File Offset: 0x0000E7A1
		public static bool IsPound(int counter)
		{
			return counter == 1 || (17 < counter && (counter - 17) % 4 == 1);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000105B9 File Offset: 0x0000E7B9
		public static double Parabola(double x)
		{
			return (x - x * x) * 4.0;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000105CC File Offset: 0x0000E7CC
		public static double SCurve(double x)
		{
			if (x < 0.5)
			{
				return (1.0 - DDUtils.Parabola(x + 0.5)) * 0.5;
			}
			return (1.0 + DDUtils.Parabola(x - 0.5)) * 0.5;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00010630 File Offset: 0x0000E830
		public static void AdjustRect(D2Size size, D4Rect rect, out D4Rect interior, out D4Rect exterior, double slideRate = 0.5)
		{
			double w_h = rect.H * size.W / size.H;
			double h_w = rect.W * size.H / size.W;
			D4Rect rect2;
			rect2.L = rect.L + (rect.W - w_h) * slideRate;
			rect2.T = rect.T;
			rect2.W = w_h;
			rect2.H = rect.H;
			D4Rect rect3;
			rect3.L = rect.L;
			rect3.T = rect.T + (rect.H - h_w) * slideRate;
			rect3.W = rect.W;
			rect3.H = h_w;
			if (w_h < rect.W)
			{
				interior = rect2;
				exterior = rect3;
				return;
			}
			interior = rect3;
			exterior = rect2;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00010704 File Offset: 0x0000E904
		public static D4Rect AdjustRectInterior(D2Size size, D4Rect rect, double slideRate = 0.5)
		{
			D4Rect interior;
			D4Rect exterior;
			DDUtils.AdjustRect(size, rect, out interior, out exterior, slideRate);
			return interior;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00010720 File Offset: 0x0000E920
		public static D4Rect AdjustRectExterior(D2Size size, D4Rect rect, double slideRate = 0.5)
		{
			D4Rect interior;
			D4Rect exterior;
			DDUtils.AdjustRect(size, rect, out interior, out exterior, slideRate);
			return exterior;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0001073A File Offset: 0x0000E93A
		public static double AToBRate(double a, double b, double rate)
		{
			return a + (b - a) * rate;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00010743 File Offset: 0x0000E943
		public static D2Point AToBRate(D2Point a, D2Point b, double rate)
		{
			return a + (b - a) * rate;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00010758 File Offset: 0x0000E958
		public static int Sign(double value)
		{
			if (value < 0.0)
			{
				return -1;
			}
			if (value > 0.0)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x04000209 RID: 521
		private const int POUND_FIRST_DELAY = 17;

		// Token: 0x0400020A RID: 522
		private const int POUND_DELAY = 4;

		// Token: 0x0400020B RID: 523
		public static DDRandom Random = new DDRandom();
	}
}
