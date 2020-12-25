using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons.MaskGZDataUtils
{
	public class MaskGZDataEng
	{
		private void MaskSignature(byte[] data)
		{
			int size = Math.Min(16, data.Length);

			for (int index = 0; index < size; index++)
			{
				data[index] ^= (byte)(index + 240);
			}
		}

		private uint X;

		private uint Rand()
		{
			// Xorshift-32

			this.X ^= this.X << 13;
			this.X ^= this.X >> 17;
			this.X ^= this.X << 5;

			return this.X;
		}

		private void Shuffle(int[] values)
		{
			for (int index = values.Length; 2 <= index; index--)
			{
				int a = index - 1;
				int b = (int)(this.Rand() % (uint)index);

				int tmp = values[a];
				values[a] = values[b];
				values[b] = tmp;
			}
		}

		private void Transpose(byte[] data, string seed)
		{
			this.MaskSignature(data);

			int[] swapIdxLst = Enumerable.Range(1, data.Length / 2).ToArray();

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

		public void Transpose(byte[] data)
		{
			this.Transpose(data, "2020092807"); // 難読化貢献のため seed を文字列化しておく
		}
	}
}
