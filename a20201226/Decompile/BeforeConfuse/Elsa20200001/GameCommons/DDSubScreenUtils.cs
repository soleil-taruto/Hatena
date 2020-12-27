using System;
using System.Collections.Generic;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000090 RID: 144
	public static class DDSubScreenUtils
	{
		// Token: 0x06000250 RID: 592 RVA: 0x0000FB29 File Offset: 0x0000DD29
		public static void Add(DDSubScreen subScreen)
		{
			DDSubScreenUtils.SubScreens.Add(subScreen);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000FB38 File Offset: 0x0000DD38
		public static void Remove(DDSubScreen subScreen)
		{
			if (DDUtils.FastDesertElement<DDSubScreen>(DDSubScreenUtils.SubScreens, (DDSubScreen i) => i == subScreen, null) == null)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000FB74 File Offset: 0x0000DD74
		public static void UnloadAll()
		{
			foreach (DDSubScreen ddsubScreen in DDSubScreenUtils.SubScreens)
			{
				ddsubScreen.Unload();
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
		public static void ChangeDrawScreen(int handle)
		{
			if (DX.SetDrawScreen(handle) != 0)
			{
				throw new DDError();
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000FBD4 File Offset: 0x0000DDD4
		public static void ChangeDrawScreen(DDSubScreen subScreen)
		{
			DDSubScreenUtils.ChangeDrawScreen(subScreen.GetHandle());
			DDSubScreenUtils.CurrDrawScreen = subScreen;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000FBE7 File Offset: 0x0000DDE7
		public static void RestoreDrawScreen()
		{
			DDSubScreenUtils.ChangeDrawScreen(DDGround.MainScreen);
		}

		// Token: 0x04000202 RID: 514
		public static List<DDSubScreen> SubScreens = new List<DDSubScreen>();

		// Token: 0x04000203 RID: 515
		public static DDSubScreen CurrDrawScreen;
	}
}
