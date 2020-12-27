using System;

namespace Charlotte.GameCommons
{
	// Token: 0x02000075 RID: 117
	public static class DDInput
	{
		// Token: 0x06000198 RID: 408 RVA: 0x0000BA6C File Offset: 0x00009C6C
		private static void MixInput(DDInput.Button button)
		{
			bool keyDown = button.KeyId != -1 && 1 <= DDKey.GetInput(button.KeyId);
			bool btnDown = button.BtnId != -1 && 1 <= DDPad.GetInput(DDGround.PrimaryPadId, button.BtnId);
			DDUtils.UpdateInput(ref button.Status, keyDown || btnDown);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000BAC8 File Offset: 0x00009CC8
		public static void EachFrame()
		{
			int freezeInputFrame = DDEngine.FreezeInputFrame;
			DDEngine.FreezeInputFrame = 0;
			DDInput.MixInput(DDInput.DIR_2);
			DDInput.MixInput(DDInput.DIR_4);
			DDInput.MixInput(DDInput.DIR_6);
			DDInput.MixInput(DDInput.DIR_8);
			DDInput.MixInput(DDInput.A);
			DDInput.MixInput(DDInput.B);
			DDInput.MixInput(DDInput.C);
			DDInput.MixInput(DDInput.D);
			DDInput.MixInput(DDInput.E);
			DDInput.MixInput(DDInput.F);
			DDInput.MixInput(DDInput.L);
			DDInput.MixInput(DDInput.R);
			DDInput.MixInput(DDInput.PAUSE);
			DDInput.MixInput(DDInput.START);
			DDEngine.FreezeInputFrame = freezeInputFrame;
		}

		// Token: 0x040001A8 RID: 424
		public static DDInput.Button DIR_2 = new DDInput.Button();

		// Token: 0x040001A9 RID: 425
		public static DDInput.Button DIR_4 = new DDInput.Button();

		// Token: 0x040001AA RID: 426
		public static DDInput.Button DIR_6 = new DDInput.Button();

		// Token: 0x040001AB RID: 427
		public static DDInput.Button DIR_8 = new DDInput.Button();

		// Token: 0x040001AC RID: 428
		public static DDInput.Button A = new DDInput.Button();

		// Token: 0x040001AD RID: 429
		public static DDInput.Button B = new DDInput.Button();

		// Token: 0x040001AE RID: 430
		public static DDInput.Button C = new DDInput.Button();

		// Token: 0x040001AF RID: 431
		public static DDInput.Button D = new DDInput.Button();

		// Token: 0x040001B0 RID: 432
		public static DDInput.Button E = new DDInput.Button();

		// Token: 0x040001B1 RID: 433
		public static DDInput.Button F = new DDInput.Button();

		// Token: 0x040001B2 RID: 434
		public static DDInput.Button L = new DDInput.Button();

		// Token: 0x040001B3 RID: 435
		public static DDInput.Button R = new DDInput.Button();

		// Token: 0x040001B4 RID: 436
		public static DDInput.Button PAUSE = new DDInput.Button();

		// Token: 0x040001B5 RID: 437
		public static DDInput.Button START = new DDInput.Button();

		// Token: 0x02000124 RID: 292
		public class Button
		{
			// Token: 0x0600062F RID: 1583 RVA: 0x000214A4 File Offset: 0x0001F6A4
			public int GetInput()
			{
				if (1 > DDEngine.FreezeInputFrame)
				{
					return this.Status;
				}
				return 0;
			}

			// Token: 0x06000630 RID: 1584 RVA: 0x000214B6 File Offset: 0x0001F6B6
			public bool IsPound()
			{
				return DDUtils.IsPound(this.GetInput());
			}

			// Token: 0x06000631 RID: 1585 RVA: 0x000214C3 File Offset: 0x0001F6C3
			public void Backup()
			{
				this.BackupData = new int[]
				{
					this.BtnId,
					this.KeyId
				};
			}

			// Token: 0x06000632 RID: 1586 RVA: 0x000214E4 File Offset: 0x0001F6E4
			public void Restore()
			{
				int c = 0;
				this.BtnId = this.BackupData[c++];
				this.KeyId = this.BackupData[c++];
				this.BackupData = null;
			}

			// Token: 0x040004DB RID: 1243
			public int BtnId = -1;

			// Token: 0x040004DC RID: 1244
			public int KeyId = -1;

			// Token: 0x040004DD RID: 1245
			public int Status;

			// Token: 0x040004DE RID: 1246
			private int[] BackupData;
		}
	}
}
