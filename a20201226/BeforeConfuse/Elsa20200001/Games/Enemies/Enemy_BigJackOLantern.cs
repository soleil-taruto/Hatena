using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	public class Enemy_BigJackOLantern : Enemy
	{
		private int ShotType;
		private int DropItemMode;
		private double R;
		private double RApproachingRate;
		private double Rot;
		private double RotAdd;
		private double XAdd;
		private double YAdd;

		public Enemy_BigJackOLantern(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double r, double rApproachingRate, double rot, double rotAdd, double xAdd, double yAdd)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
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

		protected override IEnumerable<bool> E_Draw()
		{
			double axisX = this.X;
			double axisY = this.Y;

			for (int frame = 0; ; frame++)
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

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y + 4.0), 45.0);

				yield return !EnemyCommon.IsEvacuated(this);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 10000;
		}
	}
}
