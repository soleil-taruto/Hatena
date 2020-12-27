using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Rumias
{
	public class Enemy_Rumia_Tama_03 : Enemy
	{
		private D2Point Speed;
		private EnemyCommon.TAMA_COLOR_e Color;

		public Enemy_Rumia_Tama_03(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color, int absorbableWeapon = -1)
			: base(x, y, Kind_e.TAMA, 0, 0, absorbableWeapon)
		{
			this.Speed = DDUtils.AngleToPoint(rad, 6.0);
			this.Color = color;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; ; frame++)
			{
				this.X += this.Speed.X;
				this.Y += this.Speed.Y;

				if (this.X < 0.0)
					this.Speed.X = Math.Abs(this.Speed.X);
				else if (GameConsts.FIELD_W < this.X)
					this.Speed.X = -Math.Abs(this.Speed.X);

				if (this.Y < 0.0)
					this.Speed.Y = Math.Abs(this.Speed.Y);
				else
					this.Speed.Y += 0.03; // 重力加速度

				if (this.AbsorbableWeapon != -1) // ? 吸収可能
				{
					DDDraw.SetAlpha(0.2);
					DDDraw.SetBright(0.0, 0.5, 1.0);
					DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], this.X, this.Y);
					DDDraw.DrawRotate(DDEngine.ProcFrame / 30.0);
					DDDraw.DrawZoom(1.2);
					DDDraw.DrawEnd();
					DDDraw.Reset();

					DDPrint.SetPrint((int)this.X, (int)this.Y);
					DDPrint.SetBorder(new I3Color(0, 0, 100));
					DDPrint.Print("[" + this.AbsorbableWeapon + "]");
					DDPrint.Reset();
				}

				DDDraw.DrawCenter(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);

				yield return this.Y < GameConsts.FIELD_H;
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}
	}
}
