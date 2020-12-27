using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000040 RID: 64
	public class Script_Test0002 : Script
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x00007E3B File Offset: 0x0000603B
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Enemies.Add(new Enemy_Hina());
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
			for (;;)
			{
				yield return true;
			}
			yield break;
			yield break;
		}
	}
}
