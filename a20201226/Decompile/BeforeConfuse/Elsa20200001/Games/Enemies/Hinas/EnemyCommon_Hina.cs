using System;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x02000057 RID: 87
	public static class EnemyCommon_Hina
	{
		// Token: 0x06000103 RID: 259 RVA: 0x000093C4 File Offset: 0x000075C4
		public static void Draw(double x, double y, bool spinning, double a_mahoujin = 0.0)
		{
			DDDraw.SetTaskList(Game.I.EL_AfterDrawWalls);
			DDDraw.SetAlpha(0.3 * a_mahoujin);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
			DDDraw.DrawZoom(3.0);
			DDDraw.DrawRotate((double)DDEngine.ProcFrame / 60.0);
			DDDraw.DrawEnd();
			DDDraw.Reset();
			if (spinning)
			{
				DDDraw.DrawCenter(Ground.I.Picture2.D_HINA_00[DDEngine.ProcFrame / 5 % 3], x, y);
				return;
			}
			DDDraw.DrawCenter(Ground.I.Picture2.D_HINA, x, y);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00009470 File Offset: 0x00007670
		public static void DrawOther(Enemy enemy)
		{
			DDGround.EL.Add(delegate
			{
				EnemyCommon_Hina.Draw円形体力ゲ\u30FCジ(224.0 + enemy.X, 14.0 + enemy.Y, (double)enemy.HP / (double)enemy.InitialHP, 224.0 + Game.I.Player.X, 14.0 + Game.I.Player.Y);
				return false;
			});
			EnemyCommon.DrawBossPosition(enemy.X);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000094B0 File Offset: 0x000076B0
		private static void Draw円形体力ゲ\u30FCジ(double x, double y, double hp, double plX, double plY)
		{
			for (int numer = 0; numer < 180; numer++)
			{
				double rate = (double)numer / 180.0;
				double rate2 = (double)(numer + 1) / 180.0;
				double ltx = x + Math.Cos((rate - 0.25) * 3.1415926535897931 * 2.0) * 110.0;
				double rtx = x + Math.Cos((rate2 - 0.25) * 3.1415926535897931 * 2.0) * 110.0;
				double rbx = x + Math.Cos((rate2 - 0.25) * 3.1415926535897931 * 2.0) * 100.0;
				double lbx = x + Math.Cos((rate - 0.25) * 3.1415926535897931 * 2.0) * 100.0;
				double lty = y + Math.Sin((rate - 0.25) * 3.1415926535897931 * 2.0) * 110.0;
				double rty = y + Math.Sin((rate2 - 0.25) * 3.1415926535897931 * 2.0) * 110.0;
				double rby = y + Math.Sin((rate2 - 0.25) * 3.1415926535897931 * 2.0) * 100.0;
				double lby = y + Math.Sin((rate - 0.25) * 3.1415926535897931 * 2.0) * 100.0;
				double rate3 = rate;
				I3Color bright = ((hp < rate3 && rate3 < hp * 2.0) || rate3 < hp * 2.0 - 1.0) ? new I3Color(255, 0, 0) : new I3Color(255, 255, 255);
				DDDraw.SetAlpha((DDUtils.GetDistance(new D2Point(x, y), new D2Point(plX, plY)) < 150.0) ? 0.25 : 0.5);
				DDDraw.SetBright(bright);
				DDDraw.DrawFree(DDGround.GeneralResource.WhiteBox, ltx, lty, rtx, rty, rbx, rby, lbx, lby);
				DDDraw.Reset();
			}
		}
	}
}
