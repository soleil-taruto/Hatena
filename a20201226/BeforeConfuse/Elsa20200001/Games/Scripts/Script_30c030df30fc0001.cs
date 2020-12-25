using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	public class Script_ダミー0001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
