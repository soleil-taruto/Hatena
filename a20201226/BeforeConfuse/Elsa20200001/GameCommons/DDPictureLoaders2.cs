using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// <para>ここで取得した DDPicture は Unload する必要なし</para>
	/// <para>必要あり -> DDPictureLoader</para>
	/// </summary>
	public static class DDPictureLoaders2
	{
		private class PictureWrapper : DDPicture
		{
			public Func<int> Func_GetHandle;
			public DDPicture.PictureInfo Info;

			// <---- prm

			public PictureWrapper()
				: base(() => null, v => { }, v => { })
			{ }

			protected override DDPicture.PictureInfo GetInfo()
			{
				this.Info.Handle = this.Func_GetHandle();
				return this.Info;
			}
		}

		public static DDPicture Wrapper(Func<int> getHandle, int w, int h)
		{
			DDPicture.PictureInfo info = new DDPicture.PictureInfo()
			{
				Handle = -1,
				W = w,
				H = h,
			};

			return new PictureWrapper()
			{
				Func_GetHandle = getHandle,
				Info = info,
			};
		}

		public static DDPicture Wrapper(Func<int> getHandle, I2Size size)
		{
			return Wrapper(getHandle, size.W, size.H);
		}

		public static DDPicture Wrapper(DDSubScreen subScreen)
		{
			return Wrapper(() => subScreen.GetHandle(), subScreen.GetSize());
		}
	}
}
