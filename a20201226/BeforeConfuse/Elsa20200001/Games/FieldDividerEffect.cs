using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.Games
{
	public static class FieldDividerEffect
	{
		private const int PIECE_WH = 170;
		private const int PIECE_XY_STEP = 171;
		private const int PIECES_W = 3;
		private const int PIECES_H = 3;

		//private static DDSubScreen Screen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
		private static DDSubScreen[,] PieceTable = new DDSubScreen[PIECES_W, PIECES_H];

		public static void INIT()
		{
			for (int x = 0; x < PIECES_W; x++)
				for (int y = 0; y < PIECES_H; y++)
					PieceTable[x, y] = new DDSubScreen(PIECE_WH, PIECE_WH);
		}

		private static DDSubScreen P_Field
		{
			get
			{
				return Game.I.Field_Last;
			}
		}

		private class DrawPieceTask : DDTask
		{
			private DDPicture Picture;

			private class DrawPosInfo
			{
				public double X;
				public double Y;
				public double Rot;
			}

			private DrawPosInfo DrawPos_01;
			private DrawPosInfo DrawPos_02;
			private DrawPosInfo DrawPos_03;
			private DrawPosInfo DrawPos_04;
			private DrawPosInfo DrawPos_05;

			public DrawPieceTask(int x1, int y1, int x2, int y2, bool doRot)
			{
				this.Picture = PieceTable[x1, y1].ToPicture();

				const double INTERVAL = 30.0;

				this.DrawPos_01 = new DrawPosInfo()
				{
					X = x1 * PIECE_XY_STEP + PIECE_WH / 2,
					Y = y1 * PIECE_XY_STEP + PIECE_WH / 2,
					Rot = 0.0,
				};
				this.DrawPos_02 = new DrawPosInfo()
				{
					X = x1 * PIECE_XY_STEP + PIECE_WH / 2 + (x1 - 1) * INTERVAL,
					Y = y1 * PIECE_XY_STEP + PIECE_WH / 2 + (y1 - 1) * INTERVAL,
					Rot = 0.0,
				};
				this.DrawPos_03 = new DrawPosInfo()
				{
					X = x2 * PIECE_XY_STEP + PIECE_WH / 2 + (x2 - 1) * INTERVAL,
					Y = y2 * PIECE_XY_STEP + PIECE_WH / 2 + (y2 - 1) * INTERVAL,
					Rot = 0.0,
				};
				this.DrawPos_04 = new DrawPosInfo()
				{
					X = x2 * PIECE_XY_STEP + PIECE_WH / 2 + (x2 - 1) * INTERVAL,
					Y = y2 * PIECE_XY_STEP + PIECE_WH / 2 + (y2 - 1) * INTERVAL,
					Rot = doRot ? Math.PI / 2 : 0.0,
				};
				this.DrawPos_05 = new DrawPosInfo()
				{
					X = x2 * PIECE_XY_STEP + PIECE_WH / 2,
					Y = y2 * PIECE_XY_STEP + PIECE_WH / 2,
					Rot = doRot ? Math.PI / 2 : 0.0,
				};
			}

			public override IEnumerable<bool> E_Task()
			{
				foreach (bool v in E_DrawPiace(this.DrawPos_01, this.DrawPos_01, 20)) yield return v;
				foreach (bool v in E_DrawPiace(this.DrawPos_01, this.DrawPos_02, 60)) yield return v;
				foreach (bool v in E_DrawPiace(this.DrawPos_02, this.DrawPos_03, 50)) yield return v;
				foreach (bool v in E_DrawPiace(this.DrawPos_03, this.DrawPos_04, 40)) yield return v;
				foreach (bool v in E_DrawPiace(this.DrawPos_04, this.DrawPos_05, 30)) yield return v;
			}

			private IEnumerable<bool> E_DrawPiace(DrawPosInfo drawPos_1, DrawPosInfo drawPos_2, int frameMax)
			{
				foreach (DDScene scene in DDSceneUtils.Create(frameMax))
				{
					double scRate = DDUtils.SCurve(scene.Rate);
					double x = GameConsts.FIELD_L + DDUtils.AToBRate(drawPos_1.X, drawPos_2.X, scRate);
					double y = GameConsts.FIELD_T + DDUtils.AToBRate(drawPos_1.Y, drawPos_2.Y, scRate);
					double rot = DDUtils.AToBRate(drawPos_1.Rot, drawPos_2.Rot, scRate);

					DDDraw.DrawBegin(this.Picture, x, y);
					DDDraw.DrawRotate(rot);
					DDDraw.DrawEnd();

					yield return true;
				}
			}
		}

		public static void Enter()
		{
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
						P_Field.GetHandle(),
						0
						);
				}
			}

			DDGround.MainScreen.ChangeDrawScreen();

			Func<bool> f1 = new DrawPieceTask(0, 0, 1, 0, false).Task;
			Func<bool> f2 = new DrawPieceTask(1, 0, 2, 0, true).Task;
			Func<bool> f3 = new DrawPieceTask(2, 0, 2, 1, false).Task;
			Func<bool> f4 = new DrawPieceTask(2, 1, 2, 2, false).Task;
			Func<bool> f5 = new DrawPieceTask(2, 2, 1, 2, false).Task;
			Func<bool> f6 = new DrawPieceTask(1, 2, 0, 2, false).Task;
			Func<bool> f7 = new DrawPieceTask(0, 2, 0, 1, false).Task;
			Func<bool> f8 = new DrawPieceTask(0, 1, 0, 0, false).Task;
			Func<bool> f9 = new DrawPieceTask(1, 1, 1, 1, false).Task;

			DDCurtain.SetCurtain(0, 1.0);
			DDCurtain.SetCurtain(20);

			for (; ; )
			{
				DrawWall();

				if (!f1())
					break;

				f2();
				f3();
				f4();
				f5();
				f6();
				f7();
				f8();
				f9();

				DDEngine.EachFrame();
			}

			DDCurtain.SetCurtain(0, 0.5);
			DDCurtain.SetCurtain(10);
		}

		private static DDSubScreen GrayScreen_R = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
		private static DDSubScreen GrayScreen_G = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
		private static DDSubScreen GrayScreen_B = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);

		private static void DrawWall()
		{
			{
				DDUtils.Approach(ref Game.I.BackgroundSlideRate, Game.I.Player.Y * 1.0 / GameConsts.FIELD_H, 0.99);

				D4Rect rect = DDUtils.AdjustRectExterior(
					new D2Size(GameConsts.FIELD_W, GameConsts.FIELD_H),
					new D4Rect(0, 0, DDConsts.Screen_W, DDConsts.Screen_H),
					Game.I.BackgroundSlideRate
					);

				DDDraw.DrawRect(P_Field.ToPicture(), rect);
			}

			{
				const int MARGIN = 5;

				DDDraw.SetBright(0, 0, 0);
				DDDraw.DrawRect(
					DDGround.GeneralResource.WhiteBox,
					GameConsts.FIELD_L - MARGIN,
					GameConsts.FIELD_T - MARGIN,
					GameConsts.FIELD_W + MARGIN * 2,
					GameConsts.FIELD_H + MARGIN * 2
					);
				DDDraw.Reset();
			}

			DX.GraphFilter(
				DDGround.MainScreen.GetHandle(),
				DX.DX_GRAPH_FILTER_GAUSS,
				16,
				SCommon.ToInt(500.0)
				);

			DDCurtain.DrawCurtain(0.2);
			//DDCurtain.DrawCurtain(-0.2);

			// DDGround.MainScreen をグレースケール化
			{
				DX.GraphBlend(
					GrayScreen_R.GetHandle(), // ソース画像かつ出力先
					DDGround.MainScreen.GetHandle(), // ブレンド画像
					255,
					DX.DX_GRAPH_BLEND_RGBA_SELECT_MIX,
					DX.DX_RGBA_SELECT_BLEND_R, // 出力先に適用する R 値
					DX.DX_RGBA_SELECT_BLEND_R, // 出力先に適用する G 値
					DX.DX_RGBA_SELECT_BLEND_R, // 出力先に適用する B 値
					DX.DX_RGBA_SELECT_SRC_A // 出力先に適用する A 値
					);

				// DX_RGBA_SELECT_SRC_R == ソース画像の R 値
				// DX_RGBA_SELECT_SRC_G == ソース画像の G 値
				// DX_RGBA_SELECT_SRC_B == ソース画像の B 値
				// DX_RGBA_SELECT_SRC_A == ソース画像の A 値
				// DX_RGBA_SELECT_BLEND_R == ブレンド画像の R 値
				// DX_RGBA_SELECT_BLEND_G == ブレンド画像の G 値
				// DX_RGBA_SELECT_BLEND_B == ブレンド画像の B 値
				// DX_RGBA_SELECT_BLEND_A == ブレンド画像の A 値

				DX.GraphBlend(
					GrayScreen_G.GetHandle(),
					DDGround.MainScreen.GetHandle(),
					255,
					DX.DX_GRAPH_BLEND_RGBA_SELECT_MIX,
					DX.DX_RGBA_SELECT_BLEND_G,
					DX.DX_RGBA_SELECT_BLEND_G,
					DX.DX_RGBA_SELECT_BLEND_G,
					DX.DX_RGBA_SELECT_SRC_A
					);

				DX.GraphBlend(
					GrayScreen_B.GetHandle(),
					DDGround.MainScreen.GetHandle(),
					255,
					DX.DX_GRAPH_BLEND_RGBA_SELECT_MIX,
					DX.DX_RGBA_SELECT_BLEND_B,
					DX.DX_RGBA_SELECT_BLEND_B,
					DX.DX_RGBA_SELECT_BLEND_B,
					DX.DX_RGBA_SELECT_SRC_A
					);

				DDDraw.DrawSimple(GrayScreen_R.ToPicture(), 0, 0);
				DDDraw.SetAlpha(0.5);
				DDDraw.DrawSimple(GrayScreen_G.ToPicture(), 0, 0);
				DDDraw.SetAlpha(0.333);
				DDDraw.DrawSimple(GrayScreen_B.ToPicture(), 0, 0);
				DDDraw.Reset();
			}
		}

#if true
		public static void Leave()
		{
			DDCurtain.SetCurtain(0, 0.5);
			DDCurtain.SetCurtain(10);
		}
#elif true // ng -- P_Filed は分割状態の絵
		public static void Leave()
		{
			DDCurtain.SetCurtain(0, 1.0);
			DDCurtain.SetCurtain(20);

			foreach (DDScene scene in DDSceneUtils.Create(40))
			{
				DrawWall();
				DDDraw.DrawSimple(P_Field.ToPicture(), GameConsts.FIELD_L, GameConsts.FIELD_T);

				DDEngine.EachFrame();
			}
		}
#else // ng
		public static void Leave()
		{
			DDCurtain.SetCurtain(0, 1.0);
			DDCurtain.SetCurtain(20);

			DDGround.EL.Add(SCommon.Supplier(E_Leave()));
		}

		private static IEnumerable<bool> E_Leave()
		{
			yield return true; // 1回目のフレームはスルー

			foreach (DDScene scene in DDSceneUtils.Create(40))
			{
				DrawWall();
				DDDraw.DrawSimple(P_Field.ToPicture(), GameConsts.FIELD_L, GameConsts.FIELD_T);

				DDEngine.EachFrame(); // ここで更に EL が走る。これが駄目なんじゃないか？
			}
		}
#endif
	}
}
