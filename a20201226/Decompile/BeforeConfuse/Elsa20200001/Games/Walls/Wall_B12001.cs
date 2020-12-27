using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000024 RID: 36
	public class Wall_B12001 : Wall
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00007280 File Offset: 0x00005480
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BLUETILE_01, 1, 1, 0, 0, 0.01, 1.0, true, 1.0);
		}
	}
}
