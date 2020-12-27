using System;

namespace Charlotte.Commons
{
	// Token: 0x020000AC RID: 172
	public class RandomUnit : IDisposable
	{
		// Token: 0x0600031C RID: 796 RVA: 0x00011E0C File Offset: 0x0001000C
		public RandomUnit(RandomUnit.IRandomNumberGenerator rng)
		{
			this.Rng = rng;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00011E26 File Offset: 0x00010026
		public void Dispose()
		{
			if (this.Rng != null)
			{
				this.Rng.Dispose();
				this.Rng = null;
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00011E44 File Offset: 0x00010044
		public byte GetByte()
		{
			if (this.Cache.Length <= this.RIndex)
			{
				this.Cache = this.Rng.GetBlock();
				this.RIndex = 0;
			}
			byte[] cache = this.Cache;
			int rindex = this.RIndex;
			this.RIndex = rindex + 1;
			return cache[rindex];
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00011E94 File Offset: 0x00010094
		public byte[] GetBytes(int length)
		{
			byte[] dest = new byte[length];
			for (int index = 0; index < length; index++)
			{
				dest[index] = this.GetByte();
			}
			return dest;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00011EC0 File Offset: 0x000100C0
		public uint GetUInt16()
		{
			byte[] r = this.GetBytes(2);
			return (uint)((int)r[0] | (int)r[1] << 8);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00011EE0 File Offset: 0x000100E0
		public uint GetUInt24()
		{
			byte[] r = this.GetBytes(3);
			return (uint)((int)r[0] | (int)r[1] << 8 | (int)r[2] << 16);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00011F08 File Offset: 0x00010108
		public uint GetUInt()
		{
			byte[] r = this.GetBytes(4);
			return (uint)((int)r[0] | (int)r[1] << 8 | (int)r[2] << 16 | (int)r[3] << 24);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00011F34 File Offset: 0x00010134
		public ulong GetUInt64()
		{
			byte[] r = this.GetBytes(8);
			return (ulong)r[0] | (ulong)r[1] << 8 | (ulong)r[2] << 16 | (ulong)r[3] << 24 | (ulong)r[4] << 32 | (ulong)r[5] << 40 | (ulong)r[6] << 48 | (ulong)r[7] << 56;
		}

		// Token: 0x04000249 RID: 585
		private RandomUnit.IRandomNumberGenerator Rng;

		// Token: 0x0400024A RID: 586
		private byte[] Cache = SCommon.EMPTY_BYTES;

		// Token: 0x0400024B RID: 587
		private int RIndex;

		// Token: 0x02000152 RID: 338
		public interface IRandomNumberGenerator : IDisposable
		{
			// Token: 0x060006AB RID: 1707
			byte[] GetBlock();
		}
	}
}
