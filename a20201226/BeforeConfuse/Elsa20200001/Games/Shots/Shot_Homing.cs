using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Enemies;
using Charlotte.GameCommons;
using Charlotte.Commons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	public class Shot_Homing : Shot
	{
		/// <summary>
		/// 弾の進行方向
		/// ラジアン角
		/// (0,0) -> (1,0) 方向を 0.0 とする
		/// </summary>
		private double Rot = Math.PI * 1.5;

		/// <summary>
		/// 標的とする敵
		/// null == ターゲットしていない || そもそも敵が居ない。
		/// </summary>
		private Enemy TargetEnemy = null;

		public Shot_Homing(double x, double y)
			: base(x, y, Kind_e.NORMAL, 1)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				if (this.TargetEnemy == null || this.TargetEnemy.HP == -1) // ? ターゲットしていない || ターゲットが死亡した。
					this.TargetEnemy = this.FindTargetEnemy();

				if (this.TargetEnemy != null)
				{
					double targetRot = DDUtils.GetAngle(new D2Point(this.TargetEnemy.X, this.TargetEnemy.Y) - new D2Point(this.X, this.Y));
					double diffRot = targetRot - this.Rot;

					{
						const double ROT_ADD = 0.3;

						if (diffRot < -Math.PI || 0.0 < diffRot && diffRot < Math.PI)
							this.Rot += ROT_ADD;
						else
							this.Rot -= ROT_ADD;
					}

					{
						const double ROT_MAX = Math.PI * 2.0;

						this.Rot += ROT_MAX;

						while (ROT_MAX < this.Rot)
							this.Rot -= ROT_MAX;
					}
				}

				{
					const double SPEED = 20.0;

					D2Point movePt = DDUtils.AngleToPoint(this.Rot, SPEED);

					this.X += movePt.X;
					this.Y += movePt.Y;
				}

				DDDraw.SetAlpha(ShotConsts.A);
				DDDraw.SetBright(0.0, 1.0, 1.0);
				DDDraw.DrawBegin(Ground.I.Picture2.D_LASER, this.X, this.Y);
				DDDraw.DrawZoom(1.5);
				DDDraw.DrawRotate(this.Rot - Math.PI * 1.5);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				this.Crash = DDCrashUtils.Circle(
					new D2Point(this.X, this.Y),
					40.0
					);

				yield return !DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H), 100.0);
			}
		}

		private Enemy FindTargetEnemy()
		{
			Enemy[] targets = Game.I.Enemies.Iterate()
				.Where(v => v.Kind == Enemy.Kind_e.ENEMY && v.HP != 0) // ? 敵 && 無敵ではない。
				.ToArray();

			if (targets.Length == 0)
				return null;

			return targets[Game.I.Frame % targets.Length];
		}
	}
}
