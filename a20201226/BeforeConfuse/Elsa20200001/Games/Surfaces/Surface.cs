using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	/// <summary>
	/// サーフェス
	/// </summary>
	public abstract class Surface
	{
		public string Name;
		public double X = DDConsts.Screen_W / 2.0;
		public double Y = DDConsts.Screen_H / 2.0;

		// <---- prm

		/// <summary>
		/// このサーフェスを終了するか
		/// </summary>
		public bool DeadFlag = false;

		private Func<bool> _draw = null;

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw()
		{
			if (_draw == null)
				_draw = SCommon.Supplier(this.E_Draw());

			if (!_draw())
				this.DeadFlag = true;
		}

		/// <summary>
		/// 描画シーケンス
		/// </summary>
		/// <returns>このサーフェスを継続するか</returns>
		public abstract IEnumerable<bool> E_Draw();

		/// <summary>
		/// 共通コマンド処理
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <param name="arguments">コマンド引数</param>
		public void Invoke(string command, string[] arguments)
		{
			int c = 0;

			if (command == "End")
			{
				this.DeadFlag = true;
			}
			if (command == "XY")
			{
				double x = double.Parse(arguments[c++]);
				double y = double.Parse(arguments[c++]);

				this.X = x;
				this.Y = y;
			}
			else
			{
				this.Invoke_02(command, arguments);
			}
		}

		/// <summary>
		/// 個別コマンド処理
		/// </summary>
		/// <param name="command">コマンド</param>
		/// <param name="arguments">コマンド引数</param>
		public virtual void Invoke_02(string command, string[] arguments)
		{
			throw new DDError("そんなコマンドありません：" + command);
		}
	}
}
