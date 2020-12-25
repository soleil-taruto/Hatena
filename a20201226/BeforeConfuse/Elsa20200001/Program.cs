using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			ProcMain.GUIMain(() => new MainWin());
		}
	}
}
