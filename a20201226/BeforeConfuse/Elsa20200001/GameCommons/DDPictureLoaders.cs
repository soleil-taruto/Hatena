using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// <para>ここで取得した DDPicture は Unload する必要あり</para>
	/// <para>必要なし -> DDPictureLoader2</para>
	/// </summary>
	public static class DDPictureLoaders
	{
		public static DDPicture Standard(string file)
		{
			return new DDPicture(
				() => DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file)))),
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture Inverse(string file)
		{
			return new DDPicture(
				() =>
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

							dot.R ^= 0xff;
							dot.G ^= 0xff;
							dot.B ^= 0xff;

							DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
						}
					}
					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture Mirror(string file)
		{
			return new DDPicture(
				() =>
				{
					int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
					int w;
					int h;

					DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);

					{
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
					}

					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture BgTrans(string file)
		{
			return new DDPicture(
				() =>
				{
					int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
					int w;
					int h;

					DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);

					DDPictureLoaderUtils.Dot targetDot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, 0, 0); // 左上隅のピクセル

					for (int x = 0; x < w; x++)
					{
						for (int y = 0; y < h; y++)
						{
							DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);

							if (
								targetDot.R == dot.R &&
								targetDot.G == dot.G &&
								targetDot.B == dot.B
								)
							{
								dot.A = 0;

								DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
							}
						}
					}
					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture SelectARGB(string file, string mode) // mode: "XXXX", X == "ARGB"
		{
			const string s_argb = "ARGB";

			int ia = s_argb.IndexOf(mode[0]);
			int ir = s_argb.IndexOf(mode[1]);
			int ig = s_argb.IndexOf(mode[2]);
			int ib = s_argb.IndexOf(mode[3]);

			return new DDPicture(
				() =>
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
								dot.B,
							}
							.ToList();

							dot.A = argb[ia];
							dot.R = argb[ir];
							dot.G = argb[ig];
							dot.B = argb[ib];

							DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
						}
					}
					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture Mirror(string file, I4Rect derRect)
		{
			return new DDPicture(
				() =>
				{
					int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
					//int w;
					//int h;

					//DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);

					{
						int h2 = DDPictureLoaderUtils.CreateSoftImage(derRect.W * 2, derRect.H);

						for (int x = 0; x < derRect.W; x++)
						{
							for (int y = 0; y < derRect.H; y++)
							{
								DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(
									siHandle,
									derRect.L + x,
									derRect.T + y
									);

								DDPictureLoaderUtils.SetSoftImageDot(
									h2,
									x,
									y,
									dot
									);
								DDPictureLoaderUtils.SetSoftImageDot(
									h2,
									derRect.W * 2 - 1 - x,
									y,
									dot
									);
							}
						}
						DDPictureLoaderUtils.ReleaseSoftImage(siHandle);
						siHandle = h2;
					}

					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture RGBToTrans(string file, I3Color targetColor)
		{
			return new DDPicture(
				() =>
				{
					int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
					int w;
					int h;

					DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);

					//DDPictureLoaderUtils.Dot targetDot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, 0, 0); // 左上隅のピクセル

					for (int x = 0; x < w; x++)
					{
						for (int y = 0; y < h; y++)
						{
							DDPictureLoaderUtils.Dot dot = DDPictureLoaderUtils.GetSoftImageDot(siHandle, x, y);

							if (
								targetColor.R == dot.R &&
								targetColor.G == dot.G &&
								targetColor.B == dot.B
								)
							{
								dot.A = 0;

								DDPictureLoaderUtils.SetSoftImageDot(siHandle, x, y, dot);
							}
						}
					}
					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		public static DDPicture Reduct(string file, int denom)
		{
			return new DDPicture(
				() =>
				{
					int siHandle = DDPictureLoaderUtils.FileData2SoftImage(DDPictureLoaderUtils.File2FileData(file));
					int w;
					int h;

					DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);

					{
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

								{
									DDPictureLoaderUtils.Dot dot = new DDPictureLoaderUtils.Dot()
									{
										R = SCommon.ToInt(tR / div),
										G = SCommon.ToInt(tG / div),
										B = SCommon.ToInt(tB / div),
										A = SCommon.ToInt(tA / div),
									};

									DDPictureLoaderUtils.SetSoftImageDot(new_si_h, x, y, dot);
								}
							}
						}
						DDPictureLoaderUtils.ReleaseSoftImage(siHandle);
						siHandle = new_si_h;
					}

					return DDPictureLoaderUtils.GraphicHandle2Info(DDPictureLoaderUtils.SoftImage2GraphicHandle(siHandle));
				},
				DDPictureLoaderUtils.ReleaseInfo,
				DDPictureUtils.Add
				);
		}

		// 新しい画像ローダーをここへ追加..
	}
}
