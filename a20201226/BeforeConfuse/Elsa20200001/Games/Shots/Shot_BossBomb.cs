using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons.Options;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Shots
{
	/// <summary>
	/// ボス登場時などに使用するシステム・ボム
	/// 画面上の敵を一掃する。
	/// </summary>
	public class Shot_BossBomb : Shot
	{
		public Shot_BossBomb()
			: base(0, 0, Kind_e.BOMB, SCommon.IMAX)
		{ }

		protected override IEnumerable<bool> E_Draw()
		{
#if true
			foreach (DDScene scene in DDSceneUtils.Create(60))
			{
				this.Crash = DDCrashUtils.Rect(D4Rect.LTRB(
					0,
					DDUtils.AToBRate(GameConsts.FIELD_H - 30.0, -30.0, scene.Rate),
					GameConsts.FIELD_W,
					GameConsts.FIELD_H
					));

				yield return true;
			}
#else // シンプル
			for (int c = 0; c < 10; c++)
			{
				this.Crash = DDCrashUtils.Rect(new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H));

				yield return true;
			}
#endif
		}
	}
}
