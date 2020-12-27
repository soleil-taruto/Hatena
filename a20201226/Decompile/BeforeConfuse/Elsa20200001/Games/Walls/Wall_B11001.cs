using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000022 RID: 34
	public class Wall_B11001 : Wall
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000071F0 File Offset: 0x000053F0
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BLUETILE_02_REDUCT4, 0, 2, -128, 0, 0.01, 1.0, true, 1.0);
		}
	}
}
