using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.Scripts
{
	/// <summary>
	/// スクリプト
	/// </summary>
	public abstract class Script
	{
		private Func<bool> _eachFrame = null;

		public Func<bool> EachFrame
		{
			get
			{
				if (_eachFrame == null)
					_eachFrame = SCommon.Supplier(this.E_EachFrame());

				return _eachFrame;
			}
		}

		/// <summary>
		/// 真を返し続けること。
		/// 偽を返すと、このスクリプトを終了する。
		/// 処理すべきこと：
		/// -- 処理(内部状態を1フレーム分更新)
		/// -- Game.I.Enemies への追加
		/// -- Game.I.Walls への追加
		/// -- 曲再生
		/// -- イベント発生
		/// -- その他の制御
		/// </summary>
		/// <returns>このスクリプトを継続する</returns>
		protected abstract IEnumerable<bool> E_EachFrame();
	}
}
