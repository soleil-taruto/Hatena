using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons.Options;

namespace Charlotte.GameCommons
{
	public static class DDDraw
	{
		// Extra >

		private class ExtraInfo
		{
			public DDTaskList TL = null;
			public bool BlendInv = false;
			public bool Mosaic = false;
			public bool IntPos = false;
			public bool IgnoreError = false;
			public int A = -1; // -1 == 無効
			public int BlendAdd = -1; // -1 == 無効
			public I3Color Bright = new I3Color(-1, 0, 0);
		};

		private static ExtraInfo Extra = new ExtraInfo();

		public static void Reset()
		{
			Extra = new ExtraInfo();
		}

		public static void SetTaskList(DDTaskList tl)
		{
			Extra.TL = tl;
		}

		public static void SetBlendInv()
		{
			Extra.BlendInv = true;
		}

		public static void SetMosaic()
		{
			Extra.Mosaic = true;
		}

		public static void SetIntPos()
		{
			Extra.IntPos = true;
		}

		public static void SetIgnoreError()
		{
			Extra.IgnoreError = true;
		}

		public static void SetAlpha(double a)
		{
			int pal = SCommon.ToInt(a * 255.0);

			pal = SCommon.ToRange(pal, 0, 255);

			Extra.A = pal;
		}

		public static void SetBlendAdd(double a)
		{
			int pal = SCommon.ToInt(a * 255.0);

			pal = SCommon.ToRange(pal, 0, 255);

			Extra.BlendAdd = pal;
		}

		public static void SetBright(double r, double g, double b)
		{
			int pR = SCommon.ToInt(r * 255.0);
			int pG = SCommon.ToInt(g * 255.0);
			int pB = SCommon.ToInt(b * 255.0);

			pR = SCommon.ToRange(pR, 0, 255);
			pG = SCommon.ToRange(pG, 0, 255);
			pB = SCommon.ToRange(pB, 0, 255);

			Extra.Bright = new I3Color(pR, pG, pB);
		}

		public static void SetBright(I3Color color)
		{
			color.R = SCommon.ToRange(color.R, 0, 255);
			color.G = SCommon.ToRange(color.G, 0, 255);
			color.B = SCommon.ToRange(color.B, 0, 255);

			Extra.Bright = color;
		}

		// < Extra

		private interface ILayoutInfo
		{ }

		private class FreeInfo : ILayoutInfo
		{
			public double LTX;
			public double LTY;
			public double RTX;
			public double RTY;
			public double RBX;
			public double RBY;
			public double LBX;
			public double LBY;
		}

		private class RectInfo : ILayoutInfo
		{
			public double L;
			public double T;
			public double R;
			public double B;
		}

		private class SimpleInfo : ILayoutInfo
		{
			public double X;
			public double Y;
		}

		private static void SetBlend(int mode, int pal)
		{
			if (DX.SetDrawBlendMode(mode, pal) != 0) // ? 失敗
				throw new DDError();
		}

		private static void ResetBlend()
		{
			if (DX.SetDrawBlendMode(DX.DX_BLENDMODE_NOBLEND, 0) != 0) // ? 失敗
				throw new DDError();
		}

		private static void SetBright(int r, int g, int b)
		{
			if (DX.SetDrawBright(r, g, b) != 0) // ? 失敗
				throw new DDError();
		}

		private static void ResetBright()
		{
			if (DX.SetDrawBright(255, 255, 255) != 0) // ? 失敗
				throw new DDError();
		}

		private static void DrawPic_Main(DDPicture picture, ILayoutInfo layout)
		{
			if (Extra.A != -1)
			{
				SetBlend(DX.DX_BLENDMODE_ALPHA, Extra.A);
			}
			else if (Extra.BlendAdd != -1)
			{
				SetBlend(DX.DX_BLENDMODE_ADD, Extra.BlendAdd);
			}
			else if (Extra.BlendInv)
			{
				SetBlend(DX.DX_BLENDMODE_INVSRC, 255);
			}

			if (Extra.Mosaic)
			{
				DX.SetDrawMode(DX.DX_DRAWMODE_NEAREST);
			}
			if (Extra.Bright.R != -1)
			{
				SetBright(Extra.Bright.R, Extra.Bright.G, Extra.Bright.B);
			}

			{
				FreeInfo u = layout as FreeInfo;

				if (u != null)
				{
					// ? 失敗
					if (
						Extra.IntPos ?
						DX.DrawModiGraph(
							SCommon.ToInt(u.LTX),
							SCommon.ToInt(u.LTY),
							SCommon.ToInt(u.RTX),
							SCommon.ToInt(u.RTY),
							SCommon.ToInt(u.RBX),
							SCommon.ToInt(u.RBY),
							SCommon.ToInt(u.LBX),
							SCommon.ToInt(u.LBY),
							picture.GetHandle(),
							1
							)
							!= 0
						:
						DX.DrawModiGraphF(
							(float)u.LTX,
							(float)u.LTY,
							(float)u.RTX,
							(float)u.RTY,
							(float)u.RBX,
							(float)u.RBY,
							(float)u.LBX,
							(float)u.LBY,
							picture.GetHandle(),
							1
							)
							!= 0
						)
					{
						if (!Extra.IgnoreError)
							throw new DDError();
					}
					goto endDraw;
				}
			}

			{
				RectInfo u = layout as RectInfo;

				if (u != null)
				{
					// ? 失敗
					if (
						Extra.IntPos ?
						DX.DrawExtendGraph(
							SCommon.ToInt(u.L),
							SCommon.ToInt(u.T),
							SCommon.ToInt(u.R),
							SCommon.ToInt(u.B),
							picture.GetHandle(),
							1
							)
							!= 0
						:
						DX.DrawExtendGraphF(
							(float)u.L,
							(float)u.T,
							(float)u.R,
							(float)u.B,
							picture.GetHandle(),
							1
							)
							!= 0
						)
					{
						if (!Extra.IgnoreError)
							throw new DDError();
					}
					goto endDraw;
				}
			}

			{
				SimpleInfo u = layout as SimpleInfo;

				if (u != null)
				{
					// ? 失敗
					if (
						Extra.IntPos ?
						DX.DrawGraph(
							SCommon.ToInt(u.X),
							SCommon.ToInt(u.Y),
							picture.GetHandle(),
							1
							)
							!= 0
						:
						DX.DrawGraphF(
							(float)(u.X),
							(float)(u.Y),
							picture.GetHandle(),
							1
							)
							!= 0
						)
					{
						if (!Extra.IgnoreError)
							throw new DDError();
					}
					goto endDraw;
				}
			}

			throw new DDError(); // ? 不明なレイアウト
		endDraw:

			if (Extra.A != -1 || Extra.BlendAdd != -1 || Extra.BlendInv)
			{
				ResetBlend();
			}
			if (Extra.Mosaic)
			{
				DX.SetDrawMode(DDConsts.DEFAULT_DX_DRAWMODE);
			}
			if (Extra.Bright.R != -1)
			{
				ResetBright();
			}
		}

		private static void DrawPic(DDPicture picture, ILayoutInfo layout)
		{
			if (Extra.TL == null)
			{
				DrawPic_Main(picture, layout);
			}
			else
			{
				ExtraInfo storedExtra = Extra;

				Extra.TL.Add(() =>
				{
					ExtraInfo currExtra = Extra;

					Extra = storedExtra;
					DrawPic_Main(picture, layout);
					Extra = currExtra;

					return false;
				});
			}
		}

		public static void DrawFree(DDPicture picture, double ltx, double lty, double rtx, double rty, double rbx, double rby, double lbx, double lby)
		{
			FreeInfo layout = new FreeInfo()
			{
				LTX = ltx,
				LTY = lty,
				RTX = rtx,
				RTY = rty,
				RBX = rbx,
				RBY = rby,
				LBX = lbx,
				LBY = lby,
			};

			DrawPic(picture, layout);
		}

		public static void DrawFree(DDPicture picture, D2Point lt, D2Point rt, D2Point rb, D2Point lb)
		{
			DrawFree(picture, lt.X, lt.Y, rt.X, rt.Y, rb.X, rb.Y, lb.X, lb.Y);
		}

		public static void DrawFree(DDPicture picture, P4Poly poly)
		{
			DrawFree(picture, poly.LT, poly.RT, poly.RB, poly.LB);
		}

		public static void DrawRect_LTRB(DDPicture picture, double l, double t, double r, double b)
		{
			if (
				l < -(double)SCommon.IMAX || (double)SCommon.IMAX - 1.0 < l ||
				t < -(double)SCommon.IMAX || (double)SCommon.IMAX - 1.0 < t ||
				r < l + 1.0 || (double)SCommon.IMAX < r ||
				b < t + 1.0 || (double)SCommon.IMAX < b
				)
				throw new DDError();

			RectInfo layout = new RectInfo()
			{
				L = l,
				T = t,
				R = r,
				B = b,
			};

			DrawPic(picture, layout);
		}

		public static void DrawRect(DDPicture picture, double l, double t, double w, double h)
		{
			DrawRect_LTRB(picture, l, t, l + w, t + h);
		}

		public static void DrawRect(DDPicture picture, D4Rect rect)
		{
			DrawRect(picture, rect.L, rect.T, rect.W, rect.H);
		}

		public static void DrawSimple(DDPicture picture, double x, double y)
		{
			if (
				x < -(double)SCommon.IMAX || (double)SCommon.IMAX < x ||
				y < -(double)SCommon.IMAX || (double)SCommon.IMAX < y
				)
				throw new DDError();

			SimpleInfo layout = new SimpleInfo()
			{
				X = x,
				Y = y,
			};

			DrawPic(picture, layout);
		}

		public static void DrawCenter(DDPicture picture, double x, double y)
		{
			if (
				x < -(double)SCommon.IMAX || (double)SCommon.IMAX < x ||
				y < -(double)SCommon.IMAX || (double)SCommon.IMAX < y
				)
				throw new DDError();

			DrawBegin(picture, x, y);
			DrawEnd();
		}

		// ====
		// DrawBegin ～ DrawEnd ここから
		// ====

		private struct DBInfo
		{
			public DDPicture Picture; // null == 無効
			public double X;
			public double Y;
			public FreeInfo Layout;
		}

		private static DBInfo DB = new DBInfo();

		public static void DrawBeginRect_LTRB(DDPicture picture, double l, double t, double r, double b)
		{
			DrawBeginRect(picture, l, t, r - l, b - t);
		}

		public static void DrawBeginRect(DDPicture picture, double l, double t, double w, double h)
		{
			DrawBegin(picture, l + w / 2.0, t + h / 2.0);
			DrawSetSize(w, h);
		}

		public static void DrawBegin(DDPicture picture, double x, double y)
		{
			if (DB.Picture != null)
				throw new DDError();

			double w = picture.Get_W();
			double h = picture.Get_H();

			w /= 2.0;
			h /= 2.0;

			DB.Picture = picture;
			DB.X = x;
			DB.Y = y;
			DB.Layout = new FreeInfo()
			{
				LTX = -w,
				LTY = -h,
				RTX = w,
				RTY = -h,
				RBX = w,
				RBY = h,
				LBX = -w,
				LBY = h,
			};
		}

		public static void DrawSlide(double x, double y)
		{
			if (DB.Picture == null)
				throw new DDError();

			DB.Layout.LTX += x;
			DB.Layout.LTY += y;
			DB.Layout.RTX += x;
			DB.Layout.RTY += y;
			DB.Layout.RBX += x;
			DB.Layout.RBY += y;
			DB.Layout.LBX += x;
			DB.Layout.LBY += y;
		}

		public static void DrawRotate(double rot)
		{
			if (DB.Picture == null)
				throw new DDError();

			DDUtils.Rotate(ref DB.Layout.LTX, ref DB.Layout.LTY, rot);
			DDUtils.Rotate(ref DB.Layout.RTX, ref DB.Layout.RTY, rot);
			DDUtils.Rotate(ref DB.Layout.RBX, ref DB.Layout.RBY, rot);
			DDUtils.Rotate(ref DB.Layout.LBX, ref DB.Layout.LBY, rot);
		}

		public static void DrawZoom_X(double z)
		{
			if (DB.Picture == null)
				throw new DDError();

			DB.Layout.LTX *= z;
			DB.Layout.RTX *= z;
			DB.Layout.RBX *= z;
			DB.Layout.LBX *= z;
		}

		public static void DrawZoom_Y(double z)
		{
			if (DB.Picture == null)
				throw new DDError();

			DB.Layout.LTY *= z;
			DB.Layout.RTY *= z;
			DB.Layout.RBY *= z;
			DB.Layout.LBY *= z;
		}

		public static void DrawZoom(double z)
		{
			DrawZoom_X(z);
			DrawZoom_Y(z);
		}

		public static void DrawSetSize_W(double w)
		{
			if (DB.Picture == null)
				throw new DDError();

			w /= 2.0;

			DB.Layout.LTX = -w;
			DB.Layout.RTX = w;
			DB.Layout.RBX = w;
			DB.Layout.LBX = -w;
		}

		public static void DrawSetSize_H(double h)
		{
			if (DB.Picture == null)
				throw new DDError();

			h /= 2.0;

			DB.Layout.LTY = -h;
			DB.Layout.RTY = -h;
			DB.Layout.RBY = h;
			DB.Layout.LBY = h;
		}

		public static void DrawSetSize(double w, double h)
		{
			DrawSetSize_W(w);
			DrawSetSize_H(h);
		}

		public static void DrawEnd()
		{
			if (DB.Picture == null)
				throw new DDError();

			DB.Layout.LTX += DB.X;
			DB.Layout.LTY += DB.Y;
			DB.Layout.RTX += DB.X;
			DB.Layout.RTY += DB.Y;
			DB.Layout.RBX += DB.X;
			DB.Layout.RBY += DB.Y;
			DB.Layout.LBX += DB.X;
			DB.Layout.LBY += DB.Y;

			DrawPic(DB.Picture, DB.Layout);

			DB.Picture = null;
		}

		// ====
		// DrawBegin ～ DrawEnd ここまで
		// ====
	}
}
