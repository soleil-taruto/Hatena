using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000025 RID: 37
	public class Wall_B13001 : Wall
	{
		// Token: 0x06000070 RID: 112 RVA: 0x000072C4 File Offset: 0x000054C4
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BLUETILE_03, -1, 1, 0, 0, 0.01, 1.0, true, 1.0);
		}
	}
}
