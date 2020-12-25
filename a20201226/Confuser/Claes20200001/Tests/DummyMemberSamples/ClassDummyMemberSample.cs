using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Tests.DummyMemberSamples
{
	/// <summary>
	/// 要置き換え : SSS_ to (RANDOM_WORD)_
	/// </summary>
	public class ClassDummyMemberSample
	{
		public static int SSS_Count;

		public int SSS_GetCount()
		{
			return SSS_Count;
		}

		public void SSS_SetCount(int SSS_SetCount_Prm)
		{
			SSS_Count = SSS_SetCount_Prm;
		}

		public void SSS_ResetCount()
		{
			this.SSS_SetCount(0);
		}

		public int SSS_NextCount()
		{
			return SSS_Count++;
		}

		public class SSS_ValueInfo
		{
			public int SSS_ValueInfo_A;
			public int SSS_ValueInfo_B;
			public int SSS_ValueInfo_C;
		}

		public static SSS_ValueInfo SSS_Value;

		public SSS_ValueInfo SSS_GetValue()
		{
			return SSS_Value;
		}

		public void SSS_SetValue(SSS_ValueInfo SSS_SetValue_Prm)
		{
			SSS_Value = SSS_SetValue_Prm;
		}

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
			this.SSS_Overload_04(SSS_a, SSS_b, SSS_c, this.SSS_GetValue().SSS_ValueInfo_A, this.SSS_GetValue().SSS_ValueInfo_B, this.SSS_GetValue().SSS_ValueInfo_C);
		}

		public void SSS_Overload_04(int SSS_a, int SSS_b, int SSS_c, int SSS_a2, int SSS_b2, int SSS_c2)
		{
			var SSS_infos = new[]
			{
				new { SSS_Info_P1 = SSS_a, SSS_Info_P2 = SSS_a2 },
				new { SSS_Info_P1 = SSS_b, SSS_Info_P2 = SSS_a2 },
				new { SSS_Info_P1 = SSS_c, SSS_Info_P2 = SSS_a2 },
			};

			this.SSS_SetValue(new SSS_ValueInfo()
			{
				SSS_ValueInfo_A = SSS_a,
				SSS_ValueInfo_B = SSS_b,
				SSS_ValueInfo_C = SSS_c,
			});

			if (SSS_infos[0].SSS_Info_P1 == SSS_a2) this.SSS_Overload_05(SSS_infos[0].SSS_Info_P2);
			if (SSS_infos[1].SSS_Info_P1 == SSS_b2) this.SSS_Overload_05(SSS_infos[1].SSS_Info_P2);
			if (SSS_infos[2].SSS_Info_P1 == SSS_c2) this.SSS_Overload_05(SSS_infos[2].SSS_Info_P2);
		}

		public void SSS_Overload_05(int SSS_v)
		{
			if (SSS_v != this.SSS_GetCount())
				this.SSS_SetCount(SSS_v);
			else
				this.SSS_Overload_01(SSS_v);
		}
	}
}
