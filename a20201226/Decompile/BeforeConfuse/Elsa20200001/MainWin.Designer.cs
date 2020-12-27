namespace Charlotte
{
	// Token: 0x02000005 RID: 5
	public partial class MainWin : global::System.Windows.Forms.Form
	{
		// Token: 0x06000008 RID: 8 RVA: 0x0000214A File Offset: 0x0000034A
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000216C File Offset: 0x0000036C
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::Charlotte.MainWin));
			base.SuspendLayout();
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(8f, 20f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(284, 261);
			this.Font = new global::System.Drawing.Font("メイリオ", 9.75f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 128);
			base.Icon = (global::System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Location = new global::System.Drawing.Point(-400, -400);
			base.Margin = new global::System.Windows.Forms.Padding(4, 5, 4, 5);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MainWin";
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Elsa20200001";
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.MainWin_FormClosing);
			base.FormClosed += new global::System.Windows.Forms.FormClosedEventHandler(this.MainWin_FormClosed);
			base.Load += new global::System.EventHandler(this.MainWin_Load);
			base.Shown += new global::System.EventHandler(this.MainWin_Shown);
			base.ResumeLayout(false);
		}

		// Token: 0x04000009 RID: 9
		private global::System.ComponentModel.IContainer components;
	}
}
