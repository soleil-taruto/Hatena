using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.CSSolutions
{
	public class CSSolution
	{
		private string SolutionFile;
		private string SolutionDir;
		private string ProjectName;
		private string ProjectDir;
		private string ProjectFile;
		private string BinDir;
		private string ObjDir;
		private string SuoFile;
		private string OutputExeFile;

		public CSSolution(string solutionFile)
		{
			solutionFile = SCommon.MakeFullPath(solutionFile);

			if (!File.Exists(solutionFile))
				throw new Exception("no solutionFile");

			this.SolutionFile = solutionFile;
			this.SolutionDir = Path.GetDirectoryName(solutionFile);
			this.ProjectName = Path.GetFileNameWithoutExtension(solutionFile);

			if (!Regex.IsMatch(this.ProjectName, "[_0-9A-Za-z]{1,100}"))
				throw new Exception("Bad ProjectName");

			this.ProjectDir = Path.Combine(this.SolutionDir, this.ProjectName);
			this.ProjectFile = Path.Combine(this.ProjectDir, this.ProjectName) + ".csproj";

			if (!File.Exists(this.ProjectFile))
				throw new Exception("no ProjectFile");

			this.BinDir = Path.Combine(this.ProjectDir, "bin");
			this.ObjDir = Path.Combine(this.ProjectDir, "obj");
			this.SuoFile = Path.Combine(this.SolutionDir, this.ProjectName) + ".suo";
			this.OutputExeFile = Path.Combine(this.BinDir, "Release", this.ProjectName) + ".exe";
		}

		/// <summary>
		/// bin ディレクトリを返す。
		/// </summary>
		/// <returns>bin ディレクトリ</returns>
		public string GetBinDir()
		{
			return this.BinDir;
		}

		/// <summary>
		/// 出力 exe ファイルを返す。
		/// </summary>
		/// <returns>出力 exe ファイル</returns>
		public string GetOutputExeFile()
		{
			return this.OutputExeFile;
		}

		/// <summary>
		/// クリーンアップ
		/// </summary>
		public void Clean()
		{
			SCommon.DeletePath(this.BinDir);
			SCommon.DeletePath(this.ObjDir);
			SCommon.DeletePath(this.SuoFile);
		}

		/// <summary>
		/// ビルド
		/// </summary>
		public void Build()
		{
			using (WorkingDir wd = new WorkingDir())
			{
				string[] outLines = SCommon.Batch(new string[]
				{
					@"CALL ""C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsDevCmd.bat""",
					//@"CALL ""C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat""",
					"MSBuild /property:configuration=Release " + Path.GetFileName(this.SolutionFile),
				},
				this.SolutionDir,
				SCommon.StartProcessWindowStyle_e.MINIMIZED
				);

				foreach (string line in outLines)
					Console.WriteLine(line);
			}
		}

		/// <summary>
		/// リビルド
		/// </summary>
		public void Rebuild()
		{
			this.Clean();
			this.Build();
		}

		/// <summary>
		/// 難読化する。
		/// 注意：ソースファイルを書き換える！
		/// </summary>
		public void Confuse(Action a_beforeRemoveUnnecessaryInformations)
		{
			CSFile[] csFiles = Directory.GetFiles(this.ProjectDir, "*.cs", SearchOption.AllDirectories)
				.Where(v => !SCommon.ContainsIgnoreCase(v.Substring(this.ProjectDir.Length), "\\Properties\\"))
				.Select(v => new CSFile(v))
				.ToArray();

			CSRenameVarsFilter rvf = new CSRenameVarsFilter();

			foreach (CSFile csFile in csFiles)
			{
				Console.WriteLine("file: " + csFile.GetFile());

				csFile.SolveNamespace();
				csFile.RemoveComments();
				csFile.RemovePreprocessorDirectives();
				csFile.SolveAccessModifiers();
				csFile.SolveLiteralStrings();
				csFile.OpenClosedEmptyClass();
				csFile.AddDummyMember();
				csFile.RenameEx(rvf.Filter, rvf.Is予約語クラス名);
				csFile.ShuffleMemberOrder();
				//csFile.RemoveUnnecessaryInformations(); // moved
			}

			CSProjectFile projFile = new CSProjectFile(this.ProjectFile);

			projFile.ShuffleCompileOrder();
			//projFile.RenameCompiles(rvf.CreateNameNew); // moved
			//projFile.RemoveUnnecessaryInformations(); // moved

			// ----

			a_beforeRemoveUnnecessaryInformations();

			// ----

			foreach (CSFile csFile in csFiles)
			{
				Console.WriteLine("file.2: " + csFile.GetFile());

				csFile.RemoveUnnecessaryInformations();
			}

			projFile.RenameCompiles(rvf.CreateNameNew);
			projFile.RemoveUnnecessaryInformations();
		}
	}
}
