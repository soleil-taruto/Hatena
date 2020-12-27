using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.Rumias;

namespace Charlotte.Games.Scripts
{
	public class Script_RumiaTest_0002 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			Game.I.Enemies.Add(new Enemy_Rumia_02_04(GameConsts.FIELD_W / 2, GameConsts.FIELD_H / 16, true));

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
