using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games
{
	public class GameStatus
	{
		public long Score = 0;
		public int PlayerPower = 0;
		public int PlayerZanki = GameConsts.DEFAULT_ZANKI;
		public int PlayerZanBomb = GameConsts.DEFAULT_ZAN_BOMB;
	}
}
