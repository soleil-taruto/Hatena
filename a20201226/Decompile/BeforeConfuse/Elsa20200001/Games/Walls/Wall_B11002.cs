using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000023 RID: 35
	public class Wall_B11002 : Wall
	{
		// Token: 0x0600006C RID: 108 RVA: 0x0000723C File Offset: 0x0000543C
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BW_PUMPKIN, 0, 3, -100, 0, 0.0001, 0.1, false, 1.0);
		}
	}
}
