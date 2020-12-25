using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games;

namespace Charlotte.Tests.Games
{
	public class TitleMenuTest
	{
		public void Test01()
		{
			using (new TitleMenu())
			{
				TitleMenu.I.Perform();
			}
		}
	}
}
