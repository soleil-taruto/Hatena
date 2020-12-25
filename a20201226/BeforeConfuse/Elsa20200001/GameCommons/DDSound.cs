using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	public class DDSound
	{
		private Func<byte[]> Func_GetFileData;
		private int HandleCount;
		private int[] Handles = null; // null == Unloaded

		public Action PostLoaded = () => { };

		public DDSound(string file, int handleCount)
			: this(() => DDResource.Load(file), handleCount)
		{ }

		public DDSound(Func<byte[]> getFileData, int handleCount)
		{
			this.Func_GetFileData = getFileData;
			this.HandleCount = handleCount;

			DDSoundUtils.Add(this);
		}

		public void Unload()
		{
			if (this.Handles != null)
			{
				foreach (int handle in this.Handles)
					if (DX.DeleteSoundMem(handle) != 0) // ? 失敗
						throw new DDError();

				this.Handles = null;
			}
		}

		public bool IsLoaded()
		{
			return this.Handles != null;
		}

		public int GetHandle(int handleIndex)
		{
			if (this.Handles == null)
			{
				this.Handles = new int[this.HandleCount];

				{
					byte[] fileData = this.Func_GetFileData();
					int handle = -1;

					DDSystem.PinOn(fileData, p => handle = DX.LoadSoundMemByMemImage(p, fileData.Length));

					if (handle == -1) // ? 失敗
						throw new DDError();

					this.Handles[0] = handle;
				}

				for (int index = 1; index < this.HandleCount; index++)
				{
					int handle = DX.DuplicateSoundMem(this.Handles[0]);

					if (handle == -1) // ? 失敗
						throw new DDError();

					this.Handles[index] = handle;
				}

				this.PostLoaded();
			}
			return this.Handles[handleIndex];
		}
	}
}
