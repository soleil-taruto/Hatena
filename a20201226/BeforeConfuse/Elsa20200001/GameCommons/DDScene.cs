using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	// memo: ifscene は yield retrun で代用出来そうなので実装しない。--> DDCommonEffect.GetTask()

	public struct DDScene
	{
		public int Numer;
		public int Denom;

		public DDScene(int numer, int denom)
		{
			this.Numer = numer;
			this.Denom = denom;
		}

		public double Rate
		{
			get
			{
				return this.Numer / (double)this.Denom;
			}
		}

		public int Remaining
		{
			get
			{
				return this.Denom - this.Numer;
			}
		}

		public double RemainingRate
		{
			get
			{
				return this.Remaining / (double)this.Denom;
			}
		}
	}
}
