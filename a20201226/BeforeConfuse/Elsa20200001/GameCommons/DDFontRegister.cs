using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDFontRegister
	{
		private static WorkingDir WD;

		public static void INIT()
		{
			WD = new WorkingDir();

			DDMain.Finalizers.Add(() =>
			{
				UnloadAll();

				WD.Dispose();
				WD = null;
			});
		}

		public static void Add(string file)
		{
			Add(DDResource.Load(file), Path.GetFileName(file));
		}

		public static void Add(byte[] fileData, string localFile)
		{
			string dir = WD.MakePath();
			string file = Path.Combine(dir, localFile);

			SCommon.CreateDir(dir);
			File.WriteAllBytes(file, fileData);

			if (DDWin32.AddFontResourceEx(file, DDWin32.FR_PRIVATE, IntPtr.Zero) == 0) // ? 失敗
				throw new DDError();

			FontFiles.Add(file);
		}

		private static List<string> FontFiles = new List<string>();

		private static void Unload(string file)
		{
			if (DDWin32.RemoveFontResourceEx(file, DDWin32.FR_PRIVATE, IntPtr.Zero) == 0) // ? 失敗
				throw new DDError();
		}

		private static void UnloadAll()
		{
			foreach (string file in FontFiles)
				Unload(file);
		}
	}
}
