using System;

namespace Charlotte.GameCommons
{
	// Token: 0x02000088 RID: 136
	public struct DDScene
	{
		// Token: 0x0600021A RID: 538 RVA: 0x0000E69B File Offset: 0x0000C89B
		public DDScene(int numer, int denom)
		{
			this.Numer = numer;
			this.Denom = denom;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000E6AB File Offset: 0x0000C8AB
		public double Rate
		{
			get
			{
				return (double)this.Numer / (double)this.Denom;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000E6BC File Offset: 0x0000C8BC
		public int Remaining
		{
			get
			{
				return this.Denom - this.Numer;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0000E6CB File Offset: 0x0000C8CB
		public double RemainingRate
		{
			get
			{
				return (double)this.Remaining / (double)this.Denom;
			}
		}

		// Token: 0x040001E6 RID: 486
		public int Numer;

		// Token: 0x040001E7 RID: 487
		public int Denom;
	}
}
