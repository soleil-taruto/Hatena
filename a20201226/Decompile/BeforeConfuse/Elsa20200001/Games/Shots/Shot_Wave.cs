using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000039 RID: 57
	public class Shot_Wave : Shot
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00007D87 File Offset: 0x00005F87
		public Shot_Wave(double x, double y, double r, double r_add) : base(x, y, Shot.Kind_e.NORMAL, 1)
		{
			this.R = r;
			this.RAdd = r_add;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007DA2 File Offset: 0x00005FA2
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				double ax = 0.0;
				double ay = -15.0;
				DDUtils.Rotate(ref ax, ref ay, this.R);
				this.X += ax;
				this.Y += ay;
				if (DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
				{
					break;
				}
				this.R += this.RAdd;
				DDDraw.SetAlpha(0.3);
				DDDraw.DrawBegin(Ground.I.Picture2.D_WAVESHOT, this.X, this.Y);
				DDDraw.DrawRotate(this.R);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 24.0);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x04000104 RID: 260
		private double R;

		// Token: 0x04000105 RID: 261
		private double RAdd;
	}
}
