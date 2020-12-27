using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000056 RID: 86
	public class Enemy_Tama_01 : Enemy
	{
		// Token: 0x06000100 RID: 256 RVA: 0x00009330 File Offset: 0x00007530
		public Enemy_Tama_01(double x, double y, EnemyCommon.TAMA_KIND_e tamaKind, EnemyCommon.TAMA_COLOR_e tamaColor, double speed, double angle, int absorbableWeapon = -1) : base(x, y, Enemy.Kind_e.TAMA, 0, 0, absorbableWeapon)
		{
			if (speed < 0.1 || 100.0 < speed)
			{
				throw new DDError();
			}
			if (angle < -9.42477796076938 || 9.42477796076938 < angle)
			{
				throw new DDError();
			}
			this.TamaKind = tamaKind;
			this.TamaColor = tamaColor;
			this.Speed = speed;
			this.Angle = angle;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000093A9 File Offset: 0x000075A9
		protected override IEnumerable<bool> E_Draw()
		{
			switch (this.TamaKind)
			{
			case EnemyCommon.TAMA_KIND_e.NORMAL:
			{
				double r = 8.0;
				goto IL_77;
			}
			case EnemyCommon.TAMA_KIND_e.BIG:
			{
				double r = 12.0;
				goto IL_77;
			}
			case EnemyCommon.TAMA_KIND_e.LARGE:
			{
				double r = 30.0;
				goto IL_77;
			}
			}
			throw null;
			IL_77:
			double xAdd;
			double yAdd;
			DDUtils.MakeXYSpeed(this.X, this.Y, Game.I.Player.X, Game.I.Player.Y, this.Speed, out xAdd, out yAdd);
			DDUtils.Rotate(ref xAdd, ref yAdd, this.Angle);
			DDPicture picture = EnemyCommon.GetTamaPicture(this.TamaKind, this.TamaColor);
			for (;;)
			{
				this.X += xAdd;
				this.Y += yAdd;
				DDDraw.DrawCenter(picture, this.X, this.Y);
				if (this.AbsorbableWeapon != -1)
				{
					DDDraw.SetAlpha(0.5);
					DDDraw.SetBright(0.0, 0.5, 1.0);
					DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], this.X, this.Y);
					DDDraw.DrawRotate((double)DDEngine.ProcFrame / 30.0);
					DDDraw.DrawZoom(0.5);
					DDDraw.DrawEnd();
					DDDraw.Reset();
					DDPrint.SetPrint((int)this.X, (int)this.Y, 16);
					DDPrint.SetBorder(new I3Color(0, 0, 100), 1);
					DDPrint.Print("[" + this.AbsorbableWeapon.ToString() + "]");
					DDPrint.Reset();
				}
				double r;
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), r);
				yield return !EnemyCommon.IsEvacuated(this);
			}
			yield break;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000093B9 File Offset: 0x000075B9
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}

		// Token: 0x04000148 RID: 328
		private EnemyCommon.TAMA_KIND_e TamaKind;

		// Token: 0x04000149 RID: 329
		private EnemyCommon.TAMA_COLOR_e TamaColor;

		// Token: 0x0400014A RID: 330
		private double Speed;

		// Token: 0x0400014B RID: 331
		private double Angle;
	}
}
