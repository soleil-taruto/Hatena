using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006A RID: 106
	public static class DDDatStrings
	{
		// Token: 0x06000147 RID: 327 RVA: 0x0000A120 File Offset: 0x00008320
		public static void INIT()
		{
			foreach (string text in SCommon.TextToLines(SCommon.ENCODING_SJIS.GetString(DDResource.Load("e20200001_res\\DatStrings.txt"))))
			{
				int p = text.IndexOf('=');
				if (p == -1)
				{
					throw new DDError();
				}
				string name = text.Substring(0, p);
				string value = text.Substring(p + 1);
				DDDatStrings.Name2Value.Add(name, value);
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000A18B File Offset: 0x0000838B
		private static string GetValue(string name)
		{
			if (!DDDatStrings.Name2Value.ContainsKey(name))
			{
				throw new DDError(name);
			}
			return DDDatStrings.Name2Value[name];
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000149 RID: 329 RVA: 0x0000A1AC File Offset: 0x000083AC
		public static string Title
		{
			get
			{
				return DDDatStrings.GetValue("Title");
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600014A RID: 330 RVA: 0x0000A1B8 File Offset: 0x000083B8
		public static string Author
		{
			get
			{
				return DDDatStrings.GetValue("Author");
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000A1C4 File Offset: 0x000083C4
		public static string Copyright
		{
			get
			{
				return DDDatStrings.GetValue("Copyright");
			}
		}

		// Token: 0x0400017B RID: 379
		private const string DatStringsFile = "e20200001_res\\DatStrings.txt";

		// Token: 0x0400017C RID: 380
		private static Dictionary<string, string> Name2Value = SCommon.CreateDictionary<string>();
	}
}
