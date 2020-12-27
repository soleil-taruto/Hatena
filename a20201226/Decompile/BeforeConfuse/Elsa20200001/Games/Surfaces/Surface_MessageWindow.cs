using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	// Token: 0x0200002D RID: 45
	public class Surface_MessageWindow : Surface
	{
		// Token: 0x0600008A RID: 138 RVA: 0x000020C9 File Offset: 0x000002C9
		public void MessageUpdated()
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000076C2 File Offset: 0x000058C2
		public override IEnumerable<bool> E_Draw()
		{
			for (;;)
			{
				DDUtils.Approach(ref this.A, this.Ended ? 0.0 : 1.0, 0.9);
				DDDraw.SetAlpha(this.A);
				DDDraw.DrawBegin(Ground.I.Picture.MessageWindow, this.X, this.Y);
				DDDraw.DrawZoom_X(this.LeftSide ? 1.0 : -1.0);
				DDDraw.DrawZoom_Y(-1.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				if (!this.Ended)
				{
					DDPrint.SetBorder(new I3Color(40, 0, 0), 1);
					DDPrint.SetPrint((int)this.X - 200, (int)this.Y, 16);
					DDPrint.PrintLine(this.Messages[0]);
					DDPrint.PrintLine("");
					DDPrint.PrintLine(this.Messages[1]);
					DDPrint.Reset();
				}
				yield return !this.Ended || 0.003 < this.A;
			}
			yield break;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000076D4 File Offset: 0x000058D4
		public override void Invoke_02(string command, string[] arguments)
		{
			int c = 0;
			if (command == "L")
			{
				this.X = 400.0;
				this.Y = 450.0;
				this.LeftSide = true;
				return;
			}
			if (command == "R")
			{
				this.X = 600.0;
				this.Y = 450.0;
				this.LeftSide = false;
				return;
			}
			if (command == "1")
			{
				string line = arguments[c++];
				this.Messages[0] = line;
				this.Messages[1] = "";
				this.MessageUpdated();
				return;
			}
			if (command == "2")
			{
				string line2 = arguments[c++];
				this.Messages[1] = line2;
				this.MessageUpdated();
				return;
			}
			if (command == "終了")
			{
				this.Ended = true;
				return;
			}
			base.Invoke_02(command, arguments);
		}

		// Token: 0x040000E5 RID: 229
		private double A;

		// Token: 0x040000E6 RID: 230
		private bool Ended;

		// Token: 0x040000E7 RID: 231
		private bool LeftSide;

		// Token: 0x040000E8 RID: 232
		public string[] Messages = new string[]
		{
			"",
			""
		};
	}
}
