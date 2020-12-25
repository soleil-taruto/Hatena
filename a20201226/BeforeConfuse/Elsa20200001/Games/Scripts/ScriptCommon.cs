using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Games.Scripts
{
	public static class ScriptCommon
	{
		public static IEnumerable<bool> 掛け合い(Scenario scenario)
		{
			Game.I.掛け合い中 = true;

			foreach (ScenarioCommand command in scenario.Commands)
				foreach (bool v in command.Perform())
					yield return v;

			Game.I.掛け合い中 = false;
		}
	}
}
