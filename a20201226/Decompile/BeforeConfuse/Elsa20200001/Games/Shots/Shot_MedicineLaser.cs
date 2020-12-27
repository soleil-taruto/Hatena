using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x0200003A RID: 58
	public class Shot_MedicineLaser : Shot
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00007DB2 File Offset: 0x00005FB2
		public Shot_MedicineLaser(double x, double y, int level) : base(x, y, Shot.Kind_e.NORMAL, (new int[]
		{
			3,
			6,
			7,
			13,
			15,
			21
		})[level])
		{
			this.Level = level;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00007DD7 File Offset: 0x00005FD7
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				this.Y -= 18.0;
				if (this.Y < 0.0)
				{
					break;
				}
				DDDraw.SetAlpha(0.44999999999999996);
				DDDraw.SetBright(1.0, 1.0, 0.0);
				DDDraw.DrawBegin(Ground.I.Picture2.D_LASER, this.X, this.Y);
				DDDraw.DrawZoom(1.0 + 0.5 * (double)this.Level / 5.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 32.0 + 16.0 * (double)this.Level / 5.0);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x04000106 RID: 262
		private int Level;
	}
}
