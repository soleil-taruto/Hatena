using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x0200005B RID: 91
	public class Enemy_Hina_Tama_01 : Enemy
	{
		// Token: 0x06000112 RID: 274 RVA: 0x00009883 File Offset: 0x00007A83
		public Enemy_Hina_Tama_01(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color) : base(x, y, Enemy.Kind_e.TAMA, 0, 0, -1)
		{
			this.Rad = rad;
			this.Color = color;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000098A0 File Offset: 0x00007AA0
		protected override IEnumerable<bool> E_Draw()
		{
			D2Point speed = DDUtils.AngleToPoint(this.Rad, 3.0);
			for (;;)
			{
				this.X += speed.X;
				this.Y += speed.Y;
				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.KNIFE, this.Color), this.X, this.Y);
				DDDraw.DrawZoom(2.0);
				DDDraw.DrawRotate(this.Rad + 1.5707963267948966);
				DDDraw.DrawEnd();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return !EnemyCommon.IsEvacuated(this);
			}
			yield break;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000093B9 File Offset: 0x000075B9
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}

		// Token: 0x0400014D RID: 333
		private double Rad;

		// Token: 0x0400014E RID: 334
		private EnemyCommon.TAMA_COLOR_e Color;
	}
}
