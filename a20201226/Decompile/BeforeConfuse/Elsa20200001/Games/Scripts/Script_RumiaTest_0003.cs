using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000044 RID: 68
	public class Script_RumiaTest_0003 : Script
	{
		// Token: 0x060000BD RID: 189 RVA: 0x00007E5F File Offset: 0x0000605F
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Game.I.Enemies.Add(new Enemy_Rumia_03(256.0, 32.0));
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
