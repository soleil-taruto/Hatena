using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008B RID: 139
	public static class DDSEUtils
	{
		// Token: 0x06000229 RID: 553 RVA: 0x0000E7D4 File Offset: 0x0000C9D4
		public static void Add(DDSE se)
		{
			DDSEUtils.SEList.Add(se);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000E7E4 File Offset: 0x0000C9E4
		public static bool EachFrame()
		{
			if (1 <= DDSEUtils.PlayInfos.Count)
			{
				DDSEUtils.PlayInfo info = DDSEUtils.PlayInfos.Dequeue();
				if (info != null)
				{
					switch (info.AlterCommand)
					{
					case DDSEUtils.PlayInfo.AlterCommand_e.NORMAL:
					{
						info.SE.HandleIndex %= 64;
						DDSound sound = info.SE.Sound;
						DDSE se = info.SE;
						int handleIndex = se.HandleIndex;
						se.HandleIndex = handleIndex + 1;
						DDSoundUtils.Play(sound.GetHandle(handleIndex), true, false);
						break;
					}
					case DDSEUtils.PlayInfo.AlterCommand_e.STOP:
						for (int index = 0; index < 64; index++)
						{
							DDSoundUtils.Stop(info.SE.Sound.GetHandle(index));
						}
						break;
					case DDSEUtils.PlayInfo.AlterCommand_e.LOOP:
						DDSoundUtils.Play(info.SE.Sound.GetHandle(0), false, false);
						break;
					default:
						throw new DDError();
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000E8C0 File Offset: 0x0000CAC0
		public static void Play(DDSE se)
		{
			int count = 0;
			foreach (DDSEUtils.PlayInfo info in DDSEUtils.PlayInfos)
			{
				if (info != null && info.SE == se && 2 <= ++count)
				{
					return;
				}
			}
			DDSEUtils.PlayInfos.Enqueue(new DDSEUtils.PlayInfo(se, DDSEUtils.PlayInfo.AlterCommand_e.NORMAL));
			DDSEUtils.PlayInfos.Enqueue(null);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000E940 File Offset: 0x0000CB40
		public static void Stop(DDSE se)
		{
			DDSEUtils.PlayInfos.Enqueue(new DDSEUtils.PlayInfo(se, DDSEUtils.PlayInfo.AlterCommand_e.STOP));
			DDSEUtils.PlayInfos.Enqueue(null);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000E95E File Offset: 0x0000CB5E
		public static void PlayLoop(DDSE se)
		{
			DDSEUtils.PlayInfos.Enqueue(new DDSEUtils.PlayInfo(se, DDSEUtils.PlayInfo.AlterCommand_e.LOOP));
			DDSEUtils.PlayInfos.Enqueue(null);
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000E97C File Offset: 0x0000CB7C
		public static void UpdateVolume()
		{
			foreach (DDSE ddse in DDSEUtils.SEList)
			{
				ddse.UpdateVolume();
			}
		}

		// Token: 0x040001EC RID: 492
		public static List<DDSE> SEList = new List<DDSE>();

		// Token: 0x040001ED RID: 493
		private static Queue<DDSEUtils.PlayInfo> PlayInfos = new Queue<DDSEUtils.PlayInfo>();

		// Token: 0x0200013D RID: 317
		private class PlayInfo
		{
			// Token: 0x06000674 RID: 1652 RVA: 0x00021FEC File Offset: 0x000201EC
			public PlayInfo(DDSE se, DDSEUtils.PlayInfo.AlterCommand_e alterCommand = DDSEUtils.PlayInfo.AlterCommand_e.NORMAL)
			{
				this.SE = se;
				this.AlterCommand = alterCommand;
			}

			// Token: 0x04000524 RID: 1316
			public DDSE SE;

			// Token: 0x04000525 RID: 1317
			public DDSEUtils.PlayInfo.AlterCommand_e AlterCommand;

			// Token: 0x02000166 RID: 358
			public enum AlterCommand_e
			{
				// Token: 0x04000584 RID: 1412
				NORMAL = 1,
				// Token: 0x04000585 RID: 1413
				STOP,
				// Token: 0x04000586 RID: 1414
				LOOP
			}
		}
	}
}
