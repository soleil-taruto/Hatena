using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	public static class DDCCResource
	{
		private static Dictionary<string, DDPicture> PictureCache = SCommon.CreateDictionaryIgnoreCase<DDPicture>();

		public static DDPicture GetPicture(string file)
		{
			if (!PictureCache.ContainsKey(file))
				PictureCache.Add(file, DDPictureLoaders.Standard(file));

			return PictureCache[file];
		}

		private static Dictionary<string, DDMusic> MusicCache = SCommon.CreateDictionaryIgnoreCase<DDMusic>();

		public static DDMusic GetMusic(string file)
		{
			if (!MusicCache.ContainsKey(file))
				MusicCache.Add(file, new DDMusic(file));

			return MusicCache[file];
		}

		private static Dictionary<string, DDSE> SECache = SCommon.CreateDictionaryIgnoreCase<DDSE>();

		public static DDSE GetSE(string file)
		{
			if (!SECache.ContainsKey(file))
				SECache.Add(file, new DDSE(file));

			return SECache[file];
		}
	}
}
