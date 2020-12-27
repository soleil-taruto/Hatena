using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	// Token: 0x02000019 RID: 25
	public class Scenario
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00005F70 File Offset: 0x00004170
		public Scenario(string file)
		{
			foreach (string line in SCommon.TextToLines(SCommon.ToJString(DDResource.Load(file), true, true, true, true)))
			{
				if (!(line == "") && line[0] != ';')
				{
					string[] tokens = SCommon.Tokenize(line, " ", false, true, 0);
					this.Commands.Add(new ScenarioCommand
					{
						Tokens = tokens
					});
				}
			}
		}

		// Token: 0x040000C0 RID: 192
		public List<ScenarioCommand> Commands = new List<ScenarioCommand>();
	}
}
