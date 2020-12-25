using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Enemies
{
	public class Enemy_0001 : Enemy
	{
		private EnemyCommon.FairyInfo Fairy;
		private int ShotType;
		private int DropItemMode;
		private double TargetX;
		private double TargetY;
		private double Speed;
		private int XDir;
		private double MaxY;
		private double ApproachingRate;

		public Enemy_0001(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double speed, int xDir, double maxY, double approachingRate)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
		{
			this.Fairy = new EnemyCommon.FairyInfo()
			{
				Enemy = this,
				Kind = fairyKind,
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

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (this.TargetY < this.MaxY)
				{
					this.TargetY += this.Speed;
				}
				else
				{
					this.TargetX += this.Speed * this.XDir;
				}

				DDUtils.Approach(ref this.X, this.TargetX, this.ApproachingRate);
				DDUtils.Approach(ref this.Y, this.TargetY, this.ApproachingRate);

				EnemyCommon.Shot(this, this.ShotType);

				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 1000;
		}
	}
}
