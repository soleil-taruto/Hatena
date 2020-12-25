using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games
{
	public static class Effects
	{
		public static IEnumerable<bool> PlayerDead(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(39))
			{
				DDDraw.SetAlpha(0.8);
				DDDraw.DrawBegin(Ground.I.Picture2.D_PLAYERDIE_00[scene.Numer / 4], x, y);
				DDDraw.DrawZoom(1.0 + 2.0 * scene.Rate);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}

		public static IEnumerable<bool> ShotDead(double x, double y, double r)
		{
			double z = r / 8.0;

			foreach (DDScene scene in DDSceneUtils.Create(11))
			{
				DDDraw.SetAlpha(0.4);
				DDDraw.DrawBegin(Ground.I.Picture2.D_BLAST_00[scene.Numer / 3], x, y - scene.Numer * 5.0);
				DDDraw.DrawSlide(8.0, -8.0);
				DDDraw.DrawZoom(z);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}

		public static IEnumerable<bool> EnemyDead(double x, double y)
		{
			double r = DDUtils.Random.Real2() * Math.PI * 2.0;

			foreach (DDScene scene in DDSceneUtils.Create(19))
			{
				DDDraw.SetAlpha(0.7);
				DDDraw.DrawBegin(Ground.I.Picture2.D_ENEMYDIE_00_BGRA[scene.Numer / 2], x, y);
				DDDraw.DrawRotate(r);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}

		public static IEnumerable<bool> TamaDead(double x, double y)
		{
			if (Game.I.BossBattleStarted)
			{
				foreach (bool v in 小爆発(x, y))
					yield return v;
			}
			else
			{
				foreach (DDScene scene in DDSceneUtils.Create(29))
				{
					DDDraw.SetBlendAdd(1.0);
					DDDraw.DrawCenter(Ground.I.Picture2.D_ENEMYSHOTDIE_00[scene.Numer / 3], x, y);
					DDDraw.Reset();

					yield return true;
				}
			}
		}

		public static IEnumerable<bool> 小爆発(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(10))
			{
				DDDraw.SetAlpha(1.0 - scene.Rate);
				DDDraw.SetBright(1.0, 0.5, 1.0);
				DDDraw.DrawBegin(DDGround.GeneralResource.WhiteCircle, x - DDGround.ICamera.X, y - DDGround.ICamera.Y);
				DDDraw.DrawZoom(0.5 + 1.0 * scene.Rate);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}

		public static IEnumerable<bool> Message(string message, I3Color color, I3Color borderColor)
		{
			Action a_draw_message = () =>
			{
				DDPrint.SetPrint(DDConsts.Screen_W / 2 - 4 * message.Length, DDConsts.Screen_H / 2 - 8);
				DDPrint.SetColor(color);
				DDPrint.SetBorder(borderColor);
				DDPrint.Print(message);
				DDPrint.Reset();
			};

			foreach (DDScene scene in DDSceneUtils.Create(20))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0, 0, 0);
				DDDraw.DrawRect_LTRB(
					DDGround.GeneralResource.WhiteBox,
					0,
					DDConsts.Screen_H / 2 - (10 + 40 * scene.Rate),
					DDConsts.Screen_W,
					DDConsts.Screen_H / 2 + (10 + 40 * scene.Rate)
					);
				DDDraw.Reset();

				a_draw_message();

				yield return true;
			}
			foreach (DDScene scene in DDSceneUtils.Create(180))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0, 0, 0);
				DDDraw.DrawRect_LTRB(
					DDGround.GeneralResource.WhiteBox,
					0,
					DDConsts.Screen_H / 2 - 50,
					DDConsts.Screen_W,
					DDConsts.Screen_H / 2 + 50
					);
				DDDraw.Reset();

				a_draw_message();

				yield return true;
			}
			foreach (DDScene scene in DDSceneUtils.Create(20))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0, 0, 0);
				DDDraw.DrawRect_LTRB(
					DDGround.GeneralResource.WhiteBox,
					0,
					DDConsts.Screen_H / 2 - (10 + 40 * (1.0 - scene.Rate)),
					DDConsts.Screen_W,
					DDConsts.Screen_H / 2 + (10 + 40 * (1.0 - scene.Rate))
					);
				DDDraw.Reset();

				a_draw_message();

				yield return true;
			}
		}

		public static IEnumerable<bool> ボス回復(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(5))
			{
				DDDraw.SetAlpha(0.1);
				DDDraw.SetBright(0.0, 1.0, 1.0);
				DDDraw.DrawBegin(DDGround.GeneralResource.WhiteCircle, x - DDGround.ICamera.X, y - DDGround.ICamera.Y);
				DDDraw.DrawZoom(0.5 + 0.5 * (1.0 - scene.Rate));
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}
	}
}
