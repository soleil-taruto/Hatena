using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000049 RID: 73
	public class Script_RumiaTest_0004 : Script
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x00007E8C File Offset: 0x0000608C
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Game.I.Enemies.Add(new Enemy_Rumia_02_04(256.0, 32.0, false));
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
