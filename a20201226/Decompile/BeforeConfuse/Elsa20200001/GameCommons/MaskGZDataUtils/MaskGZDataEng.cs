using System;
using System.Linq;

namespace Charlotte.GameCommons.MaskGZDataUtils
{
	// Token: 0x0200009D RID: 157
	public class MaskGZDataEng
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x00010FA8 File Offset: 0x0000F1A8
		private void MaskSignature(byte[] data)
		{
			int size = Math.Min(16, data.Length);
			for (int index = 0; index < size; index++)
			{
				int num = index;
				data[num] ^= (byte)(index + 240);
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00010FE0 File Offset: 0x0000F1E0
		private uint Rand()
		{
			this.X ^= this.X << 13;
			this.X ^= this.X >> 17;
			this.X ^= this.X << 5;
			return this.X;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00011034 File Offset: 0x0000F234
		private void Shuffle(int[] values)
		{
			int index = values.Length;
			while (2 <= index)
			{
				int a = index - 1;
				int b = (int)(this.Rand() % (uint)index);
				int tmp = values[a];
				values[a] = values[b];
				values[b] = tmp;
				index--;
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0001106C File Offset: 0x0000F26C
		private void Transpose(byte[] data, string seed)
		{
			this.MaskSignature(data);
			int[] swapIdxLst = Enumerable.Range(1, data.Length / 2).ToArray<int>();
			this.X = uint.Parse(seed);
			this.Shuffle(swapIdxLst);
			for (int index = 0; index < swapIdxLst.Length; index++)
			{
				int a = index;
				int b = data.Length - swapIdxLst[index];
				byte tmp = data[a];
				data[a] = data[b];
				data[b] = tmp;
			}
			this.MaskSignature(data);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x000110D3 File Offset: 0x0000F2D3
		public void Transpose(byte[] data)
		{
			this.Transpose(data, "2020092807");
		}

		// Token: 0x0400021B RID: 539
		private uint X;
	}
}
