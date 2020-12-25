using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.鍵山雛s
{
	public class Enemy_鍵山雛_Tama_02 : Enemy
	{
		private double Rad;
		private double RadZure;
		private EnemyCommon.TAMA_COLOR_e Color;

		public Enemy_鍵山雛_Tama_02(double x, double y, double rad, double radZure, EnemyCommon.TAMA_COLOR_e color)
			: base(x, y, Kind_e.TAMA, 0, 0)
		{
			this.Rad = rad;
			this.RadZure = radZure;
			this.Color = color;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			D2Point speed = DDUtils.AngleToPoint(this.Rad + this.RadZure, 3.0);

			for (int frame = 0; frame < 90; frame++)
			{
				speed *= 0.95;

				this.X += speed.X;
				this.Y += speed.Y;

				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.NORMAL, this.Color), this.X, this.Y);
				DDDraw.DrawRotate(this.Rad + Math.PI / 2.0);
				DDDraw.DrawEnd();

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);

				yield return true;
			}

			{
				double xa;
				double ya;

				DDUtils.MakeXYSpeed(this.X, this.Y, Game.I.Player.X, Game.I.Player.Y, 3.0, out xa, out ya);
				DDUtils.Rotate(ref xa, ref ya, this.RadZure);

				speed.X = xa;
				speed.Y = ya;
			}

			for (; ; )
			{
				this.X += speed.X;
				this.Y += speed.Y;

				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.NORMAL, this.Color), this.X, this.Y);
				DDDraw.DrawRotate(this.Rad + Math.PI / 2.0);
				DDDraw.DrawEnd();

				this.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);

				yield return !EnemyCommon.IsEvacuated(this);
			}
		}

		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}
	}
}
