using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	public static class SurfaceCreator
	{
		public static Surface Create(string typeName, string instanceName)
		{
			Surface surface;

			// typeName はクラス名 Surface_<typeName> と対応する。難読化するのでリフレクションにすることは出来ない。

			switch (typeName)
			{
				case "MessageWindow": surface = new Surface_MessageWindow(); break;
				case "キャラクタ": surface = new Surface_キャラクタ(); break;

				// 新しいサーフェスをここへ追加..

				default:
					throw new DDError("不明なタイプ名：" + typeName);
			}
			surface.Name = instanceName;
			return surface;
		}
	}
}
