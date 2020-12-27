using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000051 RID: 81
	public class Enemy_0002 : Enemy
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00008F04 File Offset: 0x00007104
		public Enemy_0002(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double targetX, double targetY, double xAdd, double yAdd, double approachingRate) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.Fairy = new EnemyCommon.FairyInfo
			{
				Enemy = this,
				Kind = fairyKind
			};
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.TargetX = targetX;
			this.TargetY = targetY;
			this.XAdd = xAdd;
			this.YAdd = yAdd;
			this.ApproachingRate = approachingRate;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008F70 File Offset: 0x00007170
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				this.TargetX += this.XAdd;
				this.TargetY += this.YAdd;
				DDUtils.Approach(ref this.X, this.TargetX, this.ApproachingRate);
				DDUtils.Approach(ref this.Y, this.TargetY, this.ApproachingRate);
				EnemyCommon.Shot(this, this.ShotType);
				yield return EnemyCommon.FairyDraw(this.Fairy);
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00008F80 File Offset: 0x00007180
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 2000L;
		}

		// Token: 0x04000125 RID: 293
		private EnemyCommon.FairyInfo Fairy;

		// Token: 0x04000126 RID: 294
		private int ShotType;

		// Token: 0x04000127 RID: 295
		private int DropItemMode;

		// Token: 0x04000128 RID: 296
		private double TargetX;

		// Token: 0x04000129 RID: 297
		private double TargetY;

		// Token: 0x0400012A RID: 298
		private double XAdd;

		// Token: 0x0400012B RID: 299
		private double YAdd;

		// Token: 0x0400012C RID: 300
		private double ApproachingRate;
	}
}
