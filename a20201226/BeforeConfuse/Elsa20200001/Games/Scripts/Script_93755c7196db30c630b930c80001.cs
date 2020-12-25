using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.鍵山雛s;

namespace Charlotte.Games.Scripts
{
	public class Script_鍵山雛テスト0001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());

			Game.I.Enemies.Add(new Enemy_鍵山雛_01(GameConsts.FIELD_W / 2, GameConsts.FIELD_H / 7));

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
