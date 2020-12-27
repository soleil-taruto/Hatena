using System;
using System.Collections.Generic;

namespace Charlotte.GameCommons
{
	// Token: 0x0200007D RID: 125
	public static class DDMusicUtils
	{
		// Token: 0x060001CC RID: 460 RVA: 0x0000C857 File Offset: 0x0000AA57
		public static void Add(DDMusic music)
		{
			DDMusicUtils.Musics.Add(music);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000C864 File Offset: 0x0000AA64
		public static void EachFrame()
		{
			if (1 <= DDMusicUtils.PlayInfos.Count)
			{
				DDMusicUtils.PlayInfo info = DDMusicUtils.PlayInfos.Dequeue();
				if (info != null)
				{
					switch (info.Command)
					{
					case DDMusicUtils.PlayInfo.Command_e.PLAY:
						DDSoundUtils.Play(info.Music.Sound.GetHandle(0), info.Once, info.Resume);
						return;
					case DDMusicUtils.PlayInfo.Command_e.VOLUME_RATE:
						DDSoundUtils.SetVolume(info.Music.Sound.GetHandle(0), DDSoundUtils.MixVolume(DDGround.MusicVolume, info.Music.Volume) * info.VolumeRate);
						return;
					case DDMusicUtils.PlayInfo.Command_e.STOP:
						DDSoundUtils.Stop(info.Music.Sound.GetHandle(0));
						return;
					default:
						throw new DDError();
					}
				}
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000C924 File Offset: 0x0000AB24
		public static void Play(DDMusic music, bool once = false, bool resume = false, double volume = 1.0, int fadeFrameMax = 30)
		{
			if (DDMusicUtils.CurrDestMusic != null)
			{
				if (DDMusicUtils.CurrDestMusic == music)
				{
					return;
				}
				if (1 <= fadeFrameMax)
				{
					DDMusicUtils.Fade(fadeFrameMax, 0.0, DDMusicUtils.CurrDestVolume);
				}
				else
				{
					DDMusicUtils.Stop();
				}
			}
			DDMusicUtils.PlayInfos.Enqueue(new DDMusicUtils.PlayInfo(DDMusicUtils.PlayInfo.Command_e.PLAY, music, once, resume, 0.0));
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(new DDMusicUtils.PlayInfo(DDMusicUtils.PlayInfo.Command_e.VOLUME_RATE, music, false, false, volume));
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.CurrDestMusic = music;
			DDMusicUtils.CurrDestVolume = volume;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000C9CA File Offset: 0x0000ABCA
		public static void Fade(int frameMax = 30, double destVolume = 0.0)
		{
			DDMusicUtils.Fade(frameMax, destVolume, DDMusicUtils.CurrDestVolume);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000C9D8 File Offset: 0x0000ABD8
		public static void Fade(int frameMax, double destVolumeRate, double startVolumeRate)
		{
			if (DDMusicUtils.CurrDestMusic == null)
			{
				return;
			}
			for (int frmcnt = 0; frmcnt <= frameMax; frmcnt++)
			{
				double volumeRate;
				if (frmcnt == 0)
				{
					volumeRate = startVolumeRate;
				}
				else if (frmcnt == frameMax)
				{
					volumeRate = destVolumeRate;
				}
				else
				{
					volumeRate = startVolumeRate + (destVolumeRate - startVolumeRate) * (double)frmcnt / (double)frameMax;
				}
				DDMusicUtils.PlayInfos.Enqueue(new DDMusicUtils.PlayInfo(DDMusicUtils.PlayInfo.Command_e.VOLUME_RATE, DDMusicUtils.CurrDestMusic, false, false, volumeRate));
			}
			DDMusicUtils.CurrDestVolume = destVolumeRate;
			if (destVolumeRate == 0.0)
			{
				DDMusicUtils.Stop();
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000CA44 File Offset: 0x0000AC44
		public static void Stop()
		{
			if (DDMusicUtils.CurrDestMusic == null)
			{
				return;
			}
			DDMusicUtils.PlayInfos.Enqueue(new DDMusicUtils.PlayInfo(DDMusicUtils.PlayInfo.Command_e.VOLUME_RATE, DDMusicUtils.CurrDestMusic, false, false, 0.0));
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(new DDMusicUtils.PlayInfo(DDMusicUtils.PlayInfo.Command_e.STOP, DDMusicUtils.CurrDestMusic, false, false, 0.0));
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.PlayInfos.Enqueue(null);
			DDMusicUtils.CurrDestMusic = null;
			DDMusicUtils.CurrDestVolume = 0.0;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000CAEF File Offset: 0x0000ACEF
		public static void UpdateVolume()
		{
			DDMusicUtils.Fade(0, 1.0);
		}

		// Token: 0x040001CA RID: 458
		public static List<DDMusic> Musics = new List<DDMusic>();

		// Token: 0x040001CB RID: 459
		private static Queue<DDMusicUtils.PlayInfo> PlayInfos = new Queue<DDMusicUtils.PlayInfo>();

		// Token: 0x040001CC RID: 460
		public static DDMusic CurrDestMusic = null;

		// Token: 0x040001CD RID: 461
		public static double CurrDestVolume = 0.0;

		// Token: 0x02000128 RID: 296
		private class PlayInfo
		{
			// Token: 0x06000644 RID: 1604 RVA: 0x000216F5 File Offset: 0x0001F8F5
			public PlayInfo(DDMusicUtils.PlayInfo.Command_e command, DDMusic music, bool once, bool resume, double volumeRate)
			{
				this.Command = command;
				this.Music = music;
				this.Once = once;
				this.Resume = resume;
				this.VolumeRate = volumeRate;
			}

			// Token: 0x040004E9 RID: 1257
			public DDMusicUtils.PlayInfo.Command_e Command;

			// Token: 0x040004EA RID: 1258
			public DDMusic Music;

			// Token: 0x040004EB RID: 1259
			public bool Once;

			// Token: 0x040004EC RID: 1260
			public bool Resume;

			// Token: 0x040004ED RID: 1261
			public double VolumeRate;

			// Token: 0x02000164 RID: 356
			public enum Command_e
			{
				// Token: 0x0400057C RID: 1404
				PLAY = 1,
				// Token: 0x0400057D RID: 1405
				VOLUME_RATE,
				// Token: 0x0400057E RID: 1406
				STOP
			}
		}
	}
}
