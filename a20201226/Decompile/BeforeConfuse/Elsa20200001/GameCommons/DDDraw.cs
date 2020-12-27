using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006D RID: 109
	public static class DDDraw
	{
		// Token: 0x06000157 RID: 343 RVA: 0x0000A4B0 File Offset: 0x000086B0
		public static void Reset()
		{
			DDDraw.Extra = new DDDraw.ExtraInfo();
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000A4BC File Offset: 0x000086BC
		public static void SetTaskList(DDTaskList tl)
		{
			DDDraw.Extra.TL = tl;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000A4C9 File Offset: 0x000086C9
		public static void SetBlendInv()
		{
			DDDraw.Extra.BlendInv = true;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000A4D6 File Offset: 0x000086D6
		public static void SetMosaic()
		{
			DDDraw.Extra.Mosaic = true;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000A4E3 File Offset: 0x000086E3
		public static void SetIntPos()
		{
			DDDraw.Extra.IntPos = true;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000A4F0 File Offset: 0x000086F0
		public static void SetIgnoreError()
		{
			DDDraw.Extra.IgnoreError = true;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000A500 File Offset: 0x00008700
		public static void SetAlpha(double a)
		{
			int pal = SCommon.ToInt(a * 255.0);
			pal = SCommon.ToRange(pal, 0, 255);
			DDDraw.Extra.A = pal;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000A538 File Offset: 0x00008738
		public static void SetBlendAdd(double a)
		{
			int pal = SCommon.ToInt(a * 255.0);
			pal = SCommon.ToRange(pal, 0, 255);
			DDDraw.Extra.BlendAdd = pal;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000A570 File Offset: 0x00008770
		public static void SetBright(double r, double g, double b)
		{
			int pR = SCommon.ToInt(r * 255.0);
			int pG = SCommon.ToInt(g * 255.0);
			int pB = SCommon.ToInt(b * 255.0);
			pR = SCommon.ToRange(pR, 0, 255);
			pG = SCommon.ToRange(pG, 0, 255);
			pB = SCommon.ToRange(pB, 0, 255);
			DDDraw.Extra.Bright = new I3Color(pR, pG, pB);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000A5EC File Offset: 0x000087EC
		public static void SetBright(I3Color color)
		{
			color.R = SCommon.ToRange(color.R, 0, 255);
			color.G = SCommon.ToRange(color.G, 0, 255);
			color.B = SCommon.ToRange(color.B, 0, 255);
			DDDraw.Extra.Bright = color;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000A64C File Offset: 0x0000884C
		private static void SetBlend(int mode, int pal)
		{
			if (DX.SetDrawBlendMode(mode, pal) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000A65D File Offset: 0x0000885D
		private static void ResetBlend()
		{
			if (DX.SetDrawBlendMode(0, 0) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000A66E File Offset: 0x0000886E
		private static void SetBright(int r, int g, int b)
		{
			if (DX.SetDrawBright(r, g, b) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000A680 File Offset: 0x00008880
		private static void ResetBright()
		{
			if (DX.SetDrawBright(255, 255, 255) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000A6A0 File Offset: 0x000088A0
		private static void DrawPic_Main(DDPicture picture, DDDraw.ILayoutInfo layout)
		{
			if (DDDraw.Extra.A != -1)
			{
				DDDraw.SetBlend(1, DDDraw.Extra.A);
			}
			else if (DDDraw.Extra.BlendAdd != -1)
			{
				DDDraw.SetBlend(2, DDDraw.Extra.BlendAdd);
			}
			else if (DDDraw.Extra.BlendInv)
			{
				DDDraw.SetBlend(10, 255);
			}
			if (DDDraw.Extra.Mosaic)
			{
				DX.SetDrawMode(0);
			}
			if (DDDraw.Extra.Bright.R != -1)
			{
				DDDraw.SetBright(DDDraw.Extra.Bright.R, DDDraw.Extra.Bright.G, DDDraw.Extra.Bright.B);
			}
			DDDraw.FreeInfo u = layout as DDDraw.FreeInfo;
			if (u != null)
			{
				if ((DDDraw.Extra.IntPos ? (DX.DrawModiGraph(SCommon.ToInt(u.LTX), SCommon.ToInt(u.LTY), SCommon.ToInt(u.RTX), SCommon.ToInt(u.RTY), SCommon.ToInt(u.RBX), SCommon.ToInt(u.RBY), SCommon.ToInt(u.LBX), SCommon.ToInt(u.LBY), picture.GetHandle(), 1) != 0) : (DX.DrawModiGraphF((float)u.LTX, (float)u.LTY, (float)u.RTX, (float)u.RTY, (float)u.RBX, (float)u.RBY, (float)u.LBX, (float)u.LBY, picture.GetHandle(), 1) != 0)) && !DDDraw.Extra.IgnoreError)
				{
					throw new DDError();
				}
			}
			else
			{
				DDDraw.RectInfo u2 = layout as DDDraw.RectInfo;
				if (u2 != null)
				{
					if ((DDDraw.Extra.IntPos ? (DX.DrawExtendGraph(SCommon.ToInt(u2.L), SCommon.ToInt(u2.T), SCommon.ToInt(u2.R), SCommon.ToInt(u2.B), picture.GetHandle(), 1) != 0) : (DX.DrawExtendGraphF((float)u2.L, (float)u2.T, (float)u2.R, (float)u2.B, picture.GetHandle(), 1) != 0)) && !DDDraw.Extra.IgnoreError)
					{
						throw new DDError();
					}
				}
				else
				{
					DDDraw.SimpleInfo u3 = layout as DDDraw.SimpleInfo;
					if (u3 == null)
					{
						throw new DDError();
					}
					if ((DDDraw.Extra.IntPos ? (DX.DrawGraph(SCommon.ToInt(u3.X), SCommon.ToInt(u3.Y), picture.GetHandle(), 1) != 0) : (DX.DrawGraphF((float)u3.X, (float)u3.Y, picture.GetHandle(), 1) != 0)) && !DDDraw.Extra.IgnoreError)
					{
						throw new DDError();
					}
				}
			}
			if (DDDraw.Extra.A != -1 || DDDraw.Extra.BlendAdd != -1 || DDDraw.Extra.BlendInv)
			{
				DDDraw.ResetBlend();
			}
			if (DDDraw.Extra.Mosaic)
			{
				DX.SetDrawMode(2);
			}
			if (DDDraw.Extra.Bright.R != -1)
			{
				DDDraw.ResetBright();
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000A9A0 File Offset: 0x00008BA0
		private static void DrawPic(DDPicture picture, DDDraw.ILayoutInfo layout)
		{
			if (DDDraw.Extra.TL == null)
			{
				DDDraw.DrawPic_Main(picture, layout);
				return;
			}
			DDDraw.ExtraInfo storedExtra = DDDraw.Extra;
			DDDraw.Extra.TL.Add(delegate
			{
				DDDraw.ExtraInfo extra = DDDraw.Extra;
				DDDraw.Extra = storedExtra;
				DDDraw.DrawPic_Main(picture, layout);
				DDDraw.Extra = extra;
				return false;
			});
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000AA08 File Offset: 0x00008C08
		public static void DrawFree(DDPicture picture, double ltx, double lty, double rtx, double rty, double rbx, double rby, double lbx, double lby)
		{
			DDDraw.FreeInfo layout = new DDDraw.FreeInfo
			{
				LTX = ltx,
				LTY = lty,
				RTX = rtx,
				RTY = rty,
				RBX = rbx,
				RBY = rby,
				LBX = lbx,
				LBY = lby
			};
			DDDraw.DrawPic(picture, layout);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000AA60 File Offset: 0x00008C60
		public static void DrawFree(DDPicture picture, D2Point lt, D2Point rt, D2Point rb, D2Point lb)
		{
			DDDraw.DrawFree(picture, lt.X, lt.Y, rt.X, rt.Y, rb.X, rb.Y, lb.X, lb.Y);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000AAA5 File Offset: 0x00008CA5
		public static void DrawFree(DDPicture picture, P4Poly poly)
		{
			DDDraw.DrawFree(picture, poly.LT, poly.RT, poly.RB, poly.LB);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000AAC8 File Offset: 0x00008CC8
		public static void DrawRect_LTRB(DDPicture picture, double l, double t, double r, double b)
		{
			if (l < -1000000000.0 || 999999999.0 < l || t < -1000000000.0 || 999999999.0 < t || r < l + 1.0 || 1000000000.0 < r || b < t + 1.0 || 1000000000.0 < b)
			{
				throw new DDError();
			}
			DDDraw.RectInfo layout = new DDDraw.RectInfo
			{
				L = l,
				T = t,
				R = r,
				B = b
			};
			DDDraw.DrawPic(picture, layout);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000AB6B File Offset: 0x00008D6B
		public static void DrawRect(DDPicture picture, double l, double t, double w, double h)
		{
			DDDraw.DrawRect_LTRB(picture, l, t, l + w, t + h);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000AB7C File Offset: 0x00008D7C
		public static void DrawRect(DDPicture picture, D4Rect rect)
		{
			DDDraw.DrawRect(picture, rect.L, rect.T, rect.W, rect.H);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000AB9C File Offset: 0x00008D9C
		public static void DrawSimple(DDPicture picture, double x, double y)
		{
			if (x < -1000000000.0 || 1000000000.0 < x || y < -1000000000.0 || 1000000000.0 < y)
			{
				throw new DDError();
			}
			DDDraw.SimpleInfo layout = new DDDraw.SimpleInfo
			{
				X = x,
				Y = y
			};
			DDDraw.DrawPic(picture, layout);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000ABFC File Offset: 0x00008DFC
		public static void DrawCenter(DDPicture picture, double x, double y)
		{
			if (x < -1000000000.0 || 1000000000.0 < x || y < -1000000000.0 || 1000000000.0 < y)
			{
				throw new DDError();
			}
			DDDraw.DrawBegin(picture, x, y);
			DDDraw.DrawEnd();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000AC4C File Offset: 0x00008E4C
		public static void DrawBeginRect_LTRB(DDPicture picture, double l, double t, double r, double b)
		{
			DDDraw.DrawBeginRect(picture, l, t, r - l, b - t);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000AC5D File Offset: 0x00008E5D
		public static void DrawBeginRect(DDPicture picture, double l, double t, double w, double h)
		{
			DDDraw.DrawBegin(picture, l + w / 2.0, t + h / 2.0);
			DDDraw.DrawSetSize(w, h);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000AC88 File Offset: 0x00008E88
		public static void DrawBegin(DDPicture picture, double x, double y)
		{
			if (DDDraw.DB.Picture != null)
			{
				throw new DDError();
			}
			double w = (double)picture.Get_W();
			double h = (double)picture.Get_H();
			w /= 2.0;
			h /= 2.0;
			DDDraw.DB.Picture = picture;
			DDDraw.DB.X = x;
			DDDraw.DB.Y = y;
			DDDraw.DB.Layout = new DDDraw.FreeInfo
			{
				LTX = -w,
				LTY = -h,
				RTX = w,
				RTY = -h,
				RBX = w,
				RBY = h,
				LBX = -w,
				LBY = h
			};
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000AD3C File Offset: 0x00008F3C
		public static void DrawSlide(double x, double y)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			DDDraw.DB.Layout.LTX += x;
			DDDraw.DB.Layout.LTY += y;
			DDDraw.DB.Layout.RTX += x;
			DDDraw.DB.Layout.RTY += y;
			DDDraw.DB.Layout.RBX += x;
			DDDraw.DB.Layout.RBY += y;
			DDDraw.DB.Layout.LBX += x;
			DDDraw.DB.Layout.LBY += y;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000AE14 File Offset: 0x00009014
		public static void DrawRotate(double rot)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			DDUtils.Rotate(ref DDDraw.DB.Layout.LTX, ref DDDraw.DB.Layout.LTY, rot);
			DDUtils.Rotate(ref DDDraw.DB.Layout.RTX, ref DDDraw.DB.Layout.RTY, rot);
			DDUtils.Rotate(ref DDDraw.DB.Layout.RBX, ref DDDraw.DB.Layout.RBY, rot);
			DDUtils.Rotate(ref DDDraw.DB.Layout.LBX, ref DDDraw.DB.Layout.LBY, rot);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000AEC4 File Offset: 0x000090C4
		public static void DrawZoom_X(double z)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			DDDraw.DB.Layout.LTX *= z;
			DDDraw.DB.Layout.RTX *= z;
			DDDraw.DB.Layout.RBX *= z;
			DDDraw.DB.Layout.LBX *= z;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000AF40 File Offset: 0x00009140
		public static void DrawZoom_Y(double z)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			DDDraw.DB.Layout.LTY *= z;
			DDDraw.DB.Layout.RTY *= z;
			DDDraw.DB.Layout.RBY *= z;
			DDDraw.DB.Layout.LBY *= z;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000AFBB File Offset: 0x000091BB
		public static void DrawZoom(double z)
		{
			DDDraw.DrawZoom_X(z);
			DDDraw.DrawZoom_Y(z);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000AFCC File Offset: 0x000091CC
		public static void DrawSetSize_W(double w)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			w /= 2.0;
			DDDraw.DB.Layout.LTX = -w;
			DDDraw.DB.Layout.RTX = w;
			DDDraw.DB.Layout.RBX = w;
			DDDraw.DB.Layout.LBX = -w;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000B03C File Offset: 0x0000923C
		public static void DrawSetSize_H(double h)
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			h /= 2.0;
			DDDraw.DB.Layout.LTY = -h;
			DDDraw.DB.Layout.RTY = -h;
			DDDraw.DB.Layout.RBY = h;
			DDDraw.DB.Layout.LBY = h;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000B0AA File Offset: 0x000092AA
		public static void DrawSetSize(double w, double h)
		{
			DDDraw.DrawSetSize_W(w);
			DDDraw.DrawSetSize_H(h);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000B0B8 File Offset: 0x000092B8
		public static void DrawEnd()
		{
			if (DDDraw.DB.Picture == null)
			{
				throw new DDError();
			}
			DDDraw.DB.Layout.LTX += DDDraw.DB.X;
			DDDraw.DB.Layout.LTY += DDDraw.DB.Y;
			DDDraw.DB.Layout.RTX += DDDraw.DB.X;
			DDDraw.DB.Layout.RTY += DDDraw.DB.Y;
			DDDraw.DB.Layout.RBX += DDDraw.DB.X;
			DDDraw.DB.Layout.RBY += DDDraw.DB.Y;
			DDDraw.DB.Layout.LBX += DDDraw.DB.X;
			DDDraw.DB.Layout.LBY += DDDraw.DB.Y;
			DDDraw.DrawPic(DDDraw.DB.Picture, DDDraw.DB.Layout);
			DDDraw.DB.Picture = null;
		}

		// Token: 0x0400017E RID: 382
		private static DDDraw.ExtraInfo Extra = new DDDraw.ExtraInfo();

		// Token: 0x0400017F RID: 383
		private static DDDraw.DBInfo DB = default(DDDraw.DBInfo);

		// Token: 0x0200011B RID: 283
		private class ExtraInfo
		{
			// Token: 0x040004B6 RID: 1206
			public DDTaskList TL;

			// Token: 0x040004B7 RID: 1207
			public bool BlendInv;

			// Token: 0x040004B8 RID: 1208
			public bool Mosaic;

			// Token: 0x040004B9 RID: 1209
			public bool IntPos;

			// Token: 0x040004BA RID: 1210
			public bool IgnoreError;

			// Token: 0x040004BB RID: 1211
			public int A = -1;

			// Token: 0x040004BC RID: 1212
			public int BlendAdd = -1;

			// Token: 0x040004BD RID: 1213
			public I3Color Bright = new I3Color(-1, 0, 0);
		}

		// Token: 0x0200011C RID: 284
		private interface ILayoutInfo
		{
		}

		// Token: 0x0200011D RID: 285
		private class FreeInfo : DDDraw.ILayoutInfo
		{
			// Token: 0x040004BE RID: 1214
			public double LTX;

			// Token: 0x040004BF RID: 1215
			public double LTY;

			// Token: 0x040004C0 RID: 1216
			public double RTX;

			// Token: 0x040004C1 RID: 1217
			public double RTY;

			// Token: 0x040004C2 RID: 1218
			public double RBX;

			// Token: 0x040004C3 RID: 1219
			public double RBY;

			// Token: 0x040004C4 RID: 1220
			public double LBX;

			// Token: 0x040004C5 RID: 1221
			public double LBY;
		}

		// Token: 0x0200011E RID: 286
		private class RectInfo : DDDraw.ILayoutInfo
		{
			// Token: 0x040004C6 RID: 1222
			public double L;

			// Token: 0x040004C7 RID: 1223
			public double T;

			// Token: 0x040004C8 RID: 1224
			public double R;

			// Token: 0x040004C9 RID: 1225
			public double B;
		}

		// Token: 0x0200011F RID: 287
		private class SimpleInfo : DDDraw.ILayoutInfo
		{
			// Token: 0x040004CA RID: 1226
			public double X;

			// Token: 0x040004CB RID: 1227
			public double Y;
		}

		// Token: 0x02000120 RID: 288
		private struct DBInfo
		{
			// Token: 0x040004CC RID: 1228
			public DDPicture Picture;

			// Token: 0x040004CD RID: 1229
			public double X;

			// Token: 0x040004CE RID: 1230
			public double Y;

			// Token: 0x040004CF RID: 1231
			public DDDraw.FreeInfo Layout;
		}
	}
}
