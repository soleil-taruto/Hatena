using System;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000081 RID: 129
	public static class DDPictureLoaders2
	{
		// Token: 0x060001E9 RID: 489 RVA: 0x0000CF38 File Offset: 0x0000B138
		public static DDPicture Wrapper(Func<int> getHandle, int w, int h)
		{
			DDPicture.PictureInfo info = new DDPicture.PictureInfo
			{
				Handle = -1,
				W = w,
				H = h
			};
			return new DDPictureLoaders2.PictureWrapper
			{
				Func_GetHandle = getHandle,
				Info = info
			};
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000CF73 File Offset: 0x0000B173
		public static DDPicture Wrapper(Func<int> getHandle, I2Size size)
		{
			return DDPictureLoaders2.Wrapper(getHandle, size.W, size.H);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CF88 File Offset: 0x0000B188
		public static DDPicture Wrapper(DDSubScreen subScreen)
		{
			return DDPictureLoaders2.Wrapper(() => subScreen.GetHandle(), subScreen.GetSize());
		}

		// Token: 0x02000132 RID: 306
		private class PictureWrapper : DDPicture
		{
			// Token: 0x06000656 RID: 1622 RVA: 0x00021CF8 File Offset: 0x0001FEF8
			public PictureWrapper() : base(() => null, delegate(DDPicture.PictureInfo v)
			{
			}, delegate(DDPicture v)
			{
			})
			{
			}

			// Token: 0x06000657 RID: 1623 RVA: 0x00021D68 File Offset: 0x0001FF68
			protected override DDPicture.PictureInfo GetInfo()
			{
				this.Info.Handle = this.Func_GetHandle();
				return this.Info;
			}

			// Token: 0x04000500 RID: 1280
			public Func<int> Func_GetHandle;

			// Token: 0x04000501 RID: 1281
			public DDPicture.PictureInfo Info;
		}
	}
}
