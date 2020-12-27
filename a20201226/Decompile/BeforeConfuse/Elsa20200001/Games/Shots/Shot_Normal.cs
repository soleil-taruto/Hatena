using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000036 RID: 54
	public class Shot_Normal : Shot
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00007D0D File Offset: 0x00005F0D
		public Shot_Normal(double x, double y, double r) : base(x, y, Shot.Kind_e.NORMAL, 2)
		{
			this.R = r;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00007D20 File Offset: 0x00005F20
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
				DDDraw.DrawRotate(this.R);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 12.0);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x040000FF RID: 255
		private double R;
	}
}
