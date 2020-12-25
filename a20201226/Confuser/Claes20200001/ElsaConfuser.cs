using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.CSSolutions;

namespace Charlotte
{
	public static class ElsaConfuser
	{
		public static void Perform(string solutionFile, string workDir)
		{
			solutionFile = SCommon.MakeFullPath(solutionFile);
			workDir = SCommon.MakeFullPath(workDir);

			if (!File.Exists(solutionFile))
				throw new Exception("no solutionFile");

			if (!Directory.Exists(workDir))
				throw new Exception("no workDir");

			string solutionDir = Path.GetDirectoryName(solutionFile);
			string workSolutionDir = Path.Combine(workDir, "tmpsol");
			string workSolutionFile = Path.Combine(workSolutionDir, Path.GetFileName(solutionFile));
			string workSolutionDir_mid = workSolutionDir + "_mid";

			SCommon.DeletePath(workSolutionDir);
			SCommon.CopyDir(solutionDir, workSolutionDir);
			SCommon.DeletePath(workSolutionDir_mid);

			// まかり間違っても masterSol.Confuse() を実行しないように！

			CSSolution sol = new CSSolution(workSolutionFile);

			sol.Clean(); // 難読化前にクリーンアップ必要
			sol.Confuse(() =>
			{
				SCommon.CopyDir(workSolutionDir, workSolutionDir_mid);
			});
			sol.Rebuild();

			CSSolution masterSol = new CSSolution(solutionFile);

			SCommon.DeletePath(masterSol.GetBinDir());

			if (File.Exists(sol.GetOutputExeFile())) // ? ビルド成功
			{
				SCommon.CopyDir(sol.GetBinDir(), masterSol.GetBinDir());
			}
			else // ? ビルド失敗
			{
				SCommon.CreateDir(masterSol.GetBinDir());

				throw new Exception("ビルド失敗");
			}
		}
	}
}
