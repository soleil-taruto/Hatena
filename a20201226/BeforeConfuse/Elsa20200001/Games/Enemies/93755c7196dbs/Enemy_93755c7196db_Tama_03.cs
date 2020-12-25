using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.鍵山雛s
{
	public class Enemy_鍵山雛_Tama_03 : Enemy
	{
		private D2Point Speed;
		private EnemyCommon.TAMA_COLOR_e Color;

		public Enemy_鍵山雛_Tama_03(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color)
			: base(x, y, Kind_e.TAMA, 0, 0)
		{
			this.Speed = DDUtils.AngleToPoint(rad, 1.0);
			this.Color = color;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				if (DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H)))
					break;

				this.X += this.Speed.X;
				this.Y += this.Speed.Y;

				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);
				DDDraw.DrawEnd();

				Game.I.EL_AfterDrawWalls.Add(SCommon.Supplier(this.E_魔法陣_1(this.X, this.Y)));

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);

				yield return true;
			}

			Game.I.EL_AfterDrawWalls.Add(SCommon.Supplier(this.E_魔法陣_V(this.X, this.Y)));

			{
				const double SPEED = 4.0;

				if (this.X < 0.0) // ? フィールド左側に衝突
				{
					this.Speed = new D2Point(SPEED, 0.0);
					this.Color = EnemyCommon.TAMA_COLOR_e.YELLOW;
				}
				else if (GameConsts.FIELD_W < this.X) // ? フィールド右側に衝突
				{
					this.Speed = new D2Point(-SPEED, 0.0);
					this.Color = EnemyCommon.TAMA_COLOR_e.YELLOW;
				}
				else if (this.Y < 0.0) // ? フィールド上側に衝突
				{
					this.Speed = new D2Point(0.0, SPEED);
					this.Color = EnemyCommon.TAMA_COLOR_e.WHITE;
				}
				else // ? フィールド下側に衝突
				{
					this.Speed = new D2Point(0.0, -SPEED);
					this.Color = EnemyCommon.TAMA_COLOR_e.WHITE;
				}
			}

			for (int frame = 0; ; frame++)
			{
				this.X += this.Speed.X;
				this.Y += this.Speed.Y;

				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);
				DDDraw.DrawEnd();

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);

				yield return !EnemyCommon.IsEvacuated(this);
			}
		}

		private IEnumerable<bool> E_魔法陣_1(double x, double y)
		{
			DDDraw.SetAlpha(0.2);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
			DDDraw.DrawRotate(DDEngine.ProcFrame / 30.0);
			DDDraw.DrawZoom(0.5);
			DDDraw.DrawEnd();
			DDDraw.Reset();

			yield return false; // once
		}

		private IEnumerable<bool> E_魔法陣_V(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(30))
			{
				DDDraw.SetAlpha(0.2 - scene.Rate * 0.2);
				DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
				DDDraw.DrawRotate(DDEngine.ProcFrame / 30.0);
				DDDraw.DrawZoom(0.5 + scene.Rate * 1.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return true;
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}
	}
}
