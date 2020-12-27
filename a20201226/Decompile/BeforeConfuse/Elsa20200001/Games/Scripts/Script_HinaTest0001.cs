using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000045 RID: 69
	public class Script_HinaTest0001 : Script
	{
		// Token: 0x060000BF RID: 191 RVA: 0x00007E68 File Offset: 0x00006068
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Enemies.Add(new Enemy_Hina_01(256.0, 73.0));
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
