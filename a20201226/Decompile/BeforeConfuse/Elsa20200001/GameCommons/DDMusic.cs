using System;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007C RID: 124
	public class DDMusic
	{
		// Token: 0x060001C5 RID: 453 RVA: 0x0000C79B File Offset: 0x0000A99B
		public DDMusic(string file) : this(new DDSound(file, 1))
		{
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000C7AA File Offset: 0x0000A9AA
		public DDMusic(Func<byte[]> getFileData) : this(new DDSound(getFileData, 1))
		{
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000C7B9 File Offset: 0x0000A9B9
		public DDMusic(DDSound sound_binding)
		{
			this.Sound = sound_binding;
			this.Sound.PostLoaded = delegate()
			{
				DDSoundUtils.SetVolume(this.Sound.GetHandle(0), 0.0);
			};
			DDMusicUtils.Add(this);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000C7F4 File Offset: 0x0000A9F4
		public DDMusic SetLoop(int loopStart, int loopLength)
		{
			DX.SetLoopSamplePosSoundMem(loopStart, this.Sound.GetHandle(0));
			DX.SetLoopStartSamplePosSoundMem(loopStart + loopLength, this.Sound.GetHandle(0));
			return this;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000C81F File Offset: 0x0000AA1F
		public void Play(bool once = false, bool resume = false, double volume = 1.0, int fadeFrameMax = 30)
		{
			DDMusicUtils.Play(this, once, resume, volume, fadeFrameMax);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000C82C File Offset: 0x0000AA2C
		public void Touch()
		{
			this.Sound.GetHandle(0);
		}

		// Token: 0x040001C8 RID: 456
		public DDSound Sound;

		// Token: 0x040001C9 RID: 457
		public double Volume = 0.5;
	}
}
