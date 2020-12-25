using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	public class Shot_Bombメディスン : Shot
	{
		public Shot_Bombメディスン(double x, double y)
			: base(x, y, Kind_e.BOMB, 10000)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			RippleEffect.Add_波紋(this.X, this.Y, 60);
			RippleEffect.Add_波紋(this.X, this.Y, 120);
			RippleEffect.Add_波紋(this.X, this.Y, 180);
			RippleEffect.Add_波紋(this.X, this.Y, 360);

			for (int frame = 0; frame < GameConsts.PLAYER_BOMB_FRAME_MAX + 60; frame++)
			{
				this.Crash = DDCrashUtils.Rect(D4Rect.LTRB(
					0,
					0,
					GameConsts.FIELD_W,
					GameConsts.FIELD_H
					));

				yield return true;
			}
		}
	}
}
