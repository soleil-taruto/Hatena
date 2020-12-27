using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Enemies;
using Charlotte.Games.Enemies.Rumias;
using Charlotte.Games.Shots;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200003D RID: 61
	public class Script_Stage_02 : Script
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00007E18 File Offset: 0x00006018
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_02.Play(false, false, 1.0, 30);
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			int num;
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 10, 0, 110, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 10, 0, 110, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			for (int c = 0; c < 120; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 30, 1, 111, 2, 4.0, 1, 250.0, 0.97));
			for (int c = 0; c < 60; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 30, 1, 111, 2, 4.0, -1, 350.0, 0.97));
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-50.0, 450.0, 150, 30, 200, 2, 1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-150.0, 450.0, 150, 30, 200, 2, 1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-250.0, 450.0, 150, 30, 200, 2, 1.0));
			for (int c = 0; c < 180; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(562.0, 450.0, 150, 30, 200, 2, -1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(662.0, 400.0, 150, 30, 200, 2, -1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(762.0, 350.0, 300, 30, 210, 2, -1.0));
			for (int c = 0; c < 500; c = num + 1)
			{
				yield return true;
				num = c;
			}
			for (int c = 0; c < 100; c = num + 1)
			{
				double rate = DDUtils.Random.Real();
				Game.I.Enemies.Add(new Enemy_0001B(-30.0, rate * 512.0, 1, 10, 1, 0, 2, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(542.0, rate * 512.0, 1, 10, 2, 0, 2, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(rate * 512.0, -30.0, 1, 10, 3, 0, 3, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(rate * 512.0, 542.0, 1, 10, 4, 0, 4, 1.0));
				for (int d = 0; d < 10; d = num + 1)
				{
					yield return true;
					num = d;
				}
				num = c;
			}
			for (int c = 0; c < 300; c = num + 1)
			{
				yield return true;
				num = c;
			}
			Game.I.Shots.Add(new Shot_BossBomb());
			Enemy_Rumia boss = new Enemy_Rumia();
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
				scenarioFile = "e20200001_res\\掛け合いシナリオ\\小悪魔_ルーミア.txt";
			}
			else
			{
				scenarioFile = "e20200001_res\\掛け合いシナリオ\\メディスン_ルーミア.txt";
			}
			foreach (bool v in ScriptCommon.掛け合い(new Scenario(scenarioFile)))
			{
				yield return v;
			}
			IEnumerator<bool> enumerator = null;
			boss.NextFlag = true;
			boss = null;
			Ground.I.Music.MUS_BOSS_02.Play(false, false, 1.0, 30);
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
			long bonus = 100000000L;
			DDGround.EL.Add(SCommon.Supplier<bool>(Effects.Message("ALL CLEAR BONUS +" + bonus.ToString(), new I3Color(64, 64, 0), new I3Color(255, 255, 0))));
			Game.I.Score += bonus;
			for (int c = 0; c < 300; c = num + 1)
			{
				yield return true;
				num = c;
			}
			yield break;
			yield break;
		}
	}
}
