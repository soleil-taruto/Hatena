using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x02000061 RID: 97
	public class Enemy_Rumia_01 : Enemy
	{
		// Token: 0x06000126 RID: 294 RVA: 0x0000976B File Offset: 0x0000796B
		public Enemy_Rumia_01(double x, double y) : base(x, y, Enemy.Kind_e.ENEMY, 1000, 0, -1)
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00009B94 File Offset: 0x00007D94
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

		// Token: 0x06000128 RID: 296 RVA: 0x00009BA4 File Offset: 0x00007DA4
		private IEnumerable<bool> E_UpdateTarget()
		{
			for (;;)
			{
				foreach (DDScene scene in DDSceneUtils.Create(100))
				{
					this.Target_X = 50.0;
					this.Target_Y = DDUtils.AToBRate(50.0, 462.0, scene.Rate);
					yield return true;
				}
				IEnumerator<DDScene> enumerator = null;
				foreach (DDScene scene2 in DDSceneUtils.Create(100))
				{
					this.Target_X = DDUtils.AToBRate(50.0, 462.0, scene2.Rate);
					this.Target_Y = 462.0;
					yield return true;
				}
				enumerator = null;
				foreach (DDScene scene3 in DDSceneUtils.Create(100))
				{
					this.Target_X = 462.0;
					this.Target_Y = DDUtils.AToBRate(462.0, 50.0, scene3.Rate);
					yield return true;
				}
				enumerator = null;
				foreach (DDScene scene4 in DDSceneUtils.Create(100))
				{
					this.Target_X = DDUtils.AToBRate(462.0, 50.0, scene4.Rate);
					this.Target_Y = 50.0;
					yield return true;
				}
				enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00009BB4 File Offset: 0x00007DB4
		private IEnumerable<bool> E_Attack()
		{
			for (;;)
			{
				int num;
				for (int loop = 1; loop <= 5; loop = num + 1)
				{
					for (int c = 1; c <= 9; c++)
					{
						if (loop == 5 && c == 5)
						{
							Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.BLUE, (double)c * 0.7, 0.0, 1));
						}
						else
						{
							Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.RED, (double)c * 0.7, 0.0, -1));
						}
					}
					for (int c2 = 0; c2 < 60; c2 = num + 1)
					{
						yield return true;
						num = c2;
					}
					num = loop;
				}
			}
			yield break;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00009BC4 File Offset: 0x00007DC4
		public override void Killed()
		{
			Ground.I.SE.SE_ENEMYKILLED.Play(true);
			Game.I.Enemies.Add(new Enemy_Rumia_02_04(this.X, this.Y, true));
			Game.I.Score += (long)(2500000 * (Game.I.PlayerWasDead ? 1 : 2));
			EnemyCommon.Drawノ\u30FCミス();
			Game.I.PlayerWasDead = false;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}

		// Token: 0x04000158 RID: 344
		private double Target_X;

		// Token: 0x04000159 RID: 345
		private double Target_Y;
	}
}
