using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	// Token: 0x0200002C RID: 44
	public class SurfaceManager
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00007504 File Offset: 0x00005704
		public void Draw()
		{
			foreach (Surface surface in this.Surfaces.Iterate())
			{
				if (!surface.DeadFlag)
				{
					surface.Draw();
				}
			}
			this.Surfaces.RemoveAll((Surface v) => v.DeadFlag);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00007588 File Offset: 0x00005788
		public void Add(Surface surface)
		{
			this.Surfaces.Add(surface);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00007596 File Offset: 0x00005796
		public void Remove(int index)
		{
			this.Surfaces.RemoveAt(index);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000075A4 File Offset: 0x000057A4
		public int GetIndex(string name, int defval)
		{
			for (int index = 0; index < this.Surfaces.Count; index++)
			{
				if (this.Surfaces[index].Name == name)
				{
					return index;
				}
			}
			return defval;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000075E3 File Offset: 0x000057E3
		public int GetIndex(string name)
		{
			int index = this.GetIndex(name, -1);
			if (index == -1)
			{
				throw new DDError("そんなサーフェスありません。" + name);
			}
			return index;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00007602 File Offset: 0x00005802
		public Surface GetSurface(string name)
		{
			return this.Surfaces[this.GetIndex(name)];
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00007616 File Offset: 0x00005816
		public IEnumerable<Surface> Iterate()
		{
			return this.Surfaces.Iterate();
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00007623 File Offset: 0x00005823
		public Surface_MessageWindow GetMessageWindow()
		{
			return (Surface_MessageWindow)this.Iterate().First((Surface v) => v is Surface_MessageWindow);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00007654 File Offset: 0x00005854
		public IEnumerable<Surface_Chara> GetAllキャラクタ()
		{
			return from v in this.Iterate()
			where v is Surface_Chara
			select (Surface_Chara)v;
		}

		// Token: 0x040000E4 RID: 228
		private DDList<Surface> Surfaces = new DDList<Surface>();
	}
}
