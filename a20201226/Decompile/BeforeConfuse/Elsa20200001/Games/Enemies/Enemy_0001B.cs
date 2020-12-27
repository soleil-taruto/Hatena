using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	// Token: 0x0200004C RID: 76
	public class Enemy_0001B : Enemy
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00007EA8 File Offset: 0x000060A8
		public Enemy_0001B(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double speed) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.Fairy = new EnemyCommon.FairyInfo
			{
				Enemy = this,
				Kind = fairyKind
			};
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.Speed = speed;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007EF4 File Offset: 0x000060F4
		protected override IEnumerable<bool> E_Draw()
		{
			for (;;)
			{
				double xa;
				double ya;
				DDUtils.MakeXYSpeed(this.X, this.Y, Game.I.Player.X, Game.I.Player.Y, this.Speed, out xa, out ya);
				this.X += xa;
				this.Y += ya;
				EnemyCommon.Shot(this, this.ShotType);
				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
			yield break;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007F04 File Offset: 0x00006104
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 1500L;
		}

		// Token: 0x04000108 RID: 264
		private EnemyCommon.FairyInfo Fairy;

		// Token: 0x04000109 RID: 265
		private int ShotType;

		// Token: 0x0400010A RID: 266
		private int DropItemMode;

		// Token: 0x0400010B RID: 267
		private double Speed;
	}
}
