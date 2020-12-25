using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies
{
	public class Enemy_0003 : Enemy
	{
		private EnemyCommon.FairyInfo Fairy;
		private int ShotType;
		private int DropItemMode;
		private double TargetX;
		private double TargetY;
		private int EvacuateFrame;
		private double EvacuateXAdd;
		private double EvacuateYAdd;

		public Enemy_0003(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double targetX, double targetY, int evacuateFrame, double evacuateXAdd, double evacuateYAdd)
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
			this.EvacuateFrame = evacuateFrame;
			this.EvacuateXAdd = evacuateXAdd;
			this.EvacuateYAdd = evacuateYAdd;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				const double AR = 0.99;
				const double ER = 1.01;

				if (frame < this.EvacuateFrame)
				{
					DDUtils.Approach(ref this.X, this.TargetX, AR);
					DDUtils.Approach(ref this.Y, this.TargetY, AR);
				}
				else
				{

					this.X += this.EvacuateXAdd;
					this.Y += this.EvacuateYAdd;

					this.EvacuateXAdd *= ER;
					this.EvacuateXAdd *= ER;
				}

				EnemyCommon.Shot(this, this.ShotType);

				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 5000;
		}
	}
}
