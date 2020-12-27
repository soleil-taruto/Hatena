using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006C RID: 108
	public static class DDDerivationUtils
	{
		// Token: 0x06000154 RID: 340 RVA: 0x0000A444 File Offset: 0x00008644
		public static void Add(DDPicture derivation)
		{
			DDDerivationUtils.Derivations.Add(derivation);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000A454 File Offset: 0x00008654
		public static void UnloadAll()
		{
			foreach (DDPicture ddpicture in DDDerivationUtils.Derivations)
			{
				ddpicture.Unload();
			}
		}

		// Token: 0x0400017D RID: 381
		public static List<DDPicture> Derivations = new List<DDPicture>();
	}
}
