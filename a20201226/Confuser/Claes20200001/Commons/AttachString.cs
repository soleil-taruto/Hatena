using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Commons
{
	public class AttachString
	{
		public static AttachString I = new AttachString();

		private AttachString()
			: this(':', '$', '.')
		{ }

		private AttachString(char delimiter, char escapeChr, char escapedDelimiter)
			: this(delimiter, new S_EscapeString(delimiter.ToString(), escapeChr, escapedDelimiter.ToString()))
		{ }

		private char Delimiter;
		private S_EscapeString ES;

		private AttachString(char delimiter, S_EscapeString es)
		{
			this.Delimiter = delimiter;
			this.ES = es;
		}

		public string Untokenize(IEnumerable<string> tokens)
		{
			List<string> dest = new List<string>();

			foreach (string token in tokens)
				dest.Add(this.ES.Encode(token));

			dest.Add("");
			return string.Join(this.Delimiter.ToString(), dest);
		}

		public string[] Tokenize(string str)
		{
			string[] tokens = SCommon.Tokenize(str, this.Delimiter.ToString());
			List<string> dest = new List<string>(tokens.Length);

			foreach (string token in tokens)
				dest.Add(this.ES.Decode(token));

			dest.RemoveAt(dest.Count - 1);
			return dest.ToArray();
		}

		private class S_EscapeString
		{
			private string DisallowedChrs;
			private char EscapeChr;
			private string AllowedChrs;

			public S_EscapeString(string disallowedChrs, char escapeChr, string allowedChrs)
			{
				if (
					disallowedChrs == null ||
					allowedChrs == null ||
					disallowedChrs.Length != allowedChrs.Length ||
					SCommon.HasSameChar(disallowedChrs + escapeChr + allowedChrs)
					)
					throw new ArgumentException();

				this.DisallowedChrs = disallowedChrs + escapeChr;
				this.EscapeChr = escapeChr;
				this.AllowedChrs = allowedChrs + escapeChr;
			}

			public string Encode(string str)
			{
				StringBuilder buff = new StringBuilder(str.Length * 2);

				foreach (char chr in str)
				{
					int chrPos = this.DisallowedChrs.IndexOf(chr);

					if (chrPos == -1)
					{
						buff.Append(chr);
					}
					else
					{
						buff.Append(this.EscapeChr);
						buff.Append(this.AllowedChrs[chrPos]);
					}
				}
				return buff.ToString();
			}

			public string Decode(string str)
			{
				StringBuilder buff = new StringBuilder(str.Length);

				for (int index = 0; index < str.Length; index++)
				{
					char chr = str[index];

					if (chr == this.EscapeChr && index + 1 < str.Length)
					{
						index++;
						chr = str[index];
						int chrPos = this.AllowedChrs.IndexOf(chr);

						if (chrPos != -1)
						{
							chr = this.DisallowedChrs[chrPos];
						}
					}
					buff.Append(chr);
				}
				return buff.ToString();
			}
		}
	}
}
