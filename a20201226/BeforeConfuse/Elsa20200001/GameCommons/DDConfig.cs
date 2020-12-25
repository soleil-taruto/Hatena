using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDConfig
	{
		// 設定項目 >

		/// <summary>
		/// -1 == デフォルト, { 0, 1, 2, ... } == { 最初のモニタ, 2番目のモニタ, 3番目のモニタ, ... }
		/// </summary>
		public static int DisplayIndex = 1;

		public static string LogFile = @"C:\tmp\Game.log";
		public static int LogCountMax = SCommon.IMAX;
		public static bool LOG_ENABLED = true;
		public static string ApplicationLogSaveDirectory = @"C:\tmp";

		// 新しい項目をここへ追加...

		// < 設定項目

		public static void Load()
		{
			if (!File.Exists(DDConsts.ConfigFile))
				return;

			string[] lines = File.ReadAllLines(DDConsts.ConfigFile, SCommon.ENCODING_SJIS).Select(line => line.Trim()).Where(line => line != "" && line[0] != ';').ToArray();
			int c = 0;

			if (lines.Length != int.Parse(lines[c++]))
				throw new DDError();

			// 設定項目 >

			DisplayIndex = int.Parse(lines[c++]);
			LogFile = lines[c++];
			LogCountMax = int.Parse(lines[c++]);
			LOG_ENABLED = int.Parse(lines[c++]) != 0;
			ApplicationLogSaveDirectory = lines[c++];

			// 新しい項目をここへ追加...

			// < 設定項目
		}
	}
}
