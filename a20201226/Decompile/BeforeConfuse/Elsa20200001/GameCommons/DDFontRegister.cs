using System;
using System.Collections.Generic;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000071 RID: 113
	public static class DDFontRegister
	{
		// Token: 0x06000184 RID: 388 RVA: 0x0000B565 File Offset: 0x00009765
		public static void INIT()
		{
			DDFontRegister.WD = new WorkingDir();
			DDMain.Finalizers.Add(delegate
			{
				DDFontRegister.UnloadAll();
				DDFontRegister.WD.Dispose();
				DDFontRegister.WD = null;
			});
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000B59A File Offset: 0x0000979A
		public static void Add(string file)
		{
			DDFontRegister.Add(DDResource.Load(file), Path.GetFileName(file));
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000B5B0 File Offset: 0x000097B0
		public static void Add(byte[] fileData, string localFile)
		{
			string text = DDFontRegister.WD.MakePath();
			string file = Path.Combine(text, localFile);
			SCommon.CreateDir(text);
			File.WriteAllBytes(file, fileData);
			if (DDWin32.AddFontResourceEx(file, 16u, IntPtr.Zero) == 0)
			{
				throw new DDError();
			}
			DDFontRegister.FontFiles.Add(file);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000B5FB File Offset: 0x000097FB
		private static void Unload(string file)
		{
			if (DDWin32.RemoveFontResourceEx(file, 16u, IntPtr.Zero) == 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000B614 File Offset: 0x00009814
		private static void UnloadAll()
		{
			foreach (string file in DDFontRegister.FontFiles)
			{
				DDFontRegister.Unload(file);
			}
		}

		// Token: 0x0400018F RID: 399
		private static WorkingDir WD;

		// Token: 0x04000190 RID: 400
		private static List<string> FontFiles = new List<string>();
	}
}
