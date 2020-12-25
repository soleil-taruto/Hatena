using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.Games
{
	public static class 画面分割
	{
		// HACK: 謎の重さがある。

		private const int PIECE_WH = 170;
		private const int PIECE_XY_STEP = 171;
		private const int PIECES_W = 3;
		private const int PIECES_H = 3;

		private static DDSubScreen Screen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
		private static DDSubScreen[,] PieceTable = new DDSubScreen[PIECES_W, PIECES_H];

		public static void INIT()
		{
			for (int x = 0; x < PIECES_W; x++)
				for (int y = 0; y < PIECES_H; y++)
					PieceTable[x, y] = new DDSubScreen(PIECE_WH, PIECE_WH);
		}

		/// <summary>
		/// 本機能を有効にするには、これを true にすること。
		/// </summary>
		public static bool Enabled = false;

		private static D2Point[,] PointTable = new D2Point[PIECES_W + 1, PIECES_H + 1];

		public static void EachFrame(DDSubScreen targetScreen)
		{
			if (!Enabled)
				return;

			for (int x = 0; x < PIECES_W; x++)
			{
				for (int y = 0; y < PIECES_H; y++)
				{
					PieceTable[x, y].ChangeDrawScreen();

					DX.DrawRectGraph(
						0,
						0,
						x * PIECE_XY_STEP,
						y * PIECE_XY_STEP,
						x * PIECE_XY_STEP + PIECE_WH,
						y * PIECE_XY_STEP + PIECE_WH,
						targetScreen.GetHandle(),
						0
						);
				}
			}

			Screen.ChangeDrawScreen();

			// Fill_Red
			DDDraw.SetBright(1.0, 0.0, 0.0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H));
			DDDraw.Reset();

			for (int x = 0; x < PIECES_W; x++)
			{
				for (int y = 0; y < PIECES_H; y++)
				{
					D2Point centerPt = new D2Point(
						x * PIECE_XY_STEP + PIECE_WH / 2,
						y * PIECE_XY_STEP + PIECE_WH / 2
						);
					double rot = 0.0;
					DDPicture picture = PieceTable[x, y].ToPicture();

					if (
						x == 0 && y == 0 ||
						x == 1 && y == 0
						)
						centerPt.X += PIECE_XY_STEP;

					if (
						x == 2 && y == 0 ||
						x == 2 && y == 1
						)
						centerPt.Y += PIECE_XY_STEP;

					if (
						x == 1 && y == 2 ||
						x == 2 && y == 2
						)
						centerPt.X -= PIECE_XY_STEP;

					if (
						x == 0 && y == 1 ||
						x == 0 && y == 2
						)
						centerPt.Y -= PIECE_XY_STEP;

					// 右上 (元：中央上) のみ 90° 回転
					if (
						x == 1 && y == 0
						)
						rot = Math.PI / 2;

					DDDraw.DrawBegin(picture, centerPt.X, centerPt.Y);
					DDDraw.DrawRotate(rot);
					DDDraw.DrawEnd();
				}
			}

			targetScreen.ChangeDrawScreen();

			DDDraw.DrawSimple(Screen.ToPicture(), 0, 0);
		}
	}
}
