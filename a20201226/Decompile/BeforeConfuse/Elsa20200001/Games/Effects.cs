using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	// Token: 0x02000013 RID: 19
	public static class Effects
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002FE8 File Offset: 0x000011E8
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
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002FFF File Offset: 0x000011FF
		public static IEnumerable<bool> ShotDead(double x, double y, double r)
		{
			double z = r / 8.0;
			foreach (DDScene scene in DDSceneUtils.Create(11))
			{
				DDDraw.SetAlpha(0.4);
				DDDraw.DrawBegin(Ground.I.Picture2.D_BLAST_00[scene.Numer / 3], x, y - (double)scene.Numer * 5.0);
				DDDraw.DrawSlide(8.0, -8.0);
				DDDraw.DrawZoom(z);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000301D File Offset: 0x0000121D
		public static IEnumerable<bool> EnemyDead(double x, double y)
		{
			double r = DDUtils.Random.Real2() * 3.1415926535897931 * 2.0;
			foreach (DDScene scene in DDSceneUtils.Create(19))
			{
				DDDraw.SetAlpha(0.7);
				DDDraw.DrawBegin(Ground.I.Picture2.D_ENEMYDIE_00_BGRA[scene.Numer / 2], x, y);
				DDDraw.DrawRotate(r);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003034 File Offset: 0x00001234
		public static IEnumerable<bool> TamaDead(double x, double y)
		{
			if (Game.I.BossBattleStarted)
			{
				foreach (bool v in Effects.小爆発(x, y))
				{
					yield return v;
				}
				IEnumerator<bool> enumerator = null;
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
				IEnumerator<DDScene> enumerator2 = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x0000304B File Offset: 0x0000124B
		public static IEnumerable<bool> 小爆発(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(10))
			{
				DDDraw.SetAlpha(1.0 - scene.Rate);
				DDDraw.SetBright(1.0, 0.5, 1.0);
				DDDraw.DrawBegin(DDGround.GeneralResource.WhiteCircle, x - (double)DDGround.ICamera.X, y - (double)DDGround.ICamera.Y);
				DDDraw.DrawZoom(0.5 + 1.0 * scene.Rate);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003062 File Offset: 0x00001262
		public static IEnumerable<bool> Message(string message, I3Color color, I3Color borderColor)
		{
			Action a_draw_message = delegate()
			{
				DDPrint.SetPrint(480 - 4 * message.Length, 262, 16);
				DDPrint.SetColor(color);
				DDPrint.SetBorder(borderColor, 1);
				DDPrint.Print(message);
				DDPrint.Reset();
			};
			foreach (DDScene scene in DDSceneUtils.Create(20))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0.0, 0.0, 0.0);
				DDDraw.DrawRect_LTRB(DDGround.GeneralResource.WhiteBox, 0.0, 270.0 - (10.0 + 40.0 * scene.Rate), 960.0, 270.0 + (10.0 + 40.0 * scene.Rate));
				DDDraw.Reset();
				a_draw_message();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			foreach (DDScene ddscene in DDSceneUtils.Create(180))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0.0, 0.0, 0.0);
				DDDraw.DrawRect_LTRB(DDGround.GeneralResource.WhiteBox, 0.0, 220.0, 960.0, 320.0);
				DDDraw.Reset();
				a_draw_message();
				yield return true;
			}
			enumerator = null;
			foreach (DDScene scene2 in DDSceneUtils.Create(20))
			{
				DDDraw.SetAlpha(0.5);
				DDDraw.SetBright(0.0, 0.0, 0.0);
				DDDraw.DrawRect_LTRB(DDGround.GeneralResource.WhiteBox, 0.0, 270.0 - (10.0 + 40.0 * (1.0 - scene2.Rate)), 960.0, 270.0 + (10.0 + 40.0 * (1.0 - scene2.Rate)));
				DDDraw.Reset();
				a_draw_message();
				yield return true;
			}
			enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003080 File Offset: 0x00001280
		public static IEnumerable<bool> ボス回復(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(5))
			{
				DDDraw.SetAlpha(0.1);
				DDDraw.SetBright(0.0, 1.0, 1.0);
				DDDraw.DrawBegin(DDGround.GeneralResource.WhiteCircle, x - (double)DDGround.ICamera.X, y - (double)DDGround.ICamera.Y);
				DDDraw.DrawZoom(0.5 + 0.5 * (1.0 - scene.Rate));
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}
	}
}
