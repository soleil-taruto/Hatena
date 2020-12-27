using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x02000097 RID: 151
	public static class DDCCResource
	{
		// Token: 0x060002A5 RID: 677 RVA: 0x00010867 File Offset: 0x0000EA67
		public static DDPicture GetPicture(string file)
		{
			if (!DDCCResource.PictureCache.ContainsKey(file))
			{
				DDCCResource.PictureCache.Add(file, DDPictureLoaders.Standard(file));
			}
			return DDCCResource.PictureCache[file];
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00010892 File Offset: 0x0000EA92
		public static DDMusic GetMusic(string file)
		{
			if (!DDCCResource.MusicCache.ContainsKey(file))
			{
				DDCCResource.MusicCache.Add(file, new DDMusic(file));
			}
			return DDCCResource.MusicCache[file];
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000108BD File Offset: 0x0000EABD
		public static DDSE GetSE(string file)
		{
			if (!DDCCResource.SECache.ContainsKey(file))
			{
				DDCCResource.SECache.Add(file, new DDSE(file));
			}
			return DDCCResource.SECache[file];
		}

		// Token: 0x0400020E RID: 526
		private static Dictionary<string, DDPicture> PictureCache = SCommon.CreateDictionaryIgnoreCase<DDPicture>();

		// Token: 0x0400020F RID: 527
		private static Dictionary<string, DDMusic> MusicCache = SCommon.CreateDictionaryIgnoreCase<DDMusic>();

		// Token: 0x04000210 RID: 528
		private static Dictionary<string, DDSE> SECache = SCommon.CreateDictionaryIgnoreCase<DDSE>();
	}
}
