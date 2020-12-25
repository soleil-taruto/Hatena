using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;
using Charlotte.Games.Walls;

namespace Charlotte.Games.Enemies.ルーミアs
{
	/// <summary>
	/// ルーミア
	/// 第02,04形態
	/// </summary>
	public class Enemy_ルーミア_02_04 : Enemy
	{
		private bool Mode_02; // ? 第02形態

		public Enemy_ルーミア_02_04(double x, double y, bool mode_02)
			: base(x, y, Kind_e.ENEMY, 3000, 0)
		{
			this.Mode_02 = mode_02;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			// ---- 環境制御 ----

			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			// ----

			Func<bool> f_attack_01 = SCommon.Supplier(this.E_Attack_01());
			Func<bool> f_attack_02 = SCommon.Supplier(this.E_Attack_02());

			const int 画面分割_開始Frame = EnemyConsts_ルーミア.TRANS_FRAME + 180;

			for (int frame = 0; ; frame++)
			{
				DDUtils.Approach(ref this.X, GameConsts.FIELD_W / 2 + Math.Sin(frame / 101.0) * 20.0, 0.993);
				DDUtils.Approach(ref this.Y, GameConsts.FIELD_H / 2 + Math.Sin(frame / 103.0) * 20.0, 0.993);

				if (EnemyConsts_ルーミア.TRANS_FRAME < frame)
				{
					if (!this.Mode_02)
					{
						if (frame < 画面分割_開始Frame)
						{
							double sec = (画面分割_開始Frame - frame) / 60.0;
							string sSec = sec.ToString("F2");

							DDGround.EL.Add(() =>
							{
								DDPrint.SetPrint(
									GameConsts.FIELD_L + GameConsts.FIELD_W / 2 - 16,
									GameConsts.FIELD_T + GameConsts.FIELD_H / 2 - 8
									);
								DDPrint.SetBorder(new I3Color(192, 0, 0));
								DDPrint.SetColor(new I3Color(255, 255, 0));
								DDPrint.Print(sSec);
								DDPrint.Reset();

								return false;
							});
						}
						else if (frame == 画面分割_開始Frame)
						{
							画面分割_Effect.Enter();
							画面分割.Enabled = true;
						}
					}

					f_attack_01();
					f_attack_02();
				}

				EnemyCommon_ルーミア.PutCrash(this, frame);
				EnemyCommon_ルーミア.Draw(this.X, this.Y);

				yield return true;
			}
		}

		private IEnumerable<bool> E_Attack_01()
		{
			for (; ; )
			{
				for (int c = -1; c <= 2; c++)
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.BIG, EnemyCommon.TAMA_COLOR_e.INDIGO, 5.0, c * Math.PI * 0.5));

				for (int c = 0; c < 6; c++)
					yield return true;
			}
		}

		private IEnumerable<bool> E_Attack_02()
		{
			for (; ; )
			{
				for (int loop = 0; loop < 5; loop++)
				{
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.PURPLE, 0.7, 0.0));

					for (int c = 0; c < 180; c++)
						yield return true;
				}

				{
					for (int c = -1; c <= 1; c++)
						Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.RED, 0.7, c * 0.5));

					for (int c = 0; c < 180; c++)
						yield return true;
				}

				{
					Game.I.Enemies.Add(new Enemy_Tama_01(this.X, this.Y, EnemyCommon.TAMA_KIND_e.LARGE, EnemyCommon.TAMA_COLOR_e.BLUE, 0.7, 0.0, 2));

					for (int c = 0; c < 180; c++)
						yield return true;
				}
			}
		}

		public override void Killed()
		{
			long score;

			if (this.Mode_02)
			{
				// 次の形態へ移行する。

				Ground.I.SE.SE_ENEMYKILLED.Play();
				Game.I.Enemies.Add(new Enemy_ルーミア_03(this.X, this.Y));
				score = 7500000;
			}
			else
			{
				画面分割_Effect.Leave();
				画面分割.Enabled = false;

				// 終了

				EnemyCommon.Killed(this, 0);

				Game.I.BossKilled = true;
				Game.I.Shots.Add(new Shot_BossBomb());
				score = 17500000;
			}

			Game.I.Score += score * (Game.I.PlayerWasDead ? 1 : 2);
			EnemyCommon.Drawノーミス();
			Game.I.PlayerWasDead = false;
		}

		public override bool IsBoss()
		{
			return true;
		}
	}
}
