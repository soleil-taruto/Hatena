using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000094 RID: 148
	public static class DDUserDatStrings
	{
		// Token: 0x0600026C RID: 620 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
		public static void INIT()
		{
			if (!File.Exists("Properties.dat"))
			{
				return;
			}
			foreach (string text in from line in File.ReadAllLines("Properties.dat", SCommon.ENCODING_SJIS)
			select line.Trim() into line
			where line != "" && line[0] != ';'
			select line)
			{
				int p = text.IndexOf('=');
				if (p == -1)
				{
					throw new DDError();
				}
				string name = text.Substring(0, p).Trim();
				string value = text.Substring(p + 1).Trim();
				DDUserDatStrings.Name2Value.Add(name, value);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
		private static string GetValue(string name, string defval)
		{
			if (!DDUserDatStrings.Name2Value.ContainsKey(name))
			{
				return defval;
			}
			return DDUserDatStrings.Name2Value[name];
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600026E RID: 622 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
		public static string Version
		{
			get
			{
				return DDUserDatStrings.GetValue("Version", "0.00");
			}
		}

		// Token: 0x04000208 RID: 520
		private static Dictionary<string, string> Name2Value = SCommon.CreateDictionary<string>();
	}
}
