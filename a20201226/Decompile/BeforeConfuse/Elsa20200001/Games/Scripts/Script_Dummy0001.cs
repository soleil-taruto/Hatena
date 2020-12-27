using System;
using System.Collections.Generic;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200003E RID: 62
	public class Script_Dummy0001 : Script
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00007E29 File Offset: 0x00006029
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
