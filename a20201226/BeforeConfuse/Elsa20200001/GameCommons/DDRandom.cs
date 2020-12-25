using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// 擬似乱数列
	/// </summary>
	public class DDRandom
	{
		private uint X;
		private uint Y;
		private uint Z;
		private uint A;

		public DDRandom()
			: this(SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt())
		{ }

		public DDRandom(uint x, uint y, uint z, uint a)
		{
			if ((x | y | z | a) == 0u)
				x = 1u;

			this.X = x;
			this.Y = y;
			this.Z = z;
			this.A = a;
		}

		/// <summary>
		/// [0,2^32)
		/// </summary>
		/// <returns>乱数</returns>
		public uint Next()
		{
			ulong uu1 = (ulong)this.Next2() << 32;
			ulong uu2 = (ulong)this.Next2() << 32;
			ulong uu3 = (ulong)this.Next2() << 32;
			ulong uu4 = (ulong)this.Next2() << 32;
			ulong uu5 = (ulong)this.Next2() << 32;
			ulong uu6 = (ulong)this.Next2() << 32;
			ulong uu7 = (ulong)this.Next2() << 32;
			ulong uu8 = (ulong)this.Next2() << 32;
			ulong uu9 = (ulong)this.Next2() << 32;

			uint u1 = (uint)(uu1 % 4294967311ul); // 2^32 以降 1 番目の素数
			uint u2 = (uint)(uu2 % 4294967357ul); // 2^32 以降 2 番目の素数
			uint u3 = (uint)(uu3 % 4294967371ul); // 2^32 以降 3 番目の素数
			uint u4 = (uint)(uu4 % 4294967377ul); // 2^32 以降 4 番目の素数
			uint u5 = (uint)(uu5 % 4294967387ul); // 2^32 以降 5 番目の素数
			uint u6 = (uint)(uu6 % 4294967389ul); // 2^32 以降 6 番目の素数
			uint u7 = (uint)(uu7 % 4294967459ul); // 2^32 以降 7 番目の素数
			uint u8 = (uint)(uu8 % 4294967477ul); // 2^32 以降 8 番目の素数
			uint u9 = (uint)(uu9 % 4294967497ul); // 2^32 以降 9 番目の素数

			return u1 ^ u2 ^ u3 ^ u4 ^ u5 ^ u6 ^ u7 ^ u8 ^ u9;
		}

		private uint Next2()
		{
			// Xorshift-128

			uint t;

			t = this.X;
			t ^= this.X << 11;
			t ^= t >> 8;
			t ^= this.A;
			t ^= this.A >> 19;
			this.X = this.Y;
			this.Y = this.Z;
			this.Z = this.A;
			this.A = t;

			return t;
		}

		/// <summary>
		/// [0,1]
		/// 0以上,1以下
		/// </summary>
		/// <returns>乱数</returns>
		public double Real()
		{
			return this.Next() / (double)uint.MaxValue;
		}

		/// <summary>
		/// [0,1)
		/// 0以上,1未満
		/// </summary>
		/// <returns>乱数</returns>
		public double Real2()
		{
			return this.Next() / (double)(uint.MaxValue + 1L);
		}

		/// <summary>
		/// (0,1)
		/// 0より大きい,1未満
		/// </summary>
		/// <returns>乱数</returns>
		public double Real3()
		{
			return this.Next() / (double)(uint.MaxValue + 1L) + 0.5;
		}

		public uint GetUInt(uint modulo)
		{
			if (modulo < 1u)
				throw new ArgumentException("Bad modulo: " + modulo);

			return this.Next() % modulo;
		}

		public int GetInt(int modulo)
		{
			return (int)this.GetUInt((uint)modulo);
		}

		public void Shuffle<T>(T[] arr)
		{
			for (int index = arr.Length; 2 <= index; index--)
				SCommon.Swap(arr, this.GetInt(index), index - 1);
		}

		public int GetRange(int minval, int maxval)
		{
			return GetInt(maxval - minval + 1) - minval;
		}
	}
}
