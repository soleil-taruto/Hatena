using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.Hinas;

namespace Charlotte.Games.Scripts
{
	public class Script_HinaTest0001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());

			Game.I.Enemies.Add(new Enemy_Hina_01(GameConsts.FIELD_W / 2, GameConsts.FIELD_H / 7));

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
