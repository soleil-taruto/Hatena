using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.ルーミアs;

namespace Charlotte.Games.Scripts
{
	public class Script_ルーミアテスト_0001小悪魔 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			Game.I.Enemies.Add(new Enemy_ルーミア());

			for (int c = 0; c < 30; c++)
				yield return true;

			foreach (bool v in ScriptCommon.掛け合い(new Scenario(@"e20200001_res\掛け合いシナリオ\小悪魔_ルーミア.txt")))
				yield return v;

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
