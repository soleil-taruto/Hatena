using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Charlotte.Commons;

namespace Charlotte.CSSolutions
{
	public class CSFile
	{
		private string _file;

		public CSFile(string file)
		{
			_file = file;
		}

		public string GetFile()
		{
			return _file;
		}

		public void SolveNamespace()
		{
			// クラス配置の平滑化
			// ROOT_NAMESPACE 配下のクラスを ROOT_NAMESPACE 直下に置く。
			// クラス名の重複は想定しない。

			const string ROOT_NAMESPACE = "Charlotte";

			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			for (int index = 0; index < lines.Length; index++)
			{
				if (lines[index].StartsWith("using " + ROOT_NAMESPACE + "."))
					lines[index] = "";
				else if (lines[index].StartsWith("namespace " + ROOT_NAMESPACE + "."))
					lines[index] = "namespace " + ROOT_NAMESPACE;
			}
			File.WriteAllLines(_file, lines, Encoding.UTF8);
		}

		public void RemoveComments()
		{
			char[] chrs = File.ReadAllText(_file, Encoding.UTF8).ToArray();

			for (int index = 0; index < chrs.Length; index++)
			{
				// ? プリプロセッサ
				if (chrs[index] == '#')
				{
					// 事故防止のためこの行のこの文字以降については危ない文字を消しておく。
					// コメントを削除する必要はあるので、index は進めない。

					for (int c = index + 1; chrs[c] != '\n'; c++)
					{
						if (
							chrs[c] == '\'' ||
							chrs[c] == '"'
							)
							chrs[c] = '_';
					}
				}
				// ? リテラル文字
				else if (chrs[index] == '\'')
				{
					index++;

					if (chrs[index] == '\\')
					{
						index++;

						if (chrs[index] == 'u')
							index += 4;
					}
					index++;

					if (chrs[index] != '\'')
						throw null; // 想定外
				}
				// ? (改行可能な)リテラル文字列
				else if (
					chrs[index + 0] == '@' &&
					chrs[index + 1] == '"'
					)
				{
					index = RC_Skip(chrs, index + 2, '"', '"', '"', '"', '"');
				}
				// ? リテラル文字列
				else if (chrs[index] == '"')
				{
					index = RC_Skip(chrs, index + 1, '\\', '\\', '\\', '"', '"');
				}
				// ? C系コメント
				else if (
					chrs[index + 0] == '/' &&
					chrs[index + 1] == '*'
					)
				{
					chrs[index + 0] = ' '; // '/' 除去
					chrs[index + 1] = ' '; // '*' 除去

					index = RC_Mask(chrs, index + 2, '*', '/');

					chrs[index + 0] = ' '; // '*' 除去
					chrs[index + 1] = ' '; // '/' 除去

					index++;
				}
				// ? C++系コメント
				else if (
					chrs[index + 0] == '/' &&
					chrs[index + 1] == '/'
					)
				{
					if (Common.IsMatch(chrs, index, CSConsts.KEEP_COMMENT_START_PATTERN.ToArray()))
					{
						// C++系コメントの後に改行が必ずあると想定する。

						index = RC_Skip_EC2(chrs, index + 2, '\r', '\n') + 1;
					}
					else
					{
						chrs[index + 0] = ' '; // '/' 除去
						chrs[index + 1] = ' '; // '/' 除去

						// C++系コメントの後に改行が必ずあると想定する。

						index = RC_Mask(chrs, index + 2, '\r', '\n') + 1;
					}
				}
			}
			File.WriteAllText(_file, new string(chrs), Encoding.UTF8);

			// コメント除去後に生じうる行末の空白を除去する。
			{
				string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

				lines = lines.Select(v => v.TrimEnd()).ToArray();

				File.WriteAllLines(_file, lines, Encoding.UTF8);
			}
		}

		private static int RC_Skip(char[] chrs, int index, char ignChr_a1, char ignChr_a2, char ignChr_b1, char ignChr_b2, char endChr)
		{
			for (; ; index++)
			{
				while (
					chrs[index + 0] == ignChr_a1 &&
					chrs[index + 1] == ignChr_a2
					||
					chrs[index + 0] == ignChr_b1 &&
					chrs[index + 1] == ignChr_b2
					)
					index += 2;

				if (chrs[index] == endChr)
					break;
			}
			return index; // endChr の位置
		}

		private static int RC_Mask(char[] chrs, int index, char endChr_1, char endChr_2)
		{
			while (
				chrs[index + 0] != endChr_1 ||
				chrs[index + 1] != endChr_2
				)
				chrs[index++] = ' ';

			return index; // endChr_1 の位置
		}

		private static int RC_Skip_EC2(char[] chrs, int index, char endChr_1, char endChr_2)
		{
			while (
				chrs[index + 0] != endChr_1 ||
				chrs[index + 1] != endChr_2
				)
				index++;

			return index; // endChr_1 の位置
		}

		public void RemovePreprocessorDirectives()
		{
			// #if の入れ子に対応していない。
			// #if, #elif の条件式は true, false, !true, !false のみ想定する。

			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			for (int index = 0; index < lines.Length; index++)
			{
				string line = lines[index];

				if (
					line == "#if true" ||
					line == "#if !false"
					)
				{
					lines[index] = "";
					index = RPD_RemoveIfTrue(lines, index + 1);
				}
				else if (
					line == "#if !true" ||
					line == "#if false"
					)
				{
					lines[index] = "";
					index = RPD_RemoveIfFalse(lines, index + 1);
				}
				else if (line.StartsWith("#")) // ? #if-系
				{
					throw new Exception("Bad #...");
				}
				else if (line.TrimStart().StartsWith("#")) // ? #region-系
				{
					lines[index] = "";
				}
			}
			File.WriteAllLines(_file, lines, Encoding.UTF8);
		}

		private int RPD_RemoveIfFalse(string[] lines, int index)
		{
			for (; ; index++)
			{
				string line = lines[index];

				if (
					line == "#else" ||
					line == "#elif true" ||
					line == "#elif !false"
					)
				{
					lines[index] = "";
					index = RPD_RemoveIfTrue(lines, index + 1);
					break;
				}
				else if (
					line == "#elif !true" ||
					line == "#elif false"
					)
				{
					// このまま続ける。
				}
				else if (line == "#endif")
				{
					lines[index] = "";
					break;
				}
				else if (line.StartsWith("#")) // ? #if-系
				{
					throw new Exception("Bad #...");
				}
				lines[index] = "";
			}
			return index;
		}

		private int RPD_RemoveIfTrue(string[] lines, int index)
		{
			for (; ; index++)
			{
				string line = lines[index];

				if (
					line == "#else" ||
					line == "#elif true" ||
					line == "#elif !false" ||
					line == "#elif !true" ||
					line == "#elif false"
					)
				{
					lines[index] = "";
					break;
				}
				else if (line == "#endif")
				{
					lines[index] = "";
					goto endFunc;
				}
				else if (line.StartsWith("#")) // ? #if-系
				{
					throw new Exception("Bad #...");
				}
				else if (line.TrimStart().StartsWith("#")) // ? #region-系
				{
					lines[index] = "";
				}
			}
			for (; ; index++)
			{
				string line = lines[index];

				if (line == "#endif")
				{
					lines[index] = "";
					break;
				}
				else if (line.StartsWith("#")) // ? #if-系
				{
					// noop -- 無視する。
				}
				lines[index] = "";
			}
		endFunc:
			return index;
		}

		public void SolveAccessModifiers()
		{
			string text = File.ReadAllText(_file, Encoding.UTF8);

			text = SAM_Replace(text, "{ get; private set; }", ";");
			text = SAM_Replace(text, "{ private get; set; }", ";");
			text = SAM_Replace(text, "{ set; private get; }", ";");
			text = SAM_Replace(text, "{ private set; get; }", ";");
#if true // string 以外 (int や double など) は const のままにする。
			text = SAM_Replace(text, "private const string", "public static string");
			text = SAM_Replace(text, "protected const string", "public static string");
			text = SAM_Replace(text, "public const string", "public static string");
			text = SAM_Replace(text, "const string", "string");
			// ----
			text = SAM_Replace(text, "private const", "public const");
			text = SAM_Replace(text, "protected const", "public const");
			//text = SAM_Replace(text, "public const", "public const"); // 同じ
#else // old
			text = SAM_Replace(text, "private const", "public static");
			text = SAM_Replace(text, "protected const", "public static");
			text = SAM_Replace(text, "public const", "public static");
			text = SAM_Replace(text, "const", "");
#endif
			text = SAM_Replace(text, "readonly", "");
			//text = SAM_Replace(text, "private", "public"); // 継承クラスに同名のメンバが居るとマズい。
			//text = SAM_Replace(text, "protected", "public"); // protected override に対応していない。
			text = SAM_Replace(text, "public static class", "public class");
			text = SAM_Replace(text, "static class Program", "public class Program"); // Program.cs 専用

			// public const -> public static 追加_v1122
			{
				text = SAM_PublicConstVarToPublicStaticVar(text);
				text = SAM_Replace(text, "const", ""); // SAM_PublicConstVarToPublicStaticVar による変換により、必要になった。
			}

			File.WriteAllText(_file, text, Encoding.UTF8);
		}

		private string SAM_Replace(string text, string targPtn, string destPtn)
		{
			for (int index = 0; ; )
			{
				int next = text.IndexOf(targPtn, index);

				if (next == -1)
					break;

				if (
					SAM_IsSpaceOrPunct(text[next - 1]) &&
					SAM_IsSpaceOrPunct(text[next + targPtn.Length])
					)
					text = text.Substring(0, next) + destPtn + text.Substring(next + targPtn.Length);

				index = next + destPtn.Length;
			}
			return text;
		}

		private static bool SAM_IsSpaceOrPunct(char chr)
		{
			return !Common.IsCSWordChar(chr);
		}

		private static string SAM_PublicConstVarToPublicStaticVar(string text)
		{
			const string START_PTN = "\tpublic const ";

			int index = 0;

			for (; ; )
			{
				int start = text.IndexOf(START_PTN, index);

				if (start == -1)
					break;

				int end = text.IndexOf('=', start + START_PTN.Length);

				if (end == -1)
					break;

				int indentLen = SAM_PCVTPSV_GetIndentLen(text, start + 1);
				string indent = new string(Enumerable.Range(0, indentLen).Select(dummy => '\t').ToArray());

				string midText = text.Substring(start, end - start);
				string midTextNew;

				//Console.WriteLine("* " + midText); // test

				string varType;
				string varName;
				string varName_INITED = SAM_CreateVarName();
				string varName_Value = SAM_CreateVarName();
				string varName_GetValue = SAM_CreateVarName();
				string varName_GetValue_Ret = SAM_CreateVarName();

				{
					string[] tokens = SCommon.Tokenize(midText, " ", false, true);

					if (tokens.Length != 4)
						throw null; // 想定外

					// [0] public
					// [1] const
					varType = tokens[2]; // 型名
					varName = tokens[3]; // フィールド名
				}

				midTextNew =
					"\tpublic static " +
					varType + " " +
					varName + " { get { if (!" +
					varName_INITED + ") { " +
					varName_INITED + " = true; " +
					varName_Value + " = " +
					varName_GetValue + "(); } return " +
					varName_Value + "; }}" +
					Consts.CRLF + indent + "public static " +
					varType + " " +
					varName_GetValue + "() { " +
					varType + " " +
					varName_GetValue_Ret + " ";

				text = text.Substring(0, start) + midTextNew + text.Substring(end);

				index = end;
				index++; // Skip '='
				index -= midText.Length;
				index += midTextNew.Length;

				// ----

				// = から ; までの間に、余計な ; は無いと思ったけど
				// 少なくとも public char SemiColon = ';'; がある。
				// ; の直後は改行のはずなので、それも検索パターンに入れて回避する。
				//
				start = text.IndexOf(";" + Consts.CRLF, index);

				if (start == -1)
					throw null; // 想定外

				// 初期化値の置き換え
				{
					midText = text.Substring(index, start - index);
					midTextNew = SAM_PCVTPSV_初期化値の置き換え(midText);

					if (midTextNew != null)
					{
						text = text.Substring(0, index) + midTextNew + text.Substring(start);

						start -= midText.Length;
						start += midTextNew.Length;
					}
				}

				end = start + 1;

				midText = text.Substring(start, end - start);
				midTextNew =
					"; return " +
					varName_GetValue_Ret + "; }" +
					Consts.CRLF + indent + "public static bool " +
					varName_INITED + ";" +
					Consts.CRLF + indent + "public static " +
					varType + " " +
					varName_Value + ";";

				text = text.Substring(0, start) + midTextNew + text.Substring(end);

				index = end;
				index -= midText.Length;
				index += midTextNew.Length;
			}
			return text;
		}

		private static int SAM_PCVTPSV_GetIndentLen(string text, int end)
		{
			int start = end;

			while (0 < start && text[start - 1] == '\t')
				start--;

			return end - start;
		}

		private static string SAM_PCVTPSV_初期化値の置き換え(string initValue) // ret: null == 置き換えナシ
		{
			initValue = initValue.Trim();

			if (Regex.IsMatch(initValue, "^[-]?[0-9]{0,10}$"))
			{
				long tmp = long.Parse(initValue);

				if ((long)int.MinValue <= tmp && tmp <= (long)int.MaxValue)
				{
					// リテラル文字列の難読化を適用させるために、改行が要る。
					// 直前が = なので、最初に空白を入れる。
					//
					return " int.Parse(" + Consts.CRLF + "\"" + initValue + "\")";
				}
			}
			return null;
		}

		private static string SAM_CreateVarName()
		{
			// crand 128 bit -> 重複を想定しない。

			return
				"SAM_a_" +
				SCommon.CRandom.GetUInt64().ToString("D20") + "_" +
				SCommon.CRandom.GetUInt64().ToString("D20") +
				"_z";
		}

		public void SolveLiteralStrings()
		{
			this.SolveLiteralStrings_01();
			this.SolveLiteralStrings_02();
		}

		private void SolveLiteralStrings_01()
		{
			char[] chrs = File.ReadAllText(_file, Encoding.UTF8).ToArray();
			StringBuilder dest = new StringBuilder();

			for (int index = 0; index < chrs.Length; index++)
			{
				// ? リテラル文字
				if (chrs[index] == '\'')
				{
					char chr;

					index++;

					if (chrs[index] == '\\')
					{
						index++;

						if (chrs[index] == '\\')
						{
							chr = '\\';
						}
						else if (chrs[index] == 't')
						{
							chr = '\t';
						}
						else if (chrs[index] == 'r')
						{
							chr = '\r';
						}
						else if (chrs[index] == 'n')
						{
							chr = '\n';
						}
						else if (
							chrs[index] == 'u' &&
							Common.IsHexadecimal(chrs[index + 1]) &&
							Common.IsHexadecimal(chrs[index + 2]) &&
							Common.IsHexadecimal(chrs[index + 3]) &&
							Common.IsHexadecimal(chrs[index + 4])
							)
						{
							chr = (char)Convert.ToUInt16(new string(new char[]
							{
								chrs[index + 1],
								chrs[index + 2],
								chrs[index + 3],
								chrs[index + 4],
							}),
							16
							);

							index += 4;
						}
						else
						{
							throw null; // 想定外
						}
					}
					else
					{
						chr = chrs[index];
					}
					index++;

					if (chrs[index] != '\'')
						throw null; // 想定外

					dest.Append("((char)" + ((int)chr) + ")");
					//dest.Append("((char)0x" + ((int)chr).ToString("x4") + ")"); // old
				}
				// ? (改行可能な)リテラル文字列
				else if (
					chrs[index + 0] == '@' &&
					chrs[index + 1] == '"'
					)
				{
					dest.Append('"');
					index += 2;

					for (; ; index++)
					{
						char chr = chrs[index];

						if (chr == '"')
						{
							if (chrs[index + 1] != '"')
								break;

							index++;
						}
						dest.Append("\\u");
						dest.Append(((ushort)chr).ToString("x4"));
					}
					dest.Append('"');
				}
				else if (chrs[index] == '"') // ? リテラル文字列
				{
					dest.Append('"');
					index++;

					for (; ; index++)
					{
						char chr = chrs[index];

						if (chr == '"')
							break;

						if (chr == '\\')
						{
							index++;
							chr = chrs[index];

							switch (chr)
							{
								case 't': chr = '\t'; break;
								case 'r': chr = '\r'; break;
								case 'n': chr = '\n'; break;
								case 'u': throw null; // 想定外？？？
								case '\\': break;

								default:
									throw null; // 想定外
							}
						}
						dest.Append("\\u");
						dest.Append(((ushort)chr).ToString("x4"));
					}
					dest.Append('"');
				}
				else
				{
					dest.Append(chrs[index]);
				}
			}
			File.WriteAllText(_file, dest.ToString(), Encoding.UTF8);
		}

		private void SolveLiteralStrings_02()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);
			List<string> varLines = new List<string>();

			for (int index = 0; index < lines.Length; index++)
			{
				string line = lines[index];

				// 以下のリテラル文字列は変数化出来ない。
				// -- switch の case
				// -- デフォルト引数
				// -- [DllImport(... とか

				if (line.Trim().StartsWith("case ")) // ? switch の case
				{
					// noop
				}
				else if (Regex.IsMatch(line, "(private|protected|public).*[(].*[=]")) // ? デフォルト引数
				{
					// noop
				}
				else if (line.Trim().StartsWith("[")) // ? [DllImport(... とか
				{
					// noop
				}
				else
				{
					for (; ; )
					{
						int c = line.IndexOf('"');

						if (c == -1)
							break;

						int c2 = line.IndexOf('"', c + 1);

						if (c2 == -1)
							throw null; // never

						c2++;
						string varName = SLS2_CreateVarName();
#if true
						// リテラル文字列難読化_v1112 >

						varLines.Add("\t\tpublic static string " +
							varName + "_String;"
							);
						varLines.Add("\t\tpublic static string " +
							varName + "() { if(" +
							varName + "_String == null) { " +
							varName + "_String = " +
							varName + "_GetString(); } return " +
							varName + "_String; }"
							);
						varLines.Add("\t\tpublic static string " +
							varName + "_GetString() { return new string(" +
							varName + "_E_GetString().Where(" +
							varName + "_Var => " +
							varName + "_Var % 65537 != 0).Select(" +
							varName + "_Var2 => (char)(" +
							varName + "_Var2 % 65537 - 1)).ToArray()); }"
							);
						varLines.Add("\t\tpublic static IEnumerable<int> " +
							varName + "_E_GetString() { " +
							string.Join(" ", SLS2_ToYR(line.Substring(c, c2 - c))) + " }"
							);

						// < リテラル文字列難読化_v1112

						line = line.Substring(0, c) + varName + "()" + line.Substring(c2);
#elif true // old
						varLines.Add("\t\tpublic static string " + varName + "() { return " + line.Substring(c, c2 - c) + "; }");
						line = line.Substring(0, c) + varName + "()" + line.Substring(c2);
#elif true // old
						varLines.Add("\t\tpublic static string " + varName + " { get { return " + line.Substring(c, c2 - c) + "; }}");
						line = line.Substring(0, c) + varName + line.Substring(c2);
#else // old
						varLines.Add("\t\tpublic static string " + varName + " = " + line.Substring(c, c2 - c) + ";");
						line = line.Substring(0, c) + varName + line.Substring(c2);
#endif
					}
					lines[index] = line;
				}
			}
			File.WriteAllLines(_file, this.SLS2_クラスの先頭に挿入(lines, varLines), Encoding.UTF8);

			// リテラル文字列難読化_v1112 >

			// IEnumerable<T>, Where(), Select() を使用するための、追加 using
			{
				this.SLS2_AddUsingLineIfNotExist("using System.Collections.Generic;");
				this.SLS2_AddUsingLineIfNotExist("using System.Linq;");
			}

			// < リテラル文字列難読化_v1112
		}

		private static string SLS2_CreateVarName()
		{
			// crand 128 bit -> 重複を想定しない。

			return
				"SLS2_a_" +
				SCommon.CRandom.GetUInt64().ToString("D20") + "_" +
				SCommon.CRandom.GetUInt64().ToString("D20") +
				"_z";
		}

		private static IEnumerable<string> SLS2_ToYR(string code)
		{
			if (code.Length < 2)
				throw null;

			if (code[0] != '"')
				throw null;

			if (code[code.Length - 1] != '"')
				throw null;

			code = code.Substring(1, code.Length - 2);

			if (code.Length % 6 != 0)
				throw null;

#if true
			{
				int dmyYRNum = SCommon.CRandom.GetRange(3, 7);

				for (int c = 0; c < dmyYRNum; c++)
					yield return SLS2_MakeYR(-1);
			}
#else // old
			if (code.Length == 0)
				yield return SL2_MakeYR(-1);
#endif

			int divFuncPermil = 100;

			for (int index = 0; index < code.Length; index += 6)
			{
				if (
					code[index + 0] != '\\' ||
					code[index + 1] != 'u' ||
					!Common.IsHexadecimal(code[index + 2]) ||
					!Common.IsHexadecimal(code[index + 3]) ||
					!Common.IsHexadecimal(code[index + 4]) ||
					!Common.IsHexadecimal(code[index + 5])
					)
					throw null;

				// メソッド分割
				{
					if (SCommon.CRandom.GetInt(1000) < divFuncPermil)
					{
						string name = SLS2_CreateVarName();

						yield return "foreach (int " +
							name + "_Char in " +
							name + "_Next()) { yield return " +
							name + "_Char; }}" +
							Consts.CRLF + "\t\tpublic static IEnumerable<int> " + // メンバーの並びシャッフルのために改行が要る。
							name + "_Next() {";

						divFuncPermil = 1;
					}
					else
						divFuncPermil++;
				}

				if (SCommon.CRandom.GetInt(2) == 0) // ランダムにダミー値を差し込む
					yield return SLS2_MakeYR(-1);

				yield return SLS2_MakeYR((int)Convert.ToUInt16(code.Substring(index + 2, 4), 16));
			}
		}

		private static string SLS2_MakeYR(int chr)
		{
			// (0x0000 + 1) + 65537 * 0 == 1
			// (0xffff + 1) + 65537 * 0 == 65536
			// (0x0000 + 1) + 65537 * 15259 == 1000029084
			// (0xffff + 1) + 65537 * 15259 == 1000094619
			// (0x0000 + 1) + 65537 * 32766 == 2147385343
			// (0xffff + 1) + 65537 * 32766 == 2147450878 (0x7fff7ffe)

			int value = (chr + 1) + 65537 * SCommon.CRandom.GetRange(15259, 32766);
			//int value = (chr + 1) + 65537 * SCommon.CRandom.GetRange(0, 32766); // old

			return "yield return " + value + ";";
		}

		private IEnumerable<string> SLS2_クラスの先頭に挿入(string[] lines, List<string> varLines)
		{
			if (varLines.Count == 0)
				return lines;

			int index = SCommon.IndexOf(lines, v => v.StartsWith("\t\t"));

			if (index == -1)
				throw new Exception("クラスの先頭を見つけられませんでした。");

			return lines.Take(index).Concat(varLines).Concat(lines.Skip(index));
		}

		private void SLS2_AddUsingLineIfNotExist(string targLine)
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			if (!lines.Any(v => v == targLine)) // ? targLine という行は存在しない。
			{
				File.WriteAllLines(_file, new string[] { targLine }.Concat(lines), Encoding.UTF8); // targLine を先頭行に追加
			}
		}

		public void OpenClosedEmptyClass()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);
			int bracketPos = SCommon.IndexOf(lines, v => v == "\t{ }");

			if (bracketPos != -1)
			{
				// このメソッドが処理した痕跡を残したいのでブロックの間にコメントを差し込んでおく
				// 単語の置換に引っかからないよう、数字で始まる「単語」にする。
				// --> // 00_ocec

				File.WriteAllLines(
					_file,
					lines.Take(bracketPos).Concat(new string[] { "\t{", "\t\t// 00_ocec", "\t}" }).Concat(lines.Skip(bracketPos + 1)), // ucc エラー回避のため --> // not_uuid
					Encoding.UTF8
					);
			}
		}

		public void AddDummyMember()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);
			bool structFlag = ADM_IsStruct(lines);
			string dmText = structFlag ? CSResources.STRUCT_DUMMY_MEMBER : CSResources.CLASS_DUMMY_MEMBER;
			string[] dmLines = SCommon.TextToLines(dmText).Where(v => v != "").ToArray();
			int end = ADM_GetClassEnd(lines);

			if (end == -1)
				throw null; // 想定外

			// ダミーメンバーの生成と挿入
			{
				List<string> dest = new List<string>();

				// ダミーメンバー_01
				{
					int dmCount = lines.Length / dmLines.Length;
					dmCount /= 3;
					dmCount++;

					for (int index = 0; index < dmCount; index++)
					{
						string ident = ADM_CreateIdent();

						foreach (string f_line in dmLines)
						{
							string line = f_line;
							line = line.Replace("SSS_", ident + "_");
							dest.Add(line);
						}
					}
				}

				// ダミーメンバー_02
				{
					int dmCount = SCommon.CRandom.GetRange(1, 13);
					string ident = ADM_CreateIdent();

					dest.Add("\t\tpublic static int " +
						ident + "_Count_0;"
						);
					dest.Add("\t\tpublic int " +
						ident + "_GetInt_0() { return " +
						ident + "_Count_0; }"
						);

					for (int index = 0; index < dmCount; index++)
					{
						int nextIndex = index + 1;

						string op = SCommon.CRandom.ChooseOne(SCommon.TextToLines(@"

+
-
== 0 ? 0 :
== 0 ? 1 :
== 1 ? 0 :
== 1 ? 1 :
!= 0 ? 0 :
!= 0 ? 1 :
!= 1 ? 0 :
!= 1 ? 1 :

").Select(v => v.Trim()).Where(v => v != "").ToArray());

						dest.Add("\t\tpublic static int " +
							ident + "_Count_" +
							nextIndex + ";"
							);
						dest.Add("\t\tpublic int " +
							ident + "_GetInt_" +
							nextIndex + "() { return " +
							ident + "_GetInt_" +
							index + "() " +
							op + " " +
							ident + "_Count_" +
							nextIndex + "; }"
							);
					}
				}

				lines = lines.Take(end).Concat(dest).Concat(lines.Skip(end)).ToArray();
			}

			File.WriteAllLines(_file, lines, Encoding.UTF8);
		}

		private static bool ADM_IsStruct(string[] lines)
		{
			return lines.Any(v => v.StartsWith("\tpublic struct "));
		}

		private static int ADM_GetClassEnd(string[] lines)
		{
			return SMO_GetClassEnd(lines);
		}

		private static string ADM_CreateIdent()
		{
			// crand 128 bit -> 重複を想定しない。

			return
				"ADM_a_" +
				SCommon.CRandom.GetUInt64().ToString("D20") + "_" +
				SCommon.CRandom.GetUInt64().ToString("D20") +
				"_z";
		}

		public void RenameEx(Func<string, string> filter, Predicate<string> f_isUnrenameableClassName)
		{
			string text = File.ReadAllText(_file, Encoding.UTF8);
			string text_bk = text;
			bool insideOfLiteralChar = false;
			bool insideOfLiteralString = false;
			Dictionary<string, string> escapedLines = SCommon.CreateDictionary<string>();

			text = RX_EscapeNoRenameLines(text, escapedLines);

			// C#の書式上「C#の単語」で終わることは無いはずだが、一応想定する。
			//
			text += " "; // 番兵設置

			for (int index = 0; index < text.Length; index++)
			{
				if (text[index] == '\\') // ? エスケープ文字 -> スキップする。
				{
					index++;
					continue;
				}
				insideOfLiteralChar ^= text[index] == '\'';
				insideOfLiteralString ^= text[index] == '"';

				if (
					!insideOfLiteralChar &&
					!insideOfLiteralString &&
					Common.IsCSWordChar(text[index])
					)
				{
					int end = index + 1;

					while (Common.IsCSWordChar(text[end]))
						end++;

					string name = text.Substring(index, end - index);
					string nameNew = filter(name);

					if (name == nameNew) // ? 置き換え禁止ワード
					{
						index = end;

						// ? 置き換え禁止クラス名
						// -> (置き換え禁止ワード).(後続のワード).(後続のワード).(後続のワード) ... の「後続のワード」も置き換え禁止とする。
						if (f_isUnrenameableClassName(name))
						{
							//Console.WriteLine("name: " + name); // test

							while (text[index] == '.' && Common.IsCSWordChar(text[index + 1]))
							{
								end = index + 2;

								while (Common.IsCSWordChar(text[end]))
									end++;

								//Console.WriteLine("trailer: " + text.Substring(index, end - index)); // test
								index = end;
							}
						}
					}
					else
					{
						text = text.Substring(0, index) + nameNew + text.Substring(end);
						index += nameNew.Length;
					}
				}
			}
			text = text.Substring(0, text.Length - 1); // 復元 -- 番兵除去

			text = RX_UnescapeNoRenameLines(text, escapedLines);
			text = RX_Mix(text, text_bk);

			File.WriteAllText(_file, text, Encoding.UTF8);
		}

		private static string RX_EscapeNoRenameLines(string text, Dictionary<string, string> escapedLines)
		{
			string[] lines = SCommon.TextToLines(text);

			lines = lines.Select(line =>
			{
				if (line.EndsWith(CSConsts.NO_RENAME_LINE_SUFFIX))
				{
					string ident = "9_" + Common.CreateNewIdent(); // 数字で始まるトークンは置き換えない。
					escapedLines.Add(ident, line);
					return ident;
				}
				return line;
			})
			.ToArray();

			return SCommon.LinesToText(lines);
		}

		private static string RX_UnescapeNoRenameLines(string text, Dictionary<string, string> escapedLines)
		{
			string[] lines = SCommon.TextToLines(text);

			lines = lines.Select(line =>
			{
				if (escapedLines.ContainsKey(line))
					return escapedLines[line];

				return line;
			})
			.ToArray();

			return SCommon.LinesToText(lines);
		}

		private static string RX_Mix(string text, string text_bk)
		{
			string[] lines = SCommon.TextToLines(text);
			string[] lines_bk = SCommon.TextToLines(text_bk);

			if (lines.Length != lines_bk.Length) // 同じはず
				throw null; // 想定外

			List<string> dest = new List<string>();

			for (int index = 0; index < lines.Length; index++)
			{
				dest.Add(lines[index]);
				dest.Add("// " + lines_bk[index]);
			}
			return SCommon.LinesToText(dest.ToArray());
		}

		private class SMO_RangeInfo
		{
			public int Start;
			public int End;
			public bool HoldingOrder;
		}

		public void ShuffleMemberOrder()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			int start;
			int end = SMO_GetClassEnd(lines);

			if (end == -1) // ? クラス閉じが見つからない -> 想定外
				throw null;

			List<SMO_RangeInfo> ranges = new List<SMO_RangeInfo>();
			bool dllImportFlag = false;

			for (int index = 0; index < end; index++)
			{
				string line = lines[index];
				bool foundMemberFlag = false;

				if (line.StartsWith("\t\t[")) // ? [DllImport(... とか
				{
					if (!dllImportFlag)
					{
						foundMemberFlag = true;
						dllImportFlag = true;
					}
				}
				else if (
					line.StartsWith("\t\tprivate ") ||
					line.StartsWith("\t\tprotected ") ||
					line.StartsWith("\t\tpublic ") ||
					line.StartsWith("\t\tstatic ") // static void Main() とか
					)
				{
					if (!dllImportFlag)
						foundMemberFlag = true;
					else
						dllImportFlag = false;
				}

				if (foundMemberFlag)
				{
					// クラスのメンバーを追加
					ranges.Add(new SMO_RangeInfo()
					{
						Start = index,
						End = -1, // ダミー
					});
				}
			}
			if (ranges.Count <= 1) // ? メンバーが１つ以下 -> シャッフル不要
			{
				Console.WriteLine("シャッフル・キャンセル -- メンバーが１つ以下");
				return;
			}
			start = ranges[0].Start;

			for (int index = 1; index < ranges.Count; index++)
				ranges[index - 1].End = ranges[index].Start;

			ranges[ranges.Count - 1].End = end;

			foreach (SMO_RangeInfo range in ranges)
				range.HoldingOrder = Regex.IsMatch(lines[range.Start], "^[^(]*[=]"); // ? 初期化有りフィールド

			// シャッフル
			{
				List<SMO_RangeInfo> hoRanges = new List<SMO_RangeInfo>();
				List<SMO_RangeInfo> unhoRanges = new List<SMO_RangeInfo>();

				foreach (SMO_RangeInfo range in ranges)
				{
					if (range.HoldingOrder)
						hoRanges.Add(range);
					else
						unhoRanges.Add(range);
				}
				ranges = hoRanges;
				hoRanges = null;

				foreach (SMO_RangeInfo range in unhoRanges)
					ranges.Insert(SCommon.CRandom.GetInt(ranges.Count + 1), range);
			}

			// ファイルの先頭
			ranges.Insert(0, new SMO_RangeInfo()
			{
				Start = 0,
				End = start,
			});

			// ファイルの終端
			ranges.Add(new SMO_RangeInfo()
			{
				Start = end,
				End = lines.Length,
			});

			File.WriteAllLines(_file, SMO_GetOrderLines(lines, ranges), Encoding.UTF8);
		}

		private static int SMO_GetClassEnd(string[] lines)
		{
			for (int index = lines.Length - 1; 0 <= index; index--)
				if (lines[index].StartsWith("\t}")) // ? クラス閉じ
					return index;

			return -1; // not found
		}

		private static IEnumerable<string> SMO_GetOrderLines(string[] lines, IEnumerable<SMO_RangeInfo> ranges)
		{
			foreach (SMO_RangeInfo range in ranges)
			{
				yield return "// HO=" + range.HoldingOrder;

				for (int index = range.Start; index < range.End; index++)
					yield return lines[index];
			}
		}

		/// <summary>
		/// ビルドに不要な情報を除去する。
		/// -- 万が一出力実行ファイルに情報が含まれていると困るので、念のため実行する。
		/// ---- 恐らく実行しなくても良い。
		/// </summary>
		public void RemoveUnnecessaryInformations()
		{
			string[] lines = File.ReadAllLines(_file, Encoding.UTF8);

			lines = lines
				.Where(line => line != "") // ? 空行ではない。
				.Where(line => !line.StartsWith("//")) // ? 行頭から始まるC++系コメント行ではない。
				.Select(line => RUI_RemoveCPPComment(line))
				.ToArray();

			File.WriteAllLines(_file, lines, Encoding.UTF8);

			this.RUI_Formatting();
		}

		/// <summary>
		/// キープされたC++コメントがこの時点まで残っているはず。
		/// これを除去する。
		/// </summary>
		/// <param name="line">行(入力)</param>
		/// <returns>行(出力)</returns>
		private static string RUI_RemoveCPPComment(string line)
		{
			// プリプロセッサの除去、Cコメントの除去、文字と文字列のエスケープ etc. は完了しているので、単純な検索で良いはず。

			int index = line.IndexOf("//");

			if (index != -1)
			{
				line = line.Substring(0, index);
				line = line.TrimEnd();
			}
			return line;
		}

		/// <summary>
		/// ソースコードの整形
		/// -- 尚更実行しなくても良い処理
		/// </summary>
		private void RUI_Formatting()
		{
			{
				string text = File.ReadAllText(_file, Encoding.UTF8);

				text = text.Replace(" {", Consts.CRLF + "\t\t{");
				text = text.Replace(" }", Consts.CRLF + "\t\t}");
				text = text.Replace("{ ", "{" + Consts.CRLF);
				text = text.Replace("} ", "}" + Consts.CRLF);
				//text = text.Replace("; ", ";" + Consts.CRLF);
				text = text.Replace("; yield return ", ";" + Consts.CRLF + "yield return ");

				// "\t\tconst int" を ("const" -> "") の置き換えを実施して "\t\t int" になった場合など
				//
				text = text.Replace("\t ", "\t");

				File.WriteAllText(_file, text, Encoding.UTF8);
			}

			{
				IEnumerable<string> lines = File.ReadAllLines(_file, Encoding.UTF8);

				lines = lines.Select(line =>
				{
					if (
						!line.StartsWith("using ") &&
						!line.StartsWith("namespace ") &&
						line != "{" &&
						line != "}" &&
						line != "" && // ? 空行ではない。
						line[0] != '\t' // ? インデント無し
						)
						line = "\t\t\t" + line;

					return line;
				});

				lines = RUI_F_P1_GetOutputLines(lines.ToArray());
				lines = RUI_F_P2A_GetOutputLines(lines.ToArray());
				lines = RUI_F_P2B_GetOutputLines(lines.ToArray());
				lines = RUI_F_P3_GetOutputLines(lines); // クラス・メソッド etc. の summary コメント

				File.WriteAllLines(_file, lines, Encoding.UTF8);
			}
		}

		private static IEnumerable<string> RUI_F_P1_GetOutputLines(string[] lines)
		{
			for (int index = 0; index < lines.Length; index++)
			{
				if (
					lines[index].StartsWith("namespace ") ||
					1 <= index &&
					lines[index - 1].Trim() != "{" &&
					(
						lines[index].Trim().StartsWith("public ") ||
						lines[index].Trim().StartsWith("protected ") ||
						lines[index].Trim().StartsWith("private ")
					))
					yield return "";

				yield return lines[index];
			}
		}

		private static IEnumerable<string> RUI_F_P2A_GetOutputLines(string[] lines)
		{
			for (int index = 0; index < lines.Length; index++)
			{
				if (
					1 <= index &&
					Common.GetIndentDepth(lines[index]) < Common.GetIndentDepth(lines[index - 1]) &&
					lines[index - 1].Trim() != "" &&
					lines[index - 0].Trim() != "" &&
					lines[index - 0].Trim() != "else" &&
					!lines[index].Trim().StartsWith("else ") &&
					Common.IsCSWordChar(lines[index - 1].Trim()[0]) &&
					Common.IsCSWordChar(lines[index - 0].Trim()[0])
					)
					yield return "";

				yield return lines[index];
			}
		}

		private static IEnumerable<string> RUI_F_P2B_GetOutputLines(string[] lines)
		{
			for (int index = 0; index < lines.Length; index++)
			{
				if (
					1 <= index &&
					Common.GetIndentDepth(lines[index]) == Common.GetIndentDepth(lines[index - 1]) &&
					lines[index - 1].Trim() != "" &&
					lines[index - 0].Trim() != "" &&
					Common.IsCSWordChar(lines[index - 1].Trim()[0]) &&
					Common.IsCSWordChar(lines[index - 0].Trim()[0]) &&
					(
						lines[index].Trim().StartsWith("if (") ||
						lines[index].Trim().StartsWith("for (") ||
						lines[index].Trim().StartsWith("foreach (") ||
						lines[index].Trim().StartsWith("switch (") ||
						lines[index].Trim().StartsWith("using (") ||
						lines[index].Trim().StartsWith("while (")
					))
					yield return "";

				yield return lines[index];
			}
		}

		//private static string RUI_F_P3_GOL_CommentMessage = null;

		private static IEnumerable<string> RUI_F_P3_GetOutputLines(IEnumerable<string> lines)
		{
			//if (RUI_F_P3_GOL_CommentMessage == null)
			//    RUI_F_P3_GOL_CommentMessage = "Confused by ConfuserElsa @ " + DateTime.Now;

			string commentMessage = "Confused by ConfuserElsa";
			//string commentMessage = RUI_F_P3_GOL_CommentMessage;

			foreach (string line in lines)
			{
				if (line.StartsWith("\tpublic ")) // ? クラス || 構造体
				{
					yield return "\t/// <summary>";

#if true
					yield return "\t/// " + commentMessage;
#else
					//for (int c = SCommon.CRandom.GetRange(3, 13); 0 < c; c--)
					for (int c = SCommon.CRandom.GetRange(3, 7); 0 < c; c--)
						yield return "\t/// " + RUI_F_P3_GOL_MakeCommentLine();
#endif

					yield return "\t/// </summary>";
				}
				else if (line.StartsWith("\t\tpublic ")) // ? メソッド || フィールド || プロパティ || サブクラス
				{
					yield return "\t\t/// <summary>";

#if true
					yield return "\t\t/// " + commentMessage;
#else
					//for (int c = SCommon.CRandom.GetRange(1, 7); 0 < c; c--)
					for (int c = SCommon.CRandom.GetRange(1, 5); 0 < c; c--)
						yield return "\t\t/// " + RUI_F_P3_GOL_MakeCommentLine();
#endif

					yield return "\t\t/// </summary>";
				}
				yield return line;
			}
		}

		private static string[] RUI_F_P3_GOL_MCL_Words = SCommon.TextToLines(CSResources.英単語リスト)
			.Select(v => v.Trim())
			.Where(v => v != "" && v[0] != ';') // ? 空行ではない && コメント行ではない
			.Select(v => v.Substring(0, v.IndexOf('\t'))) // 品詞の部分を除去
			.ToArray();

		private static string RUI_F_P3_GOL_MakeCommentLine()
		{
			//string[] tokens = new string[SCommon.CRandom.GetRange(7, 17)];
			string[] tokens = new string[SCommon.CRandom.GetRange(7, 13)];

			for (int index = 0; index < tokens.Length; index++)
				tokens[index] = SCommon.CRandom.ChooseOne(RUI_F_P3_GOL_MCL_Words);

			string line = string.Join(", ", tokens);
			line = new string(line.ToCharArray().Where(chr => chr != ',' || SCommon.CRandom.GetInt(10) == 0).ToArray());
			return line.Substring(0, 1).ToUpper() + line.Substring(1).ToLower() + ".";
		}
	}
}
