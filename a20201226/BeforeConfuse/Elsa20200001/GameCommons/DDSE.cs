using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public class DDSE
	{
		public const int HANDLE_COUNT = 64;

		public DDSound Sound;
		public double Volume = 0.5; // 0.0 ～ 1.0
		public int HandleIndex = 0;

		public DDSE(string file)
			: this(new DDSound(file, HANDLE_COUNT))
		{ }

		public DDSE(Func<byte[]> getFileData)
			: this(new DDSound(getFileData, HANDLE_COUNT))
		{ }

		public DDSE(DDSound sound_binding)
		{
			this.Sound = sound_binding;
			this.Sound.PostLoaded = this.UpdateVolume_NoCheck;

			DDSEUtils.Add(this);
		}

		public void Play(bool once = true)
		{
			if (once)
				DDSEUtils.Play(this);
			else
				DDSEUtils.PlayLoop(this);
		}

		public void Fade(int frameMax = 30)
		{
			throw null; // TODO
		}

		public void Stop()
		{
			DDSEUtils.Stop(this);
		}

		public void SetVolume(double volume)
		{
			this.Volume = volume;
			this.UpdateVolume();
		}

		public void UpdateVolume()
		{
			if (this.Sound.IsLoaded())
				this.UpdateVolume_NoCheck();
		}

		public void UpdateVolume_NoCheck()
		{
			double mixedVolume = DDSoundUtils.MixVolume(DDGround.SEVolume, this.Volume);

			for (int index = 0; index < HANDLE_COUNT; index++)
				DDSoundUtils.SetVolume(this.Sound.GetHandle(index), mixedVolume);
		}

		public void Touch()
		{
			this.Sound.GetHandle(0);
		}
	}
}
