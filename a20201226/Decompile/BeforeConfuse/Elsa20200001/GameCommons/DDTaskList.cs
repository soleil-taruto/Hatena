using System;

namespace Charlotte.GameCommons
{
	// Token: 0x02000093 RID: 147
	public class DDTaskList
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000FE30 File Offset: 0x0000E030
		public void Add(Func<bool> task)
		{
			this.Tasks.Add(task);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000FE40 File Offset: 0x0000E040
		public void ExecuteAllTask()
		{
			for (int index = 0; index < this.Tasks.Count; index++)
			{
				if (!this.Tasks[index]())
				{
					this.Tasks[index] = null;
				}
			}
			this.Tasks.RemoveAll((Func<bool> task) => task == null);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000FEAD File Offset: 0x0000E0AD
		public void Clear()
		{
			this.Tasks.Clear();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0000FEBA File Offset: 0x0000E0BA
		public int Count
		{
			get
			{
				return this.Tasks.Count;
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000FEC7 File Offset: 0x0000E0C7
		public void RemoveAt(int index)
		{
			this.Tasks.RemoveAt(index);
		}

		// Token: 0x04000207 RID: 519
		private DDList<Func<bool>> Tasks = new DDList<Func<bool>>();
	}
}
