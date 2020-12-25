﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies.鍵山雛s
{
	/// <summary>
	/// 鍵山雛
	/// 掛け合い用
	/// </summary>
	public class Enemy_鍵山雛 : Enemy
	{
		public Enemy_鍵山雛()
			: base(-20.0, -20.0, Kind_e.ENEMY, 0, 0) // 掛け合い用なので、無敵
		{ }

		public bool NextFlag = false;

		protected override IEnumerable<bool> E_Draw()
		{
			for (int frame = 0; !this.NextFlag; frame++)
			{
				DDUtils.Approach(ref this.X, GameConsts.FIELD_W / 2 + Math.Sin(DDEngine.ProcFrame / 57.0) * 3.0, 0.97);
				DDUtils.Approach(ref this.Y, GameConsts.FIELD_H / 7 + Math.Sin(DDEngine.ProcFrame / 53.0) * 5.0, 0.91);

				EnemyCommon_鍵山雛.Draw(this.X, this.Y, frame < 60);

				// 掛け合い用なので、当たり判定無し

				yield return true;
			}

			Game.I.Enemies.Add(new Enemy_鍵山雛_01(this.X, this.Y));
		}

		public override void Killed()
		{
			throw null; // never
		}

		public override bool IsBoss()
		{
			return true;
		}
	}
}
