using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x02000062 RID: 98
	public class Enemy_Rumia_02_04 : Enemy
	{
		// Token: 0x0600012C RID: 300 RVA: 0x00009C3F File Offset: 0x00007E3F
		public Enemy_Rumia_02_04(double x, double y, bool mode_02) : base(x, y, Enemy.Kind_e.ENEMY, 3000, 0, -1)
		{
			this.Mode_02 = mode_02;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00009C58 File Offset: 0x00007E58
		protected override IEnumerable<bool> E_Draw()
		{
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());
			Func<bool> f_attack_ = SCommon.Supplier<bool>(this.E_Attack_01());
			Func<bool> f_attack_2 = SCommon.Supplier<bool>(this.E_Attack_02());
			int frame = 0;
			for (;;)
			{
				DDUtils.Approach(ref this.X, 256.0 + Math.Sin((double)frame / 101.0) * 20.0, 0.993);
				DDUtils.Approach(ref this.Y, 256.0 + Math.Sin((double)frame / 103.0) * 20.0, 0.993);
				if (150 < frame)
				{
					if (!this.Mode_02)
					{
						if (frame < 330)
						{
							string sSec = ((double)(330 - frame) / 60.0).ToString("F2");
							DDGround.EL.Add(delegate
							{
								DDPrint.SetPrint(464, 262, 16);
								DDPrint.SetBorder(new I3Color(192, 0, 0), 1);
								DDPrint.SetColor(new I3Color(255, 255, 0));
								DDPrint.Print(sSec);
								DDPrint.Reset();
								return false;
							});
						}
						else if (frame == 330)
						{
							FieldDividerEffect.Enter();
							FieldDivider.Enabled = true;
						}
					}
					f_attack_();
					f_attack_2();
				}
				EnemyCommon_Rumia.PutCrash(this, frame);
				EnemyCommon_Rumia.Draw(this.X, this.Y);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00009C68 File Offset: 0x00007E68
		private IEnumerable<bool> E_Attack_01()
		{
			for (;;)
			{
				for (int c = -1; c <= 2; c++)
				{
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.INDIGO, 5.0, (double)c * 3.1415926535897931 * 0.5, -1));
				}
				int num;
				for (int c2 = 0; c2 < 6; c2 = num + 1)
				{
					yield return true;
					num = c2;
				}
			}
			yield break;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00009C78 File Offset: 0x00007E78
		private IEnumerable<bool> E_Attack_02()
		{
			for (;;)
			{
				int num;
				for (int loop = 0; loop < 5; loop = num + 1)
				{
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.PURPLE, 0.7, 0.0, -1));
					for (int c = 0; c < 180; c = num + 1)
					{
						yield return true;
						num = c;
					}
					num = loop;
				}
				for (int c2 = -1; c2 <= 1; c2++)
				{
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.RED, 0.7, (double)c2 * 0.5, -1));
				}
				for (int loop = 0; loop < 180; loop = num + 1)
				{
					yield return true;
					num = loop;
				}
				Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.BLUE, 0.7, 0.0, 2));
				for (int loop = 0; loop < 180; loop = num + 1)
				{
					yield return true;
					num = loop;
				}
			}
			yield break;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00009C88 File Offset: 0x00007E88
		public override void Killed()
		{
			long score;
			if (this.Mode_02)
			{
				Ground.I.SE.SE_ENEMYKILLED.Play(true);
				Game.I.Enemies.Add(new Enemy_Rumia_03(this.X, this.Y));
				score = 7500000L;
			}
			else
			{
				FieldDividerEffect.Leave();
				FieldDivider.Enabled = false;
				EnemyCommon.Killed(this, 0);
				Game.I.BossKilled = true;
				Game.I.Shots.Add(new Shot_BossBomb());
				score = 17500000L;
			}
			Game.I.Score += score * (Game.I.PlayerWasDead ? 1L : 2L);
			EnemyCommon.Drawノ\u30FCミス();
			Game.I.PlayerWasDead = false;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}

		// Token: 0x0400015A RID: 346
		private bool Mode_02;
	}
}
