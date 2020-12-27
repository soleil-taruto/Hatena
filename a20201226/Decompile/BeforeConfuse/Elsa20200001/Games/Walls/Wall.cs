using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.Games.Walls
{
	// Token: 0x0200001F RID: 31
	public abstract class Wall
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000716C File Offset: 0x0000536C
		public Func<bool> Draw
		{
			get
			{
				if (this._draw == null)
				{
					this._draw = SCommon.Supplier<bool>(this.E_Draw());
				}
				return this._draw;
			}
		}

		// Token: 0x06000067 RID: 103
		protected abstract IEnumerable<bool> E_Draw();

		// Token: 0x040000DD RID: 221
		public bool Filled;

		// Token: 0x040000DE RID: 222
		private Func<bool> _draw;
	}
}
