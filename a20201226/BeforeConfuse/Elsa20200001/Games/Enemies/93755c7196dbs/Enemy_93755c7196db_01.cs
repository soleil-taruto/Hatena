using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Enemies.鍵山雛s
{
	/// <summary>
	/// 鍵山雛
	/// 第01形態
	/// </summary>
	public class Enemy_鍵山雛_01 : Enemy
	{
		public Enemy_鍵山雛_01(double x, double y)
			: base(x, y, Kind_e.ENEMY, 1000, 0)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			double a_mahoujin = 0.0;

			for (int frame = 0; ; frame++)
			{
				{
					double rad = frame / 70.0;

					DDUtils.Approach(ref this.X, GameConsts.FIELD_W / 2 + Math.Sin(rad) * 180.0, 0.93);
					DDUtils.Approach(ref this.Y, GameConsts.FIELD_H / 7 + Math.Cos(rad) * 50.0, 0.93);
				}

				if (30 < frame && frame % 3 == 0)
				{
					for (int c = 0; c < 3; c++)
					{
						double rad = frame / 27.0 + (Math.PI * 2.0) * c / 3.0;
						D2Point pt = DDUtils.AngleToPoint(rad, 50.0);

						EnemyCommon.TAMA_COLOR_e color = new EnemyCommon.TAMA_COLOR_e[]
						{
							EnemyCommon.TAMA_COLOR_e.CYAN,
							EnemyCommon.TAMA_COLOR_e.YELLOW,
							EnemyCommon.TAMA_COLOR_e.PURPLE,
						}
						[c];

						Game.I.Enemies.Add(new Enemy_鍵山雛_Tama_01(this.X + pt.X, this.Y + pt.Y, rad + Math.PI / 2.0, color));
						Game.I.Enemies.Add(new Enemy_鍵山雛_Tama_01(this.X + pt.X, this.Y + pt.Y, rad - Math.PI / 2.0, color));
					}
				}

				{
					const int TRANS_FRAME = 60;

					if (frame == TRANS_FRAME)
					{
						// ボム消し
						// このフレームにおけるボム当たり判定は既に展開しているので、ボスの当たり判定を展開する前のフレームで消す。
						Game.I.Shots.RemoveAll(v => v.Kind == Shot.Kind_e.BOMB);

						// 念のためリセット
						//Game.I.BombUsed = false;
						Game.I.PlayerWasDead = false;
					}
					else if (TRANS_FRAME < frame)
					{
						DDUtils.Approach(ref a_mahoujin, 1.0, 0.99);
						this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 25.0);
					}
				}

				EnemyCommon_鍵山雛.Draw(this.X, this.Y, true, a_mahoujin);
				EnemyCommon_鍵山雛.DrawOther(this);

				yield return true;
			}
		}

		public override void Killed()
		{
			// 次の形態へ移行する。

			Ground.I.SE.SE_ENEMYKILLED.Play();
			Game.I.Enemies.Add(new Enemy_鍵山雛_02(this.X, this.Y));
			Game.I.Score += 5000000 * (Game.I.PlayerWasDead ? 1 : 2);
			EnemyCommon.Drawノーミス();
			Game.I.PlayerWasDead = false;
		}

		public override bool IsBoss()
		{
			return true;
		}
	}
}
