using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public class DDSubScreen : IDisposable
	{
		private int W; // -1 == Disposed
		private int H;
		private bool AFlag;
		private int Handle = -1; // -1 == Unloaded

		public DDSubScreen(int w, int h, bool aFlag = false)
		{
			this.W = w;
			this.H = h;
			this.AFlag = aFlag;
			this.Handle = -1;

			DDSubScreenUtils.Add(this);
		}

		public void Dispose()
		{
			if (this.W == -1) // ? Already disposed
				return;

			DDSubScreenUtils.Remove(this);

			this.Unload();

			this.W = -1;
			this.H = -1;
			this.AFlag = false;

			this.Picture = null;
		}

		public void Unload()
		{
			if (this.Handle != -1)
			{
				if (DX.DeleteGraph(this.Handle) != 0) // ? 失敗
					throw new DDError();

				this.Handle = -1;
			}
		}

		public int GetHandle()
		{
			if (this.Handle == -1)
			{
				this.Handle = DX.MakeScreen(this.W, this.H, this.AFlag ? 1 : 0);

				if (this.Handle == -1) // ? 失敗
					throw new DDError();
			}
			return this.Handle;
		}

		public void ChangeDrawScreen()
		{
			DDSubScreenUtils.ChangeDrawScreen(this);
		}

		public I2Size GetSize()
		{
			return new I2Size(this.W, this.H);
		}

		private DDPicture Picture = null;

		public DDPicture ToPicture()
		{
			if (this.Picture == null)
				this.Picture = DDPictureLoaders2.Wrapper(this);

			return this.Picture;
		}

		public IDisposable Section()
		{
#if true // ネスト対応
			DDSubScreen parentSubScreen = DDSubScreenUtils.CurrDrawScreen;
			this.ChangeDrawScreen();
			return SCommon.GetAnonyDisposable(() => DDSubScreenUtils.ChangeDrawScreen(parentSubScreen));
#else // old -- ネスト未対応
			this.ChangeDrawScreen();
			return SCommon.GetAnonyDisposable(() => DDSubScreenUtils.RestoreDrawScreen());
#endif
		}
	}
}
