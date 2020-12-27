using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;
using Charlotte.GameCommons.Options;
using Charlotte.Commons;

namespace Charlotte.Games.Enemies.Rumias
{
	public static class EnemyCommon_Rumia
	{
		public static void PutCrash(Enemy enemy, int frame)
		{
			if (frame == 0)
			{
				Game.I.Shots.Add(new Shot_BossBomb());
			}
			else if (frame < EnemyConsts_Rumia.BOSS_BOMB_FRAME)
			{
				// noop
			}
			else if (frame < EnemyConsts_Rumia.TRANS_FRAME)
			{
				Game.I.Shots.RemoveAll(v => v.Kind == Shot.Kind_e.BOMB); // ボム消し
				//Game.I.BombUsed = false; // 念のためリセット
				Game.I.PlayerWasDead = false; // 念のためリセット
			}
			else
			{
				enemy.Crash = DDCrashUtils.Circle(new D2Point(enemy.X, enemy.Y), 25.0);
			}

			// ついでに、ステータス表示
			//{
			//    DDGround.EL.Add(() =>
			//    {
			//        DDPrint.SetPrint(20, 20, 20);
			//        DDPrint.SetBorder(new I3Color(128, 0, 0));
			//        DDPrint.PrintLine("[RUMIA-HP=" + enemy.HP + "]");
			//        //DDPrint.PrintLine("[RUMIA=" + enemy.X.ToString("F1") + "," + enemy.Y.ToString("F1") + "]");
			//        DDPrint.Reset();

			//        return false;
			//    });
			//}
			DrawOther(enemy);
		}

		private static double Last_X = GameConsts.FIELD_W / 2;

		public static void Draw(double x, double y)
		{
			int picIndex;
			double xZoom;

			{
				const double MARGIN = 2.0;

				if (x < Last_X - MARGIN)
				{
					picIndex = 1;
					xZoom = 1.0;
				}
				else if (Last_X + MARGIN < x)
				{
					picIndex = 1;
					xZoom = -1.0;
				}
				else
				{
					picIndex = 0;
					xZoom = 1.0;
				}
			}

			Last_X = x;

			DDDraw.DrawBegin(Ground.I.Picture2.ルーミア[picIndex], x, y);
			DDDraw.DrawZoom_X(xZoom);
			DDDraw.DrawEnd();
		}

		public static void DrawOther(Enemy enemy)
		{
			DDGround.EL.Add(() =>
			{
				Draw垂直体力ゲージ((double)enemy.HP / enemy.InitialHP);
				return false;
			});

			EnemyCommon.DrawBossPosition(enemy.X);
		}

		private static void Draw垂直体力ゲージ(double hp)
		{
			const int L = 20;
			const int T = 20;
			const int W = 20;
			const int H = DDConsts.Screen_H - 40;

			int rem_h = (int)(H * hp);
			int emp_h = H - rem_h;

			const double a = 0.5;

			if (1 <= emp_h)
			{
				DDDraw.SetAlpha(a);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, L, T, W, emp_h);
				DDDraw.Reset();
			}
			if (1 <= rem_h)
			{
				DDDraw.SetAlpha(a);
				DDDraw.SetBright(1.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, L, T + emp_h, W, rem_h);
				DDDraw.Reset();
			}
		}
	}
}
