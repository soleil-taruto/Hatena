using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDSoundUtils
	{
		public static List<DDSound> Sounds = new List<DDSound>();

		public static void Add(DDSound sound)
		{
			Sounds.Add(sound);
		}

		public static void UnloadAll()
		{
			foreach (DDSound sound in Sounds)
				sound.Unload();
		}

		public static void Play(int handle, bool once = true, bool resume = false)
		{
			switch (DX.CheckSoundMem(handle))
			{
				case 1: // ? 再生中
					return;

				case 0: // ? 再生されていない。
					break;

				case -1: // ? エラー
					throw new DDError();

				default: // ? 不明
					throw new DDError();
			}
			if (DX.PlaySoundMem(handle, once ? DX.DX_PLAYTYPE_BACK : DX.DX_PLAYTYPE_LOOP, resume ? 0 : 1) != 0) // ? 失敗
				throw new DDError();
		}

		public static void Stop(int handle)
		{
			if (DX.StopSoundMem(handle) != 0) // ? 失敗
				throw new DDError();
		}

		public static void SetVolume(int handle, double volume)
		{
			volume = SCommon.ToRange(volume, 0.0, 1.0);

			int pal = SCommon.ToInt(volume * 255.0);

			if (pal < 0 || 255 < pal)
				throw new DDError(); // 2bs

			if (DX.ChangeVolumeSoundMem(pal, handle) != 0) // ? 失敗
				throw new DDError();
		}

		public static double MixVolume(double volume1, double volume2)
		{
			volume1 = SCommon.ToRange(volume1, 0.0, 1.0);
			volume2 = SCommon.ToRange(volume2, 0.0, 1.0);

			double mixedVolume = volume1 * volume2 * 2.0;

			mixedVolume = SCommon.ToRange(mixedVolume, 0.0, 1.0);

			return mixedVolume;
		}
	}
}
