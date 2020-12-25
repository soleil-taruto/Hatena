using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.Walls
{
	public class Wall_B11002 : Wall
	{
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BW_PUMPKIN, 0, 3, -100, 0, 0.0001, 0.1, false);
		}
	}
}
