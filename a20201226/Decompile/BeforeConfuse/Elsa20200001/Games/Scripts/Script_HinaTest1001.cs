using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Shots;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000047 RID: 71
	public class Script_HinaTest1001 : Script
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00007E7A File Offset: 0x0000607A
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_01.Play(false, false, 1.0, 30);
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Shots.Add(new Shot_BossBomb());
			Enemy_Hina boss = new Enemy_Hina();
			Game.I.Enemies.Add(boss);
			int num;
			for (int c = 0; c < 90; c = num + 1)
			{
				yield return true;
				num = c;
			}
			foreach (bool v in ScriptCommon.掛け合い(new Scenario("e20200001_res\\掛け合いシナリオ\\小悪魔_鍵山雛.txt")))
			{
				yield return v;
			}
			IEnumerator<bool> enumerator = null;
			boss.NextFlag = true;
			boss = null;
			Ground.I.Music.MUS_BOSS_01.Play(false, false, 1.0, 30);
			while (!Game.I.BossKilled)
			{
				yield return true;
			}
			for (int c = 0; c < 30; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Shots.Add(new Shot_BossBomb());
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			yield break;
			yield break;
		}
	}
}
