using System;

namespace Charlotte.Commons
{
	// Token: 0x0200009E RID: 158
	public class ArgsReader
	{
		// Token: 0x060002C7 RID: 711 RVA: 0x000110E1 File Offset: 0x0000F2E1
		public ArgsReader(string[] args, int argIndex = 0)
		{
			this.Args = args;
			this.ArgIndex = argIndex;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x000110F7 File Offset: 0x0000F2F7
		public bool HasArgs(int count = 1)
		{
			return count <= this.Args.Length - this.ArgIndex;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001110E File Offset: 0x0000F30E
		public bool ArgIs(string spell)
		{
			if (this.HasArgs(1) && this.GetArg(0).ToUpper() == spell.ToUpper())
			{
				this.ArgIndex++;
				return true;
			}
			return false;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00011143 File Offset: 0x0000F343
		public string GetArg(int index = 0)
		{
			return this.Args[this.ArgIndex + index];
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00011154 File Offset: 0x0000F354
		public string NextArg()
		{
			string arg = this.GetArg(0);
			this.ArgIndex++;
			return arg;
		}

		// Token: 0x0400021C RID: 540
		private string[] Args;

		// Token: 0x0400021D RID: 541
		private int ArgIndex;
	}
}
