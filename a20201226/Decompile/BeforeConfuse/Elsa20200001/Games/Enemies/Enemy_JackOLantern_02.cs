using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x0200004D RID: 77
	public class Enemy_JackOLantern_02 : Enemy
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00007F29 File Offset: 0x00006129
		public Enemy_JackOLantern_02(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double xAdd) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.XAdd = xAdd;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007F50 File Offset: 0x00006150
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				this.X += this.XAdd;
				EnemyCommon.Shot(this, this.ShotType);
				int koma = frame / 7;
				koma %= 2;
				DDDraw.SetMosaic();
				DDDraw.DrawBegin(Ground.I.Picture2.D_PUMPKIN_00[koma], this.X, this.Y);
				DDDraw.DrawZoom(2.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X - 1.0, this.Y + 3.0), 30.0);
				yield return !EnemyCommon.IsEvacuated(this);
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007F60 File Offset: 0x00006160
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 13000L;
		}

		// Token: 0x0400010C RID: 268
		private int ShotType;

		// Token: 0x0400010D RID: 269
		private int DropItemMode;

		// Token: 0x0400010E RID: 270
		private double XAdd;
	}
}
