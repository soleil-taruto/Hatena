using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace Charlotte.Commons
{
	public static class ProcMain
	{
		public const string APP_IDENT = "{5d31ad4d-859e-4133-a7b9-00252bdc5cd6}"; // アプリ毎に変更する。
		public const string APP_TITLE = "Claes20200001";

		public static string SelfFile;
		public static string SelfDir;

		public static ArgsReader ArgsReader;

		public static void CUIMain(Action<ArgsReader> mainFunc)
		{
			try
			{
				WriteLog = message => Console.WriteLine("[" + DateTime.Now + "] " + message);

				SelfFile = Assembly.GetEntryAssembly().Location;
				SelfDir = Path.GetDirectoryName(SelfFile);

				WorkingDir.Root = WorkingDir.CreateProcessRoot();

				ArgsReader = GetArgsReader();

				mainFunc(ArgsReader);

				WorkingDir.Root.Delete();
				WorkingDir.Root = null;
			}
			catch (Exception e)
			{
				WriteLog(e);

				Console.WriteLine("Press ENTER key.");
				Console.ReadLine();
			}
		}

		public static void GUIMain(Func<Form> getMainForm)
		{
			Application.ThreadException += new ThreadExceptionEventHandler(ApplicationThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
			SystemEvents.SessionEnding += new SessionEndingEventHandler(SessionEnding);

			//WriteLog = message => { };

			SelfFile = Assembly.GetEntryAssembly().Location;
			SelfDir = Path.GetDirectoryName(SelfFile);

			Mutex procMutex = new Mutex(false, APP_IDENT);

			if (procMutex.WaitOne(0))
			{
				if (GlobalProcMtx.Create(APP_IDENT, APP_TITLE))
				{
					CheckSelfFile();
					Directory.SetCurrentDirectory(SelfDir);
					CheckLogonUserAndTmp();

					WorkingDir.Root = WorkingDir.CreateProcessRoot();

					ArgsReader = GetArgsReader();

					// core >

					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(getMainForm());

					// < core

					WorkingDir.Root.Delete();
					WorkingDir.Root = null;

					GlobalProcMtx.Release();
				}
				procMutex.ReleaseMutex();
			}
			procMutex.Close();
		}

		public static Action<object> WriteLog = message => { };

		private static ArgsReader GetArgsReader()
		{
			return new ArgsReader(Environment.GetCommandLineArgs(), 1);
		}

		private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
		{
			try
			{
				MessageBox.Show(
					"[Application_ThreadException]\n" + e.Exception,
					APP_TITLE + " / Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
			catch
			{ }

			Environment.Exit(1);
		}

		private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				MessageBox.Show(
					"[CurrentDomain_UnhandledException]\n" + e.ExceptionObject,
					APP_TITLE + " / Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
			catch
			{ }

			Environment.Exit(2);
		}

		private static void SessionEnding(object sender, SessionEndingEventArgs e)
		{
			Environment.Exit(3);
		}

		private static void CheckSelfFile()
		{
			string file = SelfFile;
			Encoding SJIS = Encoding.GetEncoding(932);

			if (file != SJIS.GetString(SJIS.GetBytes(file)))
			{
				MessageBox.Show(
					"Shift_JIS に変換出来ない文字を含むパスからは実行できません。",
					APP_TITLE + " / エラー",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				Environment.Exit(4);
			}
			if (file.Substring(1, 2) != ":\\")
			{
				MessageBox.Show(
					"ネットワークパスからは実行できません。",
					APP_TITLE + " / エラー",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				Environment.Exit(5);
			}
		}

		private static void CheckLogonUserAndTmp()
		{
			string userName = Environment.GetEnvironmentVariable("UserName");
			Encoding SJIS = Encoding.GetEncoding(932);

			if (
				userName == null ||
				userName == "" ||
				userName != SJIS.GetString(SJIS.GetBytes(userName)) ||
				userName.StartsWith(" ") ||
				userName.EndsWith(" ")
				)
			{
				MessageBox.Show(
					"Windows ログオン・ユーザー名に問題があります。",
					APP_TITLE + " / エラー",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				Environment.Exit(7);
			}

			string tmp = Environment.GetEnvironmentVariable("TMP");

			if (
				tmp == null ||
				tmp == "" ||
				tmp != SJIS.GetString(SJIS.GetBytes(tmp)) ||
				//tmp.Length < 3 ||
				tmp.Length < 4 || // ルートDIR禁止
				tmp[1] != ':' ||
				tmp[2] != '\\' ||
				!Directory.Exists(tmp) ||
				tmp.Contains(' ')
				)
			{
				MessageBox.Show(
					"環境変数 TMP に問題があります。",
					APP_TITLE + " / エラー",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				Environment.Exit(8);
			}
		}

		private class GlobalProcMtx
		{
			private static Mutex ProcMtx;

			public static bool Create(string procMtxName, string title)
			{
				try
				{
					MutexSecurity security = new MutexSecurity();

					security.AddAccessRule(
						new MutexAccessRule(
							new SecurityIdentifier(
								WellKnownSidType.WorldSid,
								null
								),
							MutexRights.FullControl,
							AccessControlType.Allow
							)
						);

					bool createdNew;
					ProcMtx = new Mutex(false, @"Global\Global_" + procMtxName, out createdNew, security);

					if (ProcMtx.WaitOne(0))
						return true;

					ProcMtx.Close();
					ProcMtx = null;
				}
				catch (Exception e)
				{
					MessageBox.Show("" + e, title + " / Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				CloseProcMtx();

				MessageBox.Show(
					"Already started on the other logon session !",
					title + " / Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				return false;
			}

			public static void Release()
			{
				CloseProcMtx();
			}

			private static void CloseProcMtx()
			{
				try { ProcMtx.ReleaseMutex(); }
				catch { }

				try { ProcMtx.Close(); }
				catch { }

				ProcMtx = null;
			}
		}
	}
}
