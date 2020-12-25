using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	public static class DDPad
	{
		public const int PAD_MAX = 16;
		public const int PAD_BUTTON_MAX = 32;

		private static int[] ButtonStatus = new int[PAD_MAX * PAD_BUTTON_MAX]; // [padId * PAD_BUTTON_MAX + btnId]
		private static uint[] PadStatus = new uint[PAD_MAX];

		private static int PadCount = -1; // -1 == 未取得

		public static int GetPadCount()
		{
			if (PadCount == -1)
			{
				PadCount = DX.GetJoypadNum();

				if (PadCount < 0 || PAD_MAX < PadCount)
					throw new DDError();
			}
			return PadCount;
		}

		private static int PadId2InputType(int padId)
		{
			return padId + 1;
		}

		public static void EachFrame()
		{
			for (int padId = 0; padId < GetPadCount(); padId++)
			{
				uint status;

				if (DDEngine.WindowIsActive)
					status = (uint)DX.GetJoypadInputState(PadId2InputType(padId));
				else
					status = 0u;

				if (status != 0u)
				{
					for (int btnId = 0; btnId < PAD_BUTTON_MAX; btnId++)
						DDUtils.UpdateInput(ref ButtonStatus[padId * PAD_BUTTON_MAX + btnId], (status & (1u << btnId)) != 0u);
				}
				else
				{
					for (int btnId = 0; btnId < PAD_BUTTON_MAX; btnId++)
						DDUtils.UpdateInput(ref ButtonStatus[padId * PAD_BUTTON_MAX + btnId], false);
				}

				if (DDGround.PrimaryPadId == -1 && 10 < DDEngine.ProcFrame && PadStatus[padId] == 0u && status != 0u) // 最初にボタンを押下したパッドを PrimaryPadId にセット
					DDGround.PrimaryPadId = padId;

				PadStatus[padId] = status;
			}
		}

		public static int GetInput(int padId, int btnId)
		{
			if (padId == -1) // ? 未割り当て
				padId = 0;

			if (btnId == -1) // ? 割り当てナシ
				return 0;

			return 1 <= DDEngine.FreezeInputFrame ? 0 : ButtonStatus[padId * PAD_BUTTON_MAX + btnId];
		}

		public static bool IsPound(int padId, int btnId)
		{
			return DDUtils.IsPound(GetInput(padId, btnId));
		}
	}
}
