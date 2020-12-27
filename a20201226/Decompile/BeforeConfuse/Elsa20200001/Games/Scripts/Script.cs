using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200003B RID: 59
	public abstract class Script
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00007DE7 File Offset: 0x00005FE7
		public Func<bool> EachFrame
		{
			get
			{
				if (this._eachFrame == null)
				{
					this._eachFrame = SCommon.Supplier<bool>(this.E_EachFrame());
				}
				return this._eachFrame;
			}
		}

		// Token: 0x060000AC RID: 172
		protected abstract IEnumerable<bool> E_EachFrame();

		// Token: 0x04000107 RID: 263
		private Func<bool> _eachFrame;
	}
}
