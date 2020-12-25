using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Enemies;

namespace Charlotte
{
	public class ResourcePicture2
	{
		//public DDPicture[] Dummy = DDDerivations.GetAnimation(Ground.I.Picture.Dummy, 0, 0, 25, 25, 2, 2).ToArray();

		// ゴミ対策 == 周囲１ピクセルを削る。

		// -- P_KOAKUMA_P1

		public DDPicture[] D_KOAKUMA_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 0, 0, 32, 48, 4, 3).ToArray();
		public DDPicture[] D_BLAST_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 96, 48, 32, 32, 1, 4).ToArray();
		public DDPicture[] D_BOOK_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 0, 144, 24, 24, 3, 1).ToArray();
		public DDPicture D_ATARIPOINT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 72, 144, 8, 8);
		public DDPicture D_SHOT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 80, 144, 16, 40);
		public DDPicture D_WAVESHOT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 64, 184, 48, 32);
		public DDPicture D_BOOKBACK = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 64, 216, 40, 40);
		public DDPicture D_SLOWBACK = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 0 + 1, 168 + 1, 64 - 2, 64 - 2); // ゴミ対策
		public DDPicture D_ITEM_HEART = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 0, 232, 24, 24);
		public DDPicture D_ITEM_STAR = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 24, 232, 24, 24);
		public DDPicture[] D_LASERBLAST_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 128, 0, 64, 48, 1, 3).ToArray();
		//public DDPicture D_LASER = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 160, 144, 32, 56); // del
		public DDPicture D_LASER = DDPictureLoaders.Mirror(
			@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p1.png",
			new I4Rect(160, 144, 32, 56)
			);
		public DDPicture[] D_LASERFIRE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 192, 0, 48, 72, 1, 3).ToArray();

		// -- P_KOAKUMA_P3

		private const int BARAN_DIV = 32;

		public DDPicture D_DECOCIRCLE = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 0 + 1, 0 + 1, 208 - 2, 208 - 2); // ゴミ対策
		public DDPicture D_LEVELUP = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 0, 208, 168, 32);
		public DDPicture D_ITEM_CANDY = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 208 + 1, 0 + 1, 32 - 2, 24 - 2); // ゴミ対策
		public DDPicture D_ITEM_BOMB = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 240, 0, 24, 24);
		public DDPicture[] D_BARAN_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P3, 208, 24, 72, 128 / BARAN_DIV, 1, BARAN_DIV).ToArray();
		public DDPicture D_GAUGE = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 208, 152, 48, 104);
		public DDPicture D_GAUGEBAR = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 256, 200, 16, 1);

		// --

		public DDPicture[] D_ENEMYDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYDIE, 0, 0, 192, 192, 5, 2).ToArray();
		public DDPicture[] D_ENEMYDIE_00_BGRA = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYDIE_ABGR, 0, 0, 192, 192, 5, 2).ToArray();
		public DDPicture[] D_ENEMYSHOTDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYSHOTDIE, 0, 0, 60, 60, 10, 1).ToArray();
		public DDPicture[] D_PLAYERDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PLAYERDIE, 0, 0, 240, 240, 10, 1).ToArray();
		public DDPicture[] D_PUMPKIN_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PUMPKIN, 0, 0, 32, 32, 1, 2).ToArray();
		public DDPicture[] D_PUMPKIN_00_GRBA = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PUMPKIN_AGRB, 0, 0, 32, 32, 1, 2).ToArray();

		/// <summary>
		/// 添字：D_TAMA_00[EnemyCommon.TAMA_KIND_e][EnemyCommon.TAMA_COLOR_e]
		/// </summary>
		public DDPicture[][] D_TAMA_00 = Get_D_TAMA_00().ToArray();

		private static IEnumerable<DDPicture[]> Get_D_TAMA_00()
		{
			int tamaColorNum = Enum.GetValues(typeof(EnemyCommon.TAMA_COLOR_e)).Length;
			int y = 0;

			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 10, 10, tamaColorNum, 1).ToArray(); y += 10; // SMALL
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 24, 24, tamaColorNum, 1).ToArray(); y += 24; // NORMAL
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 24, 24, tamaColorNum, 1).ToArray(); y += 24; // DOUBLE
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 30, 30, tamaColorNum, 1).ToArray(); y += 30; // BIG
			yield return DDDerivations.GetAnimation_XY(Ground.I.Picture.P_TAMA_B, 0, y, 64, 64, tamaColorNum / 2, 2).ToArray(); y += 128; // LARGE

			y = 477;

			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 21, 30, tamaColorNum, 1).ToArray(); y += 30; // KNIFE
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA_B, 0, y, 29, 29, tamaColorNum, 1).ToArray(); y += 30; // ECLIPSE
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA_B, 0, y, 29, 29, tamaColorNum, 1).ToArray(); y += 30; // ECLIPSE_DOUBLE
		}

		public DDPicture[] D_DIGITS_W_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_W, 0, 0, 16, 32, 13, 1).ToArray();
		public DDPicture[] D_DIGITS_DDY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_DDY, 0, 0, 16, 32, 13, 1).ToArray();
		public DDPicture[] D_DIGITS_DY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_DY, 0, 0, 16, 32, 13, 1).ToArray();
		public DDPicture[] D_DIGITS_Y_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_Y, 0, 0, 16, 32, 13, 1).ToArray();

		public DDPicture[] D_FAIRY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 0, 64, 32, 32, 8, 8).ToArray();
		public DDPicture[] D_BIGFAIRY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 256, 0, 64, 64, 4, 4).ToArray();
		public DDPicture[] D_ONIBI_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 0, 320, 64, 64, 4, 3).ToArray();

		public DDPicture[] D_MAHOJIN_HAJIKE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_MAHOJIN_HAJIKE, 0, 0, 240, 240, 14, 1).ToArray();

		public DDPicture D_HINA = DDDerivations.GetPicture(Ground.I.Picture.P_FUJINBOSS, 128, 64, 64, 64);
		public DDPicture[] D_HINA_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FUJINBOSS, 0, 128, 128, 128, 1, 3).ToArray();

		public DDPicture[,] メディスン = DDDerivations.GetAnimation(Ground.I.Picture.メディスン, 0, 0, 64, 64, 3, 3);
		public DDPicture[] ルーミア = DDDerivations.GetAnimation_YX(Ground.I.Picture.DotOther, 0, 256, 128, 128, 4, 1).ToArray();

		public DDPicture 吸収している武器 = DDDerivations.GetPicture(Ground.I.Picture.P_FAIRYETC, 128, 0, 64, 64);
	}
}
