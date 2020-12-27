using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000050 RID: 80
	public class Enemy_0001 : Enemy
	{
		// Token: 0x060000EE RID: 238 RVA: 0x00008E5C File Offset: 0x0000705C
		public Enemy_0001(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double speed, int xDir, double maxY, double approachingRate) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.Fairy = new EnemyCommon.FairyInfo
			{
				Enemy = this,
				Kind = fairyKind
			};
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.TargetX = x;
			this.TargetY = y;
			this.Speed = speed;
			this.XDir = xDir;
			this.MaxY = maxY;
			this.ApproachingRate = approachingRate;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008ECE File Offset: 0x000070CE
		protected override IEnumerable<bool> E_Draw()
		{
			for (;;)
			{
				if (this.TargetY < this.MaxY)
				{
					this.TargetY += this.Speed;
				}
				else
				{
					this.TargetX += this.Speed * (double)this.XDir;
				}
				DDUtils.Approach(ref this.X, this.TargetX, this.ApproachingRate);
				DDUtils.Approach(ref this.Y, this.TargetY, this.ApproachingRate);
				EnemyCommon.Shot(this, this.ShotType);
				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
			yield break;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008EDE File Offset: 0x000070DE
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 1000L;
		}

		// Token: 0x0400011C RID: 284
		private EnemyCommon.FairyInfo Fairy;

		// Token: 0x0400011D RID: 285
		private int ShotType;

		// Token: 0x0400011E RID: 286
		private int DropItemMode;

		// Token: 0x0400011F RID: 287
		private double TargetX;

		// Token: 0x04000120 RID: 288
		private double TargetY;

		// Token: 0x04000121 RID: 289
		private double Speed;

		// Token: 0x04000122 RID: 290
		private int XDir;

		// Token: 0x04000123 RID: 291
		private double MaxY;

		// Token: 0x04000124 RID: 292
		private double ApproachingRate;
	}
}
