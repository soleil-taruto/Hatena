using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public class DDTable<T>
	{
		private T[] Inner;

		public int W { get; private set; }
		public int H { get; private set; }

		public DDTable(int w, int h, Func<int, int, T> getCell)
			: this(w, h)
		{
			this.SetAllCell(getCell);
		}

		public DDTable(int w, int h)
		{
			if (
				w < 1 || SCommon.IMAX < w ||
				h < 1 || SCommon.IMAX / w < h
				)
				throw new DDError();

			this.Inner = new T[w * h];
			this.W = w;
			this.H = h;
		}

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

		public T GetCell(int x, int y, T defval = default(T))
		{
			if (
				x < 0 || this.W <= x ||
				y < 0 || this.H <= y
				)
				return defval;

			return this[x, y];
		}

		public IEnumerable<T> Iterate()
		{
#if true
			for (int index = 0; index < this.Inner.Length; index++)
			{
				yield return this.Inner[index];
			}
#else
			return this.Inner;
#endif
		}
	}
}
