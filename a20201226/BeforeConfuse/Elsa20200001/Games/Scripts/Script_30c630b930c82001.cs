using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Games.Walls;
using Charlotte.Games.Enemies;
using Charlotte.Games.Enemies.鍵山雛s;
using Charlotte.Games.Shots;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Scripts
{
	public class Script_テスト2001 : Script
	{
		protected override IEnumerable<bool> E_EachFrame()
		{
			Game.I.Walls.Add(new Wall_Dark());
			Game.I.Walls.Add(new Wall_B21001());
			Game.I.Walls.Add(new Wall_B21002());

			for (int c = 0; c < 180; c++)
				yield return true;

			// All Clear Bonus
			{
				long bonus = 100000000;

				DDGround.EL.Add(SCommon.Supplier(Effects.Message(
					"ALL CLEAR BONUS +" + bonus,
					new I3Color(64, 64, 0),
					new I3Color(255, 255, 0)
					)));

				Game.I.Score += bonus;
			}

			for (int c = 0; c < 300; c++)
				yield return true;
		}
	}
}
