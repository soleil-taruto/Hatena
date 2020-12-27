using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x0200009A RID: 154
	public static class DDCrashView
	{
		// Token: 0x060002B3 RID: 691 RVA: 0x00010BFA File Offset: 0x0000EDFA
		public static void Draw(IEnumerable<DDCrash> crashes, I3Color color, double a = 1.0)
		{
			DDDraw.SetAlpha(a);
			DDDraw.SetBright(color);
			DDCrashView.Draw(crashes);
			DDDraw.Reset();
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00010C14 File Offset: 0x0000EE14
		public static void Draw(IEnumerable<DDCrash> crashes)
		{
			Queue<IEnumerable<DDCrash>> q = new Queue<IEnumerable<DDCrash>>();
			q.Enqueue(crashes);
			while (1 <= q.Count)
			{
				foreach (DDCrash crash in q.Dequeue())
				{
					switch (crash.Kind)
					{
					case DDCrashUtils.Kind_e.NONE:
						break;
					case DDCrashUtils.Kind_e.POINT:
						DDDraw.DrawBegin(DDGround.GeneralResource.WhiteBox, crash.Pt.X - (double)DDGround.ICamera.X, crash.Pt.Y - (double)DDGround.ICamera.Y);
						DDDraw.DrawSetSize(4.0, 4.0);
						DDDraw.DrawEnd();
						break;
					case DDCrashUtils.Kind_e.CIRCLE:
						DDDraw.DrawBegin(DDGround.GeneralResource.WhiteCircle, crash.Pt.X - (double)DDGround.ICamera.X, crash.Pt.Y - (double)DDGround.ICamera.Y);
						DDDraw.DrawSetSize(crash.R * 2.0, crash.R * 2.0);
						DDDraw.DrawEnd();
						break;
					case DDCrashUtils.Kind_e.RECT:
						DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, crash.Rect.L - (double)DDGround.ICamera.X, crash.Rect.T - (double)DDGround.ICamera.Y, crash.Rect.W, crash.Rect.H);
						break;
					case DDCrashUtils.Kind_e.MULTI:
						q.Enqueue(crash.Crashes);
						break;
					default:
						throw null;
					}
				}
			}
		}

		// Token: 0x04000218 RID: 536
		private const double POINT_WH = 4.0;
	}
}
