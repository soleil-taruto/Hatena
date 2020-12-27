using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000055 RID: 85
	public class Enemy_JackOLantern : Enemy
	{
		// Token: 0x060000FD RID: 253 RVA: 0x000092BB File Offset: 0x000074BB
		public Enemy_JackOLantern(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double xRate, double yAdd, double rot, double rotAdd) : base(x, y, Enemy.Kind_e.ENEMY, hp, transFrame, -1)
		{
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.XRate = xRate;
			this.YAdd = yAdd;
			this.Rot = rot;
			this.RotAdd = rotAdd;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000092FA File Offset: 0x000074FA
		protected override IEnumerable<bool> E_Draw()
		{
			double axisX = this.X;
			int frame = 0;
			for (;;)
			{
				this.X = axisX + Math.Sin(this.Rot) * this.XRate;
				this.Y += this.YAdd;
				this.Rot += this.RotAdd;
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

		// Token: 0x060000FF RID: 255 RVA: 0x0000930A File Offset: 0x0000750A
		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 7000L;
		}

		// Token: 0x04000142 RID: 322
		private int ShotType;

		// Token: 0x04000143 RID: 323
		private int DropItemMode;

		// Token: 0x04000144 RID: 324
		private double XRate;

		// Token: 0x04000145 RID: 325
		private double YAdd;

		// Token: 0x04000146 RID: 326
		private double Rot;

		// Token: 0x04000147 RID: 327
		private double RotAdd;
	}
}
