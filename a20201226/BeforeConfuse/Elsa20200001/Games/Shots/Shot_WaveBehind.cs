using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	public class Shot_WaveBehind : Shot
	{
		private double R;
		private double RAdd;
		private double RAddRatePerFrame;

		public Shot_WaveBehind(double x, double y, double r, double r_add, double r_add_ratePerFrame)
			: base(x, y, Kind_e.NORMAL, 10)
		{
			this.R = r;
			this.RAdd = r_add;
			this.RAddRatePerFrame = r_add_ratePerFrame;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				double ax = 0.0;
				double ay = -15.0;

				DDUtils.Rotate(ref ax, ref ay, this.R);

				this.X += ax;
				this.Y += ay;

				//if (DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H)))
				//    break;

				this.R += this.RAdd;
				DDUtils.Approach(ref this.RAdd, 0.0, this.RAddRatePerFrame);

				DDDraw.SetAlpha(ShotConsts.A * 2.0);
				DDDraw.SetBright(1.0, 0.3, 0.3);
				DDDraw.DrawBegin(Ground.I.Picture2.D_WAVESHOT, this.X, this.Y);
				DDDraw.DrawRotate(this.R);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				this.Crash = DDCrashUtils.Circle(
					new D2Point(this.X, this.Y),
					24.0
					);

				yield return true;
			}
		}
	}
}
