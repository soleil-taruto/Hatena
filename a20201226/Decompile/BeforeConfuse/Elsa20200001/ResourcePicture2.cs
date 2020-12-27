using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Enemies;

namespace Charlotte
{
	// Token: 0x0200000A RID: 10
	public class ResourcePicture2
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000027A6 File Offset: 0x000009A6
		private static IEnumerable<DDPicture[]> Get_D_TAMA_00()
		{
			int tamaColorNum = Enum.GetValues(typeof(EnemyCommon.TAMA_COLOR_e)).Length;
			int y = 0;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 10, 10, tamaColorNum, 1).ToArray<DDPicture>();
			y += 10;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 24, 24, tamaColorNum, 1).ToArray<DDPicture>();
			y += 24;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 24, 24, tamaColorNum, 1).ToArray<DDPicture>();
			y += 24;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 30, 30, tamaColorNum, 1).ToArray<DDPicture>();
			y += 30;
			yield return DDDerivations.GetAnimation_XY(Ground.I.Picture.P_TAMA_B, 0, y, 64, 64, tamaColorNum / 2, 2).ToArray<DDPicture>();
			y += 128;
			y = 477;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA, 0, y, 21, 30, tamaColorNum, 1).ToArray<DDPicture>();
			y += 30;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA_B, 0, y, 29, 29, tamaColorNum, 1).ToArray<DDPicture>();
			y += 30;
			yield return DDDerivations.GetAnimation_YX(Ground.I.Picture.P_TAMA_B, 0, y, 29, 29, tamaColorNum, 1).ToArray<DDPicture>();
			y += 30;
			yield break;
		}

		// Token: 0x04000041 RID: 65
		public DDPicture[] D_KOAKUMA_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 0, 0, 32, 48, 4, 3).ToArray<DDPicture>();

		// Token: 0x04000042 RID: 66
		public DDPicture[] D_BLAST_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 96, 48, 32, 32, 1, 4).ToArray<DDPicture>();

		// Token: 0x04000043 RID: 67
		public DDPicture[] D_BOOK_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 0, 144, 24, 24, 3, 1).ToArray<DDPicture>();

		// Token: 0x04000044 RID: 68
		public DDPicture D_ATARIPOINT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 72, 144, 8, 8);

		// Token: 0x04000045 RID: 69
		public DDPicture D_SHOT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 80, 144, 16, 40);

		// Token: 0x04000046 RID: 70
		public DDPicture D_WAVESHOT = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 64, 184, 48, 32);

		// Token: 0x04000047 RID: 71
		public DDPicture D_BOOKBACK = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 64, 216, 40, 40);

		// Token: 0x04000048 RID: 72
		public DDPicture D_SLOWBACK = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 1, 169, 62, 62);

		// Token: 0x04000049 RID: 73
		public DDPicture D_ITEM_HEART = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 0, 232, 24, 24);

		// Token: 0x0400004A RID: 74
		public DDPicture D_ITEM_STAR = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P1, 24, 232, 24, 24);

		// Token: 0x0400004B RID: 75
		public DDPicture[] D_LASERBLAST_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 128, 0, 64, 48, 1, 3).ToArray<DDPicture>();

		// Token: 0x0400004C RID: 76
		public DDPicture D_LASER = DDPictureLoaders.Mirror("e20200003_dat\\Shoot_old_Resource\\jiki-koakuma-sozai\\19740345_big_p1.png", new I4Rect(160, 144, 32, 56));

		// Token: 0x0400004D RID: 77
		public DDPicture[] D_LASERFIRE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P1, 192, 0, 48, 72, 1, 3).ToArray<DDPicture>();

		// Token: 0x0400004E RID: 78
		private const int BARAN_DIV = 32;

		// Token: 0x0400004F RID: 79
		public DDPicture D_DECOCIRCLE = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 1, 1, 206, 206);

		// Token: 0x04000050 RID: 80
		public DDPicture D_LEVELUP = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 0, 208, 168, 32);

		// Token: 0x04000051 RID: 81
		public DDPicture D_ITEM_CANDY = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 209, 1, 30, 22);

		// Token: 0x04000052 RID: 82
		public DDPicture D_ITEM_BOMB = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 240, 0, 24, 24);

		// Token: 0x04000053 RID: 83
		public DDPicture[] D_BARAN_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_KOAKUMA_P3, 208, 24, 72, 4, 1, 32).ToArray<DDPicture>();

		// Token: 0x04000054 RID: 84
		public DDPicture D_GAUGE = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 208, 152, 48, 104);

		// Token: 0x04000055 RID: 85
		public DDPicture D_GAUGEBAR = DDDerivations.GetPicture(Ground.I.Picture.P_KOAKUMA_P3, 256, 200, 16, 1);

		// Token: 0x04000056 RID: 86
		public DDPicture[] D_ENEMYDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYDIE, 0, 0, 192, 192, 5, 2).ToArray<DDPicture>();

		// Token: 0x04000057 RID: 87
		public DDPicture[] D_ENEMYDIE_00_BGRA = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYDIE_ABGR, 0, 0, 192, 192, 5, 2).ToArray<DDPicture>();

		// Token: 0x04000058 RID: 88
		public DDPicture[] D_ENEMYSHOTDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_ENEMYSHOTDIE, 0, 0, 60, 60, 10, 1).ToArray<DDPicture>();

		// Token: 0x04000059 RID: 89
		public DDPicture[] D_PLAYERDIE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PLAYERDIE, 0, 0, 240, 240, 10, 1).ToArray<DDPicture>();

		// Token: 0x0400005A RID: 90
		public DDPicture[] D_PUMPKIN_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PUMPKIN, 0, 0, 32, 32, 1, 2).ToArray<DDPicture>();

		// Token: 0x0400005B RID: 91
		public DDPicture[] D_PUMPKIN_00_GRBA = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_PUMPKIN_AGRB, 0, 0, 32, 32, 1, 2).ToArray<DDPicture>();

		// Token: 0x0400005C RID: 92
		public DDPicture[][] D_TAMA_00 = ResourcePicture2.Get_D_TAMA_00().ToArray<DDPicture[]>();

		// Token: 0x0400005D RID: 93
		public DDPicture[] D_DIGITS_W_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_W, 0, 0, 16, 32, 13, 1).ToArray<DDPicture>();

		// Token: 0x0400005E RID: 94
		public DDPicture[] D_DIGITS_DDY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_DDY, 0, 0, 16, 32, 13, 1).ToArray<DDPicture>();

		// Token: 0x0400005F RID: 95
		public DDPicture[] D_DIGITS_DY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_DY, 0, 0, 16, 32, 13, 1).ToArray<DDPicture>();

		// Token: 0x04000060 RID: 96
		public DDPicture[] D_DIGITS_Y_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_DIGITS_Y, 0, 0, 16, 32, 13, 1).ToArray<DDPicture>();

		// Token: 0x04000061 RID: 97
		public DDPicture[] D_FAIRY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 0, 64, 32, 32, 8, 8).ToArray<DDPicture>();

		// Token: 0x04000062 RID: 98
		public DDPicture[] D_BIGFAIRY_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 256, 0, 64, 64, 4, 4).ToArray<DDPicture>();

		// Token: 0x04000063 RID: 99
		public DDPicture[] D_ONIBI_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FAIRYETC, 0, 320, 64, 64, 4, 3).ToArray<DDPicture>();

		// Token: 0x04000064 RID: 100
		public DDPicture[] D_MAHOJIN_HAJIKE_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_MAHOJIN_HAJIKE, 0, 0, 240, 240, 14, 1).ToArray<DDPicture>();

		// Token: 0x04000065 RID: 101
		public DDPicture D_HINA = DDDerivations.GetPicture(Ground.I.Picture.P_FUJINBOSS, 128, 64, 64, 64);

		// Token: 0x04000066 RID: 102
		public DDPicture[] D_HINA_00 = DDDerivations.GetAnimation_YX(Ground.I.Picture.P_FUJINBOSS, 0, 128, 128, 128, 1, 3).ToArray<DDPicture>();

		// Token: 0x04000067 RID: 103
		public DDPicture[,] メディスン = DDDerivations.GetAnimation(Ground.I.Picture.メディスン, 0, 0, 64, 64, 3, 3);

		// Token: 0x04000068 RID: 104
		public DDPicture[] ル\u30FCミア = DDDerivations.GetAnimation_YX(Ground.I.Picture.DotOther, 0, 256, 128, 128, 4, 1).ToArray<DDPicture>();

		// Token: 0x04000069 RID: 105
		public DDPicture 吸収している武器 = DDDerivations.GetPicture(Ground.I.Picture.P_FAIRYETC, 128, 0, 64, 64);
	}
}
