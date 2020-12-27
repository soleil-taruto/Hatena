using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;
using Charlotte.Games.Enemies.Hinas;

namespace Charlotte.Games.Scripts
{
	public class Script_Stage_01 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_01.Play();

			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B11001());
			Game.I.Walls.Add(new Wall_B11002());

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -50.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -100.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -150.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -200.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -250.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));

			for (int c = 0; c < 180; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -50.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -100.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -150.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -200.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(GameConsts.FIELD_W - 40.0, -250.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));

			for (int c = 0; c < 300; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(55.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 400.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(155.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 350.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(255.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 300.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(355.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 350.0, 0.99));
			Game.I.Enemies.Add(new Enemy_0001(455.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 400.0, 0.99));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0002(GameConsts.FIELD_W, -100.0, 100, 10, 4, 200, 11, 0.0, 100.0, 1.0, 0.0, 0.98));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0002(0.0, -100.0, 100, 10, 4, 200, 11, GameConsts.FIELD_W, 200.0, -1.0, 0.0, 0.98));

			for (int c = 0; c < 120; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 6, 0, 1, 80.0, 150.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 7, 0, 1, 80.0, 300.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 6, 0, 1, 80.0, 450.0, 200, -0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 7, 0, 1, 400.0, 150.0, 200, 0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 6, 0, 1, 400.0, 300.0, 200, 0.01, -0.01));
			Game.I.Enemies.Add(new Enemy_0003(GameConsts.FIELD_W / 2, -100.0, 30, 30, 7, 0, 1, 400.0, 450.0, 200, 0.01, -0.01));

			for (int c = 0; c < 600; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_JackOLantern(55.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(155.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(255.0, -100.0, 30, 10, 100, 1, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(355.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
			Game.I.Enemies.Add(new Enemy_JackOLantern(455.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));

			for (int c = 0; c < 180; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_BigJackOLantern(GameConsts.FIELD_W / 2, 0.0, 100, 10, 100, 2, 300.0, 0.99, 4.7, -0.05, 0.0, 1.0));

			for (int c = 0; c < 200; c++)
				yield return true;

			Game.I.Walls.Add(new Wall_B12001());

			Game.I.Enemies.Add(new Enemy_0002(GameConsts.FIELD_W / 2, -100.0, 100, 10, 5, 200, 3, 0.0, 100.0, 1.0, 1.0, 0.98));
			Game.I.Enemies.Add(new Enemy_0002(GameConsts.FIELD_W / 2, -100.0, 100, 10, 5, 200, 4, GameConsts.FIELD_W, 200.0, -1.0, 1.0, 0.98));

			for (int c = 0; c < 300; c++)
				yield return true;

			Game.I.Walls.Add(new Wall_B13001());

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_BigJackOLantern(GameConsts.FIELD_W, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 22, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));

			for (int c = 0; c < 500; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_BigJackOLantern(GameConsts.FIELD_W, 0.0, 100, 10, 100, 21, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
			Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));

			for (int c = 0; c < 400; c++)
				yield return true;

			// ---- BOSS 登場

			{
				Game.I.Shots.Add(new Shot_BossBomb());

				Enemy_Hina boss = new Enemy_Hina();

				Game.I.Enemies.Add(boss);

				for (int c = 0; c < 90; c++)
					yield return true;

				string scenarioFile;

				switch (Game.I.Player.PlayerWho)
				{
					case Player.PlayerWho_e.メディスン:
						scenarioFile = @"e20200001_res\掛け合いシナリオ\メディスン_鍵山雛.txt";
						break;

					case Player.PlayerWho_e.小悪魔:
						scenarioFile = @"e20200001_res\掛け合いシナリオ\小悪魔_鍵山雛.txt";
						break;

					default:
						throw null; // never
				}
				foreach (bool v in ScriptCommon.掛け合い(new Scenario(scenarioFile)))
					yield return v;

				boss.NextFlag = true;

				// boss はすぐに消滅することに注意
			}

			Ground.I.Music.MUS_BOSS_01.Play();

			while (!Game.I.BossKilled)
				yield return true;

			for (int c = 0; c < 30; c++)
				yield return true;

			Game.I.Shots.Add(new Shot_BossBomb());

			// ---- BOSS 撃破

			for (int c = 0; c < 180; c++)
				yield return true;
		}
	}
}
