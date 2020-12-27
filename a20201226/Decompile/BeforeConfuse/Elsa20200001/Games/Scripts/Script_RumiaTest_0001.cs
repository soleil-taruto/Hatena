using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000042 RID: 66
	public class Script_RumiaTest_0001 : Script
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x00007E4D File Offset: 0x0000604D
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Enemy_Rumia boss;
			Game.I.Enemies.Add(boss = new Enemy_Rumia());
			int num;
			for (int c = 0; c < 30; c = num + 1)
			{
				yield return true;
				num = c;
			}
			foreach (bool v in ScriptCommon.掛け合い(new Scenario("e20200001_res\\掛け合いシナリオ\\メディスン_ルーミア.txt")))
			{
				yield return v;
			}
			IEnumerator<bool> enumerator = null;
			boss.NextFlag = true;
			boss = null;
			Ground.I.Music.MUS_BOSS_01.Play(false, false, 1.0, 30);
			for (;;)
			{
				yield return true;
			}
			yield break;
			yield break;
		}
	}
}
