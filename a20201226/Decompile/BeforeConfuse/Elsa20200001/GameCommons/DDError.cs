using System;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006F RID: 111
	public class DDError : Exception
	{
		// Token: 0x0600017F RID: 383 RVA: 0x0000B42C File Offset: 0x0000962C
		public DDError() : this("エラーが発生しました。")
		{
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000B439 File Offset: 0x00009639
		public DDError(string message) : base(message)
		{
		}
	}
}
