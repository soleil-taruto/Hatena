using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games
{
	public static class GameConsts
	{
		public const int FIELD_L = 224;
		public const int FIELD_T = 14;
		public const int FIELD_W = 512;
		public const int FIELD_H = 512;

		public const int ITEM_GET_BORDER_Y = 200;
		public const int ITEM_GET_MAX_Y = FIELD_H + 200;

		// プレイヤーレベルの値域 == 0 ～ PLAYER_LEVEL_MAX
		// プレイヤーパワーの値域 == 0 ～ PLAYER_POWER_MAX

		// プレイヤーレベル == INT ( プレイヤーパワー / PLAYER_POWER_PER_LEVEL )

		public const int PLAYER_LEVEL_MAX = 5;
		public const int PLAYER_POWER_PER_LEVEL = 100;
		public const int PLAYER_POWER_MAX = PLAYER_LEVEL_MAX * PLAYER_POWER_PER_LEVEL;

		public const int PLAYER_BORN_FRAME_MAX = 300;
		public const int PLAYER_DEAD_FRAME_MAX = 60;
		public const int PLAYER_BOMB_FRAME_MAX = 180;

		public const int DEFAULT_ZANKI = 3;
		public const int DEFAULT_ZAN_BOMB = 3;
		//public const int DEFAULT_ZAN_BOMB = 5;

		public const int ZANKI_MAX = 5;
		public const int ZAN_BOMB_MAX = 5;
		//public const int ZAN_BOMB_MAX = 10;
	}
}
