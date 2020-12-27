using System;
using Charlotte.Games;

namespace Charlotte.Tests.Games
{
	// Token: 0x02000010 RID: 16
	public class TitleMenuTest
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002FAC File Offset: 0x000011AC
		public void Test01()
		{
			using (new TitleMenu())
			{
				TitleMenu.I.Perform();
			}
		}
	}
}
