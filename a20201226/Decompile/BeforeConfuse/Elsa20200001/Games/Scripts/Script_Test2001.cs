using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200004B RID: 75
	public class Script_Test2001 : Script
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00007E9E File Offset: 0x0000609E
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			int num;
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			long bonus = 100000000L;
			DDGround.EL.Add(SCommon.Supplier<bool>(Effects.Message("ALL CLEAR BONUS +" + bonus.ToString(), new I3Color(64, 64, 0), new I3Color(255, 255, 0))));
			Game.I.Score += bonus;
			for (int c = 0; c < 300; c = num + 1)
			{
				yield return true;
				num = c;
			}
			yield break;
		}
	}
}
