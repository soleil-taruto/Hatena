using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	public static class DDKey
	{
		private const int KEY_MAX = 256;

		private static int[] KeyStatus = new int[KEY_MAX];
		private static byte[] StatusMap = new byte[KEY_MAX];

		public static void INIT()
		{
			DDSystem.Pin(StatusMap);
		}

		public static void EachFrame()
		{
			if (DDEngine.WindowIsActive)
			{
				if (DX.GetHitKeyStateAll(StatusMap) != 0) // ? 失敗
					throw new DDError();

				for (int keyId = 0; keyId < 256; keyId++)
					DDUtils.UpdateInput(ref KeyStatus[keyId], StatusMap[keyId] != 0);
			}
			else
			{
				for (int keyId = 0; keyId < 256; keyId++)
					DDUtils.UpdateInput(ref KeyStatus[keyId], false);
			}
		}

		public static int GetInput(int keyId)
		{
			// keyId == DX.KEY_INPUT_RETURN etc.

			return 1 <= DDEngine.FreezeInputFrame ? 0 : KeyStatus[keyId];
		}

		public static bool IsPound(int keyId)
		{
			return DDUtils.IsPound(GetInput(keyId));
		}
	}
}
