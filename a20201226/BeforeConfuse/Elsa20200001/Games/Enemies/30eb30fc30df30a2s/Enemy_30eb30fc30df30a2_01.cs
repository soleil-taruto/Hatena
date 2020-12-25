using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;
using Charlotte.Games.Enemies.鍵山雛s;
using Charlotte.Games.Shots;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.ルーミアs
{
	/// <summary>
	/// ルーミア
	/// 第01形態
	/// </summary>
	public class Enemy_ルーミア_01 : Enemy
	{
		public Enemy_ルーミア_01(double x, double y)
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
			const double MARGIN = 50.0;
			const int FRAME_MAX = 100;

			for (; ; )
			{
				foreach (DDScene scene in DDSceneUtils.Create(FRAME_MAX))
				{
					this.Target_X = MARGIN;
					this.Target_Y = DDUtils.AToBRate(MARGIN, GameConsts.FIELD_H - MARGIN, scene.Rate);

					yield return true;
				}
				foreach (DDScene scene in DDSceneUtils.Create(FRAME_MAX))
				{
					this.Target_X = DDUtils.AToBRate(MARGIN, GameConsts.FIELD_W - MARGIN, scene.Rate);
					this.Target_Y = GameConsts.FIELD_H - MARGIN;

					yield return true;
				}
				foreach (DDScene scene in DDSceneUtils.Create(FRAME_MAX))
				{
					this.Target_X = GameConsts.FIELD_W - MARGIN;
					this.Target_Y = DDUtils.AToBRate(GameConsts.FIELD_H - MARGIN, MARGIN, scene.Rate);

					yield return true;
				}
				foreach (DDScene scene in DDSceneUtils.Create(FRAME_MAX))
				{
					this.Target_X = DDUtils.AToBRate(GameConsts.FIELD_W - MARGIN, MARGIN, scene.Rate);
					this.Target_Y = MARGIN;

					yield return true;
				}
			}
		}

		private IEnumerable<bool> E_Attack()
		{
			for (; ; )
			{
				for (int loop = 1; loop <= 5; loop++)
				{
					for (int c = 1; c <= 9; c++)
					{
						if (loop == 5 && c == 5)
							Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.BLUE, c * 0.7, 0.0, 1));
						else
							Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.RED, c * 0.7, 0.0));
					}

					for (int c = 0; c < 60; c++)
						yield return true;
				}
			}
		}

		public override void Killed()
		{
			// 次の形態へ移行する。

			Ground.I.SE.SE_ENEMYKILLED.Play();
			Game.I.Enemies.Add(new Enemy_ルーミア_02_04(this.X, this.Y, true));
			Game.I.Score += 2500000 * (Game.I.PlayerWasDead ? 1 : 2);
			EnemyCommon.Drawノーミス();
			Game.I.PlayerWasDead = false;
		}

		public override bool IsBoss()
		{
			return true;
		}
	}
}
