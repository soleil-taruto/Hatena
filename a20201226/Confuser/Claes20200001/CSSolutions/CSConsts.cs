using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.CSSolutions
{
	public static class CSConsts
	{
		/// <summary>
		/// このパターンで始まるC++コメントは除去しない。
		/// この文字列は、コメント開始の // を含む
		/// </summary>
		public const string KEEP_COMMENT_START_PATTERN = "// KeepComment:@^_ConfuserElsa";

		/// <summary>
		/// このパターンで終わる行については名前の置き換えを行わない。
		/// </summary>
		public const string NO_RENAME_LINE_SUFFIX = "// NoRename:@^_ConfuserElsa";
	}
}
