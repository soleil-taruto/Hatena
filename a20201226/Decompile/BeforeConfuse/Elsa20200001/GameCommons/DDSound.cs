using System;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008D RID: 141
	public class DDSound
	{
		// Token: 0x0600023C RID: 572 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		public DDSound(string file, int handleCount) : this(() => DDResource.Load(file), handleCount)
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000F708 File Offset: 0x0000D908
		public DDSound(Func<byte[]> getFileData, int handleCount)
		{
			this.Func_GetFileData = getFileData;
			this.HandleCount = handleCount;
			DDSoundUtils.Add(this);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000F754 File Offset: 0x0000D954
		public void Unload()
		{
			if (this.Handles != null)
			{
				int[] handles = this.Handles;
				for (int i = 0; i < handles.Length; i++)
				{
					if (DX.DeleteSoundMem(handles[i]) != 0)
					{
						throw new DDError();
					}
				}
				this.Handles = null;
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000F795 File Offset: 0x0000D995
		public bool IsLoaded()
		{
			return this.Handles != null;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000F7A0 File Offset: 0x0000D9A0
		public int GetHandle(int handleIndex)
		{
			if (this.Handles == null)
			{
				this.Handles = new int[this.HandleCount];
				byte[] fileData = this.Func_GetFileData();
				int handle = -1;
				DDSystem.PinOn<byte[]>(fileData, delegate(IntPtr p)
				{
					handle = DX.LoadSoundMemByMemImage(p, fileData.Length);
				});
				if (handle == -1)
				{
					throw new DDError();
				}
				this.Handles[0] = handle;
				for (int index = 1; index < this.HandleCount; index++)
				{
					int handle2 = DX.DuplicateSoundMem(this.Handles[0]);
					if (handle2 == -1)
					{
						throw new DDError();
					}
					this.Handles[index] = handle2;
				}
				this.PostLoaded();
			}
			return this.Handles[handleIndex];
		}

		// Token: 0x040001F8 RID: 504
		private Func<byte[]> Func_GetFileData;

		// Token: 0x040001F9 RID: 505
		private int HandleCount;

		// Token: 0x040001FA RID: 506
		private int[] Handles;

		// Token: 0x040001FB RID: 507
		public Action PostLoaded = delegate()
		{
		};
	}
}
