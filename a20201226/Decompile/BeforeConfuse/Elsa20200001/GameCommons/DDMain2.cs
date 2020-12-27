using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007A RID: 122
	public static class DDMain2
	{
		// Token: 0x060001BC RID: 444 RVA: 0x0000C448 File Offset: 0x0000A648
		public static void Perform(Action routine)
		{
			List<Exception> errors = new List<Exception>();
			try
			{
				DDMain.GameStart();
				try
				{
					routine();
				}
				catch (DDCoffeeBreak)
				{
				}
				catch (Exception e)
				{
					errors.Add(e);
				}
			}
			catch (Exception e2)
			{
				errors.Add(e2);
			}
			DDMain.GameEnd(errors);
			if (1 <= errors.Count)
			{
				throw new AggregateException("ゲーム中にエラーが発生しました。", errors);
			}
		}
	}
}
