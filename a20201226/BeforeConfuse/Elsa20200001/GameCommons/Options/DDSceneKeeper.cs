using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons.Options
{
	public class DDSceneKeeper
	{
		private int FrameMax;
		private int StartedProcFrame = -1;

		public DDSceneKeeper(int frameMax)
		{
			if (frameMax < 1 || SCommon.IMAX < frameMax)
				throw new DDError();

			this.FrameMax = frameMax;
		}

		public void Fire()
		{
			this.StartedProcFrame = DDEngine.ProcFrame;
		}

		public void FireDelay(int delay = 1)
		{
			if (delay < 0 || SCommon.IMAX < delay)
				throw new DDError();

			this.StartedProcFrame = DDEngine.ProcFrame + delay;
		}

		public void Clear()
		{
			this.StartedProcFrame = -1;
		}

		public bool IsJustFired()
		{
			return this.StartedProcFrame == DDEngine.ProcFrame;
		}

		public bool IsFlaming()
		{
			return
				this.StartedProcFrame != -1 &&
				this.StartedProcFrame <= DDEngine.ProcFrame &&
				DDEngine.ProcFrame <= this.StartedProcFrame + this.FrameMax;
		}

		public int Count
		{
			get
			{
				if (!this.IsFlaming())
					throw new DDError();

				return DDEngine.ProcFrame - this.StartedProcFrame;
			}
		}

		public DDScene GetScene()
		{
			if (!this.IsFlaming())
				return new DDScene(-1, 0);

			return new DDScene(DDEngine.ProcFrame - this.StartedProcFrame, this.FrameMax);
		}
	}
}
