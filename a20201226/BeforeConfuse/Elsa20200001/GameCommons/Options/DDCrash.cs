using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.Games.Enemies;
using Charlotte.Games.Shots;

namespace Charlotte.GameCommons.Options
{
	public struct DDCrash
	{
		// アプリ固有 >

		/// <summary>
		/// この当たり判定に対応する敵
		/// Game.I.EnemyCrashes の要素のみ使用する。
		/// </summary>
		public Enemy OwnerEnemy;

		/// <summary>
		/// この当たり判定に対応する自弾
		/// Game.I.ShotCrashes の要素のみ使用する。
		/// </summary>
		public Shot OwnerShot;

		// < アプリ固有

		public DDCrashUtils.Kind_e Kind;
		public D2Point Pt;
		public double R;
		public D4Rect Rect;
		public DDCrash[] Crashes;

		public bool IsCrashed(DDCrash other)
		{
			return DDCrashUtils.IsCrashed(this, other);
		}
	}
}
