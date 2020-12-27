using System;
using Charlotte.GameCommons;

namespace Charlotte
{
	// Token: 0x02000008 RID: 8
	public class ResourceMusic
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000023B0 File Offset: 0x000005B0
		public ResourceMusic()
		{
			this.MUS_BOSS_01.Volume = 1.0;
		}

		// Token: 0x0400000A RID: 10
		public DDMusic MUS_TITLE = new DDMusic("e20200003_dat\\とぼそ\\nc161701.mp3");

		// Token: 0x0400000B RID: 11
		public DDMusic MUS_GAMEOVER = new DDMusic("e20200003_dat\\hmix\\c26.mp3");

		// Token: 0x0400000C RID: 12
		public DDMusic MUS_STAGE_01 = new DDMusic("e20200003_dat\\hmix\\v8.mp3");

		// Token: 0x0400000D RID: 13
		public DDMusic MUS_BOSS_01 = new DDMusic("e20200003_dat\\Mirror of ES\\nc213704.mp3").SetLoop(30000, 5240000);

		// Token: 0x0400000E RID: 14
		public DDMusic MUS_STAGE_02 = new DDMusic("e20200003_dat\\hmix\\n62.mp3");

		// Token: 0x0400000F RID: 15
		public DDMusic MUS_BOSS_02 = new DDMusic("e20200003_dat\\Reda\\nc136551.mp3").SetLoop(625000, 7365000);
	}
}
