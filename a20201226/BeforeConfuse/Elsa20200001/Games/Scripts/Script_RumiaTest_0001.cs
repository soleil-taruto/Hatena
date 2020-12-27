using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.Rumias;

namespace Charlotte.Games.Scripts
{
	public class Script_RumiaTest_0001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			{
				Enemy_Rumia boss;

				Game.I.Enemies.Add(boss = new Enemy_Rumia());

				for (int c = 0; c < 30; c++)
					yield return true;

				foreach (bool v in ScriptCommon.掛け合い(new Scenario(@"e20200001_res\掛け合いシナリオ\メディスン_ルーミア.txt")))
					yield return v;

				boss.NextFlag = true;
			}

			Ground.I.Music.MUS_BOSS_01.Play();

			for (; ; )
			{
				// noop

				yield return true;
			}
		}
	}
}
