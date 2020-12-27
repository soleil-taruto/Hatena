using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x0200005A RID: 90
	public class Enemy_Hina_02 : Enemy
	{
		// Token: 0x0600010E RID: 270 RVA: 0x0000980A File Offset: 0x00007A0A
		public Enemy_Hina_02(double x, double y) : base(x, y, Enemy.Kind_e.ENEMY, 2000, 0, -1)
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000981C File Offset: 0x00007A1C
		protected override IEnumerable<bool> E_Draw()
		{
			double a_mahoujin = 0.0;
			int frame = 0;
			for (;;)
			{
				DDUtils.Approach(ref this.X, 256.0 + Math.Sin((double)frame / 101.0) * 20.0, 0.993);
				DDUtils.Approach(ref this.Y, 256.0 + Math.Sin((double)frame / 103.0) * 20.0, 0.993);
				if (30 < frame && frame % 3 == 0)
				{
					for (int c = 0; c < 2; c++)
					{
						double rad = (double)frame / 27.0 + 6.2831853071795862 * (double)c / 2.0;
						D2Point pt = DDUtils.AngleToPoint(rad, 20.0);
						Game.I.Enemies.Add(new Enemy_Hina_Tama_02(this.X + pt.X, this.Y + pt.Y, rad, 0.9, EnemyCommon.TAMA_COLOR_e.RED));
						Game.I.Enemies.Add(new Enemy_Hina_Tama_02(this.X + pt.X, this.Y + pt.Y, rad, 0.0, EnemyCommon.TAMA_COLOR_e.GREEN));
						Game.I.Enemies.Add(new Enemy_Hina_Tama_02(this.X + pt.X, this.Y + pt.Y, rad, -0.9, EnemyCommon.TAMA_COLOR_e.BLUE));
					}
				}
				if (30 < frame && frame % 20 == 0)
				{
					for (int c2 = 0; c2 < 3; c2++)
					{
						double rad2 = DDUtils.GetAngle(Game.I.Player.X - this.X, Game.I.Player.Y - this.Y) + 6.2831853071795862 * (double)(c2 * 2 + 1) / 6.0;
						DDUtils.AngleToPoint(rad2, 50.0);
						Game.I.Enemies.Add(new Enemy_Hina_Tama_03(this.X, this.Y, rad2, EnemyCommon.TAMA_COLOR_e.PINK));
					}
				}
				if (frame == 60)
				{
					Game.I.Shots.RemoveAll((Shot v) => v.Kind == Shot.Kind_e.BOMB);
					Game.I.PlayerWasDead = false;
				}
				else if (60 < frame)
				{
					DDUtils.Approach(ref a_mahoujin, 1.0, 0.99);
					base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 25.0);
				}
				EnemyCommon_Hina.Draw(this.X, this.Y, true, a_mahoujin);
				EnemyCommon_Hina.DrawOther(this);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000982C File Offset: 0x00007A2C
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
			Game.I.BossKilled = true;
			Game.I.Score += (long)(10000000 * (Game.I.PlayerWasDead ? 1 : 2));
			EnemyCommon.Drawノ\u30FCミス();
			Game.I.PlayerWasDead = false;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}
	}
}
