using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008C RID: 140
	public class DDSimpleMenu
	{
		// Token: 0x06000230 RID: 560 RVA: 0x0000E9E2 File Offset: 0x0000CBE2
		public DDSimpleMenu() : this(DDUtils.GetMouseDispMode())
		{
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000E9EF File Offset: 0x0000CBEF
		public DDSimpleMenu(bool mouseUsable)
		{
			this.MouseUsable = mouseUsable;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000EA18 File Offset: 0x0000CC18
		private void DrawWallPicture()
		{
			DDDraw.DrawRect(this.WallPicture, DDUtils.AdjustRectExterior(this.WallPicture.GetSize().ToD2Size(), new D4Rect(0.0, 0.0, 960.0, 540.0), 0.5));
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000EA7C File Offset: 0x0000CC7C
		public void DrawWall()
		{
			DDCurtain.DrawCurtain(-1.0);
			if (this.WallColor != null)
			{
				DX.DrawBox(0, 0, 960, 540, DDUtils.GetColor(this.WallColor.Value), 1);
			}
			if (this.WallPicture != null)
			{
				this.DrawWallPicture();
				DDCurtain.DrawCurtain(this.WallCurtain);
			}
			if (this.WallDrawer != null)
			{
				this.WallDrawer();
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000EAF4 File Offset: 0x0000CCF4
		public int Perform(string title, string[] items, int selectIndex, bool ポ\u30FCズボタンでメニュ\u30FC終了 = false, bool noPound = false)
		{
			DDCurtain.SetCurtain(30, 0.0);
			DDEngine.FreezeInput(1);
			for (;;)
			{
				if (this.MouseUsable)
				{
					int musSelIdxY = DDMouse.Y - (this.Y + this.YStep);
					if (0 <= musSelIdxY)
					{
						int musSelIdx = musSelIdxY / this.YStep;
						if (musSelIdx < items.Length)
						{
							selectIndex = musSelIdx;
						}
					}
					if (DDMouse.L.GetInput() == -1)
					{
						goto IL_210;
					}
					if (DDMouse.R.GetInput() == -1)
					{
						break;
					}
				}
				if (ポ\u30FCズボタンでメニュ\u30FC終了 && DDInput.PAUSE.GetInput() == 1)
				{
					goto Block_7;
				}
				bool chgsel = false;
				if (DDInput.A.GetInput() == 1)
				{
					goto IL_210;
				}
				if (DDInput.B.GetInput() == 1)
				{
					if (selectIndex == items.Length - 1)
					{
						goto IL_210;
					}
					selectIndex = items.Length - 1;
					chgsel = true;
				}
				if (noPound ? (DDInput.DIR_8.GetInput() == 1) : DDInput.DIR_8.IsPound())
				{
					selectIndex--;
					chgsel = true;
				}
				if (noPound ? (DDInput.DIR_2.GetInput() == 1) : DDInput.DIR_2.IsPound())
				{
					selectIndex++;
					chgsel = true;
				}
				selectIndex += items.Length;
				selectIndex %= items.Length;
				if (this.MouseUsable && chgsel)
				{
					DDMouse.X = 0;
					DDMouse.Y = this.Y + (selectIndex + 1) * this.YStep + this.YStep / 2;
					DDMouse.PosChanged();
				}
				this.DrawWall();
				if (this.Color != null)
				{
					DDPrint.SetColor(this.Color.Value);
				}
				if (this.BorderColor != null)
				{
					DDPrint.SetBorder(this.BorderColor.Value, 1);
				}
				DDPrint.SetPrint(this.X, this.Y, this.YStep);
				DDPrint.Print(title + "\u3000(Mouse=" + this.MouseUsable.ToString() + ")");
				DDPrint.PrintRet();
				for (int c = 0; c < items.Length; c++)
				{
					DDPrint.Print(string.Format("[{0}] {1}", (selectIndex == c) ? ">" : " ", items[c]));
					DDPrint.PrintRet();
				}
				DDPrint.Reset();
				DDEngine.EachFrame();
			}
			selectIndex = items.Length - 1;
			goto IL_210;
			Block_7:
			selectIndex = items.Length - 1;
			IL_210:
			DDEngine.FreezeInput(1);
			return selectIndex;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000ED18 File Offset: 0x0000CF18
		public void PadConfig()
		{
			DDSimpleMenu.ButtonInfo[] btnInfos = new DDSimpleMenu.ButtonInfo[]
			{
				new DDSimpleMenu.ButtonInfo(DDInput.DIR_2, "下"),
				new DDSimpleMenu.ButtonInfo(DDInput.DIR_4, "左"),
				new DDSimpleMenu.ButtonInfo(DDInput.DIR_6, "右"),
				new DDSimpleMenu.ButtonInfo(DDInput.DIR_8, "上"),
				new DDSimpleMenu.ButtonInfo(DDInput.A, "低速／決定"),
				new DDSimpleMenu.ButtonInfo(DDInput.B, "ショット／キャンセル"),
				new DDSimpleMenu.ButtonInfo(DDInput.C, "ボム"),
				new DDSimpleMenu.ButtonInfo(DDInput.L, "会話スキップ"),
				new DDSimpleMenu.ButtonInfo(DDInput.R, "当たり判定表示(チート)"),
				new DDSimpleMenu.ButtonInfo(DDInput.PAUSE, "ポーズボタン")
			};
			DDSimpleMenu.ButtonInfo[] array = btnInfos;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Button.Backup();
			}
			try
			{
				array = btnInfos;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Button.BtnId = -1;
				}
				DDCurtain.SetCurtain(30, 0.0);
				DDEngine.FreezeInput(1);
				int currBtnIndex = 0;
				while (currBtnIndex < btnInfos.Length)
				{
					if (DDKey.GetInput(57) == 1)
					{
						return;
					}
					if (DDKey.GetInput(44) == 1)
					{
						currBtnIndex++;
					}
					else
					{
						int pressBtnId = -1;
						for (int padId = 0; padId < DDPad.GetPadCount(); padId++)
						{
							for (int btnId = 0; btnId < 32; btnId++)
							{
								if (DDPad.GetInput(padId, btnId) == 1)
								{
									pressBtnId = btnId;
								}
							}
						}
						for (int c = 0; c < currBtnIndex; c++)
						{
							if (btnInfos[c].Button.BtnId == pressBtnId)
							{
								pressBtnId = -1;
							}
						}
						if (pressBtnId != -1)
						{
							btnInfos[currBtnIndex].Button.BtnId = pressBtnId;
							currBtnIndex++;
						}
					}
					this.DrawWall();
					if (this.Color != null)
					{
						DDPrint.SetColor(this.Color.Value);
					}
					if (this.BorderColor != null)
					{
						DDPrint.SetBorder(this.BorderColor.Value, 1);
					}
					DDPrint.SetPrint(this.X, this.Y, this.YStep);
					DDPrint.Print("ゲームパッドのボタン設定");
					DDPrint.PrintRet();
					for (int c2 = 0; c2 < btnInfos.Length; c2++)
					{
						DDPrint.Print(string.Format("[{0}] {1}", (currBtnIndex == c2) ? ">" : " ", btnInfos[c2].Name));
						if (c2 < currBtnIndex)
						{
							int btnId2 = btnInfos[c2].Button.BtnId;
							DDPrint.Print("\u3000->\u3000");
							if (btnId2 == -1)
							{
								DDPrint.Print("割り当てナシ");
							}
							else
							{
								DDPrint.Print(btnId2.ToString() ?? "");
							}
						}
						DDPrint.PrintRet();
					}
					DDPrint.Print("★\u3000カーソルの機能に割り当てるボタンを押して下さい。");
					DDPrint.PrintRet();
					DDPrint.Print("★\u3000[Z]を押すとボタンの割り当てをスキップします。");
					DDPrint.PrintRet();
					DDPrint.Print("★\u3000スペースを押すとキャンセルします。");
					DDPrint.PrintRet();
					if (this.MouseUsable)
					{
						DDPrint.Print("★\u3000右クリックするとキャンセルします。");
						DDPrint.PrintRet();
						if (DDMouse.R.GetInput() == -1)
						{
							return;
						}
					}
					DDEngine.EachFrame();
				}
				btnInfos = null;
			}
			finally
			{
				if (btnInfos != null)
				{
					array = btnInfos;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Button.Restore();
					}
				}
				DDEngine.FreezeInput(1);
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000F064 File Offset: 0x0000D264
		private static int WinSzExp(int size, int index)
		{
			return size * (8 + index) / 8;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000F070 File Offset: 0x0000D270
		public void WindowSizeConfig()
		{
			string[] items = new string[]
			{
				string.Format("{0} x {1} (デフォルト)", 960, 540),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 1), DDSimpleMenu.WinSzExp(540, 1)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 2), DDSimpleMenu.WinSzExp(540, 2)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 3), DDSimpleMenu.WinSzExp(540, 3)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 4), DDSimpleMenu.WinSzExp(540, 4)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 5), DDSimpleMenu.WinSzExp(540, 5)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 6), DDSimpleMenu.WinSzExp(540, 6)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 7), DDSimpleMenu.WinSzExp(540, 7)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 8), DDSimpleMenu.WinSzExp(540, 8)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 9), DDSimpleMenu.WinSzExp(540, 9)),
				string.Format("{0} x {1}", DDSimpleMenu.WinSzExp(960, 10), DDSimpleMenu.WinSzExp(540, 10)),
				"フルスクリーン 画面に合わせる",
				"フルスクリーン 縦横比維持",
				"フルスクリーン 黒背景 (推奨)",
				"戻る"
			};
			int selectIndex = 0;
			for (;;)
			{
				selectIndex = this.Perform("ウィンドウサイズ設定", items, selectIndex, false, false);
				switch (selectIndex)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
					DDMain.SetScreenSize(DDSimpleMenu.WinSzExp(960, selectIndex), DDSimpleMenu.WinSzExp(540, selectIndex));
					continue;
				case 11:
					DDMain.SetScreenSize(DDGround.MonitorRect.W, DDGround.MonitorRect.H);
					continue;
				case 12:
				{
					int w = DDGround.MonitorRect.W;
					int h = 540 * DDGround.MonitorRect.W / 960;
					if (DDGround.MonitorRect.H < h)
					{
						h = DDGround.MonitorRect.H;
						w = 960 * DDGround.MonitorRect.H / 540;
						if (DDGround.MonitorRect.W < w)
						{
							goto Block_3;
						}
					}
					DDMain.SetScreenSize(w, h);
					continue;
				}
				case 13:
				{
					int w2 = DDGround.MonitorRect.W;
					int h2 = 540 * DDGround.MonitorRect.W / 960;
					if (DDGround.MonitorRect.H < h2)
					{
						h2 = DDGround.MonitorRect.H;
						w2 = 960 * DDGround.MonitorRect.H / 540;
						if (DDGround.MonitorRect.W < w2)
						{
							goto Block_5;
						}
					}
					DDMain.SetScreenSize(DDGround.MonitorRect.W, DDGround.MonitorRect.H);
					DDGround.RealScreenDraw_L = (DDGround.MonitorRect.W - w2) / 2;
					DDGround.RealScreenDraw_T = (DDGround.MonitorRect.H - h2) / 2;
					DDGround.RealScreenDraw_W = w2;
					DDGround.RealScreenDraw_H = h2;
					continue;
				}
				case 14:
					return;
				}
				break;
			}
			throw new DDError();
			Block_3:
			throw new DDError();
			Block_5:
			throw new DDError();
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000F45C File Offset: 0x0000D65C
		public int IntVolumeConfig(string title, int value, int minval, int maxval, int valStep, int valFastStep, Action<int> valChanged, Action pulse)
		{
			int origval = value;
			DDCurtain.SetCurtain(30, 0.0);
			DDEngine.FreezeInput(1);
			for (;;)
			{
				bool chgval = false;
				if (DDInput.A.GetInput() == 1 || (this.MouseUsable && DDMouse.L.GetInput() == -1))
				{
					break;
				}
				if (DDInput.B.GetInput() == 1 || (this.MouseUsable && DDMouse.R.GetInput() == -1))
				{
					if (value == origval)
					{
						break;
					}
					value = origval;
					chgval = true;
				}
				if (this.MouseUsable)
				{
					value += DDMouse.Rot;
					chgval = true;
				}
				if (DDInput.DIR_8.IsPound())
				{
					value += valFastStep;
					chgval = true;
				}
				if (DDInput.DIR_6.IsPound())
				{
					value += valStep;
					chgval = true;
				}
				if (DDInput.DIR_4.IsPound())
				{
					value -= valStep;
					chgval = true;
				}
				if (DDInput.DIR_2.IsPound())
				{
					value -= valFastStep;
					chgval = true;
				}
				if (chgval)
				{
					value = SCommon.ToRange(value, minval, maxval);
					valChanged(value);
				}
				if (DDEngine.ProcFrame % 60 == 0)
				{
					pulse();
				}
				this.DrawWall();
				if (this.Color != null)
				{
					DDPrint.SetColor(this.Color.Value);
				}
				if (this.BorderColor != null)
				{
					DDPrint.SetBorder(this.BorderColor.Value, 1);
				}
				DDPrint.SetPrint(this.X, this.Y, this.YStep);
				DDPrint.Print(title);
				DDPrint.PrintRet();
				DDPrint.Print(string.Format("[{0}]\u3000最小={1}\u3000最大={2}", value, minval, maxval));
				DDPrint.PrintRet();
				DDPrint.Print("★\u3000左＝下げる");
				DDPrint.PrintRet();
				DDPrint.Print("★\u3000右＝上げる");
				DDPrint.PrintRet();
				DDPrint.Print("★\u3000下＝速く下げる");
				DDPrint.PrintRet();
				DDPrint.Print("★\u3000上＝速く上げる");
				DDPrint.PrintRet();
				DDPrint.Print("★\u3000調整が終わったら決定ボタンを押して下さい。");
				DDPrint.PrintRet();
				DDEngine.EachFrame();
			}
			DDEngine.FreezeInput(1);
			return value;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000F64C File Offset: 0x0000D84C
		public double VolumeConfig(string title, double rate, int minval, int maxval, int valStep, int valFastStep, Action<double> valChanged, Action pulse)
		{
			return DDSimpleMenu.VolumeValueToRate(this.IntVolumeConfig(title, DDSimpleMenu.RateToVolumeValue(rate, minval, maxval), minval, maxval, valStep, valFastStep, delegate(int v)
			{
				valChanged(DDSimpleMenu.VolumeValueToRate(v, minval, maxval));
			}, pulse), minval, maxval);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000F6BE File Offset: 0x0000D8BE
		private static double VolumeValueToRate(int value, int minval, int maxval)
		{
			return (double)(value - minval) / (double)(maxval - minval);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000F6C9 File Offset: 0x0000D8C9
		private static int RateToVolumeValue(double rate, int minval, int maxval)
		{
			return minval + SCommon.ToInt(rate * (double)(maxval - minval));
		}

		// Token: 0x040001EE RID: 494
		public I3Color? Color;

		// Token: 0x040001EF RID: 495
		public I3Color? BorderColor;

		// Token: 0x040001F0 RID: 496
		public I3Color? WallColor;

		// Token: 0x040001F1 RID: 497
		public DDPicture WallPicture;

		// Token: 0x040001F2 RID: 498
		public Action WallDrawer;

		// Token: 0x040001F3 RID: 499
		public double WallCurtain;

		// Token: 0x040001F4 RID: 500
		public int X = 16;

		// Token: 0x040001F5 RID: 501
		public int Y = 16;

		// Token: 0x040001F6 RID: 502
		public int YStep = 32;

		// Token: 0x040001F7 RID: 503
		private bool MouseUsable;

		// Token: 0x0200013E RID: 318
		private class ButtonInfo
		{
			// Token: 0x06000675 RID: 1653 RVA: 0x00022002 File Offset: 0x00020202
			public ButtonInfo(DDInput.Button button, string name)
			{
				this.Button = button;
				this.Name = name;
			}

			// Token: 0x04000526 RID: 1318
			public DDInput.Button Button;

			// Token: 0x04000527 RID: 1319
			public string Name;
		}
	}
}
