using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public class DDList<T>
	{
		private List<T> Inner = new List<T>();

		public int Count { get; private set; }

		public DDList()
		{
			//this.Count = 0;
		}

		public void Clear()
		{
			this.Count = 0;
		}

		public void Add(T element)
		{
			if (this.Count < this.Inner.Count)
				this.Inner[this.Count] = element;
			else
				this.Inner.Add(element);

			this.Count++;
		}

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

		private void CheckIndex(int index)
		{
			if (index < 0 || this.Count <= index)
				throw new DDError("Bad index: " + index);
		}

		private void CheckRange(int offset, int count)
		{
			if (offset < 0 || this.Count <= offset)
				throw new DDError("Bad offset: " + offset);

			if (count < 0 || this.Count - offset < count)
				throw new DDError("Bad count: " + count);
		}

		public void RemoveAt(int index)
		{
			this.CheckIndex(index);

			this.Inner.RemoveAt(index);
			this.Count--;
		}

		public void FastRemoveAt(int index)
		{
			this.CheckIndex(index);

			this.Inner[index] = this.Inner[--this.Count];
			this.Inner[this.Count] = default(T);
		}

		public void RemoveRange(int offset, int count)
		{
			this.CheckRange(offset, count);

			for (int index = offset; index + count < this.Count; index++)
				this.Inner[index] = this.Inner[index + count];

			this.Count -= count;
		}

		public void RemoveAll(Predicate<T> match)
		{
			for (int r = 0; r < this.Count; r++)
			{
				if (match(this.Inner[r]))
				{
					int w = r;

					while (++r < this.Count)
						if (!match(this.Inner[r]))
							this.Inner[w++] = this.Inner[r];

					for (int index = w; index < this.Count; index++)
						this.Inner[index] = default(T);

					this.Count = w;
					return;
				}
			}
		}

		public IEnumerable<T> Iterate()
		{
#if false
			return this.Inner; // 要素が変更・追加されると例外を投げるっぽい。
#else
			for (int index = 0; index < this.Count; index++)
			{
				yield return this.Inner[index];
			}
#endif
		}
	}
}
