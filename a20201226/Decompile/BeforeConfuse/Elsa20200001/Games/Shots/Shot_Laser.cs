using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000035 RID: 53
	public class Shot_Laser : Shot
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public Shot_Laser(double x, double y, int level) : base(x, y, Shot.Kind_e.NORMAL, (new int[]
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

		// Token: 0x060000A0 RID: 160 RVA: 0x00007CFD File Offset: 0x00005EFD
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
				DDDraw.SetAlpha(0.3);
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

		// Token: 0x040000FE RID: 254
		private int Level;
	}
}
