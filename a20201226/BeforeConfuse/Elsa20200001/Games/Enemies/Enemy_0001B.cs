using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Enemies
{
	public class Enemy_0001B : Enemy
	{
		private EnemyCommon.FairyInfo Fairy;
		private int ShotType;
		private int DropItemMode;
		private double Speed;

		public Enemy_0001B(double x, double y, int hp, int transFrame, int fairyKind, int shotType, int dropItemType, double speed)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
		{
			this.Fairy = new EnemyCommon.FairyInfo()
			{
				Enemy = this,
				Kind = fairyKind,
			};
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.Speed = speed;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				double xa;
				double ya;

				DDUtils.MakeXYSpeed(this.X, this.Y, Game.I.Player.X, Game.I.Player.Y, this.Speed, out xa, out ya);

				this.X += xa;
				this.Y += ya;

				EnemyCommon.Shot(this, this.ShotType);

				yield return EnemyCommon.FairyDraw(this.Fairy);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 1500;
		}
	}
}
