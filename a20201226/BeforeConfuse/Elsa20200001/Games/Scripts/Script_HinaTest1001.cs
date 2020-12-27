using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Scripts
{
	// 鍵山雛通しテスト

	public class Script_HinaTest1001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_01.Play();

			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());

			// ---- BOSS 登場

			{
				Game.I.Shots.Add(new Shot_BossBomb());

				Enemy_Hina boss = new Enemy_Hina();

				Game.I.Enemies.Add(boss);

				for (int c = 0; c < 90; c++)
					yield return true;

				foreach (bool v in ScriptCommon.掛け合い(new Scenario(@"e20200001_res\掛け合いシナリオ\小悪魔_鍵山雛.txt")))
					yield return v;

				boss.NextFlag = true;

				// boss はすぐに消滅することに注意
			}

			Ground.I.Music.MUS_BOSS_01.Play();

			while (!Game.I.BossKilled)
				yield return true;

			for (int c = 0; c < 30; c++)
				yield return true;

			Game.I.Shots.Add(new Shot_BossBomb());

			// ---- BOSS 撃破

			for (int c = 0; c < 180; c++)
				yield return true;
		}
	}
}
