using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Security.Permissions;
using Charlotte.Commons;

namespace Charlotte
{
	public partial class MainWin : Form
	{
		#region ALT_F4 抑止

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x112;
			const long SC_CLOSE = 0xF060L;

			if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE)
				return;

			base.WndProc(ref m);
		}

		#endregion

		public MainWin()
		{
			InitializeComponent();
		}

		private void MainWin_Load(object sender, EventArgs e)
		{
			// noop
		}

		public static MainWin I = null;
		public Action PostGameStart_G3 = null;

		private void MainWin_Shown(object sender, EventArgs e)
		{
			{
				string logFile = ProcMain.SelfFile + ".G3_MainWin.log";

				SCommon.DeletePath(logFile);

				ProcMain.WriteLog = message =>
				{
					using (StreamWriter writer = new StreamWriter(logFile, true, Encoding.UTF8))
					{
						writer.WriteLine("[" + DateTime.Now + "] " + message);
					}
				};
			}

			bool aliving = true;

			this.PostGameStart_G3 = () =>
			{
				this.BeginInvoke((MethodInvoker)delegate
				{
					if (aliving)
						this.Visible = false;
				});

				this.PostGameStart_G3 = null;
			};

			Thread th = new Thread(() =>
			{
				I = this;
				new Program2().Main2();
				I = null;

				this.BeginInvoke((MethodInvoker)delegate
				{
					aliving = false;
					this.Close();
				});
			});

			th.Start();
		}

		private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
		{
			// noop
		}

		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
			// noop
		}
	}
}
