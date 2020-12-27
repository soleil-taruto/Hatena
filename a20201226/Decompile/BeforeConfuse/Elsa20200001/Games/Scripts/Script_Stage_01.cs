using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies;
using Charlotte.Games.Enemies.Hinas;
using Charlotte.Games.Shots;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200004A RID: 74
	public class Script_Stage_01 : Script
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x00007E95 File Offset: 0x00006095
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_01.Play(false, false, 1.0, 30);
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B11001());
			Game.I.Walls.Add(new Wall_B11002());
			int num;
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			for (int c = 0; c < 300; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(55.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 400.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(155.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 350.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(255.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 300.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(355.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 350.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(455.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 400.0, 0.99));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0002(512.0, -100.0, 100, 10, 4, 200, 11, 0.0, 100.0, 1.0, 0.0, 0.98));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0002(0.0, -100.0, 100, 10, 4, 200, 11, 512.0, 200.0, -1.0, 0.0, 0.98));
			for (int c = 0; c < 120; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 6, 0, 1, 80.0, 150.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 7, 0, 1, 80.0, 300.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 6, 0, 1, 80.0, 450.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 7, 0, 1, 400.0, 150.0, 200, 0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 6, 0, 1, 400.0, 300.0, 200, 0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(256.0, -100.0, 30, 30, 7, 0, 1, 400.0, 450.0, 200, 0.01, -0.01));
			for (int c = 0; c < 600; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_JackOLantern(55.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(155.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(255.0, -100.0, 30, 10, 100, 1, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(355.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(455.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(256.0, 0.0, 100, 10, 100, 2, 300.0, 0.99, 4.7, -0.05, 0.0, 1.0));
			for (int c = 0; c < 200; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Walls.Add(new Wall_B12001());
			Game.I.Enemies.Add(new Enemy_0002(256.0, -100.0, 100, 10, 5, 200, 3, 0.0, 100.0, 1.0, 1.0, 0.98));
			Game.I.Enemies.Add(new Enemy_0002(256.0, -100.0, 100, 10, 5, 200, 4, 512.0, 200.0, -1.0, 1.0, 0.98));
			for (int c = 0; c < 300; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Walls.Add(new Wall_B13001());
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(512.0, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 22, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));
			for (int c = 0; c < 500; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(512.0, 0.0, 100, 10, 100, 21, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));
			for (int c = 0; c < 400; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Shots.Add(new Shot_BossBomb());
			Enemy_Hina boss = new Enemy_Hina();
			Game.I.Enemies.Add(boss);
			for (int c = 0; c < 90; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Player.PlayerWho_e playerWho = Game.I.Player.PlayerWho;
			string scenarioFile;
			if (playerWho != Player.PlayerWho_e.メディスン)
			{
				if (playerWho != Player.PlayerWho_e.小悪魔)
				{
					throw null;
				}
				scenarioFile = "e20200001_res\\掛け合いシナリオ\\小悪魔_鍵山雛.txt";
			}
			else
			{
				scenarioFile = "e20200001_res\\掛け合いシナリオ\\メディスン_鍵山雛.txt";
			}
			foreach (bool v in ScriptCommon.掛け合い(new Scenario(scenarioFile)))
			{
				yield return v;
			}
			IEnumerator<bool> enumerator = null;
			boss.NextFlag = true;
			boss = null;
			Ground.I.Music.MUS_BOSS_01.Play(false, false, 1.0, 30);
			while (!Game.I.BossKilled)
			{
				yield return true;
			}
			for (int c = 0; c < 30; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Shots.Add(new Shot_BossBomb());
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			yield break;
			yield break;
		}
	}
}
