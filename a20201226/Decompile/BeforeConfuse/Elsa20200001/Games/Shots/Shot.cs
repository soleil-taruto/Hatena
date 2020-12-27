using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x0200002F RID: 47
	public abstract class Shot
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00007B3C File Offset: 0x00005D3C
		public Shot(double x, double y, Shot.Kind_e kind, int attackPoint)
		{
			this.X = x;
			this.Y = y;
			this.Kind = kind;
			this.AttackPoint = attackPoint;
			this.LastX = x;
			this.LastY = y;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00007B6F File Offset: 0x00005D6F
		public Func<bool> Draw
		{
			get
			{
				this.LastX = this.X;
				this.LastY = this.Y;
				if (this._draw == null)
				{
					this._draw = SCommon.Supplier<bool>(this.E_Draw());
				}
				return this._draw;
			}
		}

		// Token: 0x06000093 RID: 147
		protected abstract IEnumerable<bool> E_Draw();

		// Token: 0x17000009 RID: 9
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00007BA8 File Offset: 0x00005DA8
		public DDCrash Crash
		{
			set
			{
				value.OwnerShot = this;
				Game.I.ShotCrashes.Add(value);
			}
		}

		// Token: 0x040000F2 RID: 242
		public double X;

		// Token: 0x040000F3 RID: 243
		public double Y;

		// Token: 0x040000F4 RID: 244
		public Shot.Kind_e Kind;

		// Token: 0x040000F5 RID: 245
		public int AttackPoint;

		// Token: 0x040000F6 RID: 246
		public double LastX;

		// Token: 0x040000F7 RID: 247
		public double LastY;

		// Token: 0x040000F8 RID: 248
		public bool Vanished;

		// Token: 0x040000F9 RID: 249
		private Func<bool> _draw;

		// Token: 0x020000CF RID: 207
		public enum Kind_e
		{
			// Token: 0x0400031B RID: 795
			NORMAL = 1,
			// Token: 0x0400031C RID: 796
			BOMB
		}
	}
}
