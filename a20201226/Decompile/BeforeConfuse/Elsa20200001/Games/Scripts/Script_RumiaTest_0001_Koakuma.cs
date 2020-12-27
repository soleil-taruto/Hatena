using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000048 RID: 72
	public class Script_RumiaTest_0001_Koakuma : Script
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00007E83 File Offset: 0x00006083
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Game.I.Enemies.Add(new Enemy_Rumia());
			int num;
			for (int c = 0; c < 30; c = num + 1)
			{
				yield return true;
				num = c;
			}
			foreach (bool v in ScriptCommon.掛け合い(new Scenario("e20200001_res\\掛け合いシナリオ\\小悪魔_ルーミア.txt")))
			{
				yield return v;
			}
			IEnumerator<bool> enumerator = null;
			for (;;)
			{
				yield return true;
			}
			yield break;
			yield break;
		}
	}
}
