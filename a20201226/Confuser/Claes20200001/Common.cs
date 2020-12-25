using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte
{
	public static class Common
	{
		/// <summary>
		/// C#の識別子に使用可能な文字か判定する。
		/// </summary>
		/// <param name="chr">判定する文字</param>
		/// <returns>C#の識別子に使用可能な文字か</returns>
		public static bool IsCSWordChar(char chr)
		{
			return
				SCommon.ALPHA.Contains(chr) ||
				SCommon.alpha.Contains(chr) ||
				SCommon.DECIMAL.Contains(chr) ||
				chr == '_' ||
				0x100 <= (uint)chr; // ? 日本語
		}

		public static bool IsHexadecimal(char chr)
		{
			return
				SCommon.HEXADECIMAL.Contains(chr) ||
				SCommon.hexadecimal.Contains(chr);
		}

		public static string[] ParseIsland(string text, string singleTag, bool ignoreCase = false)
		{
			int start;

			if (ignoreCase)
				start = text.ToLower().IndexOf(singleTag.ToLower());
			else
				start = text.IndexOf(singleTag);

			if (start == -1)
				return null;

			int end = start + singleTag.Length;

			return new string[]
			{
				text.Substring(0, start),
				text.Substring(start, end - start),
				text.Substring(end),
			};
		}

		public static string[] ParseEnclosed(string text, string openTag, string closeTag, bool ignoreCase = false)
		{
			string[] starts = ParseIsland(text, openTag, ignoreCase);

			if (starts == null)
				return null;

			string[] ends = ParseIsland(starts[2], closeTag, ignoreCase);

			if (ends == null)
				return null;

			return new string[]
			{
				starts[0],
				starts[1],
				ends[0],
				ends[1],
				ends[2],
			};
		}

		public static int GetIndentDepth(string line)
		{
			int index;

			for (index = 0; index < line.Length; index++)
				if (line[index] != '\t')
					break;

			return index;
		}

		/// <summary>
		/// 新しい識別子を生成する。
		/// 重複を考慮しなくて良いランダムな文字列を返す。
		/// </summary>
		/// <returns>新しい識別子</returns>
		public static string CreateNewIdent()
		{
			// crand 128 bit -> 重複を想定しない。

			return
				"Common_CNI_" +
				SCommon.CRandom.GetUInt64().ToString("D20") + "_" +
				SCommon.CRandom.GetUInt64().ToString("D20") +
				"_zzz";
		}

		/// <summary>
		/// テキストの指定位置(開始位置)が指定パターンと一致するか判定する。
		/// </summary>
		/// <param name="text">テキスト</param>
		/// <param name="startIndex">指定位置(開始位置)</param>
		/// <param name="targPtn">指定パターン</param>
		/// <returns>一致するか</returns>
		public static bool IsMatch(char[] text, int startIndex, char[] targPtn)
		{
			if (text.Length < startIndex + targPtn.Length)
				return false;

			for (int index = 0; index < targPtn.Length; index++)
				if (text[startIndex + index] != targPtn[index])
					return false;

			return true;
		}
	}
}
