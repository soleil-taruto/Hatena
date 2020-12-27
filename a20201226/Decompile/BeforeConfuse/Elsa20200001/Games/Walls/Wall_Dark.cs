using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000028 RID: 40
	public class Wall_Dark : Wall
	{
		// Token: 0x06000076 RID: 118 RVA: 0x0000738F File Offset: 0x0000558F
		protected override IEnumerable<bool> E_Draw()
		{
			this.Filled = true;
			for (;;)
			{
				DDDraw.SetBright(0.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0.0, 0.0, 512.0, 512.0));
				DDDraw.Reset();
				yield return true;
			}
			yield break;
		}
	}
}
