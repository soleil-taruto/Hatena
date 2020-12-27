using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000053 RID: 83
	public class Enemy_BigJackOLantern : Enemy
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x0000904C File Offset: 0x0000724C
		public Enemy_BigJackOLantern(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double r, double rApproachingRate, double rot, double rotAdd, double xAdd, double yAdd) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.R = r;
			this.RApproachingRate = rApproachingRate;
			this.Rot = rot;
			this.RotAdd = rotAdd;
			this.XAdd = xAdd;
			this.YAdd = yAdd;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000090A6 File Offset: 0x000072A6
		protected override IEnumerable<bool> E_Draw()
		{
			double axisX = this.X;
			double axisY = this.Y;
			int frame = 0;
			for (;;)
			{
				axisX += this.XAdd;
				axisY += this.YAdd;
				DDUtils.Approach(ref this.R, 0.0, this.RApproachingRate);
				this.Rot += this.RotAdd;
				this.X = axisX + Math.Cos(this.Rot) * this.R;
				this.Y = axisY + Math.Sin(this.Rot) * this.R;
				EnemyCommon.Shot(this, this.ShotType);
				int koma = frame / 5;
				koma %= 2;
				DDDraw.SetMosaic();
				DDDraw.DrawBegin(Ground.I.Picture2.D_PUMPKIN_00_GRBA[koma], this.X, this.Y);
				DDDraw.DrawZoom(3.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y + 4.0), 45.0);
				yield return !EnemyCommon.IsEvacuated(this);
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000090B6 File Offset: 0x000072B6
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 10000L;
		}

		// Token: 0x04000135 RID: 309
		private int ShotType;

		// Token: 0x04000136 RID: 310
		private int DropItemMode;

		// Token: 0x04000137 RID: 311
		private double R;

		// Token: 0x04000138 RID: 312
		private double RApproachingRate;

		// Token: 0x04000139 RID: 313
		private double Rot;

		// Token: 0x0400013A RID: 314
		private double RotAdd;

		// Token: 0x0400013B RID: 315
		private double XAdd;

		// Token: 0x0400013C RID: 316
		private double YAdd;
	}
}
