using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007F RID: 127
	public class DDPicture
	{
		// Token: 0x060001DA RID: 474 RVA: 0x0000CC70 File Offset: 0x0000AE70
		public DDPicture(Func<DDPicture.PictureInfo> loader, Action<DDPicture.PictureInfo> unloader, Action<DDPicture> adder)
		{
			this.Loader = loader;
			this.Unloader = unloader;
			adder(this);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000CC8D File Offset: 0x0000AE8D
		public void Unload()
		{
			if (this.Info != null)
			{
				this.Unloader(this.Info);
				this.Info = null;
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000CCAF File Offset: 0x0000AEAF
		protected virtual DDPicture.PictureInfo GetInfo()
		{
			if (this.Info == null)
			{
				this.Info = this.Loader();
			}
			return this.Info;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000CCD0 File Offset: 0x0000AED0
		public int GetHandle()
		{
			return this.GetInfo().Handle;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000CCDD File Offset: 0x0000AEDD
		public int Get_W()
		{
			return this.GetInfo().W;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000CCEA File Offset: 0x0000AEEA
		public int Get_H()
		{
			return this.GetInfo().H;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000CCF7 File Offset: 0x0000AEF7
		public I2Size GetSize()
		{
			return new I2Size(this.Get_W(), this.Get_H());
		}

		// Token: 0x040001D3 RID: 467
		private DDPicture.PictureInfo Info;

		// Token: 0x040001D4 RID: 468
		private Func<DDPicture.PictureInfo> Loader;

		// Token: 0x040001D5 RID: 469
		private Action<DDPicture.PictureInfo> Unloader;

		// Token: 0x02000129 RID: 297
		public class PictureInfo
		{
			// Token: 0x040004EE RID: 1262
			public int Handle;

			// Token: 0x040004EF RID: 1263
			public int W;

			// Token: 0x040004F0 RID: 1264
			public int H;
		}
	}
}
