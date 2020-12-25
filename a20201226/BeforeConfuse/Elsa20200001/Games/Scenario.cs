using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	public class Scenario
	{
		public List<ScenarioCommand> Commands = new List<ScenarioCommand>();

		public Scenario(string file)
		{
			byte[] fileData = DDResource.Load(file);
			string text = SCommon.ToJString(fileData, true, true, true, true);
			string[] lines = SCommon.TextToLines(text);

			foreach (string line in lines)
			{
				if (line == "") // ? 空行
					continue;

				if (line[0] == ';') // ? コメント行
					continue;

				string[] tokens = SCommon.Tokenize(line, " ", false, true);

				this.Commands.Add(new ScenarioCommand()
				{
					Tokens = tokens,
				});
			}
		}
	}
}
