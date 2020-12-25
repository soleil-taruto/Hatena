using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	// memo: ifscene は yield retrun で代用出来そうなので実装しない。--> DDCommonEffect.GetTask()

	public static class DDSceneUtils
	{
		public static IEnumerable<DDScene> Create(int frameMax)
		{
			for (int frame = 0; frame <= frameMax; frame++)
			{
				yield return new DDScene(frame, frameMax);
			}
		}
	}
}
