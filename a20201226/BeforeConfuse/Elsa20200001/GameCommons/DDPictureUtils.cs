using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public static class DDPictureUtils
	{
		public static List<DDPicture> Pictures = new List<DDPicture>();

		public static void Add(DDPicture picture)
		{
			Pictures.Add(picture);
		}

		public static void UnloadAll()
		{
			DDDerivationUtils.UnloadAll(); // 先に！

			foreach (DDPicture picture in Pictures)
				picture.Unload();
		}
	}
}
