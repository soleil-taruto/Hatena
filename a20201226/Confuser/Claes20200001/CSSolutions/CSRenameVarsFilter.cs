using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.CSSolutions
{
	public class CSRenameVarsFilter
	{
		/// <summary>
		/// テスト用
		/// null == 無効
		/// null 以外 == この文字列を「置き換え禁止ワードのリスト」から除去する。
		/// </summary>
		public static string 置き換え禁止ワードの例外ワード = null;

		/// <summary>
		/// テスト用
		/// </summary>
		/// <returns>置き換え禁止ワードのリスト</returns>
		public string[] Get置き換え禁止ワードのリスト()
		{
			return this.置き換え禁止ワードのリスト;
		}

		private string[] 置き換え禁止ワードのリスト = SCommon.TextToLines(CSResources.予約語リスト + Consts.CRLF + CSResources.予約語クラス名リスト)
			.Select(v => v.Trim())
			.Where(v => v != "" && v[0] != ';') // ? 空行ではない && コメント行ではない
			.Where(v => v != 置き換え禁止ワードの例外ワード) // テスト用
			.ToArray();

		private Dictionary<string, string> 変換テーブル = SCommon.CreateDictionary<string>();

		public string Filter(string name)
		{
			if (
				name == "" ||
				SCommon.DECIMAL.Contains(name[0]) ||
				this.置き換え禁止ワードのリスト.Contains(name)
				)
				return name;

			string nameNew;

			if (this.変換テーブル.ContainsKey(name))
			{
				nameNew = this.変換テーブル[name];
			}
			else
			{
				nameNew = this.CreateNameNew();
				this.変換テーブル.Add(name, nameNew);
			}
			return nameNew;
		}

		private Dictionary<string, object> CNN_Names = SCommon.CreateDictionary<object>();

		public string CreateNameNew()
		{
			string nameNew;
			int countTry = 0;

			do
			{
				if (1000 < ++countTry)
					throw new Exception("想定外のトライ回数 -- 非常に運が悪いか NameNew をほぼ生成し尽くした。");

				nameNew = this.TryCreateNameNew();
			}
			while (this.CNN_Names.ContainsKey(nameNew) || this.置き換え禁止ワードのリスト.Contains(nameNew));

			this.CNN_Names.Add(nameNew, null);
			return nameNew;
		}

		private string[] ランダムな単語リスト = SCommon.TextToLines(CSResources.ランダムな単語リスト)
			.Select(v => v.Trim())
			.Where(v => v != "" && v[0] != ';') // ? 空行ではない && コメント行ではない
			.ToArray();

		private string[] 英単語リスト_前置詞 = Get英単語リスト("前置詞");
		private string[] 英単語リスト_形容詞 = Get英単語リスト("形容詞");
		private string[] 英単語リスト_代名詞 = Get英単語リスト("代名詞");
		private string[] 英単語リスト_名詞 = Get英単語リスト("名詞");
		private string[] 英単語リスト_副詞 = Get英単語リスト("副詞");
		private string[] 英単語リスト_動詞 = Get英単語リスト("動詞");

		private static string[] Get英単語リスト(string 品詞)
		{
			return SCommon.TextToLines(CSResources.英単語リスト)
				.Select(v => v.Trim())
				.Where(v => v != "" && v[0] != ';') // ? 空行ではない && コメント行ではない
				.Where(v => v.Contains(品詞)) // 品詞の絞り込み
				.Select(v => v.Substring(0, v.IndexOf('\t'))) // 品詞の部分を除去
				.Select(v => v.Substring(0, 1).ToUpper() + v.Substring(1).ToLower()) // 先頭の文字だけ大文字にする。-- 全て小文字のはずなので .ToLower() は不要だけど念の為
				.ToArray();
		}

		/// <summary>
		/// 新しい識別子を作成する。
		/// 標準のクラス名 List, StringBuilder などと被らない名前を返すこと。
		/// -- 今の実装は厳密にこれを回避していない。@ 2020.11.x
		/// </summary>
		/// <returns>新しい識別子</returns>
		private string TryCreateNameNew()
		{
#if !true // ランダムな単語列 ver
			StringBuilder buff = new StringBuilder();
			int count = SCommon.CRandom.GetRange(3, 5);

			for (int index = 0; index < count; index++)
				buff.Append(SCommon.CRandom.ChooseOne(this.ランダムな単語リスト));

			return buff.ToString();
#else // 英語名 ver
			return
				SCommon.CRandom.ChooseOne(this.英単語リスト_動詞) +
				SCommon.CRandom.ChooseOne(this.英単語リスト_形容詞) +
				SCommon.CRandom.ChooseOne(this.ランダムな単語リスト) +
				SCommon.CRandom.ChooseOne(this.英単語リスト_名詞);
#endif
		}

		private string[] 予約語クラス名リスト = SCommon.TextToLines(CSResources.予約語クラス名リスト)
			.Select(v => v.Trim())
			.Where(v => v != "" && v[0] != ';') // ? 空行ではない && コメント行ではない
			.ToArray();

		public bool Is予約語クラス名(string name)
		{
			return 予約語クラス名リスト.Contains(name);
		}
	}
}
