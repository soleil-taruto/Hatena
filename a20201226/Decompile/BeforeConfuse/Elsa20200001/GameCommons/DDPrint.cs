using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000084 RID: 132
	public static class DDPrint
	{
		// Token: 0x060001FB RID: 507 RVA: 0x0000D320 File Offset: 0x0000B520
		public static void Reset()
		{
			DDPrint.Extra = new DDPrint.ExtraInfo();
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000D32C File Offset: 0x0000B52C
		public static void SetTaskList(DDTaskList tl)
		{
			DDPrint.Extra.TL = tl;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000D339 File Offset: 0x0000B539
		public static void SetColor(I3Color color)
		{
			DDPrint.Extra.Color = color;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000D346 File Offset: 0x0000B546
		public static void SetBorder(I3Color color, int width = 1)
		{
			DDPrint.Extra.BorderColor = color;
			DDPrint.Extra.BorderWidth = width;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000D35E File Offset: 0x0000B55E
		public static void SetPrint(int x = 0, int y = 0, int yStep = 16)
		{
			DDPrint.P_BaseX = x;
			DDPrint.P_BaseY = y;
			DDPrint.P_YStep = yStep;
			DDPrint.P_X = 0;
			DDPrint.P_Y = 0;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000D37E File Offset: 0x0000B57E
		public static void PrintRet()
		{
			DDPrint.P_X = 0;
			DDPrint.P_Y += DDPrint.P_YStep;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000D398 File Offset: 0x0000B598
		private static void Print_Main(string line, int x, int y)
		{
			if (DDPrint.Extra.BorderWidth != 0)
			{
				for (int xc = -DDPrint.Extra.BorderWidth; xc <= DDPrint.Extra.BorderWidth; xc++)
				{
					for (int yc = -DDPrint.Extra.BorderWidth; yc <= DDPrint.Extra.BorderWidth; yc++)
					{
						DX.DrawString(x + xc, y + yc, line, DDUtils.GetColor(DDPrint.Extra.BorderColor));
					}
				}
			}
			DX.DrawString(x, y, line, DDUtils.GetColor(DDPrint.Extra.Color));
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000D424 File Offset: 0x0000B624
		public static void Print(string line)
		{
			if (line == null)
			{
				throw new DDError();
			}
			int x = DDPrint.P_BaseX + DDPrint.P_X;
			int y = DDPrint.P_BaseY + DDPrint.P_Y;
			if (DDPrint.Extra.TL == null)
			{
				DDPrint.Print_Main(line, x, y);
			}
			else
			{
				DDPrint.ExtraInfo storedExtra = DDPrint.Extra;
				DDPrint.Extra.TL.Add(delegate
				{
					DDPrint.ExtraInfo extra = DDPrint.Extra;
					DDPrint.Extra = storedExtra;
					DDPrint.Print_Main(line, x, y);
					DDPrint.Extra = extra;
					return false;
				});
			}
			int w = DX.GetDrawStringWidth(line, SCommon.ENCODING_SJIS.GetByteCount(line));
			if (w < 0 || 1000000000 < w)
			{
				throw new DDError();
			}
			DDPrint.P_X += w;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000D4F3 File Offset: 0x0000B6F3
		public static void PrintLine(string line)
		{
			DDPrint.Print(line);
			DDPrint.PrintRet();
		}

		// Token: 0x040001D7 RID: 471
		private static DDPrint.ExtraInfo Extra = new DDPrint.ExtraInfo();

		// Token: 0x040001D8 RID: 472
		private static int P_BaseX;

		// Token: 0x040001D9 RID: 473
		private static int P_BaseY;

		// Token: 0x040001DA RID: 474
		private static int P_YStep;

		// Token: 0x040001DB RID: 475
		private static int P_X;

		// Token: 0x040001DC RID: 476
		private static int P_Y;

		// Token: 0x02000136 RID: 310
		private class ExtraInfo
		{
			// Token: 0x04000509 RID: 1289
			public DDTaskList TL;

			// Token: 0x0400050A RID: 1290
			public I3Color Color = new I3Color(255, 255, 255);

			// Token: 0x0400050B RID: 1291
			public I3Color BorderColor = new I3Color(-1, 0, 0);

			// Token: 0x0400050C RID: 1292
			public int BorderWidth;
		}
	}
}
