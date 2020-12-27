using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000038 RID: 56
	public class Shot_Strong : Shot
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00007D64 File Offset: 0x00005F64
		public Shot_Strong(double x, double y, double r) : base(x, y, Shot.Kind_e.NORMAL, 3)
		{
			this.R = r;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007D77 File Offset: 0x00005F77
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				double ax = 0.0;
				double ay = -20.0;
				DDUtils.Rotate(ref ax, ref ay, this.R);
				this.X += ax;
				this.Y += ay;
				if (DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
				{
					break;
				}
				DDDraw.SetAlpha(0.3);
				DDDraw.DrawBegin(Ground.I.Picture2.D_SHOT, this.X, this.Y);
				DDDraw.DrawZoom(1.2);
				DDDraw.DrawRotate(this.R);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 14.4);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x04000103 RID: 259
		private double R;
	}
}
