using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Charlotte.Commons;

namespace Charlotte
{
	// Token: 0x02000005 RID: 5
	public partial class MainWin : Form
	{
		// Token: 0x06000002 RID: 2 RVA: 0x0000207C File Offset: 0x0000027C
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 274 && (m.WParam.ToInt64() & 65520L) == 61536L)
			{
				return;
			}
			base.WndProc(ref m);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020BB File Offset: 0x000002BB
		public MainWin()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020C9 File Offset: 0x000002C9
		private void MainWin_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020CC File Offset: 0x000002CC
		private void MainWin_Shown(object sender, EventArgs e)
		{
			string logFile = ProcMain.SelfFile + ".G3_MainWin.log";
			SCommon.DeletePath(logFile);
			ProcMain.WriteLog = delegate(object message)
			{
				using (StreamWriter writer = new StreamWriter(logFile, true, Encoding.UTF8))
				{
					writer.WriteLine("[" + DateTime.Now.ToString() + "] " + ((message != null) ? message.ToString() : null));
				}
			};
			bool aliving = true;
			MethodInvoker <>9__3;
			this.PostGameStart_G3 = delegate()
			{
				Control <>4__this = this;
				MethodInvoker method;
				if ((method = <>9__3) == null)
				{
					method = (<>9__3 = delegate()
					{
						if (aliving)
						{
							this.Visible = false;
						}
					});
				}
				<>4__this.BeginInvoke(method);
				this.PostGameStart_G3 = null;
			};
			MethodInvoker <>9__4;
			new Thread(delegate()
			{
				MainWin.I = this;
				new Program2().Main2();
				MainWin.I = null;
				Control <>4__this = this;
				MethodInvoker method;
				if ((method = <>9__4) == null)
				{
					method = (<>9__4 = delegate()
					{
						aliving = false;
						this.Close();
					});
				}
				<>4__this.BeginInvoke(method);
			}).Start();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020C9 File Offset: 0x000002C9
		private void MainWin_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020C9 File Offset: 0x000002C9
		private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		// Token: 0x04000007 RID: 7
		public static MainWin I;

		// Token: 0x04000008 RID: 8
		public Action PostGameStart_G3;
	}
}
