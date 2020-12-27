using System;
using System.Collections.Generic;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006B RID: 107
	public static class DDDerivations
	{
		// Token: 0x0600014D RID: 333 RVA: 0x0000A1DC File Offset: 0x000083DC
		public static DDPicture GetPicture(DDPicture picture, int l, int t, int w, int h)
		{
			if (l < 0 || 1000000000 < l || t < 0 || 1000000000 < t || w < 1 || 1000000000 - l < w || h < 1 || 1000000000 - t < h)
			{
				throw new DDError();
			}
			if (picture.Get_W() < l + w || picture.Get_H() < t + h)
			{
				throw new DDError();
			}
			return new DDPicture(delegate()
			{
				int handle = DX.DerivationGraph(l, t, w, h, picture.GetHandle());
				if (handle == -1)
				{
					throw new DDError();
				}
				return new DDPicture.PictureInfo
				{
					Handle = handle,
					W = w,
					H = h
				};
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDDerivationUtils.Add));
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000A2E4 File Offset: 0x000084E4
		public static DDPicture[,] GetAnimation(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return DDDerivations.GetAnimation(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000A304 File Offset: 0x00008504
		public static DDPicture[,] GetAnimation(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			DDPicture[,] table = new DDPicture[xNum, yNum];
			for (int xc = 0; xc < xNum; xc++)
			{
				for (int yc = 0; yc < yNum; yc++)
				{
					table[xc, yc] = DDDerivations.GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);
				}
			}
			return table;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000A354 File Offset: 0x00008554
		public static IEnumerable<DDPicture> GetAnimation_YX(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return DDDerivations.GetAnimation_YX(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000A374 File Offset: 0x00008574
		public static IEnumerable<DDPicture> GetAnimation_YX(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			int num;
			for (int yc = 0; yc < yNum; yc = num + 1)
			{
				for (int xc = 0; xc < xNum; xc = num + 1)
				{
					yield return DDDerivations.GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);
					num = xc;
				}
				num = yc;
			}
			yield break;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000A3CC File Offset: 0x000085CC
		public static IEnumerable<DDPicture> GetAnimation_XY(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum)
		{
			return DDDerivations.GetAnimation_XY(parentPicture, x, y, w, h, xNum, yNum, w, h);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000A3EC File Offset: 0x000085EC
		public static IEnumerable<DDPicture> GetAnimation_XY(DDPicture parentPicture, int x, int y, int w, int h, int xNum, int yNum, int xStep, int yStep)
		{
			int num;
			for (int xc = 0; xc < xNum; xc = num + 1)
			{
				for (int yc = 0; yc < yNum; yc = num + 1)
				{
					yield return DDDerivations.GetPicture(parentPicture, x + xc * xStep, y + yc * yStep, w, h);
					num = yc;
				}
				num = xc;
			}
			yield break;
		}
	}
}
