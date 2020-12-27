using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x02000059 RID: 89
	public class Enemy_Hina_01 : Enemy
	{
		// Token: 0x0600010A RID: 266 RVA: 0x0000976B File Offset: 0x0000796B
		public Enemy_Hina_01(double x, double y) : base(x, y, Enemy.Kind_e.ENEMY, 1000, 0, -1)
		{
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000977D File Offset: 0x0000797D
		protected override IEnumerable<bool> E_Draw()
		{
			double a_mahoujin = 0.0;
			int frame = 0;
			for (;;)
			{
				double rad = (double)frame / 70.0;
				DDUtils.Approach(ref this.X, 256.0 + Math.Sin(rad) * 180.0, 0.93);
				DDUtils.Approach(ref this.Y, 73.0 + Math.Cos(rad) * 50.0, 0.93);
				if (30 < frame && frame % 3 == 0)
				{
					for (int c = 0; c < 3; c++)
					{
						double rad2 = (double)frame / 27.0 + 6.2831853071795862 * (double)c / 3.0;
						D2Point pt = DDUtils.AngleToPoint(rad2, 50.0);
						EnemyCommon.TAMA_COLOR_e color = (new EnemyCommon.TAMA_COLOR_e[]
						{
							EnemyCommon.TAMA_COLOR_e.CYAN,
							EnemyCommon.TAMA_COLOR_e.YELLOW,
							EnemyCommon.TAMA_COLOR_e.PURPLE
						})[c];
						Game.I.Enemies.Add(new Enemy_Hina_Tama_01(this.X + pt.X, this.Y + pt.Y, rad2 + 1.5707963267948966, color));
						Game.I.Enemies.Add(new Enemy_Hina_Tama_01(this.X + pt.X, this.Y + pt.Y, rad2 - 1.5707963267948966, color));
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

		// Token: 0x0600010C RID: 268 RVA: 0x00009790 File Offset: 0x00007990
		public override void Killed()
		{
			Ground.I.SE.SE_ENEMYKILLED.Play(true);
			Game.I.Enemies.Add(new Enemy_Hina_02(this.X, this.Y));
			Game.I.Score += (long)(5000000 * (Game.I.PlayerWasDead ? 1 : 2));
			EnemyCommon.Drawノ\u30FCミス();
			Game.I.PlayerWasDead = false;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}
	}
}
