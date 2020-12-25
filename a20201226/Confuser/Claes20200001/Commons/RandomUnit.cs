using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Charlotte.Commons
{
	public class RandomUnit : IDisposable
	{
		public interface IRandomNumberGenerator : IDisposable
		{
			byte[] GetBlock();
		}

		private IRandomNumberGenerator Rng;

		public RandomUnit(IRandomNumberGenerator rng)
		{
			this.Rng = rng;
		}

		public void Dispose()
		{
			if (this.Rng != null)
			{
				this.Rng.Dispose();
				this.Rng = null;
			}
		}

		private byte[] Cache = SCommon.EMPTY_BYTES;
		private int RIndex = 0;

		public byte GetByte()
		{
			if (this.Cache.Length <= this.RIndex)
			{
				this.Cache = this.Rng.GetBlock();
				this.RIndex = 0;
			}
			return this.Cache[this.RIndex++];
		}

		public byte[] GetBytes(int length)
		{
			byte[] dest = new byte[length];

			for (int index = 0; index < length; index++)
				dest[index] = this.GetByte();

			return dest;
		}

		public uint GetUInt16()
		{
			byte[] r = GetBytes(2);

			return
				((uint)r[0] << 0) |
				((uint)r[1] << 8);
		}

		public uint GetUInt24()
		{
			byte[] r = GetBytes(3);

			return
				((uint)r[0] << 0) |
				((uint)r[1] << 8) |
				((uint)r[2] << 16);
		}

		public uint GetUInt()
		{
			byte[] r = GetBytes(4);

			return
				((uint)r[0] << 0) |
				((uint)r[1] << 8) |
				((uint)r[2] << 16) |
				((uint)r[3] << 24);
		}

		public ulong GetUInt64()
		{
			byte[] r = GetBytes(8);

			return
				((ulong)r[0] << 0) |
				((ulong)r[1] << 8) |
				((ulong)r[2] << 16) |
				((ulong)r[3] << 24) |
				((ulong)r[4] << 32) |
				((ulong)r[5] << 40) |
				((ulong)r[6] << 48) |
				((ulong)r[7] << 56);
		}

		public ulong GetUInt64_M(ulong modulo)
		{
			if (modulo == 0ul)
				throw new ArgumentOutOfRangeException("modulo is zero");

			ulong m = (ulong.MaxValue % modulo + 1ul) % modulo;
			ulong r;

			do
			{
				r = this.GetUInt64();
			}
			while (r < m);

			r %= modulo;

			return r;
		}

		public uint GetUInt_M(uint modulo)
		{
			return (uint)this.GetUInt64_M((ulong)modulo);
		}

		public int GetInt(int modulo)
		{
			return (int)this.GetUInt_M((uint)modulo);
		}

		public int GetRange(int minval, int maxval)
		{
			return this.GetInt(maxval - minval + 1) + minval;
		}

		public T ChooseOne<T>(T[] arr)
		{
			return arr[this.GetInt(arr.Length)];
		}

		public void Shuffle<T>(T[] arr)
		{
			for (int index = arr.Length; 1 < index; index--)
			{
				SCommon.Swap(arr, this.GetInt(index), index - 1);
			}
		}
	}
}
