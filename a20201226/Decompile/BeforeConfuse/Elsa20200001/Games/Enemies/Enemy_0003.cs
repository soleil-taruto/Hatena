using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000052 RID: 82
	public class Enemy_0003 : Enemy
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x00008FA8 File Offset: 0x000071A8
		public Enemy_0003(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double targetX, double targetY, int evacuateFrame, double evacuateXAdd, double evacuateYAdd) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
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
			this.EvacuateFrame = evacuateFrame;
			this.EvacuateXAdd = evacuateXAdd;
			this.EvacuateYAdd = evacuateYAdd;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00009014 File Offset: 0x00007214
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				if (frame < this.EvacuateFrame)
				{
					DDUtils.Approach(ref this.X, this.TargetX, 0.99);
					DDUtils.Approach(ref this.Y, this.TargetY, 0.99);
				}
				else
				{
					this.X += this.EvacuateXAdd;
					this.Y += this.EvacuateYAdd;
					this.EvacuateXAdd *= 1.01;
					this.EvacuateXAdd *= 1.01;
				}
				EnemyCommon.Shot(this, this.ShotType);
				yield return EnemyCommon.FairyDraw(this.Fairy);
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00009024 File Offset: 0x00007224
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 5000L;
		}

		// Token: 0x0400012D RID: 301
		private EnemyCommon.FairyInfo Fairy;

		// Token: 0x0400012E RID: 302
		private int ShotType;

		// Token: 0x0400012F RID: 303
		private int DropItemMode;

		// Token: 0x04000130 RID: 304
		private double TargetX;

		// Token: 0x04000131 RID: 305
		private double TargetY;

		// Token: 0x04000132 RID: 306
		private int EvacuateFrame;

		// Token: 0x04000133 RID: 307
		private double EvacuateXAdd;

		// Token: 0x04000134 RID: 308
		private double EvacuateYAdd;
	}
}
