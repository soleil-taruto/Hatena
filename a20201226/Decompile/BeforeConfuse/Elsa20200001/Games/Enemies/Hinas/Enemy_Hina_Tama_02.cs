using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x0200005C RID: 92
	public class Enemy_Hina_Tama_02 : Enemy
	{
		// Token: 0x06000115 RID: 277 RVA: 0x000098B0 File Offset: 0x00007AB0
		public Enemy_Hina_Tama_02(double x, double y, double rad, double radZure, EnemyCommon.TAMA_COLOR_e color) : base(x, y, Enemy.Kind_e.TAMA, 0, 0, -1)
		{
			this.Rad = rad;
			this.RadZure = radZure;
			this.Color = color;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000098D5 File Offset: 0x00007AD5
		protected override IEnumerable<bool> E_Draw()
		{
			D2Point speed = DDUtils.AngleToPoint(this.Rad + this.RadZure, 3.0);
			int num;
			for (int frame = 0; frame < 90; frame = num + 1)
			{
				speed *= 0.95;
				this.X += speed.X;
				this.Y += speed.Y;
				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.NORMAL, this.Color), this.X, this.Y);
				DDDraw.DrawRotate(this.Rad + 1.5707963267948966);
				DDDraw.DrawEnd();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return true;
				num = frame;
			}
			double xa;
			double ya;
			DDUtils.MakeXYSpeed(this.X, this.Y, Game.I.Player.X, Game.I.Player.Y, 3.0, out xa, out ya);
			DDUtils.Rotate(ref xa, ref ya, this.RadZure);
			speed.X = xa;
			speed.Y = ya;
			for (;;)
			{
				this.X += speed.X;
				this.Y += speed.Y;
				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.NORMAL, this.Color), this.X, this.Y);
				DDDraw.DrawRotate(this.Rad + 1.5707963267948966);
				DDDraw.DrawEnd();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return !EnemyCommon.IsEvacuated(this);
			}
			yield break;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000093B9 File Offset: 0x000075B9
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}

		// Token: 0x0400014F RID: 335
		private double Rad;

		// Token: 0x04000150 RID: 336
		private double RadZure;

		// Token: 0x04000151 RID: 337
		private EnemyCommon.TAMA_COLOR_e Color;
	}
}
