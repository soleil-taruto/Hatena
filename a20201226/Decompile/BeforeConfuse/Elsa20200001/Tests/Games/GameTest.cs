using System;
using Charlotte.Games;
using Charlotte.Games.Scripts;

namespace Charlotte.Tests.Games
{
	// Token: 0x0200000F RID: 15
	public class GameTest
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002EDC File Offset: 0x000010DC
		public void Test01()
		{
			using (new Game())
			{
				Game.I.Perform();
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002F18 File Offset: 0x00001118
		public void Test02()
		{
			using (new Game())
			{
				Game.I.Script = new Script_Test0001();
				Game.I.Perform();
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002F60 File Offset: 0x00001160
		public void Test03()
		{
			Script script = new Script_HinaTest0001();
			using (new Game())
			{
				Game.I.Script = script;
				Game.I.Perform();
			}
		}
	}
}
