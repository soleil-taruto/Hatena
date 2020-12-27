using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using DxLibDLL;

namespace Charlotte.Games
{
	// Token: 0x0200001C RID: 28
	public static class FieldDividerEffect
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00006560 File Offset: 0x00004760
		public static void INIT()
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					FieldDividerEffect.PieceTable[x, y] = new DDSubScreen(170, 170, false);
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000065A1 File Offset: 0x000047A1
		private static DDSubScreen P_Field
		{
			get
			{
				return Game.I.Field_Last;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000065B0 File Offset: 0x000047B0
		public static void Enter()
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					FieldDividerEffect.PieceTable[x, y].ChangeDrawScreen();
					DX.DrawRectGraph(0, 0, x * 171, y * 171, x * 171 + 170, y * 171 + 170, FieldDividerEffect.P_Field.GetHandle(), 0);
				}
			}
			DDGround.MainScreen.ChangeDrawScreen();
			Func<bool> f = new FieldDividerEffect.DrawPieceTask(0, 0, 1, 0, false).Task;
			Func<bool> f2 = new FieldDividerEffect.DrawPieceTask(1, 0, 2, 0, true).Task;
			Func<bool> f3 = new FieldDividerEffect.DrawPieceTask(2, 0, 2, 1, false).Task;
			Func<bool> f4 = new FieldDividerEffect.DrawPieceTask(2, 1, 2, 2, false).Task;
			Func<bool> f5 = new FieldDividerEffect.DrawPieceTask(2, 2, 1, 2, false).Task;
			Func<bool> f6 = new FieldDividerEffect.DrawPieceTask(1, 2, 0, 2, false).Task;
			Func<bool> f7 = new FieldDividerEffect.DrawPieceTask(0, 2, 0, 1, false).Task;
			Func<bool> f8 = new FieldDividerEffect.DrawPieceTask(0, 1, 0, 0, false).Task;
			Func<bool> f9 = new FieldDividerEffect.DrawPieceTask(1, 1, 1, 1, false).Task;
			DDCurtain.SetCurtain(0, 1.0);
			DDCurtain.SetCurtain(20, 0.0);
			for (;;)
			{
				FieldDividerEffect.DrawWall();
				if (!f())
				{
					break;
				}
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
			DDCurtain.SetCurtain(10, 0.0);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00006760 File Offset: 0x00004960
		private static void DrawWall()
		{
			DDUtils.Approach(ref Game.I.BackgroundSlideRate, Game.I.Player.Y * 1.0 / 512.0, 0.99);
			D4Rect rect = DDUtils.AdjustRectExterior(new D2Size(512.0, 512.0), new D4Rect(0.0, 0.0, 960.0, 540.0), Game.I.BackgroundSlideRate);
			DDDraw.DrawRect(FieldDividerEffect.P_Field.ToPicture(), rect);
			DDDraw.SetBright(0.0, 0.0, 0.0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 219.0, 9.0, 522.0, 522.0);
			DDDraw.Reset();
			DX.GraphFilter(DDGround.MainScreen.GetHandle(), 1, 16, SCommon.ToInt(500.0));
			DDCurtain.DrawCurtain(0.2);
			DX.GraphBlend(FieldDividerEffect.GrayScreen_R.GetHandle(), DDGround.MainScreen.GetHandle(), 255, 1, 4, 4, 4, 3);
			DX.GraphBlend(FieldDividerEffect.GrayScreen_G.GetHandle(), DDGround.MainScreen.GetHandle(), 255, 1, 5, 5, 5, 3);
			DX.GraphBlend(FieldDividerEffect.GrayScreen_B.GetHandle(), DDGround.MainScreen.GetHandle(), 255, 1, 6, 6, 6, 3);
			DDDraw.DrawSimple(FieldDividerEffect.GrayScreen_R.ToPicture(), 0.0, 0.0);
			DDDraw.SetAlpha(0.5);
			DDDraw.DrawSimple(FieldDividerEffect.GrayScreen_G.ToPicture(), 0.0, 0.0);
			DDDraw.SetAlpha(0.333);
			DDDraw.DrawSimple(FieldDividerEffect.GrayScreen_B.ToPicture(), 0.0, 0.0);
			DDDraw.Reset();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000697F File Offset: 0x00004B7F
		public static void Leave()
		{
			DDCurtain.SetCurtain(0, 0.5);
			DDCurtain.SetCurtain(10, 0.0);
		}

		// Token: 0x040000C5 RID: 197
		private const int PIECE_WH = 170;

		// Token: 0x040000C6 RID: 198
		private const int PIECE_XY_STEP = 171;

		// Token: 0x040000C7 RID: 199
		private const int PIECES_W = 3;

		// Token: 0x040000C8 RID: 200
		private const int PIECES_H = 3;

		// Token: 0x040000C9 RID: 201
		private static DDSubScreen[,] PieceTable = new DDSubScreen[3, 3];

		// Token: 0x040000CA RID: 202
		private static DDSubScreen GrayScreen_R = new DDSubScreen(960, 540, false);

		// Token: 0x040000CB RID: 203
		private static DDSubScreen GrayScreen_G = new DDSubScreen(960, 540, false);

		// Token: 0x040000CC RID: 204
		private static DDSubScreen GrayScreen_B = new DDSubScreen(960, 540, false);

		// Token: 0x020000C6 RID: 198
		private class DrawPieceTask : DDTask
		{
			// Token: 0x060003F7 RID: 1015 RVA: 0x00014F70 File Offset: 0x00013170
			public DrawPieceTask(int x1, int y1, int x2, int y2, bool doRot)
			{
				this.Picture = FieldDividerEffect.PieceTable[x1, y1].ToPicture();
				this.DrawPos_01 = new FieldDividerEffect.DrawPieceTask.DrawPosInfo
				{
					X = (double)(x1 * 171 + 85),
					Y = (double)(y1 * 171 + 85),
					Rot = 0.0
				};
				this.DrawPos_02 = new FieldDividerEffect.DrawPieceTask.DrawPosInfo
				{
					X = (double)(x1 * 171 + 85) + (double)(x1 - 1) * 30.0,
					Y = (double)(y1 * 171 + 85) + (double)(y1 - 1) * 30.0,
					Rot = 0.0
				};
				this.DrawPos_03 = new FieldDividerEffect.DrawPieceTask.DrawPosInfo
				{
					X = (double)(x2 * 171 + 85) + (double)(x2 - 1) * 30.0,
					Y = (double)(y2 * 171 + 85) + (double)(y2 - 1) * 30.0,
					Rot = 0.0
				};
				this.DrawPos_04 = new FieldDividerEffect.DrawPieceTask.DrawPosInfo
				{
					X = (double)(x2 * 171 + 85) + (double)(x2 - 1) * 30.0,
					Y = (double)(y2 * 171 + 85) + (double)(y2 - 1) * 30.0,
					Rot = (doRot ? 1.5707963267948966 : 0.0)
				};
				this.DrawPos_05 = new FieldDividerEffect.DrawPieceTask.DrawPosInfo
				{
					X = (double)(x2 * 171 + 85),
					Y = (double)(y2 * 171 + 85),
					Rot = (doRot ? 1.5707963267948966 : 0.0)
				};
			}

			// Token: 0x060003F8 RID: 1016 RVA: 0x00015143 File Offset: 0x00013343
			public override IEnumerable<bool> E_Task()
			{
				foreach (bool v in this.E_DrawPiace(this.DrawPos_01, this.DrawPos_01, 20))
				{
					yield return v;
				}
				IEnumerator<bool> enumerator = null;
				foreach (bool v2 in this.E_DrawPiace(this.DrawPos_01, this.DrawPos_02, 60))
				{
					yield return v2;
				}
				enumerator = null;
				foreach (bool v3 in this.E_DrawPiace(this.DrawPos_02, this.DrawPos_03, 50))
				{
					yield return v3;
				}
				enumerator = null;
				foreach (bool v4 in this.E_DrawPiace(this.DrawPos_03, this.DrawPos_04, 40))
				{
					yield return v4;
				}
				enumerator = null;
				foreach (bool v5 in this.E_DrawPiace(this.DrawPos_04, this.DrawPos_05, 30))
				{
					yield return v5;
				}
				enumerator = null;
				yield break;
				yield break;
			}

			// Token: 0x060003F9 RID: 1017 RVA: 0x00015153 File Offset: 0x00013353
			private IEnumerable<bool> E_DrawPiace(FieldDividerEffect.DrawPieceTask.DrawPosInfo drawPos_1, FieldDividerEffect.DrawPieceTask.DrawPosInfo drawPos_2, int frameMax)
			{
				foreach (DDScene scene in DDSceneUtils.Create(frameMax))
				{
					double scRate = DDUtils.SCurve(scene.Rate);
					double x = 224.0 + DDUtils.AToBRate(drawPos_1.X, drawPos_2.X, scRate);
					double y = 14.0 + DDUtils.AToBRate(drawPos_1.Y, drawPos_2.Y, scRate);
					double rot = DDUtils.AToBRate(drawPos_1.Rot, drawPos_2.Rot, scRate);
					DDDraw.DrawBegin(this.Picture, x, y);
					DDDraw.DrawRotate(rot);
					DDDraw.DrawEnd();
					yield return true;
				}
				IEnumerator<DDScene> enumerator = null;
				yield break;
				yield break;
			}

			// Token: 0x040002E5 RID: 741
			private DDPicture Picture;

			// Token: 0x040002E6 RID: 742
			private FieldDividerEffect.DrawPieceTask.DrawPosInfo DrawPos_01;

			// Token: 0x040002E7 RID: 743
			private FieldDividerEffect.DrawPieceTask.DrawPosInfo DrawPos_02;

			// Token: 0x040002E8 RID: 744
			private FieldDividerEffect.DrawPieceTask.DrawPosInfo DrawPos_03;

			// Token: 0x040002E9 RID: 745
			private FieldDividerEffect.DrawPieceTask.DrawPosInfo DrawPos_04;

			// Token: 0x040002EA RID: 746
			private FieldDividerEffect.DrawPieceTask.DrawPosInfo DrawPos_05;

			// Token: 0x02000160 RID: 352
			private class DrawPosInfo
			{
				// Token: 0x04000563 RID: 1379
				public double X;

				// Token: 0x04000564 RID: 1380
				public double Y;

				// Token: 0x04000565 RID: 1381
				public double Rot;
			}
		}
	}
}
