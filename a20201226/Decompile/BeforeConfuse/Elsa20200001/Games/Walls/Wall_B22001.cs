using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000029 RID: 41
	public class Wall_B22001 : Wall
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000073A0 File Offset: 0x000055A0
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BW_ARMY, 0, -2, 0, 0, 0.01, 1.0, true, 0.3);
		}
	}
}
