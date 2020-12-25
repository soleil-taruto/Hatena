using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Tests.DummyMemberSamples
{
	/// <summary>
	/// 要置き換え : SSS_ to (RANDOM_WORD)_
	/// </summary>
	public struct StructDummyMemberSample
	{
		public void SSS_Overload_00()
		{
			this.SSS_Overload_01(this.SSS_NextCount());
		}

		public void SSS_Overload_01(int SSS_a)
		{
			this.SSS_Overload_02(SSS_a, this.SSS_NextCount());
		}

		public void SSS_Overload_02(int SSS_a, int SSS_b)
		{
			this.SSS_Overload_03(SSS_a, SSS_b, this.SSS_NextCount());
		}

		public void SSS_Overload_03(int SSS_a, int SSS_b, int SSS_c)
		{
			this.SSS_Overload_04(SSS_a, SSS_b, SSS_c, this.SSS_NextCount());
		}

		public void SSS_Overload_04(int SSS_a, int SSS_b, int SSS_c, int SSS_d)
		{
			var SSS_infos = new[]
			{
				new { SSS_Info_P1 = SSS_a, SSS_Info_P2 = SSS_b },
				new { SSS_Info_P1 = SSS_c, SSS_Info_P2 = SSS_d },
			};

			this.SSS_AddToCount(SSS_a);
			this.SSS_AddToCount(SSS_b);
			this.SSS_AddToCount(SSS_c);
			this.SSS_AddToCount(SSS_d);

			if (SSS_infos[0].SSS_Info_P1 == this.SSS_NextCount()) this.SSS_AddToCount_02(SSS_infos[0].SSS_Info_P2);
			if (SSS_infos[1].SSS_Info_P1 == this.SSS_NextCount()) this.SSS_AddToCount_02(SSS_infos[1].SSS_Info_P2);
		}

		public static int SSS_Count;

		public int SSS_NextCount()
		{
			return SSS_Count++;
		}

		public void SSS_AddToCount(int SSS_valueForAdd)
		{
			SSS_Count += SSS_valueForAdd;
		}

		public void SSS_AddToCount_02(int SSS_valueForAdd_02)
		{
			SSS_Count -= SSS_valueForAdd_02;
			SSS_Overload_00();
		}
	}
}
