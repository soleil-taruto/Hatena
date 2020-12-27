using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Enemies.Hinas
{
	public static class EnemyCommon_Hina
	{
		public static void Draw(double x, double y, bool spinning, double a_mahoujin = 0.0)
		{
			DDDraw.SetTaskList(Game.I.EL_AfterDrawWalls);
			DDDraw.SetAlpha(0.3 * a_mahoujin);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
			DDDraw.DrawZoom(3.0);
			DDDraw.DrawRotate(DDEngine.ProcFrame / 60.0);
			DDDraw.DrawEnd();
			DDDraw.Reset();

			if (spinning)
			{
				DDDraw.DrawCenter(Ground.I.Picture2.D_HINA_00[(DDEngine.ProcFrame / 5) % 3], x, y);
			}
			else
			{
				DDDraw.DrawCenter(Ground.I.Picture2.D_HINA, x, y);
			}
		}

		/// <summary>
		/// ボスキャラ本体以外の描画
		/// -- HP, 位置
		/// </summary>
		/// <param name="enemy">鍵山雛</param>
		public static void DrawOther(Enemy enemy)
		{
			//DDGround.EL.Add(() =>
			//{
			//    DDPrint.SetPrint(525, 350, 20);
			//    DDPrint.SetBorder(new I3Color(192, 0, 0));
			//    DDPrint.PrintLine("KAGIYAMA-HINA-HP = " + enemy.HP);
			//    DDPrint.Reset();

			//    DDPrint.SetPrint(SCommon.ToInt(GameConsts.FIELD_L + enemy.X - 8 * 3), DDConsts.Screen_H - 16);
			//    DDPrint.SetBorder(new I3Color(255, 0, 0));
			//    DDPrint.Print("<BOSS>");
			//    DDPrint.Reset();

			//    return false;
			//});

			DDGround.EL.Add(() =>
			{
				Draw円形体力ゲージ(
					GameConsts.FIELD_L + enemy.X,
					GameConsts.FIELD_T + enemy.Y,
					(double)enemy.HP / enemy.InitialHP,
					GameConsts.FIELD_L + Game.I.Player.X, // ゲージの位置がスクリーン座標ので、プレイヤーの位置もスクリーン座標にする。
					GameConsts.FIELD_T + Game.I.Player.Y
					);

				return false;
			});

			EnemyCommon.DrawBossPosition(enemy.X);
		}

		private static void Draw円形体力ゲージ(double x, double y, double hp, double plX, double plY)
		{
			const int DENOM = 180;
			const double R1 = 100.0;
			const double R2 = 110.0;
			const double R3 = 150.0;

			for (int numer = 0; numer < DENOM; numer++)
			{
				double rate1 = (double)(numer + 0) / DENOM;
				double rate2 = (double)(numer + 1) / DENOM;

				double ltx = x + Math.Cos((rate1 - 0.25) * Math.PI * 2.0) * R2;
				double rtx = x + Math.Cos((rate2 - 0.25) * Math.PI * 2.0) * R2;
				double rbx = x + Math.Cos((rate2 - 0.25) * Math.PI * 2.0) * R1;
				double lbx = x + Math.Cos((rate1 - 0.25) * Math.PI * 2.0) * R1;
				double lty = y + Math.Sin((rate1 - 0.25) * Math.PI * 2.0) * R2;
				double rty = y + Math.Sin((rate2 - 0.25) * Math.PI * 2.0) * R2;
				double rby = y + Math.Sin((rate2 - 0.25) * Math.PI * 2.0) * R1;
				double lby = y + Math.Sin((rate1 - 0.25) * Math.PI * 2.0) * R1;

				double rate = rate1;
				bool colored = hp < rate && rate < hp * 2 || rate < hp * 2 - 1.0;
				I3Color color = colored ? new I3Color(255, 0, 0) : new I3Color(255, 255, 255);

				bool plNear = DDUtils.GetDistance(new D2Point(x, y), new D2Point(plX, plY)) < R3;
				double a = plNear ? 0.25 : 0.5;

				DDDraw.SetAlpha(a);
				DDDraw.SetBright(color);
				DDDraw.DrawFree(
					DDGround.GeneralResource.WhiteBox,
					ltx, lty,
					rtx, rty,
					rbx, rby,
					lbx, lby
					);
				DDDraw.Reset();
			}
		}
	}
}
