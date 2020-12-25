using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDUserDatStrings
	{
		// DatStrings.dat

		private static Dictionary<string, string> Name2Value = SCommon.CreateDictionary<string>();

		public static void INIT()
		{
			if (!File.Exists(DDConsts.UserDatStringsFile))
				return;

			IEnumerable<string> lines = File.ReadAllLines(DDConsts.UserDatStringsFile, SCommon.ENCODING_SJIS).Select(line => line.Trim()).Where(line => line != "" && line[0] != ';');

			foreach (string line in lines)
			{
				int p = line.IndexOf('=');

				if (p == -1)
					throw new DDError();

				string name = line.Substring(0, p).Trim();
				string value = line.Substring(p + 1).Trim();

				Name2Value.Add(name, value);
			}
		}

		private static string GetValue(string name, string defval)
		{
			if (!Name2Value.ContainsKey(name))
				return defval;

			return Name2Value[name];
		}

		// Accessor >

		public static string Version
		{
			get { return GetValue("Version", "0.00"); }
		}

		// < Accessor
	}
}
