using System;
using System.Collections.Generic;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000065 RID: 101
	public abstract class DDTask
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00009E5F File Offset: 0x0000805F
		public bool Execute()
		{
			return this.Task();
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00009E6C File Offset: 0x0000806C
		public Func<bool> Task
		{
			get
			{
				if (this._task == null)
				{
					this._task = SCommon.Supplier<bool>(this.E_Task());
				}
				return this._task;
			}
		}

		// Token: 0x0600013D RID: 317
		public abstract IEnumerable<bool> E_Task();

		// Token: 0x04000161 RID: 353
		private Func<bool> _task;
	}
}
