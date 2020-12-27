using System;
using Charlotte.GameCommons;

namespace Charlotte
{
	// Token: 0x0200000B RID: 11
	public class ResourceSE
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002DF0 File Offset: 0x00000FF0
		public ResourceSE()
		{
			this.SE_PLAYERSHOT.Volume = 0.1;
			this.SE_ENEMYKILLED.Volume = 0.3;
		}

		// Token: 0x0400006A RID: 106
		public DDSE SE_PLAYERSHOT = new DDSE("e20200003_dat\\Shoot_old_Resource\\beetlepancake\\shot004.wav");

		// Token: 0x0400006B RID: 107
		public DDSE SE_KASURI = new DDSE("e20200003_dat\\Shoot_old_Resource\\beetlepancake\\kasuri001.wav");

		// Token: 0x0400006C RID: 108
		public DDSE SE_ENEMYDAMAGED = new DDSE("e20200003_dat\\Shoot_old_Resource\\beetlepancake\\shot003.wav");

		// Token: 0x0400006D RID: 109
		public DDSE SE_ENEMYKILLED = new DDSE("e20200003_dat\\小森平\\explosion01.mp3");

		// Token: 0x0400006E RID: 110
		public DDSE SE_ITEMGOT = new DDSE("e20200003_dat\\小森平\\powerup03.mp3");
	}
}
