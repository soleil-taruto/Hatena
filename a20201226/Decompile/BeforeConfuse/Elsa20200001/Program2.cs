using System;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games;
using Charlotte.Tests.Games;

namespace Charlotte
{
	// Token: 0x02000007 RID: 7
	public class Program2
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000022C0 File Offset: 0x000004C0
		public void Main2()
		{
			try
			{
				this.Main3();
			}
			catch (Exception e)
			{
				ProcMain.WriteLog(e);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000022F4 File Offset: 0x000004F4
		private void Main3()
		{
			DDMain2.Perform(new Action(this.Main4));
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002307 File Offset: 0x00000507
		private void Main4()
		{
			RippleEffect.INIT();
			FieldDivider.INIT();
			FieldDividerEffect.INIT();
			if (ProcMain.ArgsReader.ArgIs("//D"))
			{
				this.Main4_Debug();
				return;
			}
			this.Main4_Release();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002336 File Offset: 0x00000536
		private void Main4_Debug()
		{
			new GameTest().Test03();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002344 File Offset: 0x00000544
		private void Main4_Release()
		{
			using (new Logo())
			{
				Logo.I.Perform();
			}
			using (new TitleMenu())
			{
				TitleMenu.I.Perform();
			}
		}
	}
}
