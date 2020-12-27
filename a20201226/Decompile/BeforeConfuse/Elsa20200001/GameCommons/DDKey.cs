using System;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000077 RID: 119
	public static class DDKey
	{
		// Token: 0x0600019E RID: 414 RVA: 0x0000BC3A File Offset: 0x00009E3A
		public static void INIT()
		{
			DDSystem.Pin<byte[]>(DDKey.StatusMap);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000BC48 File Offset: 0x00009E48
		public static void EachFrame()
		{
			if (!DDEngine.WindowIsActive)
			{
				for (int keyId = 0; keyId < 256; keyId++)
				{
					DDUtils.UpdateInput(ref DDKey.KeyStatus[keyId], false);
				}
				return;
			}
			if (DX.GetHitKeyStateAll(DDKey.StatusMap) != 0)
			{
				throw new DDError();
			}
			for (int keyId2 = 0; keyId2 < 256; keyId2++)
			{
				DDUtils.UpdateInput(ref DDKey.KeyStatus[keyId2], DDKey.StatusMap[keyId2] > 0);
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000BCBA File Offset: 0x00009EBA
		public static int GetInput(int keyId)
		{
			if (1 > DDEngine.FreezeInputFrame)
			{
				return DDKey.KeyStatus[keyId];
			}
			return 0;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000BCCD File Offset: 0x00009ECD
		public static bool IsPound(int keyId)
		{
			return DDUtils.IsPound(DDKey.GetInput(keyId));
		}

		// Token: 0x040001B6 RID: 438
		private const int KEY_MAX = 256;

		// Token: 0x040001B7 RID: 439
		private static int[] KeyStatus = new int[256];

		// Token: 0x040001B8 RID: 440
		private static byte[] StatusMap = new byte[256];
	}
}
