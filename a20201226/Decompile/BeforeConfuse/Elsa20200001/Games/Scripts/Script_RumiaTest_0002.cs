using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000043 RID: 67
	public class Script_RumiaTest_0002 : Script
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00007E56 File Offset: 0x00006056
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Game.I.Enemies.Add(new Enemy_Rumia_02_04(256.0, 32.0, true));
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
