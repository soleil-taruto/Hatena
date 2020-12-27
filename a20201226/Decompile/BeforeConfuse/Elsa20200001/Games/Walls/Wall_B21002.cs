using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000027 RID: 39
	public class Wall_B21002 : Wall
	{
		// Token: 0x06000074 RID: 116 RVA: 0x0000734C File Offset: 0x0000554C
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BW_PUMPKIN, 0, 3, -100, 0, 0.0001, 0.1, false, 1.0);
		}
	}
}
