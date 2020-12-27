using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x0200004E RID: 78
	public abstract class Enemy
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00007F85 File Offset: 0x00006185
		public bool Absorbable
		{
			get
			{
				return this.AbsorbableWeapon != -1 && this.Kind != Enemy.Kind_e.ITEM;
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007F9E File Offset: 0x0000619E
		public Enemy(double x, double y, Enemy.Kind_e kind, int hp, int transFrame, int absorbableWeapon = -1)
		{
			this.X = x;
			this.Y = y;
			this.Kind = kind;
			this.HP = hp;
			this.InitialHP = hp;
			this.TransFrame = transFrame;
			this.AbsorbableWeapon = absorbableWeapon;
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00007FDC File Offset: 0x000061DC
		public Func<bool> Draw
		{
			get
			{
				if (this._draw == null)
				{
					Func<bool> f = SCommon.Supplier<bool>(this.E_Draw());
					Func<bool> fc = SCommon.Supplier<bool>(this.E_Draw_共通());
					this._draw = (() => fc() && f());
				}
				return this._draw;
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00008031 File Offset: 0x00006231
		private IEnumerable<bool> E_Draw_共通()
		{
			while (!this.Absorbable || 1 > Game.I.Player.SlowFrame || DDUtils.GetDistance(new D2Point(Game.I.Player.X, Game.I.Player.Y), new D2Point(this.X, this.Y)) >= 100.0)
			{
				yield return true;
			}
			Game.I.Enemies.Add(new Enemy_Item(this.X, this.Y, EnemyCommon.DROP_ITEM_TYPE_e.ABSORBABLE_SHOT, 1, this.AbsorbableWeapon));
			yield break;
		}

		// Token: 0x060000D7 RID: 215
		protected abstract IEnumerable<bool> E_Draw();

		// Token: 0x060000D8 RID: 216
		public abstract void Killed();

		// Token: 0x060000D9 RID: 217 RVA: 0x00008041 File Offset: 0x00006241
		public virtual void Damaged()
		{
			EnemyCommon.Damaged(this);
		}

		// Token: 0x1700000D RID: 13
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00008049 File Offset: 0x00006249
		public DDCrash Crash
		{
			set
			{
				value.OwnerEnemy = this;
				Game.I.EnemyCrashes.Add(value);
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00008063 File Offset: 0x00006263
		public virtual bool IsBoss()
		{
			return false;
		}

		// Token: 0x0400010F RID: 271
		public double X;

		// Token: 0x04000110 RID: 272
		public double Y;

		// Token: 0x04000111 RID: 273
		public Enemy.Kind_e Kind;

		// Token: 0x04000112 RID: 274
		public int HP;

		// Token: 0x04000113 RID: 275
		public int InitialHP;

		// Token: 0x04000114 RID: 276
		public int TransFrame;

		// Token: 0x04000115 RID: 277
		public int AbsorbableWeapon;

		// Token: 0x04000116 RID: 278
		public int OnFieldFrame;

		// Token: 0x04000117 RID: 279
		private Func<bool> _draw;

		// Token: 0x020000EE RID: 238
		public enum Kind_e
		{
			// Token: 0x040003B4 RID: 948
			ENEMY = 1,
			// Token: 0x040003B5 RID: 949
			TAMA,
			// Token: 0x040003B6 RID: 950
			ITEM
		}
	}
}
