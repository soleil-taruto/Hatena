using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Charlotte.Commons
{
	public class ArgsReader
	{
		private string[] Args = null;
		private int ArgIndex;

		public ArgsReader(string[] args, int argIndex = 0)
		{
			this.Args = args;
			this.ArgIndex = argIndex;
		}

		public bool HasArgs(int count = 1)
		{
			return count <= this.Args.Length - this.ArgIndex;
		}

		public bool ArgIs(string spell)
		{
			if (this.HasArgs() && this.GetArg().ToUpper() == spell.ToUpper())
			{
				this.ArgIndex++;
				return true;
			}
			return false;
		}

		public string GetArg(int index = 0)
		{
			return this.Args[this.ArgIndex + index];
		}

		public string NextArg()
		{
			string arg = this.GetArg();
			this.ArgIndex++;
			return arg;
		}
	}
}
