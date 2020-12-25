using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.Games.Walls
{
	/// <summary>
	/// 壁紙
	/// 壁紙の重ね合わせ(壁紙リスト), Filled == true によって、それより下の(古い)壁紙が除去される方式
	/// </summary>
	public abstract class Wall
	{
		/// <summary>
		/// この壁紙によってフィールド全体が描画されたことを示す。
		/// 真である場合、この壁紙の裏側の壁紙を終了するかもしれない。
		/// よって真をセットしてから偽をセットし直すことは想定していない。
		/// </summary>
		public bool Filled = false;

		private Func<bool> _draw = null;

		public Func<bool> Draw
		{
			get
			{
				if (_draw == null)
					_draw = SCommon.Supplier(this.E_Draw());

				return _draw;
			}
		}

		/// <summary>
		/// 真を返し続けること。
		/// 偽を返すと、この壁紙を終了する。
		/// 処理すべきこと：
		/// -- 処理(内部状態を1フレーム分更新)
		/// -- 描画
		/// -- 必要に応じて Filled に真をセットする。
		/// </summary>
		/// <returns>この壁紙を継続する</returns>
		protected abstract IEnumerable<bool> E_Draw();
	}
}
