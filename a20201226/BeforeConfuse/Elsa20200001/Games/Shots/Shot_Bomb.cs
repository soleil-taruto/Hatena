using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	public class Shot_Bomb : Shot
	{
		public Shot_Bomb(double x, double y)
			: base(x, y, Kind_e.BOMB, 10000)
		{ }

		private const double BOMB_R = 200.0;

		protected override IEnumerable<bool> E_Draw()
		{
			double dc_r = Ground.I.Picture2.D_DECOCIRCLE.Get_W() / 2.0;
			double dc_z = BOMB_R / dc_r;

			Func<bool>[] circles = new Func<bool>[]
			{
				SCommon.Supplier(this.Circle(
					0.75,
					dc_z,
					0.1,
					0.05
					)),
				SCommon.Supplier(this.Circle(
					0.25,
					dc_z * 0.75,
					dc_z * 2.0,
					0.01
					)),
				SCommon.Supplier(this.Circle(
					0.5,
					dc_z * 0.5,
					0.1,
					-0.02
					)),
			};

			for (int frame = 0; frame < GameConsts.PLAYER_BOMB_FRAME_MAX + 60; frame++)
			{
				this.Y -= frame / 60.0;

				foreach (Func<bool> circle in circles)
					circle();

				this.Crash = DDCrashUtils.Circle(
					new D2Point(this.X, this.Y),
					BOMB_R
					);

				yield return true;
			}
		}

		private IEnumerable<bool> Circle(double target_a, double target_z, double z, double r_add)
		{
			double a = 0.0;
			double r = 0.0;

			for (int frame = 0; ; frame++)
			{
				if (frame == GameConsts.PLAYER_BOMB_FRAME_MAX)
					target_a = 0.0;

				DDUtils.Approach(ref a, target_a, 0.9);
				DDUtils.Approach(ref z, target_z, 0.9);
				r += r_add;

				DDDraw.SetAlpha(a);
				DDDraw.DrawBegin(Ground.I.Picture2.D_DECOCIRCLE, this.X, this.Y);
				DDDraw.DrawZoom(z);
				DDDraw.DrawRotate(r);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}
	}
}
