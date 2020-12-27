using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000080 RID: 128
	public static class DDPictureLoaders
	{
		// Token: 0x060001E1 RID: 481 RVA: 0x0000CD0A File Offset: 0x0000AF0A
		public static DDPicture Standard(string file)
		{
			return new DDPicture(() => DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file)))), new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000CD40 File Offset: 0x0000AF40
		public static DDPicture Inverse(string file)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);
						dot.R ^= 255;
						dot.G ^= 255;
						dot.B ^= 255;
						DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
					}
				}
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000CD76 File Offset: 0x0000AF76
		public static DDPicture Mirror(string file)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				int h2 = DDPictureLoaderUtils.CreateSoftImage(w * 2, h);
				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);
						DDPictureLoaderUtils.SetSoftImageDot(h2, x, y, dot);
						DDPictureLoaderUtils.SetSoftImageDot(h2, w * 2 - 1 - x, y, dot);
					}
				}
				DDPictureLoaderUtils.ReleaseSoftImage(siHandle);
				siHandle = h2;
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000CDAC File Offset: 0x0000AFAC
		public static DDPicture BgTrans(string file)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				DDPictureLoaderUtils.Dot targetDot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, 0, 0);
				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);
						if (targetDot.R == dot.R && targetDot.G == dot.G && targetDot.B == dot.B)
						{
							dot.A = 0;
							DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
						}
					}
				}
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000CDE4 File Offset: 0x0000AFE4
		public static DDPicture SelectARGB(string file, string mode)
		{
			int ia = "ARGB".IndexOf(mode[0]);
			int ir = "ARGB".IndexOf(mode[1]);
			int ig = "ARGB".IndexOf(mode[2]);
			int ib = "ARGB".IndexOf(mode[3]);
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);
						List<int> argb = new int[]
						{
							dot.A,
							dot.R,
							dot.G,
							dot.B
						}.ToList<int>();
						dot.A = argb[ia];
						dot.R = argb[ir];
						dot.G = argb[ig];
						dot.B = argb[ib];
						DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
					}
				}
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000CE81 File Offset: 0x0000B081
		public static DDPicture Mirror(string file, I4Rect derRect)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int h2 = DDPictureLoaderUtils.CreateSoftImage(derRect.W * 2, derRect.H);
				for (int x = 0; x < derRect.W; x++)
				{
					for (int y = 0; y < derRect.H; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, derRect.L + x, derRect.T + y);
						DDPictureLoaderUtils.SetSoftImageDot(h2, x, y, dot);
						DDPictureLoaderUtils.SetSoftImageDot(h2, derRect.W * 2 - 1 - x, y, dot);
					}
				}
				DDPictureLoaderUtils.ReleaseSoftImage(siHandle);
				siHandle = h2;
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000CEBE File Offset: 0x0000B0BE
		public static DDPicture RGBToTrans(string file, I3Color targetColor)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				for (int x = 0; x < w; x++)
				{
					for (int y = 0; y < h; y++)
					{
						DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);
						if (targetColor.R == dot.R && targetColor.G == dot.G && targetColor.B == dot.B)
						{
							dot.A = 0;
							DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
						}
					}
				}
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000CEFB File Offset: 0x0000B0FB
		public static DDPicture Reduct(string file, int denom)
		{
			return new DDPicture(delegate()
			{
				int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
				int w;
				int h;
				DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
				int new_w = w / denom;
				int new_h = h / denom;
				int new_si_h = DDPictureLoaderUtils.CreateSoftImage(new_w, new_h);
				for (int x = 0; x < new_w; x++)
				{
					for (int y = 0; y < new_h; y++)
					{
						int tR = 0;
						int tG = 0;
						int tB = 0;
						int tA = 0;
						for (int sx = 0; sx < denom; sx++)
						{
							for (int sy = 0; sy < denom; sy++)
							{
								DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x * denom + sx, y * denom + sy);
								tR += dot.R;
								tG += dot.G;
								tB += dot.B;
								tA += dot.A;
							}
						}
						double div = (double)(denom * denom);
						DDPictureLoaderUtils.Dot dot2 = new DDPictureLoaderUtils.Dot
						{
							R = SCommon.ToInt((double)tR / div),
							G = SCommon.ToInt((double)tG / div),
							B = SCommon.ToInt((double)tB / div),
							A = SCommon.ToInt((double)tA / div)
						};
						DDPictureLoaderUtils.SetSoftImageDot(new_si_h, x, y, dot2);
					}
				}
				DDPictureLoaderUtils.ReleaseSoftImage(siHandle);
				siHandle = new_si_h;
				return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
			}, new Action<DDPicture.PictureInfo>(DDPictureLoaderUtils.ReleaseInfo), new Action<DDPicture>(DDPictureUtils.Add));
		}
	}
}
