using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.GameCommons;
using Charlotte.Games.Surfaces;

namespace Charlotte.Games
{
	// Token: 0x0200001A RID: 26
	public class ScenarioCommand
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00005FF3 File Offset: 0x000041F3
		public IEnumerable<bool> Perform()
		{
			if (this.Tokens[0] == "P")
			{
				DDEngine.FreezeInput(5);
				while (1 > DDKey.GetInput(29) && 1 > DDInput.L.GetInput() && DDInput.A.GetInput() != 1 && DDInput.B.GetInput() != 1)
				{
					yield return true;
				}
			}
			else if (this.Tokens[1] == "=")
			{
				string instanceName = this.Tokens[0];
				string typeName = this.Tokens[2];
				Game.I.SurfaceManager.Add(SurfaceCreator.Create(typeName, instanceName));
			}
			else
			{
				string name = this.Tokens[0];
				string command = this.Tokens[1];
				string[] arguments = this.Tokens.Skip(2).ToArray<string>();
				Game.I.SurfaceManager.GetSurface(name).Invoke(command, arguments);
			}
			yield break;
		}

		// Token: 0x040000C1 RID: 193
		public string[] Tokens;
	}
}
