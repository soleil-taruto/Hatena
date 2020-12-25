using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	/// <summary>
	/// 敵
	/// </summary>
	public abstract class Enemy
	{
		public double X;
		public double Y;

		public enum Kind_e
		{
			ENEMY = 1, // 敵
			TAMA, // 敵弾(敵の弾)
			ITEM, // アイテム
		}

		/// <summary>
		/// 敵の種類
		/// </summary>
		public Kind_e Kind;

		/// <summary>
		/// 敵の体力
		/// 0 == 無敵 | 破壊できない敵弾 | アイテム
		/// -1 == 死亡
		/// </summary>
		public int HP;

		/// <summary>
		/// HPの初期値
		/// </summary>
		public int InitialHP;

		/// <summary>
		/// 無敵時間
		/// 画面に登場してから毎フレーム 0になるまで カウントダウンする。
		/// 0 == 無効
		/// 1～ == 無敵
		/// </summary>
		public int TransFrame;

		/// <summary>
		/// -1 == 吸収不能な敵
		/// 0～ == 吸収可能_吸収する武器
		/// </summary>
		public int AbsorbableWeapon;

		public bool Absorbable
		{
			get
			{
				return this.AbsorbableWeapon != -1 && this.Kind != Kind_e.ITEM;
			}
		}

		public Enemy(double x, double y, Kind_e kind, int hp, int transFrame, int absorbableWeapon = -1)
		{
			this.X = x;
			this.Y = y;
			this.Kind = kind;
			this.HP = hp;
			this.InitialHP = hp;
			this.TransFrame = transFrame;
			this.AbsorbableWeapon = absorbableWeapon;
		}

		/// <summary>
		/// 画面(フィールド)に登場してから毎フレーム インクリメントする。
		/// </summary>
		public int OnFieldFrame = 0;

		private Func<bool> _draw = null;

		public Func<bool> Draw
		{
			get
			{
				if (_draw == null)
				{
					Func<bool> f = SCommon.Supplier(this.E_Draw());
					Func<bool> fc = SCommon.Supplier(this.E_Draw_共通());

					_draw = () => fc() && f();
				}
				return _draw;
			}
		}

		private IEnumerable<bool> E_Draw_共通()
		{
			for (; ; )
			{
				if (this.Absorbable && 1 <= Game.I.Player.SlowFrame) // ? 吸収可能 && 低速移動中
				{
					double distance = DDUtils.GetDistance(
						new D2Point(Game.I.Player.X, Game.I.Player.Y),
						new D2Point(this.X, this.Y)
						);

					if (distance < 100.0) // ? 接近した。
					{
						Game.I.Enemies.Add(new Enemy_Item(this.X, this.Y, EnemyCommon.DROP_ITEM_TYPE_e.ABSORBABLE_SHOT, 1, this.AbsorbableWeapon));
						break;
					}
				}
				yield return true;
			}
		}

		/// <summary>
		/// 真を返し続けること。
		/// 偽を返すと、この敵は消滅する。
		/// 処理すべきこと：
		/// -- 行動
		/// -- 描画
		/// -- 当たり判定の設置 -- this.Crash = crash;
		/// </summary>
		/// <returns>この敵は生存しているか</returns>
		protected abstract IEnumerable<bool> E_Draw();

		/// <summary>
		/// 撃破された時に呼び出される。
		/// -- アイテムの場合はプレイヤーが取得した時に呼び出される。
		/// 処理すべきこと(例)：
		/// -- 爆死エフェクトの追加 -- Game.I.EnemyEffects
		/// -- ドロップアイテムを出現させる。
		/// -- スコア加算
		/// </summary>
		public abstract void Killed();

		/// <summary>
		/// 自弾(プレイヤーの弾)によってダメージを受けた時に呼び出される。
		/// 撃破された時は呼ばれない。
		/// -- 1撃で倒された場合、1度も呼び出されないことになる。
		/// </summary>
		public virtual void Damaged()
		{
			EnemyCommon.Damaged(this);
		}

		/// <summary>
		/// このフレームに於けるこの敵の当たり判定を設定する。
		/// 他のプロジェクトと形を合わせるために設置した。
		/// </summary>
		public DDCrash Crash
		{
			set
			{
				value.OwnerEnemy = this;
				Game.I.EnemyCrashes.Add(value);
			}
		}

		/// <summary>
		/// ボスか？
		/// </summary>
		/// <returns>ボスか</returns>
		public virtual bool IsBoss()
		{
			return false;
		}
	}
}
