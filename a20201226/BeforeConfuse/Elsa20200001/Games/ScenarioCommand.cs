using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Surfaces;

namespace Charlotte.Games
{
	public class ScenarioCommand
	{
		public string[] Tokens;

		// <---- prm

		/// <summary>
		/// コマンドを実行する。
		/// true を返すことによって、次のフレームに送る。(次のフレームまで待つ)
		/// false を返すことによって、現在のフレームにおいて次のコマンドが呼び出される。
		/// -- 連続する「最初に false を返す」コマンド列は１フレームで実行される。
		/// </summary>
		/// <returns>このコマンドを継続するか</returns>
		public IEnumerable<bool> Perform()
		{
			if (this.Tokens[0] == "P") // PAUSE (入力待ち)
			{
				DDEngine.FreezeInput(5); // HACK: 要調整

				for (; ; )
				{
					if (1 <= DDKey.GetInput(DX.KEY_INPUT_LCONTROL)) // ? 左コントロール_ホールド中
						break;

					if (1 <= DDInput.L.GetInput()) // ? 会話スキップ_ホールド中
						break;

					if (DDInput.A.GetInput() == 1) // ? 決定_押下
						break;

					if (DDInput.B.GetInput() == 1) // ? キャンセル_押下
						break;

					yield return true; // 次のフレームに送る。
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
				string[] arguments = this.Tokens.Skip(2).ToArray();

				Game.I.SurfaceManager.GetSurface(name).Invoke(command, arguments);
			}
		}
	}
}
