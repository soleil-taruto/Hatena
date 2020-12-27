using System;
using System.Collections.Generic;
using System.Text;

namespace Charlotte.Commons
{
	// Token: 0x0200009F RID: 159
	public class AttachString
	{
		// Token: 0x060002CC RID: 716 RVA: 0x0001116B File Offset: 0x0000F36B
		private AttachString() : this(':', '$', '.')
		{
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00011179 File Offset: 0x0000F379
		private AttachString(char delimiter, char escapeChr, char escapedDelimiter) : this(delimiter, new AttachString.S_EscapeString(delimiter.ToString(), escapeChr, escapedDelimiter.ToString()))
		{
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00011196 File Offset: 0x0000F396
		private AttachString(char delimiter, AttachString.S_EscapeString es)
		{
			this.Delimiter = delimiter;
			this.ES = es;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x000111AC File Offset: 0x0000F3AC
		public string Untokenize(IEnumerable<string> tokens)
		{
			List<string> dest = new List<string>();
			foreach (string token in tokens)
			{
				dest.Add(this.ES.Encode(token));
			}
			dest.Add("");
			return string.Join(this.Delimiter.ToString(), dest);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00011224 File Offset: 0x0000F424
		public string[] Tokenize(string str)
		{
			string[] array = SCommon.Tokenize(str, this.Delimiter.ToString(), false, false, 0);
			List<string> dest = new List<string>(array.Length);
			foreach (string token in array)
			{
				dest.Add(this.ES.Decode(token));
			}
			dest.RemoveAt(dest.Count - 1);
			return dest.ToArray();
		}

		// Token: 0x0400021E RID: 542
		public static AttachString I = new AttachString();

		// Token: 0x0400021F RID: 543
		private char Delimiter;

		// Token: 0x04000220 RID: 544
		private AttachString.S_EscapeString ES;

		// Token: 0x0200014F RID: 335
		private class S_EscapeString
		{
			// Token: 0x060006A0 RID: 1696 RVA: 0x00022204 File Offset: 0x00020404
			public S_EscapeString(string disallowedChrs, char escapeChr, string allowedChrs)
			{
				if (disallowedChrs == null || allowedChrs == null || disallowedChrs.Length != allowedChrs.Length || SCommon.HasSameChar(disallowedChrs + escapeChr.ToString() + allowedChrs))
				{
					throw new ArgumentException();
				}
				this.DisallowedChrs = disallowedChrs + escapeChr.ToString();
				this.EscapeChr = escapeChr;
				this.AllowedChrs = allowedChrs + escapeChr.ToString();
			}

			// Token: 0x060006A1 RID: 1697 RVA: 0x00022274 File Offset: 0x00020474
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

			// Token: 0x060006A2 RID: 1698 RVA: 0x000222F0 File Offset: 0x000204F0
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

			// Token: 0x0400054C RID: 1356
			private string DisallowedChrs;

			// Token: 0x0400054D RID: 1357
			private char EscapeChr;

			// Token: 0x0400054E RID: 1358
			private string AllowedChrs;
		}
	}
}
