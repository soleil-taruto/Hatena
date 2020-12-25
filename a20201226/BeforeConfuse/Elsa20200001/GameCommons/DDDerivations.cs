using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDDerivations
	{
		public static DDPicture GetPicture(DDPicture picture, int l, int t, int w, int h)
		{
			if (
				l < 0 || SCommon.IMAX < l ||
				t < 0 || SCommon.IMAX < t ||
				w < 1 || SCommon.IMAX - l < w ||
				h < 1 || SCommon.IMAX - t < h
				)
				throw new DDError();

			// ? 範囲外
			if (
				picture.Get_W() < l + w ||
				picture.Get_H() < t + h
				)
				throw new DDError();

			return new DDPicture(
				() =>
				{
					int handle = DX.DerivationGraph(l, t, w, h, picture.GetHandle());

					if (handle == -1) // ? 失敗
						throw new DDError();

					return new DDPicture.PictureInfo()
					{
						Handle = handle,
						W = w,
						H = h,
					};
				},
				DDPictureLoaderUtils.ReleaseInfo, // やる事同じなので共用しちゃう。
				DDDerivationUtils.Add
				);
		}

		public static DDPicture[,] GetAnimation(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return GetAnimation(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		public static DDPicture[,] GetAnimation(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			DDPicture[,] table = new DDPicture[xNum, yNum];

			for (int xc = 0; xc < xNum; xc++)
				for (int yc = 0; yc < yNum; yc++)
					table[xc, yc] = GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);

			return table;
		}

		public static IEnumerable<DDPicture> GetAnimation_YX(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return GetAnimation_YX(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		public static IEnumerable<DDPicture> GetAnimation_YX(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			for (int yc = 0; yc < yNum; yc++)
				for (int xc = 0; xc < xNum; xc++)
					yield return GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);
		}

		public static IEnumerable<DDPicture> GetAnimation_XY(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return GetAnimation_XY(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		public static IEnumerable<DDPicture> GetAnimation_XY(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			for (int xc = 0; xc < xNum; xc++)
				for (int yc = 0; yc < yNum; yc++)
					yield return GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);
		}
	}
}
