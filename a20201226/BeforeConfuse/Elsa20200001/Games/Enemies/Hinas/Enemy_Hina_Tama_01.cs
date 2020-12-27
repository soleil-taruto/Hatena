using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Hinas
{
	public class Enemy_Hina_Tama_01 : Enemy
	{
		private double Rad;
		private EnemyCommon.TAMA_COLOR_e Color;

		public Enemy_Hina_Tama_01(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color)
			: base(x, y, Kind_e.TAMA, 0, 0)
		{
			this.Rad = rad;
			this.Color = color;
		}

		protected override IEnumerable<bool> E_Draw()
		{
			D2Point speed = DDUtils.AngleToPoint(this.Rad, 3.0);

			for (; ; )
			{
				this.X += speed.X;
				this.Y += speed.Y;

				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.KNIFE, this.Color), this.X, this.Y);
				DDDraw.DrawZoom(2.0);
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
