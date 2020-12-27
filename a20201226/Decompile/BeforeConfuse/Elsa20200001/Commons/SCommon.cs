using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Charlotte.Commons
{
	// Token: 0x020000AD RID: 173
	public static class SCommon
	{
		// Token: 0x06000324 RID: 804 RVA: 0x00011F84 File Offset: 0x00010184
		public static IDisposable GetAnonyDisposable(Action routine)
		{
			return new SCommon.S_AnonyDisposable(routine);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00011F8C File Offset: 0x0001018C
		public static int Comp<T>(T[] a, T[] b, Comparison<T> comp)
		{
			int minlen = Math.Min(a.Length, b.Length);
			for (int index = 0; index < minlen; index++)
			{
				int ret = comp(a[index], b[index]);
				if (ret != 0)
				{
					return ret;
				}
			}
			return SCommon.Comp(a.Length, b.Length);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00011FD8 File Offset: 0x000101D8
		public static int IndexOf<T>(T[] arr, Predicate<T> match)
		{
			for (int index = 0; index < arr.Length; index++)
			{
				if (match(arr[index]))
				{
					return index;
				}
			}
			return -1;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00012008 File Offset: 0x00010208
		public static void Swap<T>(T[] arr, int a, int b)
		{
			T tmp = arr[a];
			arr[a] = arr[b];
			arr[b] = tmp;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00012033 File Offset: 0x00010233
		public static int Comp(byte a, byte b)
		{
			if (a < b)
			{
				return -1;
			}
			if (a > b)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00012042 File Offset: 0x00010242
		public static int Comp(byte[] a, byte[] b)
		{
			return SCommon.Comp<byte>(a, b, new Comparison<byte>(SCommon.Comp));
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00012058 File Offset: 0x00010258
		public static byte[] GetSubBytes(byte[] src, int offset, int size)
		{
			byte[] dest = new byte[size];
			Array.Copy(src, offset, dest, 0, size);
			return dest;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00012077 File Offset: 0x00010277
		public static byte[] ToBytes(int value)
		{
			return SCommon.ToBytes((uint)value);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0001207F File Offset: 0x0001027F
		public static int ToInt(byte[] src, int index = 0)
		{
			return (int)SCommon.ToUInt(src, index);
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00012088 File Offset: 0x00010288
		public static byte[] ToBytes(uint value)
		{
			byte[] dest = new byte[4];
			SCommon.ToBytes(value, dest, 0);
			return dest;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x000120A5 File Offset: 0x000102A5
		public static void ToBytes(uint value, byte[] dest, int index = 0)
		{
			dest[index] = (byte)(value & 255u);
			dest[index + 1] = (byte)(value >> 8 & 255u);
			dest[index + 2] = (byte)(value >> 16 & 255u);
			dest[index + 3] = (byte)(value >> 24 & 255u);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000120E1 File Offset: 0x000102E1
		public static uint ToUInt(byte[] src, int index = 0)
		{
			return (uint)((int)src[index] | (int)src[index + 1] << 8 | (int)src[index + 2] << 16 | (int)src[index + 3] << 24);
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00012100 File Offset: 0x00010300
		public static byte[] Join(byte[][] src)
		{
			int offset = 0;
			foreach (byte block in src)
			{
				offset += block.Length;
			}
			byte[] dest = new byte[offset];
			offset = 0;
			foreach (byte block2 in src)
			{
				Array.Copy(block2, 0, dest, offset, block2.Length);
				offset += block2.Length;
			}
			return dest;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00012160 File Offset: 0x00010360
		public static byte[] SplittableJoin(byte[][] src)
		{
			int offset = 0;
			foreach (byte block in src)
			{
				offset += 4 + block.Length;
			}
			byte[] dest = new byte[offset];
			offset = 0;
			foreach (byte block2 in src)
			{
				Array.Copy(SCommon.ToBytes(block2.Length), 0, dest, offset, 4);
				offset += 4;
				Array.Copy(block2, 0, dest, offset, block2.Length);
				offset += block2.Length;
			}
			return dest;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000121D8 File Offset: 0x000103D8
		public static byte[][] Split(byte[] src)
		{
			List<byte[]> dest = new List<byte[]>();
			int size;
			for (int offset = 0; offset < src.Length; offset += size)
			{
				size = SCommon.ToInt(src, offset);
				offset += 4;
				dest.Add(SCommon.GetSubBytes(src, offset, size));
			}
			return dest.ToArray();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00012219 File Offset: 0x00010419
		public static Dictionary<string, V> CreateDictionary<V>()
		{
			return new Dictionary<string, V>(new SCommon.IECompString());
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00012225 File Offset: 0x00010425
		public static Dictionary<string, V> CreateDictionaryIgnoreCase<V>()
		{
			return new Dictionary<string, V>(new SCommon.IECompStringIgnoreCase());
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00012231 File Offset: 0x00010431
		private static void CheckNaN(double value)
		{
			if (double.IsNaN(value))
			{
				throw new Exception("NaN");
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00012246 File Offset: 0x00010446
		public static double ToRange(double value, double minval, double maxval)
		{
			SCommon.CheckNaN(value);
			return Math.Max(minval, Math.Min(maxval, value));
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0001225B File Offset: 0x0001045B
		public static int ToInt(double value)
		{
			SCommon.CheckNaN(value);
			if (value < 0.0)
			{
				return (int)(value - 0.5);
			}
			return (int)(value + 0.5);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00012288 File Offset: 0x00010488
		public static long ToLong(double value)
		{
			SCommon.CheckNaN(value);
			if (value < 0.0)
			{
				return (long)(value - 0.5);
			}
			return (long)(value + 0.5);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000122B5 File Offset: 0x000104B5
		public static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> src)
		{
			foreach (IEnumerable<T> part in src)
			{
				foreach (T element in part)
				{
					yield return element;
				}
				IEnumerator<T> enumerator2 = null;
			}
			IEnumerator<IEnumerable<T>> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600033A RID: 826 RVA: 0x000122C5 File Offset: 0x000104C5
		public static IEnumerable<T> Sort<T>(IEnumerable<T> src, Comparison<T> comp)
		{
			T[] array = src.ToArray<T>();
			Array.Sort<T>(array, comp);
			return array;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000122D4 File Offset: 0x000104D4
		public static Func<T> Supplier<T>(IEnumerable<T> src)
		{
			IEnumerator<T> reader = src.GetEnumerator();
			return delegate()
			{
				if (reader != null)
				{
					if (reader.MoveNext())
					{
						return reader.Current;
					}
					reader.Dispose();
					reader = null;
				}
				return default(T);
			};
		}

		// Token: 0x0600033C RID: 828 RVA: 0x000122F2 File Offset: 0x000104F2
		public static T DesertElement<T>(List<T> list, int index)
		{
			T result = list[index];
			list.RemoveAt(index);
			return result;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00012302 File Offset: 0x00010502
		public static T UnaddElement<T>(List<T> list)
		{
			return SCommon.DesertElement<T>(list, list.Count - 1);
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00012314 File Offset: 0x00010514
		public static T FastDesertElement<T>(List<T> list, int index)
		{
			T ret = SCommon.UnaddElement<T>(list);
			if (index < list.Count)
			{
				T t = list[index];
				list[index] = ret;
				ret = t;
			}
			return ret;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00012344 File Offset: 0x00010544
		public static void DeletePath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("削除しようとしたパスは null 又は空文字列です。");
			}
			if (File.Exists(path))
			{
				int c = 1;
				for (;;)
				{
					try
					{
						File.Delete(path);
					}
					catch (Exception e)
					{
						Action<object> writeLog = ProcMain.WriteLog;
						Exception ex = e;
						writeLog(((ex != null) ? ex.ToString() : null) + " <---- 例外ここまで、処理を続行します。");
					}
					if (!File.Exists(path))
					{
						return;
					}
					if (10 < c)
					{
						break;
					}
					ProcMain.WriteLog("ファイルの削除をリトライします。" + path);
					Thread.Sleep(c * 100);
					c++;
				}
				throw new Exception("ファイルの削除に失敗しました。" + path);
			}
			if (Directory.Exists(path))
			{
				int c2 = 1;
				for (;;)
				{
					try
					{
						Directory.Delete(path, true);
					}
					catch (Exception e2)
					{
						Action<object> writeLog2 = ProcMain.WriteLog;
						Exception ex2 = e2;
						writeLog2(((ex2 != null) ? ex2.ToString() : null) + " <---- 例外ここまで、処理を続行します。");
					}
					if (!Directory.Exists(path))
					{
						return;
					}
					if (10 < c2)
					{
						break;
					}
					ProcMain.WriteLog("ディレクトリの削除をリトライします。" + path);
					Thread.Sleep(c2 * 100);
					c2++;
				}
				throw new Exception("ディレクトリの削除に失敗しました。" + path);
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00012478 File Offset: 0x00010678
		public static void CreateDir(string dir)
		{
			if (string.IsNullOrEmpty(dir))
			{
				throw new Exception("作成しようとしたディレクトリは null 又は空文字列です。");
			}
			int c = 1;
			for (;;)
			{
				try
				{
					Directory.CreateDirectory(dir);
				}
				catch (Exception e)
				{
					Action<object> writeLog = ProcMain.WriteLog;
					Exception ex = e;
					writeLog(((ex != null) ? ex.ToString() : null) + " <---- 例外ここまで、処理を続行します。");
				}
				if (Directory.Exists(dir))
				{
					return;
				}
				if (10 < c)
				{
					break;
				}
				ProcMain.WriteLog("ディレクトリの作成をリトライします。" + dir);
				Thread.Sleep(c * 100);
				c++;
			}
			throw new Exception("ディレクトリを作成出来ません。" + dir);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0001251C File Offset: 0x0001071C
		public static string ChangeRoot(string path, string oldRoot)
		{
			oldRoot = SCommon.PutYen(oldRoot);
			if (!SCommon.StartsWithIgnoreCase(path, oldRoot))
			{
				throw new Exception("パスの配下ではありません。" + oldRoot + " -> " + path);
			}
			return path.Substring(oldRoot.Length);
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00012552 File Offset: 0x00010752
		public static string PutYen(string path)
		{
			return SCommon.Put_INE(path, "\\");
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0001255F File Offset: 0x0001075F
		private static string Put_INE(string str, string endPtn)
		{
			if (!str.EndsWith(endPtn))
			{
				str += endPtn;
			}
			return str;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00012574 File Offset: 0x00010774
		public static string MakeFullPath(string path)
		{
			if (path == null)
			{
				throw new Exception("パスが定義されていません。(null)");
			}
			if (path == "")
			{
				throw new Exception("パスが定義されていません。(空文字列)");
			}
			path = Path.GetFullPath(path);
			if (path.Contains('/'))
			{
				throw null;
			}
			if (path.StartsWith("\\\\"))
			{
				throw new Exception("ネットワークパスまたはデバイス名は使用出来ません。");
			}
			if (path.Substring(1, 2) != ":\\")
			{
				throw null;
			}
			path = SCommon.PutYen(path) + ".";
			path = Path.GetFullPath(path);
			return path;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00012604 File Offset: 0x00010804
		public static string ToFullPath(string path)
		{
			path = Path.GetFullPath(path);
			path = SCommon.PutYen(path) + ".";
			path = Path.GetFullPath(path);
			return path;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0001262C File Offset: 0x0001082C
		public static byte[] Read(FileStream reader, int size)
		{
			byte[] buff = new byte[size];
			SCommon.Read(reader, buff, 0);
			return buff;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00012649 File Offset: 0x00010849
		public static void Read(FileStream reader, byte[] buff, int offset = 0)
		{
			SCommon.Read(reader, buff, offset, buff.Length - offset);
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00012658 File Offset: 0x00010858
		public static void Read(FileStream reader, byte[] buff, int offset, int count)
		{
			if (reader.Read(buff, offset, count) != count)
			{
				throw new Exception("データの途中でファイルの終端に到達しました。");
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00012671 File Offset: 0x00010871
		public static string LinesToText(string[] lines)
		{
			if (lines.Length != 0)
			{
				return string.Join("\r\n", lines) + "\r\n";
			}
			return "";
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00012694 File Offset: 0x00010894
		public static string[] TextToLines(string text)
		{
			text = text.Replace("\r", "");
			string[] lines = text.Split(new char[]
			{
				'\n'
			});
			if (1 <= lines.Length && lines[lines.Length - 1] == "")
			{
				lines = new List<string>(lines).GetRange(0, lines.Length - 1).ToArray();
			}
			return lines;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x000126F8 File Offset: 0x000108F8
		public static void ReadToEnd(SCommon.Read_d reader, SCommon.Write_d writer)
		{
			byte[] buff = new byte[16777216];
			for (;;)
			{
				int readSize = reader(buff, 0, buff.Length);
				if (readSize < 0)
				{
					break;
				}
				writer(buff, 0, readSize);
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00012033 File Offset: 0x00010233
		public static int Comp(int a, int b)
		{
			if (a < b)
			{
				return -1;
			}
			if (a > b)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00012033 File Offset: 0x00010233
		public static int Comp(long a, long b)
		{
			if (a < b)
			{
				return -1;
			}
			if (a > b)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001272B File Offset: 0x0001092B
		public static int ToRange(int value, int minval, int maxval)
		{
			return Math.Max(minval, Math.Min(maxval, value));
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001273A File Offset: 0x0001093A
		public static long ToRange(long value, long minval, long maxval)
		{
			return Math.Max(minval, Math.Min(maxval, value));
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00012749 File Offset: 0x00010949
		public static bool IsRange(int value, int minval, int maxval)
		{
			return minval <= value && value <= maxval;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00012749 File Offset: 0x00010949
		public static bool IsRange(long value, long minval, long maxval)
		{
			return minval <= value && value <= maxval;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00012758 File Offset: 0x00010958
		public static int ToInt(string str, int minval, int maxval, int defval)
		{
			int result;
			try
			{
				int value = int.Parse(str);
				if (value < minval || maxval < value)
				{
					throw null;
				}
				result = value;
			}
			catch
			{
				result = defval;
			}
			return result;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00012790 File Offset: 0x00010990
		public static long ToLong(string str, long minval, long maxval, long defval)
		{
			long result;
			try
			{
				long value = long.Parse(str);
				if (value < minval || maxval < value)
				{
					throw null;
				}
				result = value;
			}
			catch
			{
				result = defval;
			}
			return result;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000127C8 File Offset: 0x000109C8
		public static string ToJString(byte[] src, bool okJpn, bool okRet, bool okTab, bool okSpc)
		{
			if (src == null)
			{
				src = new byte[0];
			}
			string @string;
			using (MemoryStream dest = new MemoryStream())
			{
				int index = 0;
				while (index < src.Length)
				{
					byte chr = src[index];
					if (chr == 9)
					{
						if (okTab)
						{
							goto IL_7F;
						}
					}
					else if (chr == 10)
					{
						if (okRet)
						{
							goto IL_7F;
						}
					}
					else if (chr >= 32)
					{
						if (chr == 32)
						{
							if (okSpc)
							{
								goto IL_7F;
							}
						}
						else
						{
							if (chr <= 126)
							{
								goto IL_7F;
							}
							if (161 <= chr && chr <= 223)
							{
								if (okJpn)
								{
									goto IL_7F;
								}
							}
							else if (okJpn)
							{
								index++;
								if (src.Length <= index)
								{
									break;
								}
								if (SCommon.S_JChar.I.Contains(chr, src[index]))
								{
									dest.WriteByte(chr);
									chr = src[index];
									goto IL_7F;
								}
							}
						}
					}
					IL_86:
					index++;
					continue;
					IL_7F:
					dest.WriteByte(chr);
					goto IL_86;
				}
				@string = SCommon.ENCODING_SJIS.GetString(dest.ToArray());
			}
			return @string;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00012894 File Offset: 0x00010A94
		public static byte[] GetSHA512(byte[] src)
		{
			byte[] result;
			using (SHA512 sha512 = SHA512.Create())
			{
				result = sha512.ComputeHash(src);
			}
			return result;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x000128CC File Offset: 0x00010ACC
		private static string S_GetString_SJISHalfCodeRange(int codeMin, int codeMax)
		{
			byte[] buff = new byte[codeMax - codeMin + 1];
			for (int code = codeMin; code <= codeMax; code++)
			{
				buff[code - codeMin] = (byte)code;
			}
			return SCommon.ENCODING_SJIS.GetString(buff);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00012904 File Offset: 0x00010B04
		private static string S_GetString_SJISCodeRange(int lead, int trailMin, int trailMax)
		{
			byte[] buff = new byte[(trailMax - trailMin + 1) * 2];
			for (int trail = trailMin; trail <= trailMax; trail++)
			{
				buff[(trail - trailMin) * 2] = (byte)lead;
				buff[(trail - trailMin) * 2 + 1] = (byte)trail;
			}
			return SCommon.ENCODING_SJIS.GetString(buff);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00012949 File Offset: 0x00010B49
		public static int Comp(string a, string b)
		{
			return SCommon.Comp(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00012966 File Offset: 0x00010B66
		public static int CompIgnoreCase(string a, string b)
		{
			return SCommon.Comp(a.ToLower(), b.ToLower());
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00012979 File Offset: 0x00010B79
		public static bool EqualsIgnoreCase(string a, string b)
		{
			return a.ToLower() == b.ToLower();
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001298C File Offset: 0x00010B8C
		public static bool StartsWithIgnoreCase(string str, string ptn)
		{
			return str.ToLower().StartsWith(ptn.ToLower());
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0001299F File Offset: 0x00010B9F
		public static bool EndsWithIgnoreCase(string str, string ptn)
		{
			return str.ToLower().EndsWith(ptn.ToLower());
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000129B2 File Offset: 0x00010BB2
		public static bool ContainsIgnoreCase(string str, string ptn)
		{
			return str.ToLower().Contains(ptn.ToLower());
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000129C5 File Offset: 0x00010BC5
		public static int IndexOfIgnoreCase(string str, string ptn)
		{
			return str.ToLower().IndexOf(ptn.ToLower());
		}

		// Token: 0x0600035F RID: 863 RVA: 0x000129D8 File Offset: 0x00010BD8
		public static int IndexOfIgnoreCase(string str, char chr)
		{
			return str.ToLower().IndexOf(char.ToLower(chr));
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000129EC File Offset: 0x00010BEC
		public static int IndexOf(string[] strs, string str)
		{
			for (int index = 0; index < strs.Length; index++)
			{
				if (strs[index] == str)
				{
					return index;
				}
			}
			return -1;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00012A18 File Offset: 0x00010C18
		public static int IndexOfIgnoreCase(string[] strs, string str)
		{
			string lStr = str.ToLower();
			for (int index = 0; index < strs.Length; index++)
			{
				if (strs[index].ToLower() == lStr)
				{
					return index;
				}
			}
			return -1;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00012A50 File Offset: 0x00010C50
		public static string[] Tokenize(string str, string delimiters, bool meaningFlag = false, bool ignoreEmpty = false, int limit = 0)
		{
			StringBuilder buff = new StringBuilder();
			List<string> tokens = new List<string>();
			foreach (char chr in str)
			{
				if (tokens.Count + 1 == limit || delimiters.Contains(chr) == meaningFlag)
				{
					buff.Append(chr);
				}
				else
				{
					if (!ignoreEmpty || buff.Length != 0)
					{
						tokens.Add(buff.ToString());
					}
					buff = new StringBuilder();
				}
			}
			if (!ignoreEmpty || buff.Length != 0)
			{
				tokens.Add(buff.ToString());
			}
			return tokens.ToArray();
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00012AE4 File Offset: 0x00010CE4
		public static bool HasSameChar(string str)
		{
			for (int r = 1; r < str.Length; r++)
			{
				for (int i = 0; i < r; i++)
				{
					if (str[i] == str[r])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00012B24 File Offset: 0x00010D24
		public static byte[] Compress(byte[] src)
		{
			byte[] result;
			using (MemoryStream reader = new MemoryStream(src))
			{
				using (MemoryStream writer = new MemoryStream())
				{
					SCommon.Compress(reader, writer);
					result = writer.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00012B80 File Offset: 0x00010D80
		public static byte[] Decompress(byte[] src, int limit = -1)
		{
			byte[] result;
			using (MemoryStream reader = new MemoryStream(src))
			{
				using (MemoryStream writer = new MemoryStream())
				{
					SCommon.Decompress(reader, writer, (long)limit);
					result = writer.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00012BE0 File Offset: 0x00010DE0
		public static void Compress(string rFile, string wFile)
		{
			using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
			{
				using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
				{
					SCommon.Compress(reader, writer);
				}
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00012C38 File Offset: 0x00010E38
		public static void Decompress(string rFile, string wFile, long limit = -1L)
		{
			using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
			{
				using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
				{
					SCommon.Decompress(reader, writer, limit);
				}
			}
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00012C94 File Offset: 0x00010E94
		public static void Compress(Stream reader, Stream writer)
		{
			using (GZipStream gz = new GZipStream(writer, CompressionMode.Compress))
			{
				reader.CopyTo(gz);
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00012CCC File Offset: 0x00010ECC
		public static void Decompress(Stream reader, Stream writer, long limit = -1L)
		{
			using (GZipStream gz = new GZipStream(reader, CompressionMode.Decompress))
			{
				if (limit == -1L)
				{
					gz.CopyTo(writer);
				}
				else
				{
					SCommon.ReadToEnd(new SCommon.Read_d(gz.Read), SCommon.GetLimitedWriter(new SCommon.Write_d(writer.Write), limit));
				}
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00012D30 File Offset: 0x00010F30
		public static SCommon.Write_d GetLimitedWriter(SCommon.Write_d writer, long remaining)
		{
			return delegate(byte[] buff, int offset, int count)
			{
				if (remaining < (long)count)
				{
					throw new Exception("ストリームに書き込めるバイト数の上限を超えようとしました。");
				}
				remaining -= (long)count;
				writer(buff, offset, count);
			};
		}

		// Token: 0x0400024C RID: 588
		public static byte[] EMPTY_BYTES = new byte[0];

		// Token: 0x0400024D RID: 589
		public const double MICRO = 1E-09;

		// Token: 0x0400024E RID: 590
		public const int IMAX = 1000000000;

		// Token: 0x0400024F RID: 591
		public const long IMAX_64 = 1000000000000000000L;

		// Token: 0x04000250 RID: 592
		public static RandomUnit CRandom = new RandomUnit(new SCommon.S_CSPRandomNumberGenerator());

		// Token: 0x04000251 RID: 593
		public static string[] EMPRY_STRINGS = new string[0];

		// Token: 0x04000252 RID: 594
		public static Encoding ENCODING_SJIS = Encoding.GetEncoding(932);

		// Token: 0x04000253 RID: 595
		public static string BINADECIMAL = "01";

		// Token: 0x04000254 RID: 596
		public static string OCTODECIMAL = "012234567";

		// Token: 0x04000255 RID: 597
		public static string DECIMAL = "0123456789";

		// Token: 0x04000256 RID: 598
		public static string HEXADECIMAL = "0123456789ABCDEF";

		// Token: 0x04000257 RID: 599
		public static string hexadecimal = "0123456789abcdef";

		// Token: 0x04000258 RID: 600
		public static string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		// Token: 0x04000259 RID: 601
		public static string alpha = "abcdefghijklmnopqrstuvwxyz";

		// Token: 0x0400025A RID: 602
		public static string PUNCT = SCommon.S_GetString_SJISHalfCodeRange(33, 47) + SCommon.S_GetString_SJISHalfCodeRange(58, 64) + SCommon.S_GetString_SJISHalfCodeRange(91, 96) + SCommon.S_GetString_SJISHalfCodeRange(123, 126);

		// Token: 0x0400025B RID: 603
		public static string ASCII = SCommon.DECIMAL + SCommon.ALPHA + SCommon.alpha + SCommon.PUNCT;

		// Token: 0x0400025C RID: 604
		public static string KANA = SCommon.S_GetString_SJISHalfCodeRange(161, 223);

		// Token: 0x0400025D RID: 605
		public static string HALF = SCommon.ASCII + SCommon.KANA;

		// Token: 0x0400025E RID: 606
		public static string MBC_DECIMAL = SCommon.S_GetString_SJISCodeRange(130, 79, 88);

		// Token: 0x0400025F RID: 607
		public static string MBC_ALPHA = SCommon.S_GetString_SJISCodeRange(130, 96, 121);

		// Token: 0x04000260 RID: 608
		public static string mbc_alpha = SCommon.S_GetString_SJISCodeRange(130, 129, 154);

		// Token: 0x04000261 RID: 609
		public static string MBC_SPACE = SCommon.S_GetString_SJISCodeRange(129, 64, 64);

		// Token: 0x04000262 RID: 610
		public static string MBC_PUNCT = string.Concat(new string[]
		{
			SCommon.S_GetString_SJISCodeRange(129, 65, 126),
			SCommon.S_GetString_SJISCodeRange(129, 128, 172),
			SCommon.S_GetString_SJISCodeRange(129, 184, 191),
			SCommon.S_GetString_SJISCodeRange(129, 200, 206),
			SCommon.S_GetString_SJISCodeRange(129, 218, 232),
			SCommon.S_GetString_SJISCodeRange(129, 240, 247),
			SCommon.S_GetString_SJISCodeRange(129, 252, 252),
			SCommon.S_GetString_SJISCodeRange(131, 159, 182),
			SCommon.S_GetString_SJISCodeRange(131, 191, 214),
			SCommon.S_GetString_SJISCodeRange(132, 64, 96),
			SCommon.S_GetString_SJISCodeRange(132, 112, 126),
			SCommon.S_GetString_SJISCodeRange(132, 128, 145),
			SCommon.S_GetString_SJISCodeRange(132, 159, 190),
			SCommon.S_GetString_SJISCodeRange(135, 64, 93),
			SCommon.S_GetString_SJISCodeRange(135, 95, 117),
			SCommon.S_GetString_SJISCodeRange(135, 126, 126),
			SCommon.S_GetString_SJISCodeRange(135, 128, 156),
			SCommon.S_GetString_SJISCodeRange(238, 239, 252)
		});

		// Token: 0x04000263 RID: 611
		public static string MBC_CHOUONPU = SCommon.S_GetString_SJISCodeRange(129, 91, 91);

		// Token: 0x04000264 RID: 612
		public static string MBC_HIRA = SCommon.S_GetString_SJISCodeRange(130, 159, 241);

		// Token: 0x04000265 RID: 613
		public static string MBC_KANA = SCommon.S_GetString_SJISCodeRange(131, 64, 126) + SCommon.S_GetString_SJISCodeRange(131, 128, 150) + SCommon.MBC_CHOUONPU;

		// Token: 0x02000153 RID: 339
		private class S_AnonyDisposable : IDisposable
		{
			// Token: 0x060006AC RID: 1708 RVA: 0x000224D2 File Offset: 0x000206D2
			public S_AnonyDisposable(Action routine)
			{
				this.Routine = routine;
			}

			// Token: 0x060006AD RID: 1709 RVA: 0x000224E1 File Offset: 0x000206E1
			public void Dispose()
			{
				if (this.Routine != null)
				{
					this.Routine();
					this.Routine = null;
				}
			}

			// Token: 0x04000552 RID: 1362
			private Action Routine;
		}

		// Token: 0x02000154 RID: 340
		// (Invoke) Token: 0x060006AF RID: 1711
		public delegate int Read_d(byte[] buff, int offset, int count);

		// Token: 0x02000155 RID: 341
		// (Invoke) Token: 0x060006B3 RID: 1715
		public delegate void Write_d(byte[] buff, int offset, int count);

		// Token: 0x02000156 RID: 342
		private class S_JChar
		{
			// Token: 0x170000C5 RID: 197
			// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000224FD File Offset: 0x000206FD
			public static SCommon.S_JChar I
			{
				get
				{
					if (SCommon.S_JChar._i == null)
					{
						SCommon.S_JChar._i = new SCommon.S_JChar();
					}
					return SCommon.S_JChar._i;
				}
			}

			// Token: 0x060006B7 RID: 1719 RVA: 0x00022515 File Offset: 0x00020715
			private S_JChar()
			{
				this.Add();
			}

			// Token: 0x060006B8 RID: 1720 RVA: 0x00022534 File Offset: 0x00020734
			private void Add()
			{
				this.Add(33088, 33150);
				this.Add(33152, 33196);
				this.Add(33208, 33215);
				this.Add(33224, 33230);
				this.Add(33242, 33256);
				this.Add(33264, 33271);
				this.Add(33276, 33276);
				this.Add(33359, 33368);
				this.Add(33376, 33401);
				this.Add(33409, 33434);
				this.Add(33439, 33521);
				this.Add(33600, 33662);
				this.Add(33664, 33686);
				this.Add(33695, 33718);
				this.Add(33727, 33750);
				this.Add(33856, 33888);
				this.Add(33904, 33918);
				this.Add(33920, 33937);
				this.Add(33951, 33982);
				this.Add(34624, 34653);
				this.Add(34655, 34677);
				this.Add(34686, 34686);
				this.Add(34688, 34716);
				this.Add(34975, 35068);
				this.Add(35136, 35198);
				this.Add(35200, 35324);
				this.Add(35392, 35454);
				this.Add(35456, 35580);
				this.Add(35648, 35710);
				this.Add(35712, 35836);
				this.Add(35904, 35966);
				this.Add(35968, 36092);
				this.Add(36160, 36222);
				this.Add(36224, 36348);
				this.Add(36416, 36478);
				this.Add(36480, 36604);
				this.Add(36672, 36734);
				this.Add(36736, 36860);
				this.Add(36928, 36990);
				this.Add(36992, 37116);
				this.Add(37184, 37246);
				this.Add(37248, 37372);
				this.Add(37440, 37502);
				this.Add(37504, 37628);
				this.Add(37696, 37758);
				this.Add(37760, 37884);
				this.Add(37952, 38014);
				this.Add(38016, 38140);
				this.Add(38208, 38270);
				this.Add(38272, 38396);
				this.Add(38464, 38526);
				this.Add(38528, 38652);
				this.Add(38720, 38782);
				this.Add(38784, 38908);
				this.Add(38976, 39026);
				this.Add(39071, 39164);
				this.Add(39232, 39294);
				this.Add(39296, 39420);
				this.Add(39488, 39550);
				this.Add(39552, 39676);
				this.Add(39744, 39806);
				this.Add(39808, 39932);
				this.Add(40000, 40062);
				this.Add(40064, 40188);
				this.Add(40256, 40318);
				this.Add(40320, 40444);
				this.Add(40512, 40574);
				this.Add(40576, 40700);
				this.Add(40768, 40830);
				this.Add(40832, 40956);
				this.Add(57408, 57470);
				this.Add(57472, 57596);
				this.Add(57664, 57726);
				this.Add(57728, 57852);
				this.Add(57920, 57982);
				this.Add(57984, 58108);
				this.Add(58176, 58238);
				this.Add(58240, 58364);
				this.Add(58432, 58494);
				this.Add(58496, 58620);
				this.Add(58688, 58750);
				this.Add(58752, 58876);
				this.Add(58944, 59006);
				this.Add(59008, 59132);
				this.Add(59200, 59262);
				this.Add(59264, 59388);
				this.Add(59456, 59518);
				this.Add(59520, 59644);
				this.Add(59712, 59774);
				this.Add(59776, 59900);
				this.Add(59968, 60030);
				this.Add(60032, 60068);
				this.Add(60736, 60798);
				this.Add(60800, 60924);
				this.Add(60992, 61054);
				this.Add(61056, 61164);
				this.Add(61167, 61180);
				this.Add(64064, 64126);
				this.Add(64128, 64252);
				this.Add(64320, 64382);
				this.Add(64384, 64508);
				this.Add(64576, 64587);
			}

			// Token: 0x060006B9 RID: 1721 RVA: 0x00022BA4 File Offset: 0x00020DA4
			private void Add(ushort bgn, ushort end)
			{
				for (ushort chr = bgn; chr <= end; chr += 1)
				{
					this.Add(chr);
				}
			}

			// Token: 0x060006BA RID: 1722 RVA: 0x00022BC5 File Offset: 0x00020DC5
			private void Add(ushort chr)
			{
				this.ChrMap[(int)(chr / 64)] |= 1UL << (int)(chr % 64);
			}

			// Token: 0x060006BB RID: 1723 RVA: 0x00022BE4 File Offset: 0x00020DE4
			public bool Contains(byte lead, byte trail)
			{
				ushort chr = (ushort)(lead << 8);
				chr |= (ushort)trail;
				return this.Contains(chr);
			}

			// Token: 0x060006BC RID: 1724 RVA: 0x00022C04 File Offset: 0x00020E04
			public bool Contains(ushort chr)
			{
				return (this.ChrMap[(int)(chr / 64)] & 1UL << (int)(chr % 64)) > 0UL;
			}

			// Token: 0x04000553 RID: 1363
			private static SCommon.S_JChar _i;

			// Token: 0x04000554 RID: 1364
			private ulong[] ChrMap = new ulong[1024];
		}

		// Token: 0x02000157 RID: 343
		private class S_CSPRandomNumberGenerator : RandomUnit.IRandomNumberGenerator, IDisposable
		{
			// Token: 0x060006BD RID: 1725 RVA: 0x00022C20 File Offset: 0x00020E20
			public byte[] GetBlock()
			{
				this.Rng.GetBytes(this.Cache);
				return this.Cache;
			}

			// Token: 0x060006BE RID: 1726 RVA: 0x00022C39 File Offset: 0x00020E39
			public void Dispose()
			{
				if (this.Rng != null)
				{
					this.Rng.Dispose();
					this.Rng = null;
				}
			}

			// Token: 0x04000555 RID: 1365
			private RandomNumberGenerator Rng = new RNGCryptoServiceProvider();

			// Token: 0x04000556 RID: 1366
			private byte[] Cache = new byte[4096];
		}

		// Token: 0x02000158 RID: 344
		public static class Hex
		{
			// Token: 0x060006C0 RID: 1728 RVA: 0x00022C78 File Offset: 0x00020E78
			public static string ToString(byte[] src)
			{
				StringBuilder buff = new StringBuilder(src.Length * 2);
				foreach (byte chr in src)
				{
					buff.Append(SCommon.hexadecimal[chr >> 4]);
					buff.Append(SCommon.hexadecimal[(int)(chr & 15)]);
				}
				return buff.ToString();
			}

			// Token: 0x060006C1 RID: 1729 RVA: 0x00022CD4 File Offset: 0x00020ED4
			public static byte[] ToBytes(string src)
			{
				if (src.Length % 2 != 0)
				{
					throw new ArgumentException("入力文字列の長さに問題があります。");
				}
				byte[] dest = new byte[src.Length / 2];
				for (int index = 0; index < dest.Length; index++)
				{
					int hi = SCommon.Hex.To4Bit(src[index * 2]);
					int lw = SCommon.Hex.To4Bit(src[index * 2 + 1]);
					dest[index] = (byte)(hi << 4 | lw);
				}
				return dest;
			}

			// Token: 0x060006C2 RID: 1730 RVA: 0x00022D3C File Offset: 0x00020F3C
			private static int To4Bit(char chr)
			{
				int num = SCommon.hexadecimal.IndexOf(char.ToLower(chr));
				if (num == -1)
				{
					throw new ArgumentException("入力文字列に含まれる文字に問題があります。");
				}
				return num;
			}
		}

		// Token: 0x02000159 RID: 345
		public class IECompString : IEqualityComparer<string>
		{
			// Token: 0x060006C3 RID: 1731 RVA: 0x00022D5D File Offset: 0x00020F5D
			public bool Equals(string a, string b)
			{
				return a == b;
			}

			// Token: 0x060006C4 RID: 1732 RVA: 0x00022D66 File Offset: 0x00020F66
			public int GetHashCode(string a)
			{
				return a.GetHashCode();
			}
		}

		// Token: 0x0200015A RID: 346
		public class IECompStringIgnoreCase : IEqualityComparer<string>
		{
			// Token: 0x060006C6 RID: 1734 RVA: 0x00022D6E File Offset: 0x00020F6E
			public bool Equals(string a, string b)
			{
				return a.ToLower() == b.ToLower();
			}

			// Token: 0x060006C7 RID: 1735 RVA: 0x00022D81 File Offset: 0x00020F81
			public int GetHashCode(string a)
			{
				return a.ToLower().GetHashCode();
			}
		}
	}
}
