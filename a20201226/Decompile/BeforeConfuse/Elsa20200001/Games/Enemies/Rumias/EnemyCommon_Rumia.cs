using System;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x0200005E RID: 94
	public static class EnemyCommon_Rumia
	{
		// Token: 0x0600011D RID: 285 RVA: 0x00009950 File Offset: 0x00007B50
		public static void PutCrash(Enemy enemy, int frame)
		{
			if (frame == 0)
			{
				Game.I.Shots.Add(new Shot_BossBomb());
			}
			else if (frame >= 10)
			{
				if (frame < 150)
				{
					Game.I.Shots.RemoveAll((Shot v) => v.Kind == Shot.Kind_e.BOMB);
					Game.I.PlayerWasDead = false;
				}
				else
				{
					enemy.Crash = DDCrashUtils.Circle(new D2Point(enemy.X, enemy.Y), 25.0);
				}
			}
			EnemyCommon_Rumia.DrawOther(enemy);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000099EC File Offset: 0x00007BEC
		public static void Draw(double x, double y)
		{
			int picIndex;
			double xZoom;
			if (x < EnemyCommon_Rumia.Last_X - 2.0)
			{
				picIndex = 1;
				xZoom = 1.0;
			}
			else if (EnemyCommon_Rumia.Last_X + 2.0 < x)
			{
				picIndex = 1;
				xZoom = -1.0;
			}
			else
			{
				picIndex = 0;
				xZoom = 1.0;
			}
			EnemyCommon_Rumia.Last_X = x;
			DDDraw.DrawBegin(Ground.I.Picture2.ル\u30FCミア[picIndex], x, y);
			DDDraw.DrawZoom_X(xZoom);
			DDDraw.DrawEnd();
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00009A70 File Offset: 0x00007C70
		public static void DrawOther(Enemy enemy)
		{
			DDGround.EL.Add(delegate
			{
				EnemyCommon_Rumia.Draw垂直体力ゲ\u30FCジ((double)enemy.HP / (double)enemy.InitialHP);
				return false;
			});
			EnemyCommon.DrawBossPosition(enemy.X);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00009AB0 File Offset: 0x00007CB0
		private static void Draw垂直体力ゲ\u30FCジ(double hp)
		{
			int rem_h = (int)(500.0 * hp);
			int emp_h = 500 - rem_h;
			if (1 <= emp_h)
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 20.0, 20.0, 20.0, (double)emp_h);
				DDDraw.Reset();
			}
			if (1 <= rem_h)
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(1.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 20.0, (double)(20 + emp_h), 20.0, (double)rem_h);
				DDDraw.Reset();
			}
		}

		// Token: 0x04000154 RID: 340
		private static double Last_X = 256.0;
	}
}
