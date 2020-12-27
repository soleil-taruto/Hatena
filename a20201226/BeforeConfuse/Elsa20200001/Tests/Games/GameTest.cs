using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games;
using Charlotte.Games.Scripts;

namespace Charlotte.Tests.Games
{
	public class GameTest
	{
		public void Test01()
		{
			using (new Game())
			{
				Game.I.Perform();
			}
		}

		public void Test02()
		{
			using (new Game())
			{
				Game.I.Script = new Script_Test0001();
				Game.I.Perform();
			}
		}

		public void Test03()
		{
			Script script;

			// ---- chooese one ----

			//script = new Script_ダミー0001();
			//script = new Script_テスト0001();
			//script = new Script_テスト0002();
			//script = new Script_テスト1001(); // サンプルゲーム用メイン0001
			//script = new Script_テスト2001();
			script = new Script_HinaTest0001();
			//script = new Script_鍵山雛テスト0002();
			//script = new Script_鍵山雛通しテスト0001();
			//script = new Script_ステージ_01();
			//script = new Script_ルーミアテスト_0001();
			//script = new Script_ルーミアテスト_0001小悪魔();
			//script = new Script_ルーミアテスト_0002();
			//script = new Script_ルーミアテスト_0003();
			//script = new Script_ルーミアテスト_0004();

			// ----

			using (new Game())
			{
				Game.I.Script = script;
				//Game.I.Player.PlayerWho = Player.PlayerWho_e.小悪魔; // test
				Game.I.Perform();
			}
		}
	}
}
