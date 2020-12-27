using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x02000092 RID: 146
	public class DDTable<T>
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000259 RID: 601 RVA: 0x0000FC7F File Offset: 0x0000DE7F
		// (set) Token: 0x0600025A RID: 602 RVA: 0x0000FC87 File Offset: 0x0000DE87
		public int W { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0000FC90 File Offset: 0x0000DE90
		// (set) Token: 0x0600025C RID: 604 RVA: 0x0000FC98 File Offset: 0x0000DE98
		public int H { get; private set; }

		// Token: 0x0600025D RID: 605 RVA: 0x0000FCA1 File Offset: 0x0000DEA1
		public DDTable(int w, int h, Func<int, int, T> getCell) : this(w, h)
		{
			this.SetAllCell(getCell);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000FCB4 File Offset: 0x0000DEB4
		public DDTable(int w, int h)
		{
			if (w < 1 || 1000000000 < w || h < 1 || 1000000000 / w < h)
			{
				throw new DDError();
			}
			this.Inner = new T[w * h];
			this.W = w;
			this.H = h;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000FD04 File Offset: 0x0000DF04
		public void SetAllCell(Func<int, int, T> getCell)
		{
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					this[x, y] = getCell(x, y);
				}
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000FD44 File Offset: 0x0000DF44
		public void GetAllCell(Action<int, int, T> setCell)
		{
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					setCell(x, y, this[x, y]);
				}
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000FD84 File Offset: 0x0000DF84
		public void ChangeAllCell(Func<int, int, T, T> chgCell)
		{
			for (int x = 0; x < this.W; x++)
			{
				for (int y = 0; y < this.H; y++)
				{
					this[x, y] = chgCell(x, y, this[x, y]);
				}
			}
		}

		// Token: 0x1700001A RID: 26
		public T this[int x, int y]
		{
			get
			{
				return this.Inner[x + y * this.W];
			}
			set
			{
				this.Inner[x + y * this.W] = value;
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000FDFA File Offset: 0x0000DFFA
		public T GetCell(int x, int y, T defval = default(T))
		{
			if (x < 0 || this.W <= x || y < 0 || this.H <= y)
			{
				return defval;
			}
			return this[x, y];
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000FE20 File Offset: 0x0000E020
		public IEnumerable<T> Iterate()
		{
			int num;
			for (int index = 0; index < this.Inner.Length; index = num + 1)
			{
				yield return this.Inner[index];
				num = index;
			}
			yield break;
		}

		// Token: 0x04000204 RID: 516
		private T[] Inner;
	}
}
