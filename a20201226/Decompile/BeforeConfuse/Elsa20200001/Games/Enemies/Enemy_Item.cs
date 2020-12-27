using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x02000054 RID: 84
	public class Enemy_Item : Enemy
	{
		// Token: 0x060000FA RID: 250 RVA: 0x000090DC File Offset: 0x000072DC
		public Enemy_Item(double x, double y, EnemyCommon.DROP_ITEM_TYPE_e dropItemType, int vacuumMode = 0, int absorbableWeapon = -1) : base(x, y, Enemy.Kind_e.ITEM, 0, 0, -1)
		{
			this.DropItemType = dropItemType;
			this.VacuumMode = vacuumMode;
			this.AbsorbableWeapon = absorbableWeapon;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00009159 File Offset: 0x00007359
		protected override IEnumerable<bool> E_Draw()
		{
			for (;;)
			{
				if (this.VacuumMode == 0)
				{
					if (Game.I.Input.Slow && DDUtils.GetDistance(new D2Point(Game.I.Player.X, Game.I.Player.Y), new D2Point(this.X, this.Y)) < 100.0)
					{
						this.VacuumMode = 1;
					}
					else if (Game.I.Player.Y < 200.0 && this.Y < 712.0)
					{
						this.VacuumMode = 2;
					}
				}
				if (this.VacuumMode == 0)
				{
					this.Y += this.YAdd;
					this.YAdd += 0.1;
					if (this.X < 0.0)
					{
						this.X += 1.0;
					}
					else if (512.0 < this.X)
					{
						this.X -= 1.0;
					}
				}
				else
				{
					double speed;
					if (this.VacuumMode == 2)
					{
						speed = 12.0;
					}
					else
					{
						speed = 6.0;
					}
					double targetX;
					double targetY;
					if (1 <= Game.I.Player.BornFrame)
					{
						targetX = Game.I.Player.BornFollowX;
						targetY = Game.I.Player.BornFollowY;
					}
					else
					{
						targetX = Game.I.Player.X;
						targetY = Game.I.Player.Y;
					}
					double xa;
					double ya;
					DDUtils.MakeXYSpeed(this.X, this.Y, targetX, targetY, speed, out xa, out ya);
					this.X += xa;
					this.Y += ya;
				}
				this.Rot += this.RotAdd;
				if (0.0 < this.YAdd)
				{
					this.YAdd *= 0.97;
				}
				DDPicture picture;
				switch (this.DropItemType)
				{
				case EnemyCommon.DROP_ITEM_TYPE_e.STAR:
					picture = Ground.I.Picture2.D_ITEM_STAR;
					goto IL_341;
				case EnemyCommon.DROP_ITEM_TYPE_e.CANDY:
					picture = Ground.I.Picture2.D_ITEM_CANDY;
					goto IL_341;
				case EnemyCommon.DROP_ITEM_TYPE_e.HEART:
					picture = Ground.I.Picture2.D_ITEM_HEART;
					goto IL_341;
				case EnemyCommon.DROP_ITEM_TYPE_e.BOMB:
					picture = Ground.I.Picture2.D_ITEM_BOMB;
					goto IL_341;
				case EnemyCommon.DROP_ITEM_TYPE_e.ABSORBABLE_SHOT:
					picture = Ground.I.Picture2.吸収している武器;
					DDDraw.SetAlpha(0.5);
					DDDraw.SetBright(0.0, 0.5, 1.0);
					DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], this.X, this.Y);
					DDDraw.DrawRotate((double)DDEngine.ProcFrame / 30.0);
					DDDraw.DrawZoom(0.5);
					DDDraw.DrawEnd();
					DDDraw.Reset();
					goto IL_341;
				}
				break;
				IL_341:
				DDDraw.DrawBegin(picture, this.X, this.Y);
				DDDraw.DrawRotate(this.Rot);
				DDDraw.DrawEnd();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 16.0);
				yield return this.Y < 712.0;
			}
			throw null;
			yield break;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000916C File Offset: 0x0000736C
		public override void Killed()
		{
			Ground.I.SE.SE_ITEMGOT.Play(true);
			switch (this.DropItemType)
			{
			case EnemyCommon.DROP_ITEM_TYPE_e.STAR:
				Game.I.Score += 100L;
				return;
			case EnemyCommon.DROP_ITEM_TYPE_e.CANDY:
				Game.I.Player.Power += 17;
				Game.I.Player.Power = Math.Min(Game.I.Player.Power, 500);
				return;
			case EnemyCommon.DROP_ITEM_TYPE_e.HEART:
				Game.I.Zanki++;
				Game.I.Zanki = Math.Min(Game.I.Zanki, 5);
				return;
			case EnemyCommon.DROP_ITEM_TYPE_e.BOMB:
				Game.I.ZanBomb++;
				Game.I.ZanBomb = Math.Min(Game.I.ZanBomb, 5);
				return;
			case EnemyCommon.DROP_ITEM_TYPE_e.ABSORBABLE_SHOT:
				Game.I.Player.AbsorbedWeapon = this.AbsorbableWeapon;
				Game.I.Player.Power += 50;
				Game.I.Player.Power = Math.Min(Game.I.Player.Power, 500);
				return;
			default:
				throw null;
			}
		}

		// Token: 0x0400013D RID: 317
		private EnemyCommon.DROP_ITEM_TYPE_e DropItemType;

		// Token: 0x0400013E RID: 318
		private int VacuumMode;

		// Token: 0x0400013F RID: 319
		public double YAdd = -4.0;

		// Token: 0x04000140 RID: 320
		public double Rot = DDUtils.Random.Real2() * 3.1415926535897931 * 2.0;

		// Token: 0x04000141 RID: 321
		public double RotAdd = DDUtils.Random.Real2() * 0.01;
	}
}
