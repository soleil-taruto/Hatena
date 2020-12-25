using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.CSSolutions;

namespace Charlotte.Tests
{
	public class CheckRemovableCSWordsTest
	{
		private const string OUTPUT_FILE = @"C:\temp\置き換え禁止ワードから除去可能なワードリスト.txt";

		/// <summary>
		/// CSResources.予約語リスト から除去可能な単語を OUTPUT_FILE に出力する。
		/// </summary>
		public void Perform()
		{
			SCommon.DeletePath(OUTPUT_FILE);
			File.WriteAllBytes(OUTPUT_FILE, SCommon.EMPTY_BYTES); // 書き込みテスト
			SCommon.DeletePath(OUTPUT_FILE);

			this.TryBuild(); // ビルドのテスト

			string[] csWords = new CSRenameVarsFilter().Get置き換え禁止ワードのリスト();
			List<string> removableCSWords = new List<string>();

			foreach (string csWord in csWords)
			{
				CSRenameVarsFilter.置き換え禁止ワードの例外ワード = csWord;

				try
				{
					this.TryBuild();

					removableCSWords.Add(csWord);
				}
				catch
				{ }

				CSRenameVarsFilter.置き換え禁止ワードの例外ワード = null; // restore

				File.WriteAllLines(OUTPUT_FILE + "_途中経過.txt", removableCSWords, Encoding.UTF8); // test
			}
			File.WriteAllLines(OUTPUT_FILE, removableCSWords, Encoding.UTF8);
		}

		private void TryBuild()
		{
			// -- choose one or more --

			TryBuild_Main("e20200928_NovelAdv");
			TryBuild_Main("e20201003_NovelAdv");
			//TryBuild_Main("e20201008_NovelAdv_Demo");
			TryBuild_Main("e20201010_TateShoot");
			TryBuild_Main("e20201018_TateShoot_Demo");
			TryBuild_Main("e20201020_YokoShoot");
			//TryBuild_Main("e20201027_YokoActTM_Demo");
			TryBuild_Main("e20201109_YokoActTK_Demo");
			TryBuild_Main("e20201115_Dungeon");

			// --
		}

		private void TryBuild_Main(string projectName)
		{
			ElsaConfuser.Perform(@"C:\Dev\Elsa\" + projectName + @"\Elsa20200001\Elsa20200001.sln", @"C:\temp");
		}
	}
}
