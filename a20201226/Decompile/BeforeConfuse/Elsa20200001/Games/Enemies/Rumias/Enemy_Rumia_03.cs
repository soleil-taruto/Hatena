using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x02000063 RID: 99
	public class Enemy_Rumia_03 : Enemy
	{
		// Token: 0x06000132 RID: 306 RVA: 0x00009D47 File Offset: 0x00007F47
		public Enemy_Rumia_03(double x, double y) : base(x, y, Enemy.Kind_e.ENEMY, 1000, 0, -1)
		{
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00009D77 File Offset: 0x00007F77
		protected override IEnumerable<bool> E_Draw()
		{
			Game.I.Walls.Add(new Wall_B22001());
			Func<bool> f_updateTarget = SCommon.Supplier<bool>(this.E_UpdateTarget());
			Func<bool> f_attack = SCommon.Supplier<bool>(this.E_Attack());
			int frame = 0;
			for (;;)
			{
				f_updateTarget();
				double apprRate = 1.0 - Math.Min(1.0, (double)frame / 60.0) * 0.05;
				DDUtils.Approach(ref this.X, this.Target_X, apprRate);
				DDUtils.Approach(ref this.Y, this.Target_Y, apprRate);
				if (150 < frame)
				{
					f_attack();
				}
				EnemyCommon_Rumia.PutCrash(this, frame);
				EnemyCommon_Rumia.Draw(this.X, this.Y);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00009D87 File Offset: 0x00007F87
		private IEnumerable<bool> E_UpdateTarget()
		{
			for (;;)
			{
				this.Target_X = (this.RandForUT.Real() * 0.9 + 0.05) * 512.0;
				this.Target_Y = (this.RandForUT.Real() * 0.1 + 0.85) * 512.0;
				int num;
				for (int c = 0; c < 120; c = num + 1)
				{
					yield return true;
					num = c;
				}
			}
			yield break;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00009D97 File Offset: 0x00007F97
		private IEnumerable<bool> E_Attack()
		{
			int waveCount = 0;
			for (;;)
			{
				int num;
				for (int c = 0; c < 30; c = num + 1)
				{
					double angle = DDUtils.GetAngle(new D2Point(Game.I.Player.X, Game.I.Player.Y) - new D2Point(this.X, this.Y));
					num = waveCount;
					waveCount = num + 1;
					double wave = Math.Sin((double)num / 10.0);
					EnemyCommon.TAMA_COLOR_e color = (EnemyCommon.TAMA_COLOR_e)this.RandForColor.GetInt(10);
					int absorbableWeapon = -1;
					if (color == EnemyCommon.TAMA_COLOR_e.BLUE)
					{
						absorbableWeapon = 3;
					}
					Game.I.Enemies.Add(new Enemy_Rumia_Tama_03(this.X, this.Y, angle + wave * 1.2, color, absorbableWeapon));
					for (int w = 0; w < 3; w = num + 1)
					{
						yield return true;
						num = w;
					}
					num = c;
				}
				for (int c = 0; c < 150; c = num + 1)
				{
					yield return true;
					num = c;
				}
			}
			yield break;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00009DA8 File Offset: 0x00007FA8
		public override void Killed()
		{
			Ground.I.SE.SE_ENEMYKILLED.Play(true);
			Game.I.Enemies.Add(new Enemy_Rumia_02_04(this.X, this.Y, false));
			Game.I.Score += (long)(12500000 * (Game.I.PlayerWasDead ? 1 : 2));
			EnemyCommon.Drawノ\u30FCミス();
			Game.I.PlayerWasDead = false;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}

		// Token: 0x0400015B RID: 347
		private DDRandom RandForUT = new DDRandom(1u, 2u, 3u, 4u);

		// Token: 0x0400015C RID: 348
		private DDRandom RandForColor = new DDRandom(5u, 6u, 7u, 8u);

		// Token: 0x0400015D RID: 349
		private double Target_X;

		// Token: 0x0400015E RID: 350
		private double Target_Y;
	}
}
