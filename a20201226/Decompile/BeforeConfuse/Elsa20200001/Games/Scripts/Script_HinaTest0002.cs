using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x02000046 RID: 70
	public class Script_HinaTest0002 : Script
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00007E71 File Offset: 0x00006071
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Enemies.Add(new Enemy_Hina_02(256.0, 73.0));
			for (;;)
			{
				yield return true;
			}
			yield break;
		}
	}
}
