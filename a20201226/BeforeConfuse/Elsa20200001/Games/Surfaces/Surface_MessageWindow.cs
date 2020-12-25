using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	public class Surface_MessageWindow : Surface
	{
		private double A = 0.0;
		private bool Ended = false;
		private bool LeftSide = false;

		/// <summary>
		/// メッセージ
		/// 2行
		/// </summary>
		public string[] Messages = new string[]
		{
			"",
			"",
		};

		public void MessageUpdated()
		{
			// TODO
		}

		public override IEnumerable<bool> E_Draw()
		{
			for (; ; )
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
					DDPrint.SetBorder(new I3Color(40, 0, 0));
					DDPrint.SetPrint(
						(int)this.X - 200,
						(int)this.Y - 0
						);
					DDPrint.PrintLine(this.Messages[0]);
					DDPrint.PrintLine("");
					DDPrint.PrintLine(this.Messages[1]);
					DDPrint.Reset();
				}
				yield return !this.Ended || 0.003 < this.A;
			}
		}

		public override void Invoke_02(string command, string[] arguments)
		{
			int c = 0;

			if (command == "L")
			{
				this.X = 400;
				this.Y = 450;
				this.LeftSide = true;
			}
			else if (command == "R")
			{
				this.X = 600;
				this.Y = 450;
				this.LeftSide = false;
			}
			else if (command == "1")
			{
				string line = arguments[c++];

				this.Messages[0] = line;
				this.Messages[1] = "";
				this.MessageUpdated();
			}
			else if (command == "2")
			{
				string line = arguments[c++];

				this.Messages[1] = line;
				this.MessageUpdated();
			}
			else if (command == "終了")
			{
				this.Ended = true;
			}
			else
			{
				base.Invoke_02(command, arguments);
			}
		}
	}
}
