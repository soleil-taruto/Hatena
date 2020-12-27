using System;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008F RID: 143
	public class DDSubScreen : IDisposable
	{
		// Token: 0x06000248 RID: 584 RVA: 0x0000F9F1 File Offset: 0x0000DBF1
		public DDSubScreen(int w, int h, bool aFlag = false)
		{
			this.W = w;
			this.H = h;
			this.AFlag = aFlag;
			this.Handle = -1;
			DDSubScreenUtils.Add(this);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000FA22 File Offset: 0x0000DC22
		public void Dispose()
		{
			if (this.W == -1)
			{
				return;
			}
			DDSubScreenUtils.Remove(this);
			this.Unload();
			this.W = -1;
			this.H = -1;
			this.AFlag = false;
			this.Picture = null;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000FA56 File Offset: 0x0000DC56
		public void Unload()
		{
			if (this.Handle != -1)
			{
				if (DX.DeleteGraph(this.Handle) != 0)
				{
					throw new DDError();
				}
				this.Handle = -1;
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000FA7C File Offset: 0x0000DC7C
		public int GetHandle()
		{
			if (this.Handle == -1)
			{
				this.Handle = DX.MakeScreen(this.W, this.H, this.AFlag ? 1 : 0);
				if (this.Handle == -1)
				{
					throw new DDError();
				}
			}
			return this.Handle;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000FACA File Offset: 0x0000DCCA
		public void ChangeDrawScreen()
		{
			DDSubScreenUtils.ChangeDrawScreen(this);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000FAD2 File Offset: 0x0000DCD2
		public I2Size GetSize()
		{
			return new I2Size(this.W, this.H);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000FAE5 File Offset: 0x0000DCE5
		public DDPicture ToPicture()
		{
			if (this.Picture == null)
			{
				this.Picture = DDPictureLoaders2.Wrapper(this);
			}
			return this.Picture;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000FB01 File Offset: 0x0000DD01
		public IDisposable Section()
		{
			DDSubScreen parentSubScreen = DDSubScreenUtils.CurrDrawScreen;
			this.ChangeDrawScreen();
			return SCommon.GetAnonyDisposable(delegate
			{
				DDSubScreenUtils.ChangeDrawScreen(parentSubScreen);
			});
		}

		// Token: 0x040001FD RID: 509
		private int W;

		// Token: 0x040001FE RID: 510
		private int H;

		// Token: 0x040001FF RID: 511
		private bool AFlag;

		// Token: 0x04000200 RID: 512
		private int Handle = -1;

		// Token: 0x04000201 RID: 513
		private DDPicture Picture;
	}
}
