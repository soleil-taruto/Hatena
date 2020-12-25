using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public static class DDMain2
	{
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
				{ }
				catch (Exception e)
				{
					errors.Add(e);
				}
			}
			catch (Exception e)
			{
				errors.Add(e);
			}

			DDMain.GameEnd(errors);

			if (1 <= errors.Count)
				throw new AggregateException("ゲーム中にエラーが発生しました。", errors);
		}
	}
}
