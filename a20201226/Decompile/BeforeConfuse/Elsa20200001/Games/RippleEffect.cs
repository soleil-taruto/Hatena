using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Shots;
using DxLibDLL;

namespace Charlotte.Games
{
	// Token: 0x0200001D RID: 29
	public static class RippleEffect
	{
		// Token: 0x0600005A RID: 90 RVA: 0x000069F8 File Offset: 0x00004BF8
		public static void INIT()
		{
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					RippleEffect.PieceTable[x, y] = new DDSubScreen(64, 64, false);
					RippleEffect.ReverbShiftTable[x, y] = new D2Point(0.0, 0.0);
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00006A56 File Offset: 0x00004C56
		public static void Clear()
		{
			RippleEffect.波紋s.Clear();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006A62 File Offset: 0x00004C62
		private static void Add(DDTask 波紋)
		{
			if (10 <= RippleEffect.波紋s.Count)
			{
				RippleEffect.波紋s.RemoveAt(0);
			}
			RippleEffect.波紋s.Add(波紋.Task);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00006A8D File Offset: 0x00004C8D
		public static int Count
		{
			get
			{
				return RippleEffect.波紋s.Count;
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006A9C File Offset: 0x00004C9C
		public static void EachFrame(DDSubScreen targetScreen)
		{
			for (int x = 0; x <= 8; x++)
			{
				for (int y = 0; y <= 8; y++)
				{
					RippleEffect.PointTable[x, y] = new D2Point((double)(x * 64), (double)(y * 64));
				}
			}
			RippleEffect.固定効果();
			RippleEffect.波紋s.ExecuteAllTask();
			for (int x2 = 0; x2 < 8; x2++)
			{
				for (int y2 = 0; y2 < 8; y2++)
				{
					RippleEffect.PieceTable[x2, y2].ChangeDrawScreen();
					DX.DrawRectGraph(0, 0, x2 * 64, y2 * 64, (x2 + 1) * 64, (y2 + 1) * 64, targetScreen.GetHandle(), 0);
				}
			}
			RippleEffect.Screen.ChangeDrawScreen();
			for (int x3 = 0; x3 <= 8; x3++)
			{
				DDUtils.Minim(ref RippleEffect.PointTable[x3, 0].Y, 0.0);
				DDUtils.Maxim(ref RippleEffect.PointTable[x3, 8].Y, 512.0);
			}
			for (int y3 = 0; y3 <= 8; y3++)
			{
				DDUtils.Minim(ref RippleEffect.PointTable[0, y3].X, 0.0);
				DDUtils.Maxim(ref RippleEffect.PointTable[8, y3].X, 512.0);
			}
			for (int x4 = 0; x4 < 8; x4++)
			{
				for (int y4 = 0; y4 < 8; y4++)
				{
					D2Point lt = RippleEffect.PointTable[x4, y4];
					D2Point rt = RippleEffect.PointTable[x4 + 1, y4];
					D2Point rb = RippleEffect.PointTable[x4 + 1, y4 + 1];
					D2Point lb = RippleEffect.PointTable[x4, y4 + 1];
					DDDraw.SetIgnoreError();
					DDDraw.DrawFree(RippleEffect.PieceTable[x4, y4].ToPicture(), lt, rt, rb, lb);
					DDDraw.Reset();
				}
			}
			targetScreen.ChangeDrawScreen();
			DDDraw.DrawSimple(RippleEffect.Screen.ToPicture(), 0.0, 0.0);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00006CA0 File Offset: 0x00004EA0
		private static void 固定効果()
		{
			for (int x = 0; x <= 8; x++)
			{
				for (int y = 0; y <= 8; y++)
				{
					D2Point ptShift = RippleEffect.固定効果_GetShift(new D2Point((double)(x * 64), (double)(y * 64)));
					RippleEffect.ReverbShiftTable[x, y] += ptShift;
					RippleEffect.ReverbShiftTable[x, y] *= 0.9;
					RippleEffect.PointTable[x, y] += RippleEffect.ReverbShiftTable[x, y];
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00006D50 File Offset: 0x00004F50
		private static D2Point 固定効果_GetShift(D2Point pt)
		{
			D2Point ptShift = new D2Point(0.0, 0.0);
			foreach (Shot shot in Game.I.Shots.Iterate())
			{
				D2Point shotPt = new D2Point(shot.X, shot.Y);
				double distance = DDUtils.GetDistance(pt, shotPt);
				if (distance < 100.0)
				{
					distance /= 100.0;
					distance = 1.0 - distance;
					ptShift.X += (shot.X - shot.LastX) * distance * (double)shot.AttackPoint * 0.01;
					ptShift.Y += (shot.Y - shot.LastY) * distance * (double)shot.AttackPoint * 0.01;
				}
			}
			return ptShift;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00006E60 File Offset: 0x00005060
		public static void Add_波紋(double x, double y, int frameMax)
		{
			RippleEffect.Add(new RippleEffect.波紋Task_波紋
			{
				Center_X = x,
				Center_Y = y,
				FrameMax = frameMax
			});
		}

		// Token: 0x040000CD RID: 205
		private const int PIECE_WH = 64;

		// Token: 0x040000CE RID: 206
		private const int PIECES_W = 8;

		// Token: 0x040000CF RID: 207
		private const int PIECES_H = 8;

		// Token: 0x040000D0 RID: 208
		private static DDSubScreen Screen = new DDSubScreen(960, 540, false);

		// Token: 0x040000D1 RID: 209
		private static DDSubScreen[,] PieceTable = new DDSubScreen[8, 8];

		// Token: 0x040000D2 RID: 210
		private static D2Point[,] ReverbShiftTable = new D2Point[9, 9];

		// Token: 0x040000D3 RID: 211
		private static DDTaskList 波紋s = new DDTaskList();

		// Token: 0x040000D4 RID: 212
		private static D2Point[,] PointTable = new D2Point[9, 9];

		// Token: 0x020000C7 RID: 199
		private class 波紋Task_波紋 : DDTask
		{
			// Token: 0x060003FA RID: 1018 RVA: 0x00015178 File Offset: 0x00013378
			public override IEnumerable<bool> E_Task()
			{
				foreach (DDScene scene in DDSceneUtils.Create(this.FrameMax))
				{
					for (int x = 0; x <= 8; x++)
					{
						for (int y = 0; y <= 8; y++)
						{
							RippleEffect.PointTable[x, y] = this.ShiftPt(RippleEffect.PointTable[x, y], scene.Rate);
						}
					}
					yield return true;
				}
				IEnumerator<DDScene> enumerator = null;
				yield break;
				yield break;
			}

			// Token: 0x060003FB RID: 1019 RVA: 0x00015188 File Offset: 0x00013388
			public D2Point ShiftPt(D2Point pt, double rate)
			{
				pt.X -= this.Center_X;
				pt.Y -= this.Center_Y;
				double wave_r = rate * 1000.0;
				double distance = DDUtils.GetDistance(pt);
				double d = distance;
				d -= wave_r;
				d /= 50.0;
				d += 1.0;
				d /= 2.0;
				if (0.0 < d && d < 1.0)
				{
					d *= 2.0;
					if (1.0 < d)
					{
						d = 2.0 - d;
					}
					distance += DDUtils.SCurve(d) * 50.0 * (1.0 - rate * 0.5);
					DDUtils.MakeXYSpeed(0.0, 0.0, pt.X, pt.Y, distance, out pt.X, out pt.Y);
				}
				pt.X += this.Center_X;
				pt.Y += this.Center_Y;
				return pt;
			}

			// Token: 0x040002EB RID: 747
			public double Center_X;

			// Token: 0x040002EC RID: 748
			public double Center_Y;

			// Token: 0x040002ED RID: 749
			public int FrameMax;
		}
	}
}
