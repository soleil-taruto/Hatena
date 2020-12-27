using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000082 RID: 130
	public static class DDPictureLoaderUtils
	{
		// Token: 0x060001EC RID: 492 RVA: 0x0000CFBE File Offset: 0x0000B1BE
		public static byte[] File2FileData(string file)
		{
			return DDResource.Load(file);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		public static int FileData2SoftImage(byte[] fileData)
		{
			int siHandle = -1;
			DDSystem.PinOn<byte[]>(fileData, delegate(IntPtr p)
			{
				siHandle = DX.LoadSoftImageToMem(p, fileData.Length);
			});
			if (siHandle == -1)
			{
				throw new DDError();
			}
			int w;
			int h;
			DDPictureLoaderUtils.GetSoftImageSize(siHandle, out w, out h);
			int h2 = DX.MakeARGB8ColorSoftImage(w, h);
			if (h2 == -1)
			{
				throw new DDError();
			}
			if (DX.BltSoftImage(0, 0, w, h, siHandle, 0, 0, h2) != 0)
			{
				throw new DDError();
			}
			if (DX.DeleteSoftImage(siHandle) != 0)
			{
				throw new DDError();
			}
			siHandle = h2;
			return siHandle;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000D06A File Offset: 0x0000B26A
		public static int SoftImage2GraphicHandle(int siHandle_binding)
		{
			int num = DX.CreateGraphFromSoftImage(siHandle_binding);
			if (num == -1)
			{
				throw new DDError();
			}
			if (DX.DeleteSoftImage(siHandle_binding) != 0)
			{
				throw new DDError();
			}
			return num;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000D08C File Offset: 0x0000B28C
		public static DDPicture.PictureInfo GraphicHandle2Info(int gHandle_binding)
		{
			int w;
			int h;
			DDPictureLoaderUtils.GetGraphicHandleSize(gHandle_binding, out w, out h);
			return new DDPicture.PictureInfo
			{
				Handle = gHandle_binding,
				W = w,
				H = h
			};
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000D0BD File Offset: 0x0000B2BD
		public static void GetSoftImageSize(int siHandle, out int w, out int h)
		{
			if (DX.GetSoftImageSize(siHandle, out w, out h) != 0)
			{
				throw new DDError();
			}
			if (w < 1 || 1000000000 < w || h < 1 || 1000000000 < h)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000D0F1 File Offset: 0x0000B2F1
		public static void GetGraphicHandleSize(int gHandle, out int w, out int h)
		{
			if (DX.GetGraphSize(gHandle, out w, out h) != 0)
			{
				throw new DDError();
			}
			if (w < 1 || 1000000000 < w || h < 1 || 1000000000 < h)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000D128 File Offset: 0x0000B328
		public static DDPictureLoaderUtils.Dot GetSoftImageDot(int siHandle, int x, int y)
		{
			DDPictureLoaderUtils.Dot dot = new DDPictureLoaderUtils.Dot();
			if (DX.GetPixelSoftImage(siHandle, x, y, out dot.R, out dot.G, out dot.B, out dot.A) != 0)
			{
				throw new DDError();
			}
			if (dot.R < 0 || 255 < dot.R || dot.G < 0 || 255 < dot.G || dot.B < 0 || 255 < dot.B || dot.A < 0 || 255 < dot.A)
			{
				throw new DDError();
			}
			return dot;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000D1C4 File Offset: 0x0000B3C4
		public static void SetSoftImageDot(int siHandle, int x, int y, DDPictureLoaderUtils.Dot dot)
		{
			dot.R = SCommon.ToRange(dot.R, 0, 255);
			dot.G = SCommon.ToRange(dot.G, 0, 255);
			dot.B = SCommon.ToRange(dot.B, 0, 255);
			dot.A = SCommon.ToRange(dot.A, 0, 255);
			if (DX.DrawPixelSoftImage(siHandle, x, y, dot.R, dot.G, dot.B, dot.A) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000D255 File Offset: 0x0000B455
		public static int CreateSoftImage(int w, int h)
		{
			if (w < 1 || 1000000000 < w || h < 1 || 1000000000 < h)
			{
				throw new DDError();
			}
			int num = DX.MakeARGB8ColorSoftImage(w, h);
			if (num == -1)
			{
				throw new DDError();
			}
			return num;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000D286 File Offset: 0x0000B486
		public static void ReleaseSoftImage(int siHandle)
		{
			if (DX.DeleteSoftImage(siHandle) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000D296 File Offset: 0x0000B496
		public static void ReleaseGraphicHandle(int gHandle)
		{
			if (DX.DeleteGraph(gHandle) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000D2A6 File Offset: 0x0000B4A6
		public static void ReleaseInfo(DDPicture.PictureInfo info)
		{
			DDPictureLoaderUtils.ReleaseGraphicHandle(info.Handle);
		}

		// Token: 0x02000134 RID: 308
		public class Dot
		{
			// Token: 0x04000503 RID: 1283
			public int R;

			// Token: 0x04000504 RID: 1284
			public int G;

			// Token: 0x04000505 RID: 1285
			public int B;

			// Token: 0x04000506 RID: 1286
			public int A;
		}
	}
}
