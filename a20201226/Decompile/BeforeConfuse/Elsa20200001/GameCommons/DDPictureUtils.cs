using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x02000083 RID: 131
	public static class DDPictureUtils
	{
		// Token: 0x060001F8 RID: 504 RVA: 0x0000D2B3 File Offset: 0x0000B4B3
		public static void Add(DDPicture picture)
		{
			DDPictureUtils.Pictures.Add(picture);
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000D2C0 File Offset: 0x0000B4C0
		public static void UnloadAll()
		{
			DDDerivationUtils.UnloadAll();
			foreach (DDPicture ddpicture in DDPictureUtils.Pictures)
			{
				ddpicture.Unload();
			}
		}

		// Token: 0x040001D6 RID: 470
		public static List<DDPicture> Pictures = new List<DDPicture>();
	}
}
