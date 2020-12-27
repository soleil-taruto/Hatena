using System;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007E RID: 126
	public static class DDPad
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000CB2A File Offset: 0x0000AD2A
		public static int GetPadCount()
		{
			if (DDPad.PadCount == -1)
			{
				DDPad.PadCount = DX.GetJoypadNum();
				if (DDPad.PadCount < 0 || 16 < DDPad.PadCount)
				{
					throw new DDError();
				}
			}
			return DDPad.PadCount;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000CB5A File Offset: 0x0000AD5A
		private static int PadId2InputType(int padId)
		{
			return padId + 1;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000CB60 File Offset: 0x0000AD60
		public static void EachFrame()
		{
			for (int padId = 0; padId < DDPad.GetPadCount(); padId++)
			{
				uint status;
				if (DDEngine.WindowIsActive)
				{
					status = (uint)DX.GetJoypadInputState(DDPad.PadId2InputType(padId));
				}
				else
				{
					status = 0u;
				}
				if (status != 0u)
				{
					for (int btnId = 0; btnId < 32; btnId++)
					{
						DDUtils.UpdateInput(ref DDPad.ButtonStatus[padId * 32 + btnId], (status & 1u << btnId) > 0u);
					}
				}
				else
				{
					for (int btnId2 = 0; btnId2 < 32; btnId2++)
					{
						DDUtils.UpdateInput(ref DDPad.ButtonStatus[padId * 32 + btnId2], false);
					}
				}
				if (DDGround.PrimaryPadId == -1 && 10 < DDEngine.ProcFrame && DDPad.PadStatus[padId] == 0u && status != 0u)
				{
					DDGround.PrimaryPadId = padId;
				}
				DDPad.PadStatus[padId] = status;
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000CC1A File Offset: 0x0000AE1A
		public static int GetInput(int padId, int btnId)
		{
			if (padId == -1)
			{
				padId = 0;
			}
			if (btnId == -1)
			{
				return 0;
			}
			if (1 > DDEngine.FreezeInputFrame)
			{
				return DDPad.ButtonStatus[padId * 32 + btnId];
			}
			return 0;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000CC3F File Offset: 0x0000AE3F
		public static bool IsPound(int padId, int btnId)
		{
			return DDUtils.IsPound(DDPad.GetInput(padId, btnId));
		}

		// Token: 0x040001CE RID: 462
		public const int PAD_MAX = 16;

		// Token: 0x040001CF RID: 463
		public const int PAD_BUTTON_MAX = 32;

		// Token: 0x040001D0 RID: 464
		private static int[] ButtonStatus = new int[512];

		// Token: 0x040001D1 RID: 465
		private static uint[] PadStatus = new uint[16];

		// Token: 0x040001D2 RID: 466
		private static int PadCount = -1;
	}
}
