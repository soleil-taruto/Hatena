using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Commons;

namespace Charlotte.Games.Enemies
{
	public class Enemy_JackOLantern_02 : Enemy
	{
		private int ShotType;
		private int DropItemMode;
		private double XAdd;

		public Enemy_JackOLantern_02(double x, double y, int hp, int transFrame, int shotType, int dropItemType, double xAdd)
			: base(x, y, Kind_e.ENEMY, hp, transFrame)
		{
			this.ShotType = shotType;
			this.DropItemMode = dropItemType;
			this.XAdd = xAdd;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
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

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X - 1.0, this.Y + 3.0), 30.0);

				yield return !EnemyCommon.IsEvacuated(this);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, this.DropItemMode);
			Game.I.Score += 13000;
		}
	}
}
