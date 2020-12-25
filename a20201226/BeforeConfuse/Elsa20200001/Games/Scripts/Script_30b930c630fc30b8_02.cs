using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;
using Charlotte.Games.Enemies.ルーミアs;
using Charlotte.Commons;

namespace Charlotte.Games.Scripts
{
	public class Script_ステージ_02 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_02.Play();

			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 10, 0, 110, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 10, 0, 100, 2, 4.0, 1, 250.0, 0.97));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 10, 0, 110, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));

			for (int c = 0; c < 120; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -100.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -150.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -200.0, 1, 30, 1, 101, 2, 4.0, 1, 250.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(40.0, -250.0, 1, 30, 1, 111, 2, 4.0, 1, 250.0, 0.97));

			for (int c = 0; c < 60; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_0001(472.0, -50.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -100.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -150.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -200.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
			Game.I.Enemies.Add(new Enemy_0001(472.0, -250.0, 1, 30, 1, 111, 2, 4.0, -1, 350.0, 0.97));

			for (int c = 0; c < 180; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-50.0, 450.0, 150, 30, 200, 2, 1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-150.0, 450.0, 150, 30, 200, 2, 1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(-250.0, 450.0, 150, 30, 200, 2, 1.0));

			for (int c = 0; c < 180; c++)
				yield return true;

			Game.I.Enemies.Add(new Enemy_JackOLantern_02(GameConsts.FIELD_W + 50.0, 450.0, 150, 30, 200, 2, -1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(GameConsts.FIELD_W + 150.0, 400.0, 150, 30, 200, 2, -1.0));
			Game.I.Enemies.Add(new Enemy_JackOLantern_02(GameConsts.FIELD_W + 250.0, 350.0, 300, 30, 210, 2, -1.0));

			for (int c = 0; c < 500; c++)
				yield return true;

			for (int c = 0; c < 100; c++)
			{
				double rate = DDUtils.Random.Real();

				Game.I.Enemies.Add(new Enemy_0001B(-30.0,
					rate * GameConsts.FIELD_H, 1, 10, 1, 0, 2, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(GameConsts.FIELD_W + 30.0,
					rate * GameConsts.FIELD_H, 1, 10, 2, 0, 2, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(rate * GameConsts.FIELD_W, -30.0,
					1, 10, 3, 0, 3, 1.0));
				Game.I.Enemies.Add(new Enemy_0001B(rate * GameConsts.FIELD_W, GameConsts.FIELD_H + 30.0,
					1, 10, 4, 0, 4, 1.0));

				for (int d = 0; d < 10; d++)
					yield return true;
			}

			for (int c = 0; c < 300; c++)
				yield return true;

			// ---- BOSS 登場

			{
				Game.I.Shots.Add(new Shot_BossBomb());

				Enemy_ルーミア boss = new Enemy_ルーミア();

				Game.I.Enemies.Add(boss);

				for (int c = 0; c < 90; c++)
					yield return true;

				string scenarioFile;

				switch (Game.I.Player.PlayerWho)
				{
					case Player.PlayerWho_e.メディスン:
						scenarioFile = @"e20200001_res\掛け合いシナリオ\メディスン_ルーミア.txt";
						break;

					case Player.PlayerWho_e.小悪魔:
						scenarioFile = @"e20200001_res\掛け合いシナリオ\小悪魔_ルーミア.txt";
						break;

					default:
						throw null; // never
				}
				foreach (bool v in ScriptCommon.掛け合い(new Scenario(scenarioFile)))
					yield return v;

				boss.NextFlag = true;

				// boss はすぐに消滅することに注意
			}

			Ground.I.Music.MUS_BOSS_02.Play();

			while (!Game.I.BossKilled)
				yield return true;

			for (int c = 0; c < 30; c++)
				yield return true;

			Game.I.Shots.Add(new Shot_BossBomb());

			// ---- BOSS 撃破

			for (int c = 0; c < 180; c++)
				yield return true;

			// All Clear Bonus
			{
				long bonus = 100000000;

				DDGround.EL.Add(SCommon.Supplier(Effects.Message(
					"ALL CLEAR BONUS +" + bonus,
					new I3Color(64, 64, 0),
					new I3Color(255, 255, 0)
					)));

				Game.I.Score += bonus;
			}

			for (int c = 0; c < 300; c++)
				yield return true;
		}
	}
}
