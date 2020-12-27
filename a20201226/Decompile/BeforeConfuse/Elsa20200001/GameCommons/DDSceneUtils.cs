using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x02000089 RID: 137
	public static class DDSceneUtils
	{
		// Token: 0x0600021E RID: 542 RVA: 0x0000E6DC File Offset: 0x0000C8DC
		public static IEnumerable<DDScene> Create(int frameMax)
		{
			int num;
			for (int frame = 0; frame <= frameMax; frame = num + 1)
			{
				yield return new DDScene(frame, frameMax);
				num = frame;
			}
			yield break;
		}
	}
}
