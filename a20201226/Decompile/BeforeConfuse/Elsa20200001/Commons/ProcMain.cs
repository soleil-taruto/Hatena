using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Charlotte.Commons
{
	// Token: 0x020000AB RID: 171
	public static class ProcMain
	{
		// Token: 0x06000313 RID: 787 RVA: 0x00011A3C File Offset: 0x0000FC3C
		public static void CUIMain(Action<ArgsReader> mainFunc)
		{
			try
			{
				ProcMain.WriteLog = delegate(object message)
				{
					Console.WriteLine("[" + DateTime.Now.ToString() + "] " + ((message != null) ? message.ToString() : null));
				};
				ProcMain.SelfFile = Assembly.GetEntryAssembly().Location;
				ProcMain.SelfDir = Path.GetDirectoryName(ProcMain.SelfFile);
				WorkingDir.Root = WorkingDir.CreateProcessRoot();
				ProcMain.ArgsReader = ProcMain.GetArgsReader();
				mainFunc(ProcMain.ArgsReader);
				WorkingDir.Root.Delete();
				WorkingDir.Root = null;
			}
			catch (Exception e)
			{
				ProcMain.WriteLog(e);
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00011ADC File Offset: 0x0000FCDC
		public static void GUIMain(Func<Form> getMainForm)
		{
			Application.ThreadException += ProcMain.ApplicationThreadException;
			AppDomain.CurrentDomain.UnhandledException += ProcMain.CurrentDomainUnhandledException;
			SystemEvents.SessionEnding += ProcMain.SessionEnding;
			ProcMain.SelfFile = Assembly.GetEntryAssembly().Location;
			ProcMain.SelfDir = Path.GetDirectoryName(ProcMain.SelfFile);
			Mutex procMutex = new Mutex(false, "{6519e425-fd6e-4762-a840-7391b5dd8632}");
			if (procMutex.WaitOne(0))
			{
				if (ProcMain.GlobalProcMtx.Create("{6519e425-fd6e-4762-a840-7391b5dd8632}", "Elsa20200001"))
				{
					ProcMain.CheckSelfFile();
					Directory.SetCurrentDirectory(ProcMain.SelfDir);
					ProcMain.CheckLogonUserAndTmp();
					WorkingDir.Root = WorkingDir.CreateProcessRoot();
					ProcMain.ArgsReader = ProcMain.GetArgsReader();
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(getMainForm());
					WorkingDir.Root.Delete();
					WorkingDir.Root = null;
					ProcMain.GlobalProcMtx.Release();
				}
				procMutex.ReleaseMutex();
			}
			procMutex.Close();
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00011BC4 File Offset: 0x0000FDC4
		private static ArgsReader GetArgsReader()
		{
			return new ArgsReader(Environment.GetCommandLineArgs(), 1);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00011BD4 File Offset: 0x0000FDD4
		private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			try
			{
				string str = "[Application_ThreadException]\n";
				Exception exception = e.Exception;
				MessageBox.Show(str + ((exception != null) ? exception.ToString() : null), "Elsa20200001 / Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch
			{
			}
			Environment.Exit(1);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00011C28 File Offset: 0x0000FE28
		private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				string str = "[CurrentDomain_UnhandledException]\n";
				object exceptionObject = e.ExceptionObject;
				MessageBox.Show(str + ((exceptionObject != null) ? exceptionObject.ToString() : null), "Elsa20200001 / Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			catch
			{
			}
			Environment.Exit(2);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00011C7C File Offset: 0x0000FE7C
		private static void SessionEnding(object sender, SessionEndingEventArgs e)
		{
			Environment.Exit(3);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00011C84 File Offset: 0x0000FE84
		private static void CheckSelfFile()
		{
			string file = ProcMain.SelfFile;
			Encoding SJIS = Encoding.GetEncoding(932);
			if (file != SJIS.GetString(SJIS.GetBytes(file)))
			{
				MessageBox.Show("Shift_JIS に変換出来ない文字を含むパスからは実行できません。", "Elsa20200001 / エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(4);
			}
			if (file.Substring(1, 2) != ":\\")
			{
				MessageBox.Show("ネットワークパスからは実行できません。", "Elsa20200001 / エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(5);
			}
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00011D00 File Offset: 0x0000FF00
		private static void CheckLogonUserAndTmp()
		{
			string userName = Environment.GetEnvironmentVariable("UserName");
			Encoding SJIS = Encoding.GetEncoding(932);
			if (userName == null || userName == "" || userName != SJIS.GetString(SJIS.GetBytes(userName)) || userName.StartsWith(" ") || userName.EndsWith(" "))
			{
				MessageBox.Show("Windows ログオン・ユーザー名に問題があります。", "Elsa20200001 / エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(7);
			}
			string tmp = Environment.GetEnvironmentVariable("TMP");
			if (tmp == null || tmp == "" || tmp != SJIS.GetString(SJIS.GetBytes(tmp)) || tmp.Length < 4 || tmp[1] != ':' || tmp[2] != '\\' || !Directory.Exists(tmp) || tmp.Contains(' '))
			{
				MessageBox.Show("環境変数 TMP に問題があります。", "Elsa20200001 / エラー", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(8);
			}
		}

		// Token: 0x04000243 RID: 579
		public const string APP_IDENT = "{6519e425-fd6e-4762-a840-7391b5dd8632}";

		// Token: 0x04000244 RID: 580
		public const string APP_TITLE = "Elsa20200001";

		// Token: 0x04000245 RID: 581
		public static string SelfFile;

		// Token: 0x04000246 RID: 582
		public static string SelfDir;

		// Token: 0x04000247 RID: 583
		public static ArgsReader ArgsReader;

		// Token: 0x04000248 RID: 584
		public static Action<object> WriteLog = delegate(object message)
		{
		};

		// Token: 0x02000150 RID: 336
		private class GlobalProcMtx
		{
			// Token: 0x060006A3 RID: 1699 RVA: 0x00022370 File Offset: 0x00020570
			public static bool Create(string procMtxName, string title)
			{
				try
				{
					MutexSecurity security = new MutexSecurity();
					security.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow));
					bool createdNew;
					ProcMain.GlobalProcMtx.ProcMtx = new Mutex(false, "Global\\Global_" + procMtxName, ref createdNew, security);
					if (ProcMain.GlobalProcMtx.ProcMtx.WaitOne(0))
					{
						return true;
					}
					ProcMain.GlobalProcMtx.ProcMtx.Close();
					ProcMain.GlobalProcMtx.ProcMtx = null;
				}
				catch (Exception ex)
				{
					MessageBox.Show(((ex != null) ? ex.ToString() : null) ?? "", title + " / Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				ProcMain.GlobalProcMtx.CloseProcMtx();
				MessageBox.Show("Already started on the other logon session !", title + " / Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}

			// Token: 0x060006A4 RID: 1700 RVA: 0x00022434 File Offset: 0x00020634
			public static void Release()
			{
				ProcMain.GlobalProcMtx.CloseProcMtx();
			}

			// Token: 0x060006A5 RID: 1701 RVA: 0x0002243C File Offset: 0x0002063C
			private static void CloseProcMtx()
			{
				try
				{
					ProcMain.GlobalProcMtx.ProcMtx.ReleaseMutex();
				}
				catch
				{
				}
				try
				{
					ProcMain.GlobalProcMtx.ProcMtx.Close();
				}
				catch
				{
				}
				ProcMain.GlobalProcMtx.ProcMtx = null;
			}

			// Token: 0x0400054F RID: 1359
			private static Mutex ProcMtx;
		}
	}
}
