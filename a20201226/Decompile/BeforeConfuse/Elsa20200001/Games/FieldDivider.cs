using System;
using Charlotte.Commons;
using Charlotte.GameCommons;
using DxLibDLL;

namespace Charlotte.Games
{
	// Token: 0x0200001E RID: 30
	public static class FieldDivider
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00006ED8 File Offset: 0x000050D8
		public static void INIT()
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					FieldDivider.PieceTable[x, y] = new DDSubScreen(170, 170, false);
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00006F1C File Offset: 0x0000511C
		public static void EachFrame(DDSubScreen targetScreen)
		{
			if (!FieldDivider.Enabled)
			{
				return;
			}
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					FieldDivider.PieceTable[x, y].ChangeDrawScreen();
					DX.DrawRectGraph(0, 0, x * 171, y * 171, x * 171 + 170, y * 171 + 170, targetScreen.GetHandle(), 0);
				}
			}
			FieldDivider.Screen.ChangeDrawScreen();
			DDDraw.SetBright(1.0, 0.0, 0.0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0.0, 0.0, 512.0, 512.0));
			DDDraw.Reset();
			for (int x2 = 0; x2 < 3; x2++)
			{
				for (int y2 = 0; y2 < 3; y2++)
				{
					D2Point centerPt = new D2Point((double)(x2 * 171 + 85), (double)(y2 * 171 + 85));
					double rot = 0.0;
					DDPicture picture = FieldDivider.PieceTable[x2, y2].ToPicture();
					if ((x2 == 0 && y2 == 0) || (x2 == 1 && y2 == 0))
					{
						centerPt.X += 171.0;
					}
					if ((x2 == 2 && y2 == 0) || (x2 == 2 && y2 == 1))
					{
						centerPt.Y += 171.0;
					}
					if ((x2 == 1 && y2 == 2) || (x2 == 2 && y2 == 2))
					{
						centerPt.X -= 171.0;
					}
					if ((x2 == 0 && y2 == 1) || (x2 == 0 && y2 == 2))
					{
						centerPt.Y -= 171.0;
					}
					if (x2 == 1 && y2 == 0)
					{
						rot = 1.5707963267948966;
					}
					DDDraw.DrawBegin(picture, centerPt.X, centerPt.Y);
					DDDraw.DrawRotate(rot);
					DDDraw.DrawEnd();
				}
			}
			targetScreen.ChangeDrawScreen();
			DDDraw.DrawSimple(FieldDivider.Screen.ToPicture(), 0.0, 0.0);
		}

		// Token: 0x040000D5 RID: 213
		private const int PIECE_WH = 170;

		// Token: 0x040000D6 RID: 214
		private const int PIECE_XY_STEP = 171;

		// Token: 0x040000D7 RID: 215
		private const int PIECES_W = 3;

		// Token: 0x040000D8 RID: 216
		private const int PIECES_H = 3;

		// Token: 0x040000D9 RID: 217
		private static DDSubScreen Screen = new DDSubScreen(960, 540, false);

		// Token: 0x040000DA RID: 218
		private static DDSubScreen[,] PieceTable = new DDSubScreen[3, 3];

		// Token: 0x040000DB RID: 219
		public static bool Enabled = false;

		// Token: 0x040000DC RID: 220
		private static D2Point[,] PointTable = new D2Point[4, 4];
	}
}
