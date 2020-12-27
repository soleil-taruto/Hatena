using System;
using System.Collections.Generic;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000026 RID: 38
	public class Wall_B21001 : Wall
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00007308 File Offset: 0x00005508
		protected override IEnumerable<bool> E_Draw()
		{
			return WallCommon.Standard(this, Ground.I.Picture.P_BW_NAVY, 0, 2, 0, 0, 0.01, 1.0, true, 0.3);
		}
	}
}
