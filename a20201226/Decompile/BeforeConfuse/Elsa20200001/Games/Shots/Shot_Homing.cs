using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Enemies;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000034 RID: 52
	public class Shot_Homing : Shot
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00007C4F File Offset: 0x00005E4F
		public Shot_Homing(double x, double y) : base(x, y, Shot.Kind_e.NORMAL, 1)
		{
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00007C6A File Offset: 0x00005E6A
		protected override IEnumerable<bool> E_Draw()
		{
			for (;;)
			{
				if (this.TargetEnemy == null || this.TargetEnemy.HP == -1)
				{
					this.TargetEnemy = this.FindTargetEnemy();
				}
				if (this.TargetEnemy != null)
				{
					double diffRot = DDUtils.GetAngle(new D2Point(this.TargetEnemy.X, this.TargetEnemy.Y) - new D2Point(this.X, this.Y)) - this.Rot;
					if (diffRot < -3.1415926535897931 || (0.0 < diffRot && diffRot < 3.1415926535897931))
					{
						this.Rot += 0.3;
					}
					else
					{
						this.Rot -= 0.3;
					}
					this.Rot += 6.2831853071795862;
					while (6.2831853071795862 < this.Rot)
					{
						this.Rot -= 6.2831853071795862;
					}
				}
				D2Point movePt = DDUtils.AngleToPoint(this.Rot, 20.0);
				this.X += movePt.X;
				this.Y += movePt.Y;
				DDDraw.SetAlpha(0.3);
				DDDraw.SetBright(0.0, 1.0, 1.0);
				DDDraw.DrawBegin(Ground.I.Picture2.D_LASER, this.X, this.Y);
				DDDraw.DrawZoom(1.5);
				DDDraw.DrawRotate(this.Rot - 4.71238898038469);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 40.0);
				yield return !DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 100.0);
			}
			yield break;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00007C7C File Offset: 0x00005E7C
		private Enemy FindTargetEnemy()
		{
			Enemy[] targets = (from v in Game.I.Enemies.Iterate()
			where v.Kind == Enemy.Kind_e.ENEMY && v.HP != 0
			select v).ToArray<Enemy>();
			if (targets.Length == 0)
			{
				return null;
			}
			return targets[Game.I.Frame % targets.Length];
		}

		// Token: 0x040000FC RID: 252
		private double Rot = 4.71238898038469;

		// Token: 0x040000FD RID: 253
		private Enemy TargetEnemy;
	}
}
