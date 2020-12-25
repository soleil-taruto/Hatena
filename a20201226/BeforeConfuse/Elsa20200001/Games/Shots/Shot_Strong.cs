using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	public class Shot_Strong : Shot
	{
		private double R;

		public Shot_Strong(double x, double y, double r)
			: base(x, y, Kind_e.NORMAL, 3)
		{
			this.R = r;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				double ax = 0.0;
				double ay = -20.0;

				DDUtils.Rotate(ref ax, ref ay, this.R);

				this.X += ax;
				this.Y += ay;

				if (DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H)))
					break;

				DDDraw.SetAlpha(ShotConsts.A);
				DDDraw.DrawBegin(Ground.I.Picture2.D_SHOT, this.X, this.Y);
				DDDraw.DrawZoom(1.2);
				DDDraw.DrawRotate(this.R);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				this.Crash = DDCrashUtils.Circle(
					new D2Point(this.X, this.Y),
					14.4
					);

				yield return true;
			}
		}
	}
}
