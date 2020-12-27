using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000072 RID: 114
	public static class DDFontUtils
	{
		// Token: 0x0600018A RID: 394 RVA: 0x0000B670 File Offset: 0x00009870
		public static void Add(DDFont font)
		{
			DDFontUtils.Fonts.Add(font);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000B680 File Offset: 0x00009880
		public static void UnloadAll()
		{
			foreach (DDFont ddfont in DDFontUtils.Fonts)
			{
				ddfont.Unload();
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000B6D0 File Offset: 0x000098D0
		public static DDFont GetFont(string fontName, int fontSize, int fontThick = 6, bool antiAliasing = true, int edgeSize = 0, bool italicFlag = false)
		{
			DDFont font = DDFontUtils.Fonts.FirstOrDefault((DDFont v) => v.FontName == fontName && v.FontSize == fontSize && v.FontThick == fontThick && v.AntiAliasing == antiAliasing && v.EdgeSize == edgeSize && v.ItalicFlag == italicFlag);
			if (font == null)
			{
				font = new DDFont(fontName, fontSize, fontThick, antiAliasing, edgeSize, italicFlag);
			}
			return font;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000B754 File Offset: 0x00009954
		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag = false)
		{
			DDFontUtils.DrawString(x, y, str, font, tategakiFlag, new I3Color(255, 255, 255));
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000B778 File Offset: 0x00009978
		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color)
		{
			DDFontUtils.DrawString(x, y, str, font, tategakiFlag, color, new I3Color(0, 0, 0));
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000B79A File Offset: 0x0000999A
		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color, I3Color edgeColor)
		{
			DX.DrawStringToHandle(x, y, str, DDUtils.GetColor(color), font.GetHandle(), DDUtils.GetColor(edgeColor), tategakiFlag ? 1 : 0);
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000B7C1 File Offset: 0x000099C1
		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag = false)
		{
			x -= DDFontUtils.GetDrawStringWidth(str, font, tategakiFlag) / 2;
			DDFontUtils.DrawString(x, y, str, font, tategakiFlag);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000B7DD File Offset: 0x000099DD
		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color)
		{
			x -= DDFontUtils.GetDrawStringWidth(str, font, tategakiFlag) / 2;
			DDFontUtils.DrawString(x, y, str, font, tategakiFlag, color);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000B7FB File Offset: 0x000099FB
		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color, I3Color edgeColor)
		{
			x -= DDFontUtils.GetDrawStringWidth(str, font, tategakiFlag) / 2;
			DDFontUtils.DrawString(x, y, str, font, tategakiFlag, color, edgeColor);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000B81B File Offset: 0x00009A1B
		public static int GetDrawStringWidth(string str, DDFont font, bool tategakiFlag = false)
		{
			return DX.GetDrawStringWidthToHandle(str, SCommon.ENCODING_SJIS.GetByteCount(str), font.GetHandle(), tategakiFlag ? 1 : 0);
		}

		// Token: 0x04000191 RID: 401
		public static List<DDFont> Fonts = new List<DDFont>();
	}
}
