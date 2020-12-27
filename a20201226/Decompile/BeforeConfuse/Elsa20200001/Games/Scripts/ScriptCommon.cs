using System;
using System.Collections.Generic;

namespace Charlotte.Games.Scripts
{
	// Token: 0x0200003C RID: 60
	public static class ScriptCommon
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00007E08 File Offset: 0x00006008
		public static IEnumerable<bool> 掛け合い(Scenario scenario)
		{
			Game.I.掛け合い中 = true;
			foreach (ScenarioCommand command in scenario.Commands)
			{
				foreach (bool v in command.Perform())
				{
					yield return v;
				}
				IEnumerator<bool> enumerator2 = null;
			}
			List<ScenarioCommand>.Enumerator enumerator = default(List<ScenarioCommand>.Enumerator);
			Game.I.掛け合い中 = false;
			yield break;
			yield break;
		}
	}
}
