using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDFontUtils
	{
		public static List<DDFont> Fonts = new List<DDFont>();

		public static void Add(DDFont font)
		{
			Fonts.Add(font);
		}

		public static void UnloadAll()
		{
			foreach (DDFont font in Fonts)
				font.Unload();
		}

		public static DDFont GetFont(string fontName, int fontSize, int fontThick = 6, bool antiAliasing = true, int edgeSize = 0, bool italicFlag = false)
		{
			DDFont font = Fonts.FirstOrDefault(v =>
				v.FontName == fontName &&
				v.FontSize == fontSize &&
				v.FontThick == fontThick &&
				v.AntiAliasing == antiAliasing &&
				v.EdgeSize == edgeSize &&
				v.ItalicFlag == italicFlag
				);

			if (font == null)
				font = new DDFont(fontName, fontSize, fontThick, antiAliasing, edgeSize, italicFlag);

			return font;
		}

		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag = false)
		{
			DrawString(x, y, str, font, tategakiFlag, new I3Color(255, 255, 255));
		}

		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color)
		{
			DrawString(x, y, str, font, tategakiFlag, color, new I3Color(0, 0, 0));
		}

		public static void DrawString(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color, I3Color edgeColor)
		{
			DX.DrawStringToHandle(x, y, str, DDUtils.GetColor(color), font.GetHandle(), DDUtils.GetColor(edgeColor), tategakiFlag ? 1 : 0);
		}

		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag = false)
		{
			x -= GetDrawStringWidth(str, font, tategakiFlag) / 2;

			DrawString(x, y, str, font, tategakiFlag);
		}

		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color)
		{
			x -= GetDrawStringWidth(str, font, tategakiFlag) / 2;

			DrawString(x, y, str, font, tategakiFlag, color);
		}

		public static void DrawString_XCenter(int x, int y, string str, DDFont font, bool tategakiFlag, I3Color color, I3Color edgeColor)
		{
			x -= GetDrawStringWidth(str, font, tategakiFlag) / 2;

			DrawString(x, y, str, font, tategakiFlag, color, edgeColor);
		}

		public static int GetDrawStringWidth(string str, DDFont font, bool tategakiFlag = false)
		{
			return DX.GetDrawStringWidthToHandle(str, SCommon.ENCODING_SJIS.GetByteCount(str), font.GetHandle(), tategakiFlag ? 1 : 0);
		}
	}
}
