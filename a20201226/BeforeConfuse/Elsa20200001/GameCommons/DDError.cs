using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public class DDError : Exception
	{
		public DDError()
			: this("エラーが発生しました。")
		{ }

		public DDError(string message) // 難読化のため、デフォルト引数をオーバーロードの引数に指定する。
			: base(message)
		{ }
	}
}
