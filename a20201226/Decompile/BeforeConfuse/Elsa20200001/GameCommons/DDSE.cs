using System;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008A RID: 138
	public class DDSE
	{
		// Token: 0x0600021F RID: 543 RVA: 0x0000E6EC File Offset: 0x0000C8EC
		public DDSE(string file) : this(new DDSound(file, 64))
		{
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000E6FC File Offset: 0x0000C8FC
		public DDSE(Func<byte[]> getFileData) : this(new DDSound(getFileData, 64))
		{
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000E70C File Offset: 0x0000C90C
		public DDSE(DDSound sound_binding)
		{
			this.Sound = sound_binding;
			this.Sound.PostLoaded = new Action(this.UpdateVolume_NoCheck);
			DDSEUtils.Add(this);
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000E747 File Offset: 0x0000C947
		public void Play(bool once = true)
		{
			if (once)
			{
				DDSEUtils.Play(this);
				return;
			}
			DDSEUtils.PlayLoop(this);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00009765 File Offset: 0x00007965
		public void Fade(int frameMax = 30)
		{
			throw null;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000E759 File Offset: 0x0000C959
		public void Stop()
		{
			DDSEUtils.Stop(this);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000E761 File Offset: 0x0000C961
		public void SetVolume(double volume)
		{
			this.Volume = volume;
			this.UpdateVolume();
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000E770 File Offset: 0x0000C970
		public void UpdateVolume()
		{
			if (this.Sound.IsLoaded())
			{
				this.UpdateVolume_NoCheck();
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000E788 File Offset: 0x0000C988
		public void UpdateVolume_NoCheck()
		{
			double mixedVolume = DDSoundUtils.MixVolume(DDGround.SEVolume, this.Volume);
			for (int index = 0; index < 64; index++)
			{
				DDSoundUtils.SetVolume(this.Sound.GetHandle(index), mixedVolume);
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000E7C5 File Offset: 0x0000C9C5
		public void Touch()
		{
			this.Sound.GetHandle(0);
		}

		// Token: 0x040001E8 RID: 488
		public const int HANDLE_COUNT = 64;

		// Token: 0x040001E9 RID: 489
		public DDSound Sound;

		// Token: 0x040001EA RID: 490
		public double Volume = 0.5;

		// Token: 0x040001EB RID: 491
		public int HandleIndex;
	}
}
