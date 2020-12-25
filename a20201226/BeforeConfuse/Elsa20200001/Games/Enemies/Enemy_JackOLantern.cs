using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	public class Enemy_JackOLantern : Enemy
	{
		private int ShotType;
		private int DropItemMode;
		private double XRate;
		private double YAdd;
		private double Rot;
		private double RotAdd;

		public Enemy_JackOLantern(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double xRate, double yAdd, double rot, double rotAdd)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
		{
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.XRate = xRate;
			this.YAdd = yAdd;
			this.Rot = rot;
			this.RotAdd = rotAdd;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			double axisX = this.X;

			for (int frame = 0; ; frame++)
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

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X - 1.0, this.Y + 3.0), 30.0);

				yield return !EnemyCommon.IsEvacuated(this);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 7000;
		}
	}
}
