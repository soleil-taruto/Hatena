using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.ルーミアs
{
	/// <summary>
	/// ルーミア
	/// 第03形態
	/// </summary>
	public class Enemy_ルーミア_03 : Enemy
	{
		DDRandom RandForUT = new DDRandom(1, 2, 3, 4); // UpdateTarget_専用乱数
		DDRandom RandForColor = new DDRandom(5, 6, 7, 8); // 色_専用乱数

		public Enemy_ルーミア_03(double x, double y)
			: base(x, y, Kind_e.ENEMY, 1000, 0)
		{ }

		private double Target_X;
		private double Target_Y;

		protected override IEnumerable<bool> E_Draw()
		{
			// ---- 環境制御 ----

			Game.I.Walls.Add(new Wall_B22001());

			// ----

			Func<bool> f_updateTarget = SCommon.Supplier(this.E_UpdateTarget());
			Func<bool> f_attack = SCommon.Supplier(this.E_Attack());

			for (int frame = 0; ; frame++)
			{
				f_updateTarget();

				double apprRate = 1.0 - Math.Min(1.0, frame / 60.0) * 0.05;

				DDUtils.Approach(ref this.X, this.Target_X, apprRate);
				DDUtils.Approach(ref this.Y, this.Target_Y, apprRate);

				if (EnemyConsts_ルーミア.TRANS_FRAME < frame)
				{
					f_attack();
				}

				EnemyCommon_ルーミア.PutCrash(this, frame);
				EnemyCommon_ルーミア.Draw(this.X, this.Y);

				yield return true;
			}
		}

		private IEnumerable<bool> E_UpdateTarget()
		{
			for (; ; )
			{
				this.Target_X = (this.RandForUT.Real() * 0.9 + 0.05) * GameConsts.FIELD_W;
				this.Target_Y = (this.RandForUT.Real() * 0.1 + 0.85) * GameConsts.FIELD_H;

				for (int c = 0; c < 120; c++)
					yield return true;
			}
		}

		private IEnumerable<bool> E_Attack()
		{
			int waveCount = 0;

			for (; ; )
			{
				for (int c = 0; c < 30; c++)
				{
					double angle = DDUtils.GetAngle(new D2Point(Game.I.Player.X, Game.I.Player.Y) - new D2Point(this.X, this.Y));
					double wave = Math.Sin(waveCount++ / 10.0);

					EnemyCommon.TAMA_COLOR_e color = (EnemyCommon.TAMA_COLOR_e)this.RandForColor.GetInt(10);
					int absorbableWeapon = -1;

					if (color == EnemyCommon.TAMA_COLOR_e.BLUE)
						absorbableWeapon = 3;

					Game.I.Enemies.Add(new Enemy_ルーミア_Tama_03(this.X, this.Y, angle + wave * 1.2, color, absorbableWeapon));

					for (int w = 0; w < 3; w++)
						yield return true;
				}

				for (int c = 0; c < 150; c++)
					yield return true;
			}
		}

		public override void Killed()
		{
			// 次の形態へ移行する。

			Ground.I.SE.SE_ENEMYKILLED.Play();
			Game.I.Enemies.Add(new Enemy_ルーミア_02_04(this.X, this.Y, false));
			Game.I.Score += 12500000 * (Game.I.PlayerWasDead ? 1 : 2);
			EnemyCommon.Drawノーミス();
			Game.I.PlayerWasDead = false;
		}

		public override bool IsBoss()
		{
			return true;
		}
	}
}
