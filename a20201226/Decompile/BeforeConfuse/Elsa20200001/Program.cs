using System;
using Charlotte.Commons;

namespace Charlotte
{
	// Token: 0x02000006 RID: 6
	internal static class Program
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002297 File Offset: 0x00000497
		[STAThread]
		private static void Main()
		{
			ProcMain.GUIMain(() => new MainWin());
		}
	}
}
