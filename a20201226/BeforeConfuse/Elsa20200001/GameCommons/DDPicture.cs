using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public class DDPicture
	{
		public class PictureInfo
		{
			public int Handle;
			public int W;
			public int H;
		}

		private PictureInfo Info = null; // null == Unloaded
		private Func<PictureInfo> Loader;
		private Action<PictureInfo> Unloader;

		public DDPicture(Func<PictureInfo> loader, Action<PictureInfo> unloader, Action<DDPicture> adder)
		{
			this.Loader = loader;
			this.Unloader = unloader;

			adder(this);
		}

		public void Unload()
		{
			// この画像を参照している derivation を先に Unload しなければならない。

			if (this.Info != null)
			{
				this.Unloader(this.Info);
				this.Info = null;
			}
		}

		protected virtual PictureInfo GetInfo()
		{
			if (this.Info == null)
				this.Info = this.Loader();

			return this.Info;
		}

		public int GetHandle()
		{
			return this.GetInfo().Handle;
		}

		public int Get_W()
		{
			return this.GetInfo().W;
		}

		public int Get_H()
		{
			return this.GetInfo().H;
		}

		public I2Size GetSize()
		{
			return new I2Size(this.Get_W(), this.Get_H());
		}
	}
}
