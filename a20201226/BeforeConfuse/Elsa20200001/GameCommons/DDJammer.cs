using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons.MaskGZDataUtils;

namespace Charlotte.GameCommons
{
	public static class DDJammer
	{
		public static byte[] Encode(byte[] data)
		{
			data = SCommon.Compress(data);
			MaskGZData(data);
			return data;
		}

		public static byte[] Decode(byte[] data)
		{
			MaskGZData(data);
			byte[] ret = SCommon.Decompress(data);
			//MaskGZData(data); // 復元
			return ret;
		}

		private static void MaskGZData(byte[] data)
		{
			new MaskGZDataEng().Transpose(data);
		}
	}
}
