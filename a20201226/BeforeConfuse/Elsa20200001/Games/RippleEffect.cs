using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;

namespace Charlotte.Games
{
	public static class RippleEffect
	{
		// HACK: 謎の重さがある。

		private const int PIECE_WH = 64;
		private const int PIECES_W = 8;
		private const int PIECES_H = 8;

		private static DDSubScreen Screen = new DDSubScreen(DDConsts.Screen_W, DDConsts.Screen_H);
		private static DDSubScreen[,] PieceTable = new DDSubScreen[PIECES_W, PIECES_H];
		private static D2Point[,] ReverbShiftTable = new D2Point[PIECES_W + 1, PIECES_H + 1];

		public static void INIT()
		{
			for (int x = 0; x < PIECES_W; x++)
			{
				for (int y = 0; y < PIECES_H; y++)
				{
					PieceTable[x, y] = new DDSubScreen(PIECE_WH, PIECE_WH);
					ReverbShiftTable[x, y] = new D2Point(0, 0);
				}
			}
		}

		private static DDTaskList 波紋s = new DDTaskList();

		public static void Clear()
		{
			波紋s.Clear();
		}

		private static void Add(DDTask 波紋)
		{
			if (10 <= 波紋s.Count)
			{
				波紋s.RemoveAt(0);
			}
			波紋s.Add(波紋.Task);
		}

		public static int Count
		{
			get
			{
				return 波紋s.Count;
			}
		}

		private static D2Point[,] PointTable = new D2Point[PIECES_W + 1, PIECES_H + 1];

		public static void EachFrame(DDSubScreen targetScreen)
		{
			// 固定効果有り
			//if (波紋s.Count == 0)
			//    return;

			for (int x = 0; x <= PIECES_W; x++)
				for (int y = 0; y <= PIECES_H; y++)
					PointTable[x, y] = new D2Point(x * PIECE_WH, y * PIECE_WH);

			固定効果();
			波紋s.ExecuteAllTask();

			for (int x = 0; x < PIECES_W; x++)
			{
				for (int y = 0; y < PIECES_H; y++)
				{
					PieceTable[x, y].ChangeDrawScreen();

					DX.DrawRectGraph(0, 0, x * PIECE_WH, y * PIECE_WH, (x + 1) * PIECE_WH, (y + 1) * PIECE_WH, targetScreen.GetHandle(), 0);
				}
			}

			Screen.ChangeDrawScreen();

			// フィールドの淵がめくれないように
			for (int x = 0; x <= PIECES_W; x++)
			{
				DDUtils.Minim(ref PointTable[x, 0].Y, 0.0);
				DDUtils.Maxim(ref PointTable[x, PIECES_H].Y, GameConsts.FIELD_H);
			}
			for (int y = 0; y <= PIECES_H; y++)
			{
				DDUtils.Minim(ref PointTable[0, y].X, 0.0);
				DDUtils.Maxim(ref PointTable[PIECES_W, y].X, GameConsts.FIELD_W);
			}

			for (int x = 0; x < PIECES_W; x++)
			{
				for (int y = 0; y < PIECES_H; y++)
				{
					D2Point lt = PointTable[x + 0, y + 0];
					D2Point rt = PointTable[x + 1, y + 0];
					D2Point rb = PointTable[x + 1, y + 1];
					D2Point lb = PointTable[x + 0, y + 1];

					DDDraw.SetIgnoreError();
					DDDraw.DrawFree(PieceTable[x, y].ToPicture(), lt, rt, rb, lb);
					DDDraw.Reset();
				}
			}

			targetScreen.ChangeDrawScreen();

			DDDraw.DrawSimple(Screen.ToPicture(), 0, 0);
		}

		private static void 固定効果()
		{
			for (int x = 0; x <= PIECES_W; x++)
			{
				for (int y = 0; y <= PIECES_H; y++)
				{
					D2Point pt = new D2Point(x * PIECE_WH, y * PIECE_WH);
					D2Point ptShift = 固定効果_GetShift(pt);

					ReverbShiftTable[x, y] += ptShift;
					ReverbShiftTable[x, y] *= 0.9;

					PointTable[x, y] += ReverbShiftTable[x, y];
				}
			}
		}

		private static D2Point 固定効果_GetShift(D2Point pt)
		{
			D2Point ptShift = new D2Point(0, 0);

			foreach (Shot shot in Game.I.Shots.Iterate())
			{
				D2Point shotPt = new D2Point(shot.X, shot.Y);
				double distance = DDUtils.GetDistance(pt, shotPt);

				if (distance < 100.0)
				{
					// To 0.0 ～ 1.0
					distance /= 100.0;
					distance = 1.0 - distance;

					ptShift.X += (shot.X - shot.LastX) * distance * shot.AttackPoint * 0.01;
					ptShift.Y += (shot.Y - shot.LastY) * distance * shot.AttackPoint * 0.01;
				}
			}

			// ここへ追加..

			return ptShift;
		}

		public static void Add_波紋(double x, double y, int frameMax)
		{
			Add(new 波紋Task_波紋()
			{
				Center_X = x,
				Center_Y = y,
				FrameMax = frameMax,
			});
		}

		private class 波紋Task_波紋 : DDTask
		{
			public double Center_X;
			public double Center_Y;
			public int FrameMax;

			// <---- prm

			public override IEnumerable<bool> E_Task()
			{
				foreach (DDScene scene in DDSceneUtils.Create(this.FrameMax))
				{
					for (int x = 0; x <= PIECES_W; x++)
						for (int y = 0; y <= PIECES_H; y++)
							PointTable[x, y] = this.ShiftPt(PointTable[x, y], scene.Rate);

					yield return true;
				}
			}

			public D2Point ShiftPt(D2Point pt, double rate)
			{
				// pt == フィールド上の座標

				// 波紋の中心からの相対座標に変更
				pt.X -= this.Center_X;
				pt.Y -= this.Center_Y;

				double wave_r = rate * 1000.0;
				double distance = DDUtils.GetDistance(pt);
				double d = distance;

				d -= wave_r;

				// d の -50 ～ 50 を 0.0 ～ 1.0 にする。
				d /= 50.0;
				d += 1.0;
				d /= 2.0;

				if (0.0 < d && d < 1.0)
				{
					d *= 2.0;

					if (1.0 < d)
						d = 2.0 - d;

					distance += DDUtils.SCurve(d) * 50.0 * (1.0 - rate * 0.5);

					DDUtils.MakeXYSpeed(0.0, 0.0, pt.X, pt.Y, distance, out pt.X, out pt.Y); // distance を pt に反映する。
				}

				// restore -- 波紋の中心からの相対座標 -> フィールド上の座標
				pt.X += this.Center_X;
				pt.Y += this.Center_Y;

				return pt;
			}
		}
	}
}
