using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	/// <summary>
	/// 自弾
	/// プレイヤーのショット
	/// </summary>
	public abstract class Shot
	{
		public double X;
		public double Y;

		public enum Kind_e
		{
			NORMAL = 1, // 通常弾
			BOMB, // ボム
		}

		/// <summary>
		/// 弾の種類
		/// コンストラクタで値を設定すること。
		/// </summary>
		public Kind_e Kind;

		/// <summary>
		/// 攻撃力
		/// コンストラクタで値を設定すること。
		/// </summary>
		public int AttackPoint;

		public double LastX;
		public double LastY;

		public Shot(double x, double y, Kind_e kind, int attackPoint)
		{
			this.X = x;
			this.Y = y;
			this.Kind = kind;
			this.AttackPoint = attackPoint;
			this.LastX = x;
			this.LastY = y;
		}

		/// <summary>
		/// 敵に当たって消滅したか
		/// </summary>
		public bool Vanished = false;

		private Func<bool> _draw = null;

		public Func<bool> Draw
		{
			get
			{
				this.LastX = this.X;
				this.LastY = this.Y;

				if (_draw == null)
					_draw = SCommon.Supplier(this.E_Draw());

				return _draw;
			}
		}

		/// <summary>
		/// 真を返し続けること。
		/// 偽を返すと、このショットは消滅する。
		/// 処理すべきこと：
		/// -- 行動
		/// -- 描画
		/// -- 当たり判定の設置 -- this.Crash = crash;
		/// </summary>
		/// <returns>このショットは生存しているか</returns>
		protected abstract IEnumerable<bool> E_Draw();

		/// <summary>
		/// このフレームに於けるこの自弾の当たり判定を設定する。
		/// 他のプロジェクトと形を合わせるために設置した。
		/// </summary>
		public DDCrash Crash
		{
			set
			{
				value.OwnerShot = this;
				Game.I.ShotCrashes.Add(value);
			}
		}
	}
}
