using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	public class Enemy_0002 : Enemy
	{
		private EnemyCommon.FairyInfo Fairy;
		private int ShotType;
		private int DropItemMode;
		private double TargetX;
		private double TargetY;
		private double XAdd;
		private double YAdd;
		private double ApproachingRate;

		public Enemy_0002(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double targetX, double targetY, double xAdd, double yAdd, double approachingRate)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
		{
			this.Fairy = new EnemyCommon.FairyInfo()
			{
				Enemy = this,
				Kind = fairyKind,
			};
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.TargetX = targetX;
			this.TargetY = targetY;
			this.XAdd = xAdd;
			this.YAdd = yAdd;
			this.ApproachingRate = approachingRate;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				this.TargetX += this.XAdd;
				this.TargetY += this.YAdd;

				DDUtils.Approach(ref this.X, this.TargetX, this.ApproachingRate);
				DDUtils.Approach(ref this.Y, this.TargetY, this.ApproachingRate);

				EnemyCommon.Shot(this, this.ShotType);

				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 2000;
		}
	}
}
