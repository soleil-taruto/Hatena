using System;
using System.Collections.Generic;
using Charlotte.Commons;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x0200008E RID: 142
	public static class DDSoundUtils
	{
		// Token: 0x06000241 RID: 577 RVA: 0x0000F860 File Offset: 0x0000DA60
		public static void Add(DDSound sound)
		{
			DDSoundUtils.Sounds.Add(sound);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000F870 File Offset: 0x0000DA70
		public static void UnloadAll()
		{
			foreach (DDSound ddsound in DDSoundUtils.Sounds)
			{
				ddsound.Unload();
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
		public static void Play(int handle, bool once = true, bool resume = false)
		{
			switch (DX.CheckSoundMem(handle))
			{
			case -1:
				throw new DDError();
			case 0:
				if (DX.PlaySoundMem(handle, once ? 1 : 3, resume ? 0 : 1) != 0)
				{
					throw new DDError();
				}
				return;
			case 1:
				return;
			default:
				throw new DDError();
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000F913 File Offset: 0x0000DB13
		public static void Stop(int handle)
		{
			if (DX.StopSoundMem(handle) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000F924 File Offset: 0x0000DB24
		public static void SetVolume(int handle, double volume)
		{
			volume = SCommon.ToRange(volume, 0.0, 1.0);
			int pal = SCommon.ToInt(volume * 255.0);
			if (pal < 0 || 255 < pal)
			{
				throw new DDError();
			}
			if (DX.ChangeVolumeSoundMem(pal, handle) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000F980 File Offset: 0x0000DB80
		public static double MixVolume(double volume1, double volume2)
		{
			volume1 = SCommon.ToRange(volume1, 0.0, 1.0);
			volume2 = SCommon.ToRange(volume2, 0.0, 1.0);
			return SCommon.ToRange(volume1 * volume2 * 2.0, 0.0, 1.0);
		}

		// Token: 0x040001FC RID: 508
		public static List<DDSound> Sounds = new List<DDSound>();
	}
}
