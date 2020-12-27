using System;
using Charlotte.Commons;
using Charlotte.GameCommons.MaskGZDataUtils;

namespace Charlotte.GameCommons
{
	// Token: 0x02000076 RID: 118
	public static class DDJammer
	{
		// Token: 0x0600019B RID: 411 RVA: 0x0000BC0D File Offset: 0x00009E0D
		public static byte[] Encode(byte[] data)
		{
			data = SCommon.Compress(data);
			DDJammer.MaskGZData(data);
			return data;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000BC1E File Offset: 0x00009E1E
		public static byte[] Decode(byte[] data)
		{
			DDJammer.MaskGZData(data);
			return SCommon.Decompress(data, -1);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000BC2D File Offset: 0x00009E2D
		private static void MaskGZData(byte[] data)
		{
			new MaskGZDataEng().Transpose(data);
		}
	}
}
