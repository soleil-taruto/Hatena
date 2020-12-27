using System;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	// Token: 0x0200002B RID: 43
	public static class SurfaceCreator
	{
		// Token: 0x0600007F RID: 127 RVA: 0x000074B0 File Offset: 0x000056B0
		public static Surface Create(string typeName, string instanceName)
		{
			Surface surface;
			if (!(typeName == "MessageWindow"))
			{
				if (!(typeName == "キャラクタ"))
				{
					throw new DDError("不明なタイプ名：" + typeName);
				}
				surface = new Surface_Chara();
			}
			else
			{
				surface = new Surface_MessageWindow();
			}
			surface.Name = instanceName;
			return surface;
		}
	}
}
