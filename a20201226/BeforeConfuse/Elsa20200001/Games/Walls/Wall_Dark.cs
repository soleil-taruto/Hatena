using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Walls
{
	public class Wall_Dark : Wall
	{
		protected override IEnumerable<bool> E_Draw()
		{
			this.Filled = true;

			for (; ; )
			{
				DDDraw.SetBright(0, 0, 0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H));
				DDDraw.Reset();

				yield return true;
			}
		}
	}
}
