using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies;
using Charlotte.Games.Enemies.鍵山雛s;

namespace Charlotte.Games.Scripts
{
	public class Script_テスト0002 : Script
	{
		/// <summary>
		/// 掛け合いテスト
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());

			Game.I.Enemies.Add(new Enemy_鍵山雛());

			for (int c = 0; c < 90; c++)
				yield return true;

			foreach (bool v in ScriptCommon.掛け合い(new Scenario(@"e20200001_res\掛け合いシナリオ\小悪魔_鍵山雛.txt")))
				yield return v;

			for (; ; )
			{
				yield return true;
			}
		}
	}
}
