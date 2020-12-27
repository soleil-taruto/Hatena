using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x02000064 RID: 100
	public class Enemy_Rumia_Tama_03 : Enemy
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00009E23 File Offset: 0x00008023
		public Enemy_Rumia_Tama_03(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color, int absorbableWeapon = -1) : base(x, y, Enemy.Kind_e.TAMA, 0, 0, absorbableWeapon)
		{
			this.Speed = DDUtils.AngleToPoint(rad, 6.0);
			this.Color = color;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00009E4F File Offset: 0x0000804F
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			for (;;)
			{
				this.X += this.Speed.X;
				this.Y += this.Speed.Y;
				if (this.X < 0.0)
				{
					this.Speed.X = Math.Abs(this.Speed.X);
				}
				else if (512.0 < this.X)
				{
					this.Speed.X = -Math.Abs(this.Speed.X);
				}
				if (this.Y < 0.0)
				{
					this.Speed.Y = Math.Abs(this.Speed.Y);
				}
				else
				{
					this.Speed.Y = this.Speed.Y + 0.03;
				}
				if (this.AbsorbableWeapon != -1)
				{
					DDDraw.SetAlpha(0.2);
					DDDraw.SetBright(0.0, 0.5, 1.0);
					DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], this.X, this.Y);
					DDDraw.DrawRotate((double)DDEngine.ProcFrame / 30.0);
					DDDraw.DrawZoom(1.2);
					DDDraw.DrawEnd();
					DDDraw.Reset();
					DDPrint.SetPrint((int)this.X, (int)this.Y, 16);
					DDPrint.SetBorder(new I3Color(0, 0, 100), 1);
					DDPrint.Print("[" + this.AbsorbableWeapon.ToString() + "]");
					DDPrint.Reset();
				}
				DDDraw.DrawCenter(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return this.Y < 512.0;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000093B9 File Offset: 0x000075B9
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}

		// Token: 0x0400015F RID: 351
		private D2Point Speed;

		// Token: 0x04000160 RID: 352
		private EnemyCommon.TAMA_COLOR_e Color;
	}
}
