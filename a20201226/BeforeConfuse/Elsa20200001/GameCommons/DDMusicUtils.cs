using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public static class DDMusicUtils
	{
		public static List<DDMusic> Musics = new List<DDMusic>();

		public static void Add(DDMusic music)
		{
			Musics.Add(music);
		}

		private class PlayInfo
		{
			public enum Command_e
			{
				PLAY = 1,
				VOLUME_RATE,
				STOP,
			}

			public Command_e Command;
			public DDMusic Music;
			public bool Once;
			public bool Resume;
			public double VolumeRate;

			public PlayInfo(Command_e command, DDMusic music, bool once, bool resume, double volumeRate)
			{
				this.Command = command;
				this.Music = music;
				this.Once = once;
				this.Resume = resume;
				this.VolumeRate = volumeRate;
			}
		}

		private static Queue<PlayInfo> PlayInfos = new Queue<PlayInfo>();

		public static void EachFrame()
		{
			if (1 <= PlayInfos.Count)
			{
				PlayInfo info = PlayInfos.Dequeue();

				if (info != null)
				{
					switch (info.Command)
					{
						case PlayInfo.Command_e.PLAY:
							DDSoundUtils.Play(info.Music.Sound.GetHandle(0), info.Once, info.Resume);
							break;

						case PlayInfo.Command_e.VOLUME_RATE:
							DDSoundUtils.SetVolume(info.Music.Sound.GetHandle(0), DDSoundUtils.MixVolume(DDGround.MusicVolume, info.Music.Volume) * info.VolumeRate);
							break;

						case PlayInfo.Command_e.STOP:
							DDSoundUtils.Stop(info.Music.Sound.GetHandle(0));
							break;

						default:
							throw new DDError();
					}
				}
			}
		}

		public static DDMusic CurrDestMusic = null;
		public static double CurrDestVolume = 0.0;

		public static void Play(DDMusic music, bool once = false, bool resume = false, double volume = 1.0, int fadeFrameMax = 30)
		{
			if (CurrDestMusic != null) // ? 再生中
			{
				if (CurrDestMusic == music)
					return;

				if (1 <= fadeFrameMax)
					Fade(fadeFrameMax, 0.0, CurrDestVolume);
				else
					Stop();
			}
			PlayInfos.Enqueue(new PlayInfo(PlayInfo.Command_e.PLAY, music, once, resume, 0.0));
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(new PlayInfo(PlayInfo.Command_e.VOLUME_RATE, music, false, false, volume));
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);

			CurrDestMusic = music;
			CurrDestVolume = volume;
		}

		public static void Fade(int frameMax = 30, double destVolume = 0.0)
		{
			Fade(frameMax, destVolume, CurrDestVolume);
		}

		public static void Fade(int frameMax, double destVolumeRate, double startVolumeRate)
		{
			if (CurrDestMusic == null)
				return;

			for (int frmcnt = 0; frmcnt <= frameMax; frmcnt++)
			{
				double volumeRate;

				if (frmcnt == 0)
					volumeRate = startVolumeRate;
				else if (frmcnt == frameMax)
					volumeRate = destVolumeRate;
				else
					volumeRate = startVolumeRate + ((destVolumeRate - startVolumeRate) * frmcnt) / frameMax;

				PlayInfos.Enqueue(new PlayInfo(PlayInfo.Command_e.VOLUME_RATE, CurrDestMusic, false, false, volumeRate));
			}
			CurrDestVolume = destVolumeRate;

			if (destVolumeRate == 0.0) // ? フェード目標音量ゼロ -> 曲停止
			{
				Stop();
			}
		}

		public static void Stop()
		{
			if (CurrDestMusic == null)
				return;

			PlayInfos.Enqueue(new PlayInfo(PlayInfo.Command_e.VOLUME_RATE, CurrDestMusic, false, false, 0.0));
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(new PlayInfo(PlayInfo.Command_e.STOP, CurrDestMusic, false, false, 0.0));
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);
			PlayInfos.Enqueue(null);

			CurrDestMusic = null;
			CurrDestVolume = 0.0;
		}

		public static void UpdateVolume()
		{
			Fade(0, 1.0);
		}
	}
}
