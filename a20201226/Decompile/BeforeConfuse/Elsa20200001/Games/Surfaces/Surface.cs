using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	// Token: 0x0200002A RID: 42
	public abstract class Surface
	{
		// Token: 0x0600007A RID: 122 RVA: 0x000073E3 File Offset: 0x000055E3
		public void Draw()
		{
			if (this._draw == null)
			{
				this._draw = SCommon.Supplier<bool>(this.E_Draw());
			}
			if (!this._draw())
			{
				this.DeadFlag = true;
			}
		}

		// Token: 0x0600007B RID: 123
		public abstract IEnumerable<bool> E_Draw();

		// Token: 0x0600007C RID: 124 RVA: 0x00007414 File Offset: 0x00005614
		public void Invoke(string command, string[] arguments)
		{
			int c = 0;
			if (command == "End")
			{
				this.DeadFlag = true;
			}
			if (command == "XY")
			{
				double x = double.Parse(arguments[c++]);
				double y = double.Parse(arguments[c++]);
				this.X = x;
				this.Y = y;
				return;
			}
			this.Invoke_02(command, arguments);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00007475 File Offset: 0x00005675
		public virtual void Invoke_02(string command, string[] arguments)
		{
			throw new DDError("そんなコマンドありません：" + command);
		}

		// Token: 0x040000DF RID: 223
		public string Name;

		// Token: 0x040000E0 RID: 224
		public double X = 480.0;

		// Token: 0x040000E1 RID: 225
		public double Y = 270.0;

		// Token: 0x040000E2 RID: 226
		public bool DeadFlag;

		// Token: 0x040000E3 RID: 227
		private Func<bool> _draw;
	}
}
