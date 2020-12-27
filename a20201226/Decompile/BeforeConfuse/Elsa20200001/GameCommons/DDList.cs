using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x02000078 RID: 120
	public class DDList<T>
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000BCFA File Offset: 0x00009EFA
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000BD02 File Offset: 0x00009F02
		public int Count { get; private set; }

		// Token: 0x060001A6 RID: 422 RVA: 0x0000BD1E File Offset: 0x00009F1E
		public void Clear()
		{
			this.Count = 0;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000BD28 File Offset: 0x00009F28
		public void Add(T element)
		{
			if (this.Count < this.Inner.Count)
			{
				this.Inner[this.Count] = element;
			}
			else
			{
				this.Inner.Add(element);
			}
			int count = this.Count;
			this.Count = count + 1;
		}

		// Token: 0x17000013 RID: 19
		public T this[int index]
		{
			get
			{
				return this.Inner[index];
			}
			set
			{
				this.Inner[index] = value;
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000BD95 File Offset: 0x00009F95
		private void CheckIndex(int index)
		{
			if (index < 0 || this.Count <= index)
			{
				throw new DDError("Bad index: " + index.ToString());
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000BDBC File Offset: 0x00009FBC
		private void CheckRange(int offset, int count)
		{
			if (offset < 0 || this.Count <= offset)
			{
				throw new DDError("Bad offset: " + offset.ToString());
			}
			if (count < 0 || this.Count - offset < count)
			{
				throw new DDError("Bad count: " + count.ToString());
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000BE14 File Offset: 0x0000A014
		public void RemoveAt(int index)
		{
			this.CheckIndex(index);
			this.Inner.RemoveAt(index);
			int count = this.Count;
			this.Count = count - 1;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000BE44 File Offset: 0x0000A044
		public void FastRemoveAt(int index)
		{
			this.CheckIndex(index);
			List<T> inner = this.Inner;
			List<T> inner2 = this.Inner;
			int num = this.Count - 1;
			this.Count = num;
			inner[index] = inner2[num];
			this.Inner[this.Count] = default(T);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000BE9C File Offset: 0x0000A09C
		public void RemoveRange(int offset, int count)
		{
			this.CheckRange(offset, count);
			int index = offset;
			while (index + count < this.Count)
			{
				this.Inner[index] = this.Inner[index + count];
				index++;
			}
			this.Count -= count;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		public void RemoveAll(Predicate<T> match)
		{
			for (int r = 0; r < this.Count; r++)
			{
				if (match(this.Inner[r]))
				{
					int w = r;
					while (++r < this.Count)
					{
						if (!match(this.Inner[r]))
						{
							this.Inner[w++] = this.Inner[r];
						}
					}
					for (int index = w; index < this.Count; index++)
					{
						this.Inner[index] = default(T);
					}
					this.Count = w;
					return;
				}
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000BF93 File Offset: 0x0000A193
		public IEnumerable<T> Iterate()
		{
			int num;
			for (int index = 0; index < this.Count; index = num + 1)
			{
				yield return this.Inner[index];
				num = index;
			}
			yield break;
		}

		// Token: 0x040001B9 RID: 441
		private List<T> Inner = new List<T>();
	}
}
