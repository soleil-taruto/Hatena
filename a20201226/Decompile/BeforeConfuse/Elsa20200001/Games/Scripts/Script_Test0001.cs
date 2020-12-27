using System;
using System.Collections.Generic;
using Charlotte.Games.Enemies;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200003F RID: 63
	public class Script_Test0001 : Script
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00007E32 File Offset: 0x00006032
		protected override IEnumerable<bool> E_EachFrame()
		{
			Ground.I.Music.MUS_STAGE_01.Play(false, false, 1.0, 30);
			Game.I.Walls.Add(new Wall_Dark());
			for (;;)
			{
				Game.I.Walls.Add(new Wall_B21001());
				Game.I.Walls.Add(new Wall_B21002());
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
				Game.I.Enemies.Add(new Enemy_0001(440.0, -50.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -100.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -150.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -200.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -250.0, 1, 10, 0, 100, 2, 4.0, -1, 350.0, 0.97));
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
				Game.I.Enemies.Add(new Enemy_0001(440.0, -50.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -100.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -150.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -200.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -250.0, 1, 30, 1, 101, 2, 4.0, -1, 350.0, 0.97));
				for (int c = 0; c < 300; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_0001(40.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 400.0, 0.99));
				Game.I.Enemies.Add(new Enemy_0001(140.0, -50.0, 10, 10, 2, 100, 1, 2.0, -1, 350.0, 0.99));
				Game.I.Enemies.Add(new Enemy_0001(240.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 300.0, 0.99));
				Game.I.Enemies.Add(new Enemy_0001(340.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 350.0, 0.99));
				Game.I.Enemies.Add(new Enemy_0001(440.0, -50.0, 10, 10, 2, 100, 1, 2.0, 1, 400.0, 0.99));
				for (int c = 0; c < 60; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_0002(480.0, -100.0, 100, 10, 4, 200, 11, 0.0, 100.0, 1.0, 0.0, 0.98));
				for (int c = 0; c < 60; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_0002(0.0, -100.0, 100, 10, 4, 200, 11, 480.0, 200.0, -1.0, 0.0, 0.98));
				for (int c = 0; c < 120; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 6, 0, 1, 80.0, 150.0, 200, -0.01, -0.01));
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 7, 0, 1, 80.0, 300.0, 200, -0.01, -0.01));
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 6, 0, 1, 80.0, 450.0, 200, -0.01, -0.01));
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 7, 0, 1, 400.0, 150.0, 200, 0.01, -0.01));
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 6, 0, 1, 400.0, 300.0, 200, 0.01, -0.01));
				Game.I.Enemies.Add(new Enemy_0003(240.0, -100.0, 30, 30, 7, 0, 1, 400.0, 450.0, 200, 0.01, -0.01));
				for (int c = 0; c < 600; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_JackOLantern(40.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
				Game.I.Enemies.Add(new Enemy_JackOLantern(140.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
				Game.I.Enemies.Add(new Enemy_JackOLantern(240.0, -100.0, 30, 10, 100, 1, 50.0, 2.0, 0.0, 0.1));
				Game.I.Enemies.Add(new Enemy_JackOLantern(340.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
				Game.I.Enemies.Add(new Enemy_JackOLantern(440.0, -100.0, 30, 10, 100, 0, 50.0, 2.0, 0.0, 0.1));
				for (int c = 0; c < 180; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_BigJackOLantern(240.0, 0.0, 100, 10, 100, 2, 300.0, 0.99, 4.7, -0.05, 0.0, 1.0));
				for (int c = 0; c < 200; c = num + 1)
				{
					yield return true;
					num = c;
				}
				for (int c = 0; c < 300; c = num + 1)
				{
					yield return true;
					num = c;
				}
				for (int c = 0; c < 60; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_BigJackOLantern(480.0, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
				Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 11, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));
				for (int c = 0; c < 500; c = num + 1)
				{
					yield return true;
					num = c;
				}
				Game.I.Enemies.Add(new Enemy_BigJackOLantern(480.0, 0.0, 100, 10, 100, 21, 100.0, 0.996, 4.7, -0.03, -0.5, 0.3));
				Game.I.Enemies.Add(new Enemy_BigJackOLantern(0.0, 0.0, 100, 10, 100, 22, 100.0, 0.996, 4.7, 0.03, 0.5, 0.5));
				for (int c = 0; c < 400; c = num + 1)
				{
					yield return true;
					num = c;
				}
			}
			yield break;
		}
	}
}
