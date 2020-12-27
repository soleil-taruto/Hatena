using System;
using System.Threading;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200006E RID: 110
	public static class DDEngine
	{
		// Token: 0x0600017C RID: 380 RVA: 0x0000B214 File Offset: 0x00009414
		private static void CheckHz()
		{
			long currTime = DDUtils.GetCurrTime();
			DDEngine.HzChaserTime += 16L;
			DDEngine.HzChaserTime = SCommon.ToRange(DDEngine.HzChaserTime, currTime - 100L, currTime + 100L);
			while (currTime < DDEngine.HzChaserTime)
			{
				Thread.Sleep(1);
				currTime = DDUtils.GetCurrTime();
			}
			DDEngine.FrameStartTime = currTime;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000B26C File Offset: 0x0000946C
		public static void EachFrame()
		{
			DDGround.EL.ExecuteAllTask();
			DDMouse.PosChanged_Delay();
			DDCurtain.EachFrame();
			if (!DDSEUtils.EachFrame())
			{
				DDMusicUtils.EachFrame();
			}
			DDSubScreenUtils.ChangeDrawScreen(-2);
			if (DDGround.RealScreenDraw_W == -1)
			{
				if (DX.DrawExtendGraph(0, 0, DDGround.RealScreen_W, DDGround.RealScreen_H, DDGround.MainScreen.GetHandle(), 0) != 0)
				{
					throw new DDError();
				}
			}
			else
			{
				if (DX.DrawBox(0, 0, DDGround.RealScreen_W, DDGround.RealScreen_H, DX.GetColor(0, 0, 0), 1) != 0)
				{
					throw new DDError();
				}
				if (DX.DrawExtendGraph(DDGround.RealScreenDraw_L, DDGround.RealScreenDraw_T, DDGround.RealScreenDraw_L + DDGround.RealScreenDraw_W, DDGround.RealScreenDraw_T + DDGround.RealScreenDraw_H, DDGround.MainScreen.GetHandle(), 0) != 0)
				{
					throw new DDError();
				}
			}
			GC.Collect(0);
			DDEngine.FrameProcessingMillis = (int)(DDUtils.GetCurrTime() - DDEngine.FrameStartTime);
			if (DDEngine.FrameProcessingMillis_Worst < DDEngine.FrameProcessingMillis || !DDUtils.CountDown(ref DDEngine.FrameProcessingMillis_WorstFrame))
			{
				DDEngine.FrameProcessingMillis_Worst = DDEngine.FrameProcessingMillis;
				DDEngine.FrameProcessingMillis_WorstFrame = 120;
			}
			DX.ScreenFlip();
			if (DX.CheckHitKey(1) == 1 || DX.ProcessMessage() == -1)
			{
				throw new DDCoffeeBreak();
			}
			DDEngine.CheckHz();
			DDEngine.ProcFrame++;
			DDUtils.CountDown(ref DDEngine.FreezeInputFrame);
			DDEngine.WindowIsActive = DDUtils.IsWindowActive();
			if (1000000000 < DDEngine.ProcFrame)
			{
				DDEngine.ProcFrame = 1000000000;
				throw new DDError();
			}
			DDPad.EachFrame();
			DDKey.EachFrame();
			DDInput.EachFrame();
			DDMouse.EachFrame();
			DDSubScreen mainScreen = DDGround.MainScreen;
			DDGround.MainScreen = DDGround.LastMainScreen;
			DDGround.LastMainScreen = mainScreen;
			DDGround.MainScreen.ChangeDrawScreen();
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000B3F7 File Offset: 0x000095F7
		public static void FreezeInput(int frame = 1)
		{
			if (frame < 1 || 1000000000 < frame)
			{
				throw new DDError("frame: " + frame.ToString());
			}
			DDEngine.FreezeInputFrame = Math.Max(DDEngine.FreezeInputFrame, frame);
		}

		// Token: 0x04000180 RID: 384
		public static long FrameStartTime;

		// Token: 0x04000181 RID: 385
		public static long HzChaserTime;

		// Token: 0x04000182 RID: 386
		public static int FrameProcessingMillis;

		// Token: 0x04000183 RID: 387
		public static int FrameProcessingMillis_Worst;

		// Token: 0x04000184 RID: 388
		public static int FrameProcessingMillis_WorstFrame;

		// Token: 0x04000185 RID: 389
		public static int ProcFrame;

		// Token: 0x04000186 RID: 390
		public static int FreezeInputFrame;

		// Token: 0x04000187 RID: 391
		public static bool WindowIsActive;
	}
}
