using System;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x0200009B RID: 155
	public class DDSceneKeeper
	{
		// Token: 0x060002B5 RID: 693 RVA: 0x00010DE0 File Offset: 0x0000EFE0
		public DDSceneKeeper(int frameMax)
		{
			if (frameMax < 1 || 1000000000 < frameMax)
			{
				throw new DDError();
			}
			this.FrameMax = frameMax;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00010E08 File Offset: 0x0000F008
		public void Fire()
		{
			this.StartedProcFrame = DDEngine.ProcFrame;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00010E15 File Offset: 0x0000F015
		public void FireDelay(int delay = 1)
		{
			if (delay < 0 || 1000000000 < delay)
			{
				throw new DDError();
			}
			this.StartedProcFrame = DDEngine.ProcFrame + delay;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00010E36 File Offset: 0x0000F036
		public void Clear()
		{
			this.StartedProcFrame = -1;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00010E3F File Offset: 0x0000F03F
		public bool IsJustFired()
		{
			return this.StartedProcFrame == DDEngine.ProcFrame;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00010E4E File Offset: 0x0000F04E
		public bool IsFlaming()
		{
			return this.StartedProcFrame != -1 && this.StartedProcFrame <= DDEngine.ProcFrame && DDEngine.ProcFrame <= this.StartedProcFrame + this.FrameMax;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060002BB RID: 699 RVA: 0x00010E7F File Offset: 0x0000F07F
		public int Count
		{
			get
			{
				if (!this.IsFlaming())
				{
					throw new DDError();
				}
				return DDEngine.ProcFrame - this.StartedProcFrame;
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00010E9B File Offset: 0x0000F09B
		public DDScene GetScene()
		{
			if (!this.IsFlaming())
			{
				return new DDScene(-1, 0);
			}
			return new DDScene(DDEngine.ProcFrame - this.StartedProcFrame, this.FrameMax);
		}

		// Token: 0x04000219 RID: 537
		private int FrameMax;

		// Token: 0x0400021A RID: 538
		private int StartedProcFrame = -1;
	}
}
