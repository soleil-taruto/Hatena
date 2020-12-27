using System;
using System.IO;
using System.Linq;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000067 RID: 103
	public static class DDConfig
	{
		// Token: 0x06000140 RID: 320 RVA: 0x00009E98 File Offset: 0x00008098
		public static void Load()
		{
			if (!File.Exists("Config.conf"))
			{
				return;
			}
			string[] lines = (from line in File.ReadAllLines("Config.conf", SCommon.ENCODING_SJIS)
			select line.Trim() into line
			where line != "" && line[0] != ';'
			select line).ToArray<string>();
			int c = 0;
			if (lines.Length != int.Parse(lines[c++]))
			{
				throw new DDError();
			}
			DDConfig.DisplayIndex = int.Parse(lines[c++]);
			DDConfig.LogFile = lines[c++];
			DDConfig.LogCountMax = int.Parse(lines[c++]);
			DDConfig.LOG_ENABLED = (int.Parse(lines[c++]) != 0);
			DDConfig.ApplicationLogSaveDirectory = lines[c++];
		}

		// Token: 0x04000162 RID: 354
		public static int DisplayIndex = 1;

		// Token: 0x04000163 RID: 355
		public static string LogFile = "C:\\tmp\\Game.log";

		// Token: 0x04000164 RID: 356
		public static int LogCountMax = 1000000000;

		// Token: 0x04000165 RID: 357
		public static bool LOG_ENABLED = true;

		// Token: 0x04000166 RID: 358
		public static string ApplicationLogSaveDirectory = "C:\\tmp";
	}
}
