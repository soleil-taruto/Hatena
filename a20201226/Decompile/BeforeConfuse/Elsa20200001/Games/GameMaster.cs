using System;
using Charlotte.Games.Scripts;

namespace Charlotte.Games
{
	// Token: 0x02000015 RID: 21
	public static class GameMaster
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00004C2C File Offset: 0x00002E2C
		public static void Start(int startStageIndex, Player.PlayerWho_e plWho)
		{
			for (;;)
			{
				IL_00:
				GameStatus gameStatus = new GameStatus();
				for (int stageIndex = startStageIndex; stageIndex < GameMaster.Stages.Length; stageIndex++)
				{
					GameMaster.RestartFlag = false;
					GameMaster.ReturnToTitleMenu = false;
					using (new Game())
					{
						Game.I.Script = GameMaster.Stages[stageIndex].CreateScript();
						Game.I.Player.PlayerWho = plWho;
						Game.I.Status = gameStatus;
						Game.I.Perform();
					}
					if (GameMaster.RestartFlag)
					{
						goto IL_00;
					}
					if (GameMaster.ReturnToTitleMenu)
					{
						return;
					}
				}
				return;
			}
		}

		// Token: 0x040000A4 RID: 164
		public static GameMaster.StageInfo[] Stages = new GameMaster.StageInfo[]
		{
			new GameMaster.StageInfo(() => new Script_Stage_01(), "01.通常ステージ"),
			new GameMaster.StageInfo(() => new Script_Stage_02(), "02.敵弾吸収テスト_ステージ")
		};

		// Token: 0x040000A5 RID: 165
		public static bool RestartFlag = false;

		// Token: 0x040000A6 RID: 166
		public static bool ReturnToTitleMenu = false;

		// Token: 0x020000BF RID: 191
		public class StageInfo
		{
			// Token: 0x060003D9 RID: 985 RVA: 0x00014BC0 File Offset: 0x00012DC0
			public StageInfo(Func<Script> f_createScript, string name)
			{
				this.Name = name;
				this.CreateScript = f_createScript;
			}

			// Token: 0x040002CD RID: 717
			public string Name;

			// Token: 0x040002CE RID: 718
			public Func<Script> CreateScript;
		}
	}
}
