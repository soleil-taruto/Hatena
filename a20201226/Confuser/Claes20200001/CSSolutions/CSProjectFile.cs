using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.CSSolutions
{
	public class CSProjectFile
	{
		private string _file;

		public CSProjectFile(string file)
		{
			_file = file;
		}

		private class SCO_RangeInfo
		{
			public int Start;
			public int End;

			public IEnumerable<int> Sequence()
			{
				return Enumerable.Range(this.Start, this.End - this.Start);
			}
		}

		public void ShuffleCompileOrder()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			SCO_RangeInfo[] ranges = SCO_GetRanges(lines).ToArray();

			SCO_RangeInfo[] shuffledRanges = ranges.ToArray(); // .ToArray() as shallow copy
			SCommon.CRandom.Shuffle(shuffledRanges);

			File.WriteAllLines(_file, SCO_GetOutputLines(lines, ranges, shuffledRanges), Encoding.UTF8);
		}

		private static IEnumerable<SCO_RangeInfo> SCO_GetRanges(string[] lines)
		{
			for (int index = 0; index < lines.Length; index++)
			{
				if (lines[index].Trim().StartsWith("<Compile "))
				{
					if (lines[index].Trim().EndsWith("/>"))
					{
						yield return new SCO_RangeInfo()
						{
							Start = index,
							End = index + 1,
						};
					}
					else
					{
						int end;

						for (end = index + 1; ; end++)
							if (lines[end].Trim() == "</Compile>")
								break;

						yield return new SCO_RangeInfo()
						{
							Start = index,
							End = end + 1,
						};

						index = end;
					}
				}
			}
		}

		private static IEnumerable<string> SCO_GetOutputLines(string[] lines, SCO_RangeInfo[] ranges, SCO_RangeInfo[] shuffledRanges)
		{
			int lineIndex = 0;

			for (int rangeIndex = 0; rangeIndex < ranges.Length; rangeIndex++)
			{
				SCO_RangeInfo range = ranges[rangeIndex];

				while (lineIndex < range.Start)
					yield return lines[lineIndex++];

				foreach (string line in shuffledRanges[rangeIndex].Sequence().Select(v => lines[v]))
					yield return line;

				lineIndex = range.End;
			}
			while (lineIndex < lines.Length)
				yield return lines[lineIndex++];
		}

		private Dictionary<string, string> RC_RenamedPairs = SCommon.CreateDictionaryIgnoreCase<string>();

		/// <summary>
		/// ソースファイル名を変更する。
		/// ソースファイル名は出力実行ファイルに含まれない様なので、本メソッドは実行する必要無いかもしれない。
		/// </summary>
		public void RenameCompiles(Func<string> f_createNameNew)
		{
			this.RC_RenamedPairs.Clear(); // reset

			string projectDir = Path.GetDirectoryName(_file);

			// リネーム
			{
				string homeDir = Directory.GetCurrentDirectory();
				try
				{
					Directory.SetCurrentDirectory(projectDir);

					foreach (string file in RC_GetFiles(projectDir))
					{
						if (!SCommon.EndsWithIgnoreCase(file, ".cs")) // ? *.cs ファイルではない。-> 除外
							continue;

						if (SCommon.EndsWithIgnoreCase(file, ".Designer.cs")) // ? *.Designer.cs ファイル -> 除外
							continue;

						if (SCommon.StartsWithIgnoreCase(file, "Properties\\")) // ? Properties フォルダの配下 -> 除外
							continue;

						if (!File.Exists(file)) // ? 既にリネームした。
							continue;

						string name = file.Substring(0, file.Length - 3);
						string file_designer = name + ".Designer.cs";
						string file_resx = name + ".resx";

						string nameNew = f_createNameNew();
						//string nameNew = RC_CreateNameNew(); // old
						string fileNew = nameNew + ".cs";
						string fileNew_designer = nameNew + ".Designer.cs";
						string fileNew_resx = nameNew + ".resx";

						Action<string, string> a_rename = (pFile, pFileNew) =>
						{
							if (!File.Exists(pFile)) // ? 存在しない。
								return;

							File.Move(pFile, pFileNew);

							Action<string, string> a_addPair = (xFile, xFileNew) =>
							{
								if (!this.RC_RenamedPairs.ContainsKey(xFile))
									this.RC_RenamedPairs.Add(xFile, xFileNew);
							};

							a_addPair(pFile, pFileNew);
							a_addPair(Path.GetFileName(pFile), pFileNew);
						};

						a_rename(file, fileNew);
						a_rename(file_designer, fileNew_designer);
						a_rename(file_resx, fileNew_resx);
					}
				}
				finally
				{
					Directory.SetCurrentDirectory(homeDir);
				}
			}

			this.RC_RenameCompiles("<Compile Include=\"", "\"");
			this.RC_RenameCompiles("<EmbeddedResource Include=\"", "\"");
			this.RC_RenameCompiles("<DependentUpon>", "</DependentUpon>");
		}

		private static IEnumerable<string> RC_GetFiles(string dir)
		{
			dir = SCommon.MakeFullPath(dir);

			foreach (string f_file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
			{
				string file = f_file;

				file = SCommon.MakeFullPath(file);
				file = SCommon.ChangeRoot(file, dir);

				yield return file;
			}
		}

#if false // old
		private static string RC_CreateNameNew()
		{
			// crand 128 bit -> 重複を想定しない。

			return
				"Elsa_" +
				SCommon.CRandom.GetUInt64().ToString("D20") + "_de_" +
				SCommon.CRandom.GetUInt64().ToString("D20") +
				"_Sica";
		}
#endif

		private void RC_RenameCompiles(string startPtn, string endPtn)
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			for (int index = 0; index < lines.Length; index++)
			{
				string line = lines[index];
				string[] parts = Common.ParseEnclosed(line, startPtn, endPtn);

				if (parts != null)
				{
					string file = parts[2];

					if (this.RC_RenamedPairs.ContainsKey(file))
					{
						string fileNew = this.RC_RenamedPairs[file];

						line = parts[0] + parts[1] + fileNew + parts[3] + parts[4];
						lines[index] = line;
					}
				}
			}
			File.WriteAllLines(_file, lines, Encoding.UTF8);
		}

		/// <summary>
		/// ビルドに不要な情報を除去する。
		/// -- 万が一出力実行ファイルに情報が含まれていると困るので、念のため実行する。
		/// ---- 恐らく実行しなくても良い。
		/// </summary>
		public void RemoveUnnecessaryInformations()
		{
			string projectDir = Path.GetDirectoryName(_file);

			foreach (string dir in Directory.GetDirectories(projectDir))
			{
				if (SCommon.EqualsIgnoreCase(Path.GetFileName(dir), "Properties")) // ? Properties フォルダ -> 除外
					continue;

				SCommon.DeletePath(dir);
			}
		}
	}
}
