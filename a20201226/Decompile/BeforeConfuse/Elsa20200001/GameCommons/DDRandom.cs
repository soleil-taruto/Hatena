using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000085 RID: 133
	public class DDRandom
	{
		// Token: 0x06000205 RID: 517 RVA: 0x0000D500 File Offset: 0x0000B700
		public DDRandom() : this(SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt(), SCommon.CRandom.GetUInt())
		{
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000D530 File Offset: 0x0000B730
		public DDRandom(uint x, uint y, uint z, uint a)
		{
			if ((x | y | z | a) == 0u)
			{
				x = 1u;
			}
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.A = a;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000D564 File Offset: 0x0000B764
		public uint Next()
		{
			long num = (long)((long)((ulong)this.Next2()) << 32);
			ulong uu2 = (ulong)this.Next2() << 32;
			ulong uu3 = (ulong)this.Next2() << 32;
			ulong uu4 = (ulong)this.Next2() << 32;
			ulong uu5 = (ulong)this.Next2() << 32;
			ulong uu6 = (ulong)this.Next2() << 32;
			ulong uu7 = (ulong)this.Next2() << 32;
			ulong uu8 = (ulong)this.Next2() << 32;
			ulong uu9 = (ulong)this.Next2() << 32;
			uint num2 = (uint)(num % 4294967311L);
			uint u2 = (uint)(uu2 % 4294967357UL);
			uint u3 = (uint)(uu3 % 4294967371UL);
			uint u4 = (uint)(uu4 % 4294967377UL);
			uint u5 = (uint)(uu5 % 4294967387UL);
			uint u6 = (uint)(uu6 % 4294967389UL);
			uint u7 = (uint)(uu7 % 4294967459UL);
			uint u8 = (uint)(uu8 % 4294967477UL);
			uint u9 = (uint)(uu9 % 4294967497UL);
			return num2 ^ u2 ^ u3 ^ u4 ^ u5 ^ u6 ^ u7 ^ u8 ^ u9;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000D670 File Offset: 0x0000B870
		private uint Next2()
		{
			uint t = this.X;
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

		// Token: 0x06000209 RID: 521 RVA: 0x0000D6D7 File Offset: 0x0000B8D7
		public double Real()
		{
			return this.Next() / 4294967295.0;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000D6EB File Offset: 0x0000B8EB
		public double Real2()
		{
			return this.Next() / 4294967296.0;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000D6FF File Offset: 0x0000B8FF
		public double Real3()
		{
			return this.Next() / 4294967296.0 + 0.5;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000D71D File Offset: 0x0000B91D
		public uint GetUInt(uint modulo)
		{
			if (modulo < 1u)
			{
				throw new ArgumentException("Bad modulo: " + modulo.ToString());
			}
			return this.Next() % modulo;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D742 File Offset: 0x0000B942
		public int GetInt(int modulo)
		{
			return (int)this.GetUInt((uint)modulo);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000D74C File Offset: 0x0000B94C
		public void Shuffle<T>(T[] arr)
		{
			int index = arr.Length;
			while (2 <= index)
			{
				SCommon.Swap<T>(arr, this.GetInt(index), index - 1);
				index--;
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000D777 File Offset: 0x0000B977
		public int GetRange(int minval, int maxval)
		{
			return this.GetInt(maxval - minval + 1) - minval;
		}

		// Token: 0x040001DD RID: 477
		private uint X;

		// Token: 0x040001DE RID: 478
		private uint Y;

		// Token: 0x040001DF RID: 479
		private uint Z;

		// Token: 0x040001E0 RID: 480
		private uint A;
	}
}
