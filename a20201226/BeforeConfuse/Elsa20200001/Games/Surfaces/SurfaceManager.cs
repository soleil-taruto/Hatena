using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	public class SurfaceManager
	{
		private DDList<Surface> Surfaces = new DDList<Surface>();

		public void Draw()
		{
			foreach (Surface surface in this.Surfaces.Iterate())
				if (!surface.DeadFlag)
					surface.Draw();

			this.Surfaces.RemoveAll(v => v.DeadFlag);
		}

		public void Add(Surface surface)
		{
			this.Surfaces.Add(surface);
		}

		public void Remove(int index)
		{
			this.Surfaces.RemoveAt(index);
		}

		public int GetIndex(string name, int defval)
		{
			for (int index = 0; index < this.Surfaces.Count; index++)
				if (this.Surfaces[index].Name == name)
					return index;

			return defval; // not found
		}

		public int GetIndex(string name)
		{
			int index = this.GetIndex(name, -1);

			if (index == -1)
				throw new DDError("そんなサーフェスありません。" + name);

			return index;
		}

		public Surface GetSurface(string name)
		{
			return this.Surfaces[this.GetIndex(name)];
		}

		public IEnumerable<Surface> Iterate()
		{
			return this.Surfaces.Iterate();
		}

		// 特別なサーフェス >

		public Surface_MessageWindow GetMessageWindow()
		{
			return (Surface_MessageWindow)this.Iterate().First(v => v is Surface_MessageWindow);
		}

		public IEnumerable<Surface_キャラクタ> GetAllキャラクタ()
		{
			return this.Iterate().Where(v => v is Surface_キャラクタ).Select(v => (Surface_キャラクタ)v);
		}

		// < 特別なサーフェス
	}
}
