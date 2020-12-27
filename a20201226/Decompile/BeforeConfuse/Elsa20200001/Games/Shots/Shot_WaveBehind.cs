using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000037 RID: 55
	public class Shot_WaveBehind : Shot
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x00007D30 File Offset: 0x00005F30
		public Shot_WaveBehind(double x, double y, double r, double r_add, double r_add_ratePerFrame) : base(x, y, Shot.Kind_e.NORMAL, 10)
		{
			this.R = r;
			this.RAdd = r_add;
			this.RAddRatePerFrame = r_add_ratePerFrame;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007D54 File Offset: 0x00005F54
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
				this.R += this.RAdd;
				DDUtils.Approach(ref this.RAdd, 0.0, this.RAddRatePerFrame);
				DDDraw.SetAlpha(0.6);
				DDDraw.SetBright(1.0, 0.3, 0.3);
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

		// Token: 0x04000100 RID: 256
		private double R;

		// Token: 0x04000101 RID: 257
		private double RAdd;

		// Token: 0x04000102 RID: 258
		private double RAddRatePerFrame;
	}
}
