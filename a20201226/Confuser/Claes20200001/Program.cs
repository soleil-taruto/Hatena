using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Charlotte.Commons;
using Charlotte.Tests;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			ProcMain.CUIMain(new Program().Main2);
		}

		private void Main2(ArgsReader ar)
		{
			if (ar.ArgIs("//D"))
			{
				// -- choose one --

				//new CheckRemovableCSWordsTest().Perform(); // CSResources.予約語リスト から除去可能な単語を列挙する。
				new Test0001().Test01();
				//new Test0001().Test02();
				//new Test0001().Test03();

				// --
			}
			else
			{
				this.Main3(ar);
			}
		}

		private void Main3(ArgsReader ar)
		{
			string solutionFile = ar.NextArg();
			string workDir = ar.NextArg();

			try
			{
				ElsaConfuser.Perform(solutionFile, workDir);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);

				Console.WriteLine("★★★ 警告 ★★★");
				Console.WriteLine("難読化の処理中に例外を投げました。");
				Console.WriteLine("以下を確認して下さい。");
				Console.WriteLine("-- 予約語リスト");
				Console.WriteLine("★★★");

				Console.WriteLine("エンターキーを押して下さい。");
				Console.ReadLine();
			}
		}
	}
}
