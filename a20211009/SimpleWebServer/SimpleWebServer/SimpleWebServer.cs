using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleWebServer
{
	public class SimpleWebServer
	{
		public void Perform()
		{
			HTTPServer hs = new HTTPServer();

			hs.HTTPConnected = this.P_Connected;

			hs.Perform();
		}

		private string DocRoot = @"C:\temp";

		private void P_Connected(HTTPServerChannel channel)
		{
			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "クライアント：" + channel.Channel.Handler.RemoteEndPoint);

			if (10 < channel.Method.Length) // ラフなしきい値
				throw new Exception("Received method is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "要求メソッド：" + channel.Method);

			bool head;
			if (channel.Method == "GET")
				head = false;
			else if (channel.Method == "HEAD")
				head = true;
			else
				throw new Exception("Unsupported method: " + channel.Method);

			string urlPath = channel.PathQuery;

			// クエリ除去
			{
				int ques = urlPath.IndexOf('?');

				if (ques != -1)
					urlPath = urlPath.Substring(0, ques);
			}

			if (1000 < urlPath.Length) // ラフなしきい値
				throw new Exception("Received path is too long");

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "要求パス：" + urlPath);

			string[] pTkns = urlPath.Split('/').Where(v => v != "").Select(v => Common.ToFairLocalPath(v, 0)).ToArray();
			string path = Path.Combine(new string[] { this.DocRoot }.Concat(pTkns).ToArray());

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "目的パス：" + path);

			if (urlPath.EndsWith("/"))
			{
				path = Path.Combine(path, "index.htm");

				if (!File.Exists(path))
					path += "l";
			}
			else if (Directory.Exists(path))
			{
				channel.ResStatus = 301;
				channel.ResHeaderPairs.Add(new string[] { "Location", "http://" + GetHeaderValue(channel, "Host") + "/" + string.Join("", pTkns.Select(v => EncodeUrl(v) + "/")) });
				//channel.ResBody = null;

				goto endFunc;
			}
			if (File.Exists(path))
			{
				//channel.ResStatus = 200;
				channel.ResHeaderPairs.Add(new string[] { "Content-Type", ContentTypeCollection.I.GetContentType(Path.GetExtension(path)) });
				channel.ResBody = E_ReadFile(path);
			}
			else
			{
				channel.ResStatus = 404;
				//channel.ResHeaderPairs.Add();
				//channel.ResBody = null;
			}

		endFunc:
			channel.ResHeaderPairs.Add(new string[] { "Server", "HTTCmd" });

			if (head && channel.ResBody != null)
			{
				FileInfo fileInfo = new FileInfo(path);

				channel.ResHeaderPairs.Add(new string[] { "Content-Length", fileInfo.Length.ToString() });
				channel.ResHeaderPairs.Add(new string[] { "X-Last-Modified-Time", new SCommon.SimpleDateTime(fileInfo.LastWriteTime).ToString("{0}/{1:D2}/{2:D2} {4:D2}:{5:D2}:{6:D2}") });

				channel.ResBody = null;
			}

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-STATUS " + channel.ResStatus);

			foreach (string[] pair in channel.ResHeaderPairs)
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-HEADER " + pair[0] + " = " + pair[1]);

			SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "RES-BODY " + (channel.ResBody != null));
		}

		private static string GetHeaderValue(HTTPServerChannel channel, string name)
		{
			foreach (string[] pair in channel.HeaderPairs)
				if (SCommon.EqualsIgnoreCase(pair[0], name))
					return pair[1];

			throw new Exception("No header key: " + name);
		}

		private static string EncodeUrl(string str)
		{
			StringBuilder buff = new StringBuilder();

			foreach (byte chr in Encoding.UTF8.GetBytes(str))
			{
				buff.Append('%');
				buff.Append(chr.ToString("x2"));
			}
			return buff.ToString();
		}

		private static IEnumerable<byte[]> E_ReadFile(string file)
		{
			long fileSize = new FileInfo(file).Length;

			for (long offset = 0L; offset < fileSize;)
			{
				int readSize = (int)Math.Min(fileSize - offset, 2000000L);
				byte[] buff = new byte[readSize];

				//SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "READ " + offset + " " + readSize + " " + fileSize + " " + (offset * 100.0 / fileSize).ToString("F2") + " " + ((offset + readSize) * 100.0 / fileSize).ToString("F2")); // 頻出するので抑止

				using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
				{
					reader.Seek(offset, SeekOrigin.Begin);
					reader.Read(buff, 0, readSize);
				}
				yield return buff;

				offset += (long)readSize;
			}
		}

		public class ContentTypeCollection
		{
			private static ContentTypeCollection _i = null;

			public static ContentTypeCollection I
			{
				get
				{
					if (_i == null)
						_i = new ContentTypeCollection();

					return _i;
				}
			}

			#region Extension2ContentTypeResource

			/// <summary>
			/// https://github.com/stackprobe/HTT/blob/master/doc/MimeType.tsv
			/// </summary>
			private static string[] Extension2ContentTypeResource = new string[]
			{
			".323", "text/h323",
			".acx", "application/internet-property-stream",
			".ai", "application/postscript",
			".aif", "audio/x-aiff",
			".aifc", "audio/x-aiff",
			".aiff", "audio/x-aiff",
			".asf", "video/x-ms-asf",
			".asr", "video/x-ms-asf",
			".asx", "video/x-ms-asf",
			".au", "audio/basic",
			".avi", "video/x-msvideo",
			".axs", "application/olescript",
			".bas", "text/plain",
			".bcpio", "application/x-bcpio",
			".bin", "application/octet-stream",
			".bmp", "image/bmp",
			".c", "text/plain",
			".cat", "application/vnd.ms-pkiseccat",
			".cdf", "application/x-cdf",
			".cer", "application/x-x509-ca-cert",
			".class", "application/octet-stream",
			".clp", "application/x-msclip",
			".cmx", "image/x-cmx",
			".cod", "image/cis-cod",
			".cpio", "application/x-cpio",
			".crd", "application/x-mscardfile",
			".crl", "application/pkix-crl",
			".crt", "application/x-x509-ca-cert",
			".csh", "application/x-csh",
			".css", "text/css",
			".dcr", "application/x-director",
			".der", "application/x-x509-ca-cert",
			".dir", "application/x-director",
			".dll", "application/x-msdownload",
			".dms", "application/octet-stream",
			".doc", "application/msword",
			".dot", "application/msword",
			".dvi", "application/x-dvi",
			".dxr", "application/x-director",
			".eps", "application/postscript",
			".etx", "text/x-setext",
			".evy", "application/envoy",
			".exe", "application/octet-stream",
			".fif", "application/fractals",
			".flr", "x-world/x-vrml",
			".gif", "image/gif",
			".gtar", "application/x-gtar",
			".gz", "application/x-gzip",
			".h", "text/plain",
			".hdf", "application/x-hdf",
			".hlp", "application/winhlp",
			".hqx", "application/mac-binhex40",
			".hta", "application/hta",
			".htc", "text/x-component",
			".htm", "text/html",
			".html", "text/html",
			".htt", "text/webviewhtml",
			".ico", "image/x-icon",
			".ief", "image/ief",
			".iii", "application/x-iphone",
			".ins", "application/x-internet-signup",
			".isp", "application/x-internet-signup",
			".jfif", "image/pipeg",
			".jpe", "image/jpeg",
			".jpeg", "image/jpeg",
			".jpg", "image/jpeg",
			".js", "application/x-javascript",
			".json", "application/json",
			".latex", "application/x-latex",
			".lha", "application/octet-stream",
			".lsf", "video/x-la-asf",
			".lsx", "video/x-la-asf",
			".lzh", "application/octet-stream",
			".m13", "application/x-msmediaview",
			".m14", "application/x-msmediaview",
			".m3u", "audio/x-mpegurl",
			".m4a", "audio/aac",
			".m4v", "video/mp4",
			".man", "application/x-troff-man",
			".mdb", "application/x-msaccess",
			".me", "application/x-troff-me",
			".mht", "message/rfc822",
			".mhtml", "message/rfc822",
			".mid", "audio/midi",
			".midi", "audio/midi",
			".mny", "application/x-msmoney",
			".mov", "video/quicktime",
			".movie", "video/x-sgi-movie",
			".mp2", "video/mpeg",
			".mp3", "audio/mpeg",
			".mp4", "video/mp4",
			".mpa", "video/mpeg",
			".mpe", "video/mpeg",
			".mpeg", "video/mpeg",
			".mpg", "video/mpeg",
			".mpga", "audio/mpeg",
			".mpp", "application/vnd.ms-project",
			".mpv2", "video/mpeg",
			".ms", "application/x-troff-ms",
			".mvb", "application/x-msmediaview",
			".nws", "message/rfc822",
			".oda", "application/oda",
			".ogv", "video/ogg",
			".p10", "application/pkcs10",
			".p12", "application/x-pkcs12",
			".p7b", "application/x-pkcs7-certificates",
			".p7c", "application/x-pkcs7-mime",
			".p7m", "application/x-pkcs7-mime",
			".p7r", "application/x-pkcs7-certreqresp",
			".p7s", "application/x-pkcs7-signature",
			".pbm", "image/x-portable-bitmap",
			".pdf", "application/pdf",
			".pfx", "application/x-pkcs12",
			".pgm", "image/x-portable-graymap",
			".pko", "application/ynd.ms-pkipko",
			".pma", "application/x-perfmon",
			".pmc", "application/x-perfmon",
			".pml", "application/x-perfmon",
			".pmr", "application/x-perfmon",
			".pmw", "application/x-perfmon",
			".png", "image/png",
			".pnm", "image/x-portable-anymap",
			".pot", "application/vnd.ms-powerpoint",
			".ppm", "image/x-portable-pixmap",
			".pps", "application/vnd.ms-powerpoint",
			".ppt", "application/vnd.ms-powerpoint",
			".prf", "application/pics-rules",
			".ps", "application/postscript",
			".pub", "application/x-mspublisher",
			".qt", "video/quicktime",
			".ra", "audio/x-pn-realaudio",
			".ram", "audio/x-pn-realaudio",
			".ras", "image/x-cmu-raster",
			".rgb", "image/x-rgb",
			".rmi", "audio/mid",
			".roff", "application/x-troff",
			".rtf", "application/rtf",
			".rtx", "text/richtext",
			".scd", "application/x-msschedule",
			".sct", "text/scriptlet",
			".setpay", "application/set-payment-initiation",
			".setreg", "application/set-registration-initiation",
			".sh", "application/x-sh",
			".shar", "application/x-shar",
			".sit", "application/x-stuffit",
			".snd", "audio/basic",
			".spc", "application/x-pkcs7-certificates",
			".spl", "application/futuresplash",
			".src", "application/x-wais-source",
			".sst", "application/vnd.ms-pkicertstore",
			".stl", "application/vnd.ms-pkistl",
			".stm", "text/html",
			".sv4cpio", "application/x-sv4cpio",
			".sv4crc", "application/x-sv4crc",
			".svg", "image/svg+xml",
			".swf", "application/x-shockwave-flash",
			".t", "application/x-troff",
			".tar", "application/x-tar",
			".tcl", "application/x-tcl",
			".tex", "application/x-tex",
			".texi", "application/x-texinfo",
			".texinfo", "application/x-texinfo",
			".tgz", "application/x-compressed",
			".tif", "image/tiff",
			".tiff", "image/tiff",
			".tr", "application/x-troff",
			".trm", "application/x-msterminal",
			".tsv", "text/tab-separated-values",
			".txt", "text/plain",
			".uls", "text/iuls",
			".ustar", "application/x-ustar",
			".vcf", "text/x-vcard",
			".vrml", "x-world/x-vrml",
			".wav", "audio/x-wav",
			".wcm", "application/vnd.ms-works",
			".wdb", "application/vnd.ms-works",
			".webm", "video/webm",
			".wks", "application/vnd.ms-works",
			".wmf", "application/x-msmetafile",
			".wps", "application/vnd.ms-works",
			".wri", "application/x-mswrite",
			".wrl", "x-world/x-vrml",
			".wrz", "x-world/x-vrml",
			".xaf", "x-world/x-vrml",
			".xbm", "image/x-xbitmap",
			".xht", "application/xhtml+xml",
			".xhtml", "application/xhtml+xml",
			".xla", "application/vnd.ms-excel",
			".xlc", "application/vnd.ms-excel",
			".xlm", "application/vnd.ms-excel",
			".xls", "application/vnd.ms-excel",
			".xlt", "application/vnd.ms-excel",
			".xlw", "application/vnd.ms-excel",
			".xml", "text/xml",
			".xof", "x-world/x-vrml",
			".xpm", "image/x-xpixmap",
			".xsl", "text/xml",
			".xwd", "image/x-xwindowdump",
			".z", "application/x-compress",
			".zip", "application/zip",
			};

			#endregion

			private Dictionary<string, string> Extension2ContentType = SCommon.CreateDictionaryIgnoreCase<string>();

			private ContentTypeCollection()
			{
				for (int index = 0; index < Extension2ContentTypeResource.Length; index += 2)
					this.Extension2ContentType.Add(Extension2ContentTypeResource[index], Extension2ContentTypeResource[index + 1]);
			}

			public string GetContentType(string ext)
			{
				if (this.Extension2ContentType.ContainsKey(ext))
					return this.Extension2ContentType[ext];

				return "application/octet-stream"; // デフォルト
			}
		}

		public static class Common
		{
			#region ToFairLocalPath

			/// <summary>
			/// Windowsのローカル名に使用出来ない予約名のリストを返す。
			/// 今では使用可能なものも含む。
			/// 元にしたコード：
			/// https://github.com/stackprobe/Factory/blob/master/Common/DataConv.c#L460-L491
			/// </summary>
			/// <returns>予約名リスト</returns>
			private static IEnumerable<string> GetReservedWordsForWindowsPath()
			{
				yield return "AUX";
				yield return "CON";
				yield return "NUL";
				yield return "PRN";

				for (int no = 1; no <= 9; no++)
				{
					yield return "COM" + no;
					yield return "LPT" + no;
				}

				// グレーゾーン
				{
					yield return "COM0";
					yield return "LPT0";
					yield return "CLOCK$";
					yield return "CONFIG$";
				}
			}

			/// <summary>
			/// 歴としたローカル名に変換する。
			/// 実際に使用可能なローカル名より基準が厳しい。
			/// 元にしたコード：
			/// https://github.com/stackprobe/Factory/blob/master/Common/DataConv.c#L503-L552
			/// </summary>
			/// <param name="str">対象文字列(対象パス)</param>
			/// <param name="dirSize">対象パスが存在するディレクトリのフルパスの長さ、考慮しない場合は 0 を指定すること。</param>
			/// <returns>ローカル名</returns>
			public static string ToFairLocalPath(string str, int dirSize)
			{
				const int MY_PATH_MAX = 250;
				const string NG_CHARS = "\"*/:<>?\\|";
				const string ALT_WORD = "_";

				int localPathSizeMax = Math.Max(0, MY_PATH_MAX - dirSize);

				if (localPathSizeMax < str.Length) // HACK: 元にしたコードではバイト列の長さで判定している。
					str = str.Substring(0, localPathSizeMax);

				str = SCommon.ToJString(SCommon.ENCODING_SJIS.GetBytes(str), true, false, false, true);

				string[] words = str.Split('.');

				for (int index = 0; index < words.Length; index++)
				{
					string word = words[index];

					word = word.Trim();

					if (
						index == 0 &&
						GetReservedWordsForWindowsPath().Any(resWord => SCommon.EqualsIgnoreCase(resWord, word)) ||
						word.Any(chr => NG_CHARS.IndexOf(chr) != -1)
						)
						word = ALT_WORD;

					words[index] = word;
				}
				str = string.Join(".", words);

				if (str == "")
					str = ALT_WORD;

				if (str.EndsWith("."))
					str = str.Substring(0, str.Length - 1) + ALT_WORD;

				return str;
			}

			#endregion
		}

		public class HTTPServer : SockServer
		{
			/// <summary>
			/// サーバーロジック
			/// 引数：
			/// -- channel: 接続チャネル
			/// </summary>
			public Action<HTTPServerChannel> HTTPConnected = channel => { };

			// <---- prm

			public HTTPServer()
			{
				PortNo = 80;
			}

			/// <summary>
			/// Keep-Alive-タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int KeepAliveTimeoutMillis = 5000;

			protected override IEnumerable<int> E_Connected(SockChannel channel)
			{
				DateTime startedTime = DateTime.Now;

				for (; ; )
				{
					HTTPServerChannel hsChannel = new HTTPServerChannel();
					int retval = -1;

					hsChannel.Channel = channel;

					foreach (int size in hsChannel.RecvRequest())
					{
						if (size == 0)
							throw null; // never

						if (size < 0)
						{
							yield return retval;
							retval = -1;
						}
						else
							retval = 1;
					}

					SockCommon.NB("svlg", () =>
					{
						HTTPConnected(hsChannel);
						return -1; // dummy
					});

					if (KeepAliveTimeoutMillis != -1 && KeepAliveTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						hsChannel.KeepAlive = false;
					}

					foreach (int size in hsChannel.SendResponse())
					{
						if (size == 0)
							throw null; // never

						if (size < 0)
						{
							yield return retval;
							retval = -1;
						}
						else
							retval = 1;
					}

					if (!hsChannel.KeepAlive)
						break;
				}
			}
		}

		public class HTTPServerChannel
		{
			public SockChannel Channel;

			/// <summary>
			/// 要求タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int RequestTimeoutMillis = -1;

			/// <summary>
			/// 応答タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int ResponseTimeoutMillis = -1;

			// memo: チャンク毎のタイムアウトは IdleTimeoutMillis で代替する。

			/// <summary>
			/// リクエストの最初の行のみの無通信タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int FirstLineTimeoutMillis = 2000;

			/// <summary>
			/// リクエストの最初の行以外の(レスポンスも含む)無通信タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int IdleTimeoutMillis = 180000; // 3 min

			/// <summary>
			/// リクエストのボディの最大サイズ_バイト数
			/// -1 == INFINITE
			/// </summary>
			public static int BodySizeMax = 300000000; // 300 MB

			public IEnumerable<int> RecvRequest()
			{
				this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(RequestTimeoutMillis);
				this.Channel.IdleTimeoutMillis = FirstLineTimeoutMillis;

				this.Channel.FirstLineRecving = true;

				foreach (int relay in this.RecvLine(ret => this.FirstLine = ret))
					yield return relay;

				this.Channel.FirstLineRecving = false;

				{
					string[] tokens = this.FirstLine.Split(' ');

					this.Method = tokens[0];
					this.PathQuery = DecodeURL(tokens[1]);
					this.HTTPVersion = tokens[2];
				}

				this.Channel.IdleTimeoutMillis = IdleTimeoutMillis;

				foreach (int relay in this.RecvHeader())
					yield return relay;

				this.CheckHeader();

				if (this.Expect100Continue)
				{
					foreach (int relay in this.SendLine("HTTP/1.1 100 Continue")
						.Concat(this.Channel.Send(CRLF)))
						yield return relay;
				}
				foreach (int relay in this.RecvBody())
					yield return relay;
			}

			private static DateTime? TimeoutMillisToDateTime(int millis)
			{
				if (millis == -1)
					return null;

				return DateTime.Now + TimeSpan.FromMilliseconds((double)millis);
			}

			private static string DecodeURL(string path)
			{
				byte[] src = Encoding.ASCII.GetBytes(path);

				using (MemoryStream dest = new MemoryStream())
				{
					for (int index = 0; index < src.Length; index++)
					{
						if (src[index] == 0x25) // ? '%'
						{
							dest.WriteByte((byte)Convert.ToInt32(Encoding.ASCII.GetString(SCommon.GetSubBytes(src, index + 1, 2)), 16));
							index += 2;
						}
						else if (src[index] == 0x2b) // ? '+'
						{
							dest.WriteByte(0x20); // ' '
						}
						else
						{
							dest.WriteByte(src[index]);
						}
					}

					byte[] bytes = dest.ToArray();

					if (!SockCommon.P_UTF8Check.Check(bytes))
						throw new Exception("URL is not Japanese UTF-8");

					return Encoding.UTF8.GetString(bytes);
				}
			}

			public string FirstLine;
			public string Method;
			public string PathQuery;
			public string HTTPVersion;
			public List<string[]> HeaderPairs = new List<string[]>();
			public byte[] Body;

			private const byte CR = 0x0d;
			private const byte LF = 0x0a;

			private readonly byte[] CRLF = new byte[] { CR, LF };

			private IEnumerable<int> RecvLine(Action<string> a_return)
			{
				const int LINE_LEN_MAX = 512000;

				List<byte> buff = new List<byte>(LINE_LEN_MAX);

				for (; ; )
				{
					byte[] chrs = null;

					foreach (int relay in this.Channel.Recv(1, ret => chrs = ret))
						yield return relay;

					byte chr = chrs[0];

					if (chr == CR)
						continue;

					if (chr == LF)
						break;

					if (LINE_LEN_MAX < buff.Count)
						throw new OverflowException("Received line is too long");

					if (chr < 0x20 || 0x7e < chr) // ? not ASCII -> SPACE
						chr = 0x20;

					buff.Add(chr);
				}
				a_return(Encoding.ASCII.GetString(buff.ToArray()));
			}

			private IEnumerable<int> RecvHeader()
			{
				const int HEADERS_LEN_MAX = 612000;
				const int WEIGHT = 1000;

				int roughHeaderLength = 0;

				for (; ; )
				{
					string line = null;

					foreach (int relay in this.RecvLine(ret => line = ret))
						yield return relay;

					if (line == null)
						throw null; // never

					if (line == "")
						break;

					roughHeaderLength += line.Length + WEIGHT;

					if (HEADERS_LEN_MAX < roughHeaderLength)
						throw new OverflowException("Received header is too long");

					if (line[0] <= ' ')
					{
						this.HeaderPairs[this.HeaderPairs.Count - 1][1] += " " + line.Trim();
					}
					else
					{
						int colon = line.IndexOf(':');

						if (colon == -1)
							throw new Exception("Bad header line (no colon)");

						this.HeaderPairs.Add(new string[]
						{
						line.Substring(0, colon).Trim(),
						line.Substring(colon + 1).Trim(),
						});
					}
				}
			}

			public int ContentLength = 0;
			public bool Chunked = false;
			public string ContentType = null;
			public bool Expect100Continue = false;
			public bool KeepAlive = false;

			private void CheckHeader()
			{
				foreach (string[] pair in this.HeaderPairs)
				{
					string key = pair[0];
					string value = pair[1];

					if (1000 < key.Length || 1000 < value.Length) // ラフなしきい値
					{
						SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "Ignore gen-header key and value (too long)");
						continue;
					}

					if (SCommon.EqualsIgnoreCase(key, "Content-Length"))
					{
						if (value.Length < 1 || 10 < value.Length)
							throw new Exception("Bad Content-Length value");

						this.ContentLength = int.Parse(value);
					}
					else if (SCommon.EqualsIgnoreCase(key, "Transfer-Encoding"))
					{
						this.Chunked = SCommon.ContainsIgnoreCase(value, "chunked");
					}
					else if (SCommon.EqualsIgnoreCase(key, "Content-Type"))
					{
						this.ContentType = value;
					}
					else if (SCommon.EqualsIgnoreCase(key, "Expect"))
					{
						this.Expect100Continue = SCommon.ContainsIgnoreCase(value, "100-continue");
					}
					else if (SCommon.EqualsIgnoreCase(key, "Connection"))
					{
						this.KeepAlive = SCommon.ContainsIgnoreCase(value, "keep-alive");
					}
				}
			}

			private IEnumerable<int> RecvBody()
			{
				const int READ_SIZE_MAX = 2000000; // 2 MB

				HTTPBodyOutputStream buff = this.Channel.BodyOutputStream;

				if (this.Chunked)
				{
					for (; ; )
					{
						string line = null;

						foreach (int relay in this.RecvLine(ret => line = ret))
							yield return relay;

						if (line == null)
							throw null; // never

						// chunk-extension の削除
						{
							int i = line.IndexOf(';');

							if (i != -1)
								line = line.Substring(0, i);
						}

						line = line.Trim();

						if (line.Length < 1 || 8 < line.Length)
							throw new Exception("Bad chunk-size line");

						int size = Convert.ToInt32(line, 16);

						if (size == 0)
							break;

						if (size < 0)
							throw new Exception("不正なチャンクサイズです。" + size);

						if (BodySizeMax != -1 && BodySizeMax - buff.Count < size)
							throw new Exception("ボディサイズが大きすぎます。" + buff.Count + " + " + size);

						int chunkEnd = buff.Count + size;

						while (buff.Count < chunkEnd)
						{
							byte[] data = null;

							foreach (int relay in this.Channel.Recv(Math.Min(READ_SIZE_MAX, chunkEnd - buff.Count), ret => data = ret))
								yield return relay;

							if (data == null)
								throw null; // never

							buff.Write(data);
						}
						foreach (int relay in this.Channel.Recv(2, ret => { })) // CR-LF
							yield return relay;
					}

					for (; ; ) // RFC 7230 4.1.2 Chunked Trailer Part
					{
						string line = null;

						foreach (int relay in this.RecvLine(ret => line = ret))
							yield return relay;

						if (line == null)
							throw null; // never

						if (line == "")
							break;
					}
				}
				else
				{
					if (this.ContentLength < 0)
						throw new Exception("不正なボディサイズです。" + this.ContentLength);

					if (BodySizeMax != -1 && BodySizeMax < this.ContentLength)
						throw new Exception("ボディサイズが大きすぎます。" + this.ContentLength);

					while (buff.Count < this.ContentLength)
					{
						byte[] data = null;

						foreach (int relay in this.Channel.Recv(Math.Min(READ_SIZE_MAX, this.ContentLength - buff.Count), ret => data = ret))
							yield return relay;

						if (data == null)
							throw null; // never

						buff.Write(data);
					}
				}
				this.Body = buff.ToByteArray();
			}

			// HTTPConnected 内で(必要に応じて)設定しなければならないフィールド -->

			public int ResStatus = 200;
			public List<string[]> ResHeaderPairs = new List<string[]>();
			public IEnumerable<byte[]> ResBody = null;

			// <-- HTTPConnected 内で(必要に応じて)設定しなければならないフィールド

			public IEnumerable<int> SendResponse()
			{
				this.Body = null;
				this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(ResponseTimeoutMillis);

				foreach (int relay in this.SendLine("HTTP/1.1 " + this.ResStatus + " Happy Tea Time"))
					yield return relay;

				foreach (string[] pair in this.ResHeaderPairs)
					foreach (int relay in this.SendLine(pair[0] + ": " + pair[1]))
						yield return relay;

				if (this.ResBody == null)
				{
					foreach (int relay in this.EndHeader())
						yield return relay;
				}
				else
				{
					IEnumerator<byte[]> resBodyIterator = this.ResBody.GetEnumerator();

					if (SockCommon.NB("chu1", () => resBodyIterator.MoveNext()))
					{
						byte[] first = resBodyIterator.Current;

						if (SockCommon.NB("chu2", () => resBodyIterator.MoveNext()))
						{
							foreach (int relay in this.SendLine("Transfer-Encoding: chunked")
								.Concat(this.EndHeader())
								.Concat(this.SendChunk(first)))
								yield return relay;

							do
							{
								foreach (int relay in this.SendChunk(resBodyIterator.Current))
									yield return relay;
							}
							while (SockCommon.NB("chux", () => resBodyIterator.MoveNext()));

							foreach (int relay in this.SendLine("0")
								.Concat(this.Channel.Send(CRLF)))
								yield return relay;
						}
						else
						{
							foreach (int relay in this.SendLine("Content-Length: " + first.Length)
								.Concat(this.EndHeader())
								.Concat(this.Channel.Send(first)))
								yield return relay;
						}
					}
					else
					{
						foreach (int relay in this.SendLine("Content-Length: 0")
							.Concat(this.EndHeader()))
							yield return relay;
					}
				}
			}

			private IEnumerable<int> EndHeader()
			{
				foreach (int relay in this.SendLine("Connection: " + (this.KeepAlive ? "keep-alive" : "close"))
					.Concat(this.Channel.Send(CRLF)))
					yield return relay;
			}

			private IEnumerable<int> SendChunk(byte[] chunk)
			{
				if (1 <= chunk.Length)
				{
					foreach (int relay in this.SendLine(chunk.Length.ToString("x"))
						.Concat(this.Channel.Send(chunk))
						.Concat(this.Channel.Send(CRLF)))
						yield return relay;
				}
			}

			private IEnumerable<int> SendLine(string line)
			{
				foreach (int relay in this.Channel.Send(Encoding.ASCII.GetBytes(line))
					.Concat(this.Channel.Send(CRLF)))
					yield return relay;
			}
		}

		public class HTTPBodyOutputStream : IDisposable
		{
			private WorkingDir WD = null;
			private string BuffFile = null;
			private int WroteSize = 0;

			private string GetBuffFile()
			{
				if (this.WD == null)
				{
					this.WD = new WorkingDir();
					this.BuffFile = this.WD.MakePath();
				}
				return this.BuffFile;
			}

			public void Write(byte[] data, int offset = 0)
			{
				this.Write(data, offset, data.Length - offset);
			}

			public void Write(byte[] data, int offset, int count)
			{
				using (FileStream writer = new FileStream(this.GetBuffFile(), FileMode.Append, FileAccess.Write))
				{
					writer.Write(data, offset, count);
				}
				this.WroteSize += count;
			}

			public int Count
			{
				get
				{
					return this.WroteSize;
				}
			}

			public byte[] ToByteArray()
			{
				byte[] data;

				if (this.WroteSize == 0)
				{
					data = SCommon.EMPTY_BYTES;
				}
				else
				{
					data = File.ReadAllBytes(this.BuffFile);
					File.WriteAllBytes(this.BuffFile, SCommon.EMPTY_BYTES);
					this.WroteSize = 0;
				}
				return data;
			}

			public void Dispose()
			{
				if (this.WD != null)
				{
					this.WD.Dispose();
					this.WD = null;
				}
			}
		}

		public abstract class SockServer
		{
			/// <summary>
			/// ポート番号
			/// </summary>
			public int PortNo = 59999;

			/// <summary>
			/// 接続待ちキューの長さ
			/// </summary>
			public int Backlog = 300;

			/// <summary>
			/// 最大同時接続数
			/// </summary>
			public int ConnectMax = 100;

			/// <summary>
			/// 処理の合間に呼ばれる処理
			/// 戻り値：
			/// -- サーバーを継続するか
			/// </summary>
			public Func<bool> Interlude = () => !Console.KeyAvailable;

			// <---- prm

			/// <summary>
			/// サーバーロジック
			/// 通信量：
			/// -- 0 == 通信終了 -- SCommon.Supplier の最後の要素の次以降 0 (default(int)) になるため
			/// -- 0 未満 == 通信無し
			/// -- 1 以上 == 通信有り
			/// </summary>
			/// <param name="channel">接続チャネル</param>
			/// <returns>通信量</returns>
			protected abstract IEnumerable<int> E_Connected(SockChannel channel);

			private List<SockChannel> Channels = new List<SockChannel>();

			public void Perform()
			{
				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しています...");

				try
				{
					using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						IPEndPoint endPoint = new IPEndPoint(0L, this.PortNo);

						listener.Bind(endPoint);
						listener.Listen(this.Backlog);
						listener.Blocking = false;

						SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを開始しました。");

						int waitMillis = 0;

						while (this.Interlude())
						{
							if (waitMillis < 100)
								waitMillis++;

							for (int c = 0; c < 30; c++) // HACK: 繰り返し回数_適当
							{
								Socket handler = this.Channels.Count < this.ConnectMax ? this.Connect(listener) : null;

								if (handler == null) // ? 接続無し || 最大同時接続数に達している。
									break;

								waitMillis = 0; // reset

								SockCommon.TimeWaitMonitor.Connected();

								{
									SockChannel channel = new SockChannel();

									channel.Handler = handler;
									handler = null; // もう使わない。
									channel.Handler.Blocking = false;
									channel.ID = SockCommon.IDIssuer.Issue();
									channel.Connected = SCommon.Supplier(this.E_Connected(channel));
									channel.BodyOutputStream = new HTTPBodyOutputStream();

									this.Channels.Add(channel);

									SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信開始 " + channel.ID);
								}
							}
							for (int index = this.Channels.Count - 1; 0 <= index; index--)
							{
								SockChannel channel = this.Channels[index];
								int size;

								try
								{
									size = channel.Connected();

									if (0 < size) // ? 通信有り
									{
										waitMillis = 0; // reset
									}
								}
								catch (Exception e)
								{
									if (channel.FirstLineRecving && e is SockChannel.RecvIdleTimeoutException)
										SockCommon.WriteLog(SockCommon.ErrorLevel_e.FIRST_LINE_TIMEOUT, null);
									else
										SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK_OR_SERVER_LOGIC, e);

									size = 0;
								}

								if (size == 0) // ? 切断
								{
									SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "通信終了 " + channel.ID);

									this.Disconnect(channel);
									SCommon.FastDesertElement(this.Channels, index);

									SockCommon.TimeWaitMonitor.Disconnect();
								}
							}

							//GC.Collect(); // GeoDemo の Server.sln が重くなるため、暫定削除 @ 2019.4.9

							if (0 < waitMillis)
								Thread.Sleep(waitMillis);
						}

						SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しています...");

						this.Stop();
					}
				}
				catch (Exception e)
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.FATAL, e);
				}

				SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "サーバーを終了しました。");
			}

			private Socket Connect(Socket listener) // ret: null == 接続タイムアウト
			{
				try
				{
					return SockCommon.NB("conn", () => listener.Accept());
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("接続エラー", e);
					}
					return null;
				}
			}

			private void Stop()
			{
				foreach (SockChannel channel in this.Channels)
					this.Disconnect(channel);

				this.Channels.Clear();
			}

			private void Disconnect(SockChannel channel)
			{
				try
				{
					channel.Handler.Shutdown(SocketShutdown.Both);
				}
				catch (Exception e)
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, e);
				}

				try
				{
					channel.Handler.Close();
				}
				catch (Exception e)
				{
					SockCommon.WriteLog(SockCommon.ErrorLevel_e.NETWORK, e);
				}

				channel.BodyOutputStream.Dispose();

				SockCommon.IDIssuer.Discard(channel.ID);
			}
		}

		public class SockChannel
		{
			public Socket Handler;
			public int ID;
			public Func<int> Connected;
			public HTTPBodyOutputStream BodyOutputStream;

			// <---- prm

			public bool FirstLineRecving = false;

			/// <summary>
			/// セッションタイムアウト日時
			/// null == INFINITE
			/// </summary>
			public DateTime? SessionTimeoutTime = null;

			/// <summary>
			/// スレッド占用タイムアウト日時
			/// null == リセット状態
			/// </summary>
			public DateTime? ThreadTimeoutTime = null;

			/// <summary>
			/// スレッド占用タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public static int ThreadTimeoutMillis = 100;

			/// <summary>
			/// 無通信タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public int IdleTimeoutMillis = -1;

			private IEnumerable<int> PreRecvSend()
			{
				if (this.SessionTimeoutTime != null && this.SessionTimeoutTime.Value < DateTime.Now)
				{
					throw new Exception("セッション時間切れ");
				}
				if (this.ThreadTimeoutTime == null)
				{
					if (ThreadTimeoutMillis != -1)
						this.ThreadTimeoutTime = DateTime.Now + TimeSpan.FromMilliseconds((double)ThreadTimeoutMillis);
				}
				else if (this.ThreadTimeoutTime.Value < DateTime.Now)
				{
					//SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "スレッド占用タイムアウト"); // 頻出するので抑止

					this.ThreadTimeoutTime = null;
					yield return -1;
				}
			}

			public IEnumerable<int> Recv(int size, Action<byte[]> a_return)
			{
				byte[] data = new byte[size];
				int offset = 0;

				while (1 <= size)
				{
					int? recvSize = null;

					foreach (int relay in this.TryRecv(data, offset, size, ret => recvSize = ret))
						yield return relay;

					size -= recvSize.Value;
					offset += recvSize.Value;
				}
				a_return(data);
			}

			public IEnumerable<int> TryRecv(byte[] data, int offset, int size, Action<int> a_return)
			{
				DateTime startedTime = DateTime.Now;

				for (; ; )
				{
					foreach (int relay in this.PreRecvSend())
						yield return relay;

					try
					{
						int recvSize = SockCommon.NB("recv", () => this.Handler.Receive(data, offset, size, SocketFlags.None));

						if (recvSize <= 0)
						{
							throw new Exception("受信エラー(切断)");
						}
						if (10.0 <= (DateTime.Now - startedTime).TotalSeconds) // 長い無通信時間をモニタする。
						{
							SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, "IDLE-RECV " + (DateTime.Now - startedTime).TotalSeconds.ToString("F3"));
						}
						a_return(recvSize);
						break;
					}
					catch (SocketException e)
					{
						if (e.ErrorCode != 10035)
						{
							throw new Exception("受信エラー", e);
						}
					}
					if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						throw new RecvIdleTimeoutException();
					}
					this.ThreadTimeoutTime = null;
					yield return -1;
				}
				yield return 1;
			}

			/// <summary>
			/// 受信の無通信タイムアウト
			/// </summary>
			public class RecvIdleTimeoutException : Exception
			{ }

			public IEnumerable<int> Send(byte[] data)
			{
				int offset = 0;
				int size = data.Length;

				while (1 <= size)
				{
					int? sentSize = null;

					foreach (int relay in this.TrySend(data, offset, Math.Min(4 * 1024 * 1024, size), ret => sentSize = ret))
						yield return relay;

					size -= sentSize.Value;
					offset += sentSize.Value;
				}
			}

			private IEnumerable<int> TrySend(byte[] data, int offset, int size, Action<int> a_return)
			{
				DateTime startedTime = DateTime.Now;

				for (; ; )
				{
					foreach (int relay in this.PreRecvSend())
						yield return relay;

					try
					{
						int sentSize = SockCommon.NB("send", () => this.Handler.Send(data, offset, size, SocketFlags.None));

						if (sentSize <= 0)
						{
							throw new Exception("送信エラー(切断)");
						}
						if (10.0 <= (DateTime.Now - startedTime).TotalSeconds) // 長い無通信時間をモニタする。
						{
							SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, "IDLE-SEND " + (DateTime.Now - startedTime).TotalSeconds.ToString("F3"));
						}
						a_return(sentSize);
						break;
					}
					catch (SocketException e)
					{
						if (e.ErrorCode != 10035)
						{
							throw new Exception("送信エラー", e);
						}
					}
					if (this.IdleTimeoutMillis != -1 && this.IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						throw new Exception("送信の無通信タイムアウト");
					}
					this.ThreadTimeoutTime = null;
					yield return -1;
				}
				yield return 1;
			}
		}

		public static class SockCommon
		{
			public enum ErrorLevel_e
			{
				INFO = 1,
				WARNING,
				FIRST_LINE_TIMEOUT,
				NETWORK,
				NETWORK_OR_SERVER_LOGIC,
				FATAL,
			}

			public static void WriteLog(ErrorLevel_e errorLevel, object message)
			{
				//if (message is Exception)
				//message = ((Exception)message).Message; // スタックトレース_他のメッセージが見えなくなるので抑止

				switch (errorLevel)
				{
					case ErrorLevel_e.INFO:
						ProcMain.WriteLog(message);
						break;

					case ErrorLevel_e.WARNING:
						ProcMain.WriteLog("[WARNING] " + message);
						break;

					case ErrorLevel_e.FIRST_LINE_TIMEOUT:
						ProcMain.WriteLog("[FIRST-LINE-TIMEOUT]");
						break;

					case ErrorLevel_e.NETWORK:
						ProcMain.WriteLog("[NETWORK] " + message);
						break;

					case ErrorLevel_e.NETWORK_OR_SERVER_LOGIC:
						ProcMain.WriteLog("[NETWORK-SERVER-LOGIC] " + message);
						break;

					case ErrorLevel_e.FATAL:
						ProcMain.WriteLog("[FATAL] " + message);
						break;

					default:
						throw null; // never
				}
			}

			public static T NB<T>(string title, Func<T> routine)
			{
#if true
				return routine();
#else // 遅延モニタ_抑止
				DateTime startedTime = DateTime.Now;
				try
				{
					return routine();
				}
				finally
				{
					double millis = (DateTime.Now - startedTime).TotalMilliseconds;

					const double MILLIS_LIMIT = 50.0;

					if (MILLIS_LIMIT < millis)
						SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, "NB-処理に掛かった時間 " + title + " " + Thread.CurrentThread.ManagedThreadId + " " + millis.ToString("F0"));
				}
#endif
			}

			public static UTF8Check P_UTF8Check = new UTF8Check();

			public class UTF8Check
			{
				private object[] Home = CreateByteCodeBranchTable();

				public UTF8Check()
				{
					for (byte chr = 0x20; chr <= 0x7e; chr++) // ASCII
						this.Add(new byte[] { chr });

					for (byte chr = 0xa1; chr <= 0xdf; chr++) // 半角カナ
						this.Add(Encoding.UTF8.GetBytes(SCommon.ENCODING_SJIS.GetString(new byte[] { chr })));

					foreach (char chr in SCommon.GetJChars()) // 2バイト文字
						this.Add(Encoding.UTF8.GetBytes(new string(new char[] { chr })));
				}

				private void Add(byte[] bytes)
				{
					if (bytes.Length < 1)
						throw null; // never

					object[] curr = this.Home;

					foreach (byte bChr in bytes.Take(bytes.Length - 1))
					{
						int index = (int)bChr;

						if (curr[index] == null)
							curr[index] = CreateByteCodeBranchTable();

						curr = (object[])curr[index];

						if (curr == this.Home)
							throw null; // never
					}

					{
						int index = (int)bytes[bytes.Length - 1];

						if (curr[index] != null)
							if (curr[index] != this.Home) // 同じ文字が追加されることがある。
								throw null; // never

						curr[index] = this.Home;
					}
				}

				private static object[] CreateByteCodeBranchTable()
				{
					return new object[256];
				}

				public bool Check(byte[] bytes)
				{
					object[] curr = this.Home;

					foreach (byte bChr in bytes)
					{
						curr = (object[])curr[(int)bChr];

						if (curr == null)
							return false;
					}
					return curr == this.Home;
				}
			}

			public static class IDIssuer
			{
				private static Queue<int> Stocks = new Queue<int>(Enumerable.Range(1, 9));
				private static int Counter = 10;

				public static int Issue()
				{
					if (Stocks.Count == 0)
						Stocks.Enqueue(Counter++);

					return Stocks.Dequeue();
				}

				public static void Discard(int id)
				{
					Stocks.Enqueue(id);
				}
			}

			public static class TimeWaitMonitor
			{
				// 参考値：
				// 動的ポートの数 16384 (49152 ～ 65535), TIME_WAIT-タイムアウト 4 min (240 sec) の場合 (Windowsの既定値)
				// CTR_ROT_SEC = 60
				// COUNTER_NUM = 5       -- 直近 4 ～ 5 分間の切断回数を保持
				// COUNT_LIMIT = 10000   -- 50 ミリ秒間隔で接続＆切断し続けた場合 4 分間に 4800 回 --> TIME_WAIT 数 14800 (COUNT_LIMIT + 4800) を超えない。
				// - - -
				// 動的ポートの数 64511 (1025 ～ 65535), TIME_WAIT-タイムアウト 1 min (60 sec) の場合 (動的ポート最大)
				// CTR_ROT_SEC = 30
				// COUNTER_NUM = 3       -- 直近 1 ～ 1.5 分間の切断回数を保持
				// COUNT_LIMIT = 60000   -- 50 ミリ秒間隔で接続＆切断し続けた場合 1 分間に 1200 回 --> TIME_WAIT 数 61200 (COUNT_LIMIT + 1200) を超えない。

				private const int CTR_ROT_SEC = 60;
				private const int COUNTER_NUM = 5;
				private const int COUNT_LIMIT = 10000;

				private static int[] Counters = new int[COUNTER_NUM]; // 直近数分間に発生した切断(TIME_WAIT)の回数
				private static int CounterIndex = 0;
				private static DateTime NextRotateTime = GetNextRotateTime();

				public static void Connected()
				{
					KickCounter(0);

					if (COUNT_LIMIT < Counters.Sum()) // ? TIME_WAIT 多すぎ -> 時間当たりの接続数を制限する。-- TIME_WAIT を減らす。
					{
						SockCommon.WriteLog(SockCommon.ErrorLevel_e.WARNING, "PORT-EXHAUSTION");

						Thread.Sleep(50); // HACK: 送受信も止める。
					}
				}

				public static void Disconnect()
				{
					KickCounter(1);
				}

				private static void KickCounter(int valAdd)
				{
					if (NextRotateTime < DateTime.Now)
					{
						CounterIndex++;
						CounterIndex %= Counters.Length;
						Counters[CounterIndex] = valAdd;
						NextRotateTime = GetNextRotateTime();
					}
					else
						Counters[CounterIndex] += valAdd;

					SockCommon.WriteLog(SockCommon.ErrorLevel_e.INFO, "TIME-WAIT-MONITOR: " + valAdd + ", " + Counters.Sum());
				}

				private static DateTime GetNextRotateTime()
				{
					return DateTime.Now + TimeSpan.FromSeconds((double)CTR_ROT_SEC);
				}
			}
		}

		public static class SCommon
		{
			private class S_AnonyDisposable : IDisposable
			{
				private Action Routine;

				public S_AnonyDisposable(Action routine)
				{
					this.Routine = routine;
				}

				public void Dispose()
				{
					if (this.Routine != null)
					{
						this.Routine();
						this.Routine = null;
					}
				}
			}

			public static IDisposable GetAnonyDisposable(Action routine)
			{
				return new S_AnonyDisposable(routine);
			}

			public static int Comp<T>(IList<T> a, IList<T> b, Comparison<T> comp)
			{
				int minlen = Math.Min(a.Count, b.Count);

				for (int index = 0; index < minlen; index++)
				{
					int ret = comp(a[index], b[index]);

					if (ret != 0)
						return ret;
				}
				return Comp(a.Count, b.Count);
			}

			public static int IndexOf<T>(IList<T> list, Predicate<T> match)
			{
				for (int index = 0; index < list.Count; index++)
					if (match(list[index]))
						return index;

				return -1; // not found
			}

			public static void Swap<T>(IList<T> list, int a, int b)
			{
				T tmp = list[a];
				list[a] = list[b];
				list[b] = tmp;
			}

			public static void Swap<T>(ref T a, ref T b)
			{
				T tmp = a;
				a = b;
				b = tmp;
			}

			public static byte[] EMPTY_BYTES = new byte[0];

			public static int Comp(byte a, byte b)
			{
				return (int)a - (int)b;
			}

			public static int Comp(byte[] a, byte[] b)
			{
				return Comp(a, b, Comp);
			}

			public static byte[] GetSubBytes(byte[] src, int offset, int size)
			{
				byte[] dest = new byte[size];
				Array.Copy(src, offset, dest, 0, size);
				return dest;
			}

			public static byte[] ToBytes(int value)
			{
				return ToBytes((uint)value);
			}

			public static int ToInt(byte[] src, int index = 0)
			{
				return (int)ToUInt(src, index);
			}

			public static byte[] ToBytes(uint value)
			{
				byte[] dest = new byte[4];
				ToBytes(value, dest);
				return dest;
			}

			public static void ToBytes(uint value, byte[] dest, int index = 0)
			{
				dest[index + 0] = (byte)((value >> 0) & 0xff);
				dest[index + 1] = (byte)((value >> 8) & 0xff);
				dest[index + 2] = (byte)((value >> 16) & 0xff);
				dest[index + 3] = (byte)((value >> 24) & 0xff);
			}

			public static uint ToUInt(byte[] src, int index = 0)
			{
				return
					((uint)src[index + 0] << 0) |
					((uint)src[index + 1] << 8) |
					((uint)src[index + 2] << 16) |
					((uint)src[index + 3] << 24);
			}

			/// <summary>
			/// バイト列を連結する。
			/// 例：{ BYTE_ARR_1, BYTE_ARR_2, BYTE_ARR_3 } -> BYTE_ARR_1 + BYTE_ARR_2 + BYTE_ARR_3
			/// </summary>
			/// <param name="src">バイト列の引数配列</param>
			/// <returns>連結したバイト列</returns>
			public static byte[] Join(IList<byte[]> src)
			{
				int offset = 0;

				foreach (byte[] block in src)
					offset += block.Length;

				byte[] dest = new byte[offset];
				offset = 0;

				foreach (byte[] block in src)
				{
					Array.Copy(block, 0, dest, offset, block.Length);
					offset += block.Length;
				}
				return dest;
			}

			/// <summary>
			/// バイト列を再分割可能なように連結する。
			/// 再分割するには BinTools.Split を使用すること。
			/// 例：{ BYTE_ARR_1, BYTE_ARR_2, BYTE_ARR_3 } -> SIZE(BYTE_ARR_1) + BYTE_ARR_1 + SIZE(BYTE_ARR_2) + BYTE_ARR_2 + SIZE(BYTE_ARR_3) + BYTE_ARR_3
			/// SIZE(b) は BinTools.ToBytes(b.Length) である。
			/// </summary>
			/// <param name="src">バイト列の引数配列</param>
			/// <returns>連結したバイト列</returns>
			public static byte[] SplittableJoin(IList<byte[]> src)
			{
				int offset = 0;

				foreach (byte[] block in src)
					offset += 4 + block.Length;

				byte[] dest = new byte[offset];
				offset = 0;

				foreach (byte[] block in src)
				{
					Array.Copy(ToBytes(block.Length), 0, dest, offset, 4);
					offset += 4;
					Array.Copy(block, 0, dest, offset, block.Length);
					offset += block.Length;
				}
				return dest;
			}

			/// <summary>
			/// バイト列を再分割する。
			/// </summary>
			/// <param name="src">連結したバイト列</param>
			/// <returns>再分割したバイト列の配列</returns>
			public static byte[][] Split(byte[] src)
			{
				List<byte[]> dest = new List<byte[]>();

				for (int offset = 0; offset < src.Length;)
				{
					int size = ToInt(src, offset);
					offset += 4;
					dest.Add(GetSubBytes(src, offset, size));
					offset += size;
				}
				return dest.ToArray();
			}

			public class Serializer
			{
				public static Serializer I = new Serializer();

				private Serializer()
				{ }

				private const char DELIMITER = ':';

				/// <summary>
				/// 文字列のリストを連結してシリアライズします。
				/// シリアライズされた文字列：
				/// -- 空文字列ではない。
				/// -- 書式 == ^[+/:=0-9A-Za-z]+$
				/// </summary>
				/// <param name="plainStrings">任意の文字列のリスト</param>
				/// <returns>シリアライズされた文字列</returns>
				public string Join(IList<string> plainStrings)
				{
					return DELIMITER + string.Join(string.Empty, plainStrings.Select(plainString => DELIMITER + Encode(plainString)));
				}

				/// <summary>
				/// シリアライズされた文字列から文字列のリストを復元します。
				/// </summary>
				/// <param name="serializedString">シリアライズされた文字列</param>
				/// <returns>元の文字列リスト</returns>
				public string[] Split(string serializedString)
				{
					return serializedString.Split(DELIMITER).Skip(2).Select(encodedString => Decode(encodedString)).ToArray();
				}

				private string Encode(string plainString)
				{
					return SCommon.Base64.I.Encode(Encoding.UTF8.GetBytes(plainString));
				}

				private string Decode(string encodedString)
				{
					return Encoding.UTF8.GetString(SCommon.Base64.I.Decode(encodedString));
				}
			}

			public static Dictionary<string, V> CreateDictionary<V>()
			{
				return new Dictionary<string, V>(new IECompString());
			}

			public static Dictionary<string, V> CreateDictionaryIgnoreCase<V>()
			{
				return new Dictionary<string, V>(new IECompStringIgnoreCase());
			}

			public const double MICRO = 1.0 / IMAX;

			private static void CheckNaN(double value)
			{
				if (double.IsNaN(value))
					throw new Exception("NaN");
			}

			public static double ToRange(double value, double minval, double maxval)
			{
				CheckNaN(value);

				return Math.Max(minval, Math.Min(maxval, value));
			}

			public static int ToInt(double value)
			{
				CheckNaN(value);

				if (value < 0.0)
					return (int)(value - 0.5);
				else
					return (int)(value + 0.5);
			}

			public static long ToLong(double value)
			{
				CheckNaN(value);

				if (value < 0.0)
					return (long)(value - 0.5);
				else
					return (long)(value + 0.5);
			}

			/// <summary>
			/// 列挙の列挙(2次元列挙)を列挙(1次元列挙)に変換する。
			/// 例：{{ A, B, C }, { D, E, F }, { G, H, I }} -> { A, B, C, D, E, F, G, H, I }
			/// 但し Concat(new X[] { AAA, BBB, CCC }) は AAA.Concat(BBB).Concat(CCC) と同じ。
			/// </summary>
			/// <typeparam name="T">要素の型</typeparam>
			/// <param name="src">列挙の列挙(2次元列挙)</param>
			/// <returns>列挙(1次元列挙)</returns>
			public static IEnumerable<T> Concat<T>(IEnumerable<IEnumerable<T>> src)
			{
				foreach (IEnumerable<T> part in src)
					foreach (T element in part)
						yield return element;
			}

			/// <summary>
			/// 列挙をゲッターメソッドでラップします。
			/// 例：{ A, B, C } -> 呼び出し毎に右の順で戻り値を返す { A, B, C, default(T), default(T), default(T), ... }
			/// </summary>
			/// <typeparam name="T">要素の型</typeparam>
			/// <param name="src">列挙</param>
			/// <returns>ゲッターメソッド</returns>
			public static Func<T> Supplier<T>(IEnumerable<T> src)
			{
				IEnumerator<T> reader = src.GetEnumerator();

				return () =>
				{
					if (reader != null)
					{
						if (reader.MoveNext())
							return reader.Current;

						reader.Dispose();
						reader = null;
					}
					return default(T);
				};
			}

			public static T DesertElement<T>(List<T> list, int index)
			{
				T ret = list[index];
				list.RemoveAt(index);
				return ret;
			}

			public static T UnaddElement<T>(List<T> list)
			{
				return DesertElement(list, list.Count - 1);
			}

			public static T FastDesertElement<T>(List<T> list, int index)
			{
				T ret = UnaddElement(list);

				if (index < list.Count)
				{
					T ret2 = list[index];
					list[index] = ret;
					ret = ret2;
				}
				return ret;
			}

			private const int IO_TRY_MAX = 10;

			public static void DeletePath(string path)
			{
				if (string.IsNullOrEmpty(path))
					throw new Exception("削除しようとしたパスは null 又は空文字列です。");

				if (File.Exists(path))
				{
					for (int trycnt = 1; ; trycnt++)
					{
						try
						{
							File.Delete(path);
						}
						catch (Exception ex)
						{
							if (IO_TRY_MAX <= trycnt)
								throw new Exception("ファイルの削除に失敗しました。" + path + "\r\n" + ex);
						}
						if (!File.Exists(path))
							break;

						if (IO_TRY_MAX <= trycnt)
							throw new Exception("ファイルの削除に失敗しました。" + path);

						ProcMain.WriteLog("ファイルの削除をリトライします。" + path);
						Thread.Sleep(trycnt * 100);
					}
				}
				else if (Directory.Exists(path))
				{
					for (int trycnt = 1; ; trycnt++)
					{
						try
						{
							Directory.Delete(path, true);
						}
						catch (Exception ex)
						{
							if (IO_TRY_MAX <= trycnt)
								throw new Exception("ディレクトリの削除に失敗しました。" + path + "\r\n" + ex);
						}
						if (!Directory.Exists(path))
							break;

						if (IO_TRY_MAX <= trycnt)
							throw new Exception("ディレクトリの削除に失敗しました。" + path);

						ProcMain.WriteLog("ディレクトリの削除をリトライします。" + path);
						Thread.Sleep(trycnt * 100);
					}
				}
			}

			public static void CreateDir(string dir)
			{
				if (string.IsNullOrEmpty(dir))
					throw new Exception("作成しようとしたディレクトリは null 又は空文字列です。");

				for (int trycnt = 1; ; trycnt++)
				{
					try
					{
						Directory.CreateDirectory(dir); // ディレクトリが存在するときは何もしない。
					}
					catch (Exception ex)
					{
						if (IO_TRY_MAX <= trycnt)
							throw new Exception("ディレクトリを作成出来ません。" + dir + "\r\n" + ex);
					}
					if (Directory.Exists(dir))
						break;

					if (IO_TRY_MAX <= trycnt)
						throw new Exception("ディレクトリを作成出来ません。" + dir);

					ProcMain.WriteLog("ディレクトリの作成をリトライします。" + dir);
					Thread.Sleep(trycnt * 100);
				}
			}

#if false // 削除予定
			public static string EraseExt(string path)
			{
				return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
			}

			public static string ChangeRoot(string path, string oldRoot, string rootNew)
			{
				return PutYen(rootNew) + ChangeRoot(path, oldRoot);
			}

			public static string ChangeRoot(string path, string oldRoot)
			{
				oldRoot = PutYen(oldRoot);

				if (!StartsWithIgnoreCase(path, oldRoot))
					throw new Exception("パスの配下ではありません。" + oldRoot + " -> " + path);

				return path.Substring(oldRoot.Length);
			}

			public static string PutYen(string path)
			{
				return Put_INE(path, "\\");
			}

			private static string Put_INE(string str, string endPtn)
			{
				if (!str.EndsWith(endPtn))
					str += endPtn;

				return str;
			}

			/// <summary>
			/// 厳しいフルパス化
			/// </summary>
			/// <param name="path">パス</param>
			/// <returns>フルパス</returns>
			public static string MakeFullPath(string path)
			{
				if (path == null)
					throw new Exception("パスが定義されていません。(null)");

				if (path == "")
					throw new Exception("パスが定義されていません。(空文字列)");

				path = Path.GetFullPath(path);

				if (path.Contains('/')) // Path.GetFullPath が '/' を '\\' に置換するはず。
					throw null;

				if (path.StartsWith("\\\\"))
					throw new Exception("ネットワークパスまたはデバイス名は使用出来ません。");

				if (path.Substring(1, 2) != ":\\") // ネットワークパスでないならローカルパスのはず。
					throw null;

				path = PutYen(path) + ".";
				path = Path.GetFullPath(path);

				return path;
			}

			/// <summary>
			/// ゆるいフルパス化
			/// </summary>
			/// <param name="path">パス</param>
			/// <returns>フルパス</returns>
			public static string ToFullPath(string path)
			{
				path = Path.GetFullPath(path);
				path = PutYen(path) + ".";
				path = Path.GetFullPath(path);

				return path;
			}
#endif

			public static byte[] Read(FileStream reader, int size)
			{
				byte[] buff = new byte[size];
				Read(reader, buff);
				return buff;
			}

			public static void Read(FileStream reader, byte[] buff, int offset = 0)
			{
				Read(reader, buff, offset, buff.Length - offset);
			}

			public static void Read(FileStream reader, byte[] buff, int offset, int count)
			{
				if (reader.Read(buff, offset, count) != count)
				{
					throw new Exception("データの途中でファイルの終端に到達しました。");
				}
			}

			public static void Write(FileStream writer, byte[] buff, int offset = 0)
			{
				writer.Write(buff, offset, buff.Length - offset);
			}

			/// <summary>
			/// 行リストをテキストに変換します。
			/// </summary>
			/// <param name="lines">行リスト</param>
			/// <returns>テキスト</returns>
			public static string LinesToText(IList<string> lines)
			{
				return lines.Count == 0 ? "" : string.Join("\r\n", lines) + "\r\n";
			}

			/// <summary>
			/// テキストを行リストに変換します。
			/// </summary>
			/// <param name="text">テキスト</param>
			/// <returns>行リスト</returns>
			public static string[] TextToLines(string text)
			{
				text = text.Replace("\r", "");

				string[] lines = text.Split('\n');

				if (1 <= lines.Length && lines[lines.Length - 1] == "")
				{
					lines = lines.Take(lines.Length - 1).ToArray();
				}
				return lines;
			}

			/// <summary>
			/// ファイル読み込みハンドルっぽいコールバック
			/// </summary>
			/// <param name="buff">読み込んだデータの書き込み先</param>
			/// <param name="offset">書き込み開始位置</param>
			/// <param name="count">書き込みサイズ</param>
			/// <returns>実際に読み込んだサイズ(1～), ～0 == これ以上読み込めない</returns>
			public delegate int Read_d(byte[] buff, int offset, int count);

			/// <summary>
			/// ファイル書き込みハンドルっぽいコールバック
			/// </summary>
			/// <param name="buff">書き込むデータの読み込み先</param>
			/// <param name="offset">読み込み開始位置</param>
			/// <param name="count">読み込みサイズ</param>
			public delegate void Write_d(byte[] buff, int offset, int count);

			public static void ReadToEnd(Read_d reader, Write_d writer)
			{
				byte[] buff = new byte[2 * 1024 * 1024];

				for (; ; )
				{
					int readSize = reader(buff, 0, buff.Length);

					if (readSize <= 0)
						break;

					writer(buff, 0, readSize);
				}
			}

			public const int IMAX = 1000000000; // 10^9
			public const long IMAX_64 = 1000000000000000000L; // 10^18

			public static int Comp(int a, int b)
			{
				if (a < b)
					return -1;

				if (a > b)
					return 1;

				return 0;
			}

			public static int Comp(long a, long b)
			{
				if (a < b)
					return -1;

				if (a > b)
					return 1;

				return 0;
			}

			public static int Comp(double a, double b)
			{
				if (a < b)
					return -1;

				if (a > b)
					return 1;

				return 0;
			}

			public static int ToRange(int value, int minval, int maxval)
			{
				return Math.Max(minval, Math.Min(maxval, value));
			}

			public static long ToRange(long value, long minval, long maxval)
			{
				return Math.Max(minval, Math.Min(maxval, value));
			}

			public static bool IsRange(int value, int minval, int maxval)
			{
				return minval <= value && value <= maxval;
			}

			public static bool IsRange(long value, long minval, long maxval)
			{
				return minval <= value && value <= maxval;
			}

			public static bool IsRange(double value, double minval, double maxval)
			{
				return minval <= value && value <= maxval;
			}

			public static int ToInt(string str, int minval, int maxval, int defval)
			{
				try
				{
					int value = int.Parse(str);

					if (value < minval || maxval < value)
						throw null;

					return value;
				}
				catch
				{
					return defval;
				}
			}

			public static long ToLong(string str, long minval, long maxval, long defval)
			{
				try
				{
					long value = long.Parse(str);

					if (value < minval || maxval < value)
						throw null;

					return value;
				}
				catch
				{
					return defval;
				}
			}

			public static string ToJString(byte[] src, bool okJpn, bool okRet, bool okTab, bool okSpc)
			{
				if (src == null)
					src = new byte[0];

				using (MemoryStream dest = new MemoryStream())
				{
					for (int index = 0; index < src.Length; index++)
					{
						byte chr = src[index];

						if (chr == 0x09) // ? '\t'
						{
							if (!okTab)
								continue;
						}
						else if (chr == 0x0a) // ? '\n'
						{
							if (!okRet)
								continue;
						}
						else if (chr < 0x20) // ? other control code
						{
							continue;
						}
						else if (chr == 0x20) // ? ' '
						{
							if (!okSpc)
								continue;
						}
						else if (chr <= 0x7e) // ? ascii
						{
							// noop
						}
						else if (0xa1 <= chr && chr <= 0xdf) // ? kana
						{
							if (!okJpn)
								continue;
						}
						else // ? 全角文字の前半 || 破損
						{
							if (!okJpn)
								continue;

							index++;

							if (src.Length <= index) // ? 後半欠損
								break;

							if (!S_JChar.I.Contains(chr, src[index])) // ? 破損
								continue;

							dest.WriteByte(chr);
							chr = src[index];
						}
						dest.WriteByte(chr);
					}
					return ENCODING_SJIS.GetString(dest.ToArray());
				}
			}

			/// <summary>
			/// SJIS(CP-932)の2バイト文字を全て返す。
			/// </summary>
			/// <returns>SJIS(CP-932)の2バイト文字の文字列</returns>
			public static string GetJChars()
			{
				return ENCODING_SJIS.GetString(GetJCharBytes().ToArray());
			}

			/// <summary>
			/// SJIS(CP-932)の2バイト文字を全て返す。
			/// </summary>
			/// <returns>SJIS(CP-932)の2バイト文字のバイト列</returns>
			public static IEnumerable<byte> GetJCharBytes()
			{
				foreach (UInt16 chr in GetJCharCodes())
				{
					yield return (byte)(chr >> 8);
					yield return (byte)(chr & 0xff);
				}
			}

			/// <summary>
			/// SJIS(CP-932)の2バイト文字を全て返す。
			/// </summary>
			/// <returns>SJIS(CP-932)の2バイト文字の列挙</returns>
			public static IEnumerable<UInt16> GetJCharCodes()
			{
				for (UInt16 chr = S_JChar.CHR_MIN; chr <= S_JChar.CHR_MAX; chr++)
					if (S_JChar.I.Contains(chr))
						yield return chr;
			}

			private class S_JChar
			{
				private static S_JChar _i = null;

				public static S_JChar I
				{
					get
					{
						if (_i == null)
							_i = new S_JChar();

						return _i;
					}
				}

				private UInt64[] ChrMap = new UInt64[0x10000 / 64];

				private S_JChar()
				{
					this.Add();
				}

				public const UInt16 CHR_MIN = 0x8140;
				public const UInt16 CHR_MAX = 0xfc4b;

				#region Add Method

				/// <summary>
				/// generated by https://github.com/stackprobe/Factory/blob/master/Labo/GenData/IsJChar.c
				/// </summary>
				private void Add()
				{
					this.Add(0x8140, 0x817e);
					this.Add(0x8180, 0x81ac);
					this.Add(0x81b8, 0x81bf);
					this.Add(0x81c8, 0x81ce);
					this.Add(0x81da, 0x81e8);
					this.Add(0x81f0, 0x81f7);
					this.Add(0x81fc, 0x81fc);
					this.Add(0x824f, 0x8258);
					this.Add(0x8260, 0x8279);
					this.Add(0x8281, 0x829a);
					this.Add(0x829f, 0x82f1);
					this.Add(0x8340, 0x837e);
					this.Add(0x8380, 0x8396);
					this.Add(0x839f, 0x83b6);
					this.Add(0x83bf, 0x83d6);
					this.Add(0x8440, 0x8460);
					this.Add(0x8470, 0x847e);
					this.Add(0x8480, 0x8491);
					this.Add(0x849f, 0x84be);
					this.Add(0x8740, 0x875d);
					this.Add(0x875f, 0x8775);
					this.Add(0x877e, 0x877e);
					this.Add(0x8780, 0x879c);
					this.Add(0x889f, 0x88fc);
					this.Add(0x8940, 0x897e);
					this.Add(0x8980, 0x89fc);
					this.Add(0x8a40, 0x8a7e);
					this.Add(0x8a80, 0x8afc);
					this.Add(0x8b40, 0x8b7e);
					this.Add(0x8b80, 0x8bfc);
					this.Add(0x8c40, 0x8c7e);
					this.Add(0x8c80, 0x8cfc);
					this.Add(0x8d40, 0x8d7e);
					this.Add(0x8d80, 0x8dfc);
					this.Add(0x8e40, 0x8e7e);
					this.Add(0x8e80, 0x8efc);
					this.Add(0x8f40, 0x8f7e);
					this.Add(0x8f80, 0x8ffc);
					this.Add(0x9040, 0x907e);
					this.Add(0x9080, 0x90fc);
					this.Add(0x9140, 0x917e);
					this.Add(0x9180, 0x91fc);
					this.Add(0x9240, 0x927e);
					this.Add(0x9280, 0x92fc);
					this.Add(0x9340, 0x937e);
					this.Add(0x9380, 0x93fc);
					this.Add(0x9440, 0x947e);
					this.Add(0x9480, 0x94fc);
					this.Add(0x9540, 0x957e);
					this.Add(0x9580, 0x95fc);
					this.Add(0x9640, 0x967e);
					this.Add(0x9680, 0x96fc);
					this.Add(0x9740, 0x977e);
					this.Add(0x9780, 0x97fc);
					this.Add(0x9840, 0x9872);
					this.Add(0x989f, 0x98fc);
					this.Add(0x9940, 0x997e);
					this.Add(0x9980, 0x99fc);
					this.Add(0x9a40, 0x9a7e);
					this.Add(0x9a80, 0x9afc);
					this.Add(0x9b40, 0x9b7e);
					this.Add(0x9b80, 0x9bfc);
					this.Add(0x9c40, 0x9c7e);
					this.Add(0x9c80, 0x9cfc);
					this.Add(0x9d40, 0x9d7e);
					this.Add(0x9d80, 0x9dfc);
					this.Add(0x9e40, 0x9e7e);
					this.Add(0x9e80, 0x9efc);
					this.Add(0x9f40, 0x9f7e);
					this.Add(0x9f80, 0x9ffc);
					this.Add(0xe040, 0xe07e);
					this.Add(0xe080, 0xe0fc);
					this.Add(0xe140, 0xe17e);
					this.Add(0xe180, 0xe1fc);
					this.Add(0xe240, 0xe27e);
					this.Add(0xe280, 0xe2fc);
					this.Add(0xe340, 0xe37e);
					this.Add(0xe380, 0xe3fc);
					this.Add(0xe440, 0xe47e);
					this.Add(0xe480, 0xe4fc);
					this.Add(0xe540, 0xe57e);
					this.Add(0xe580, 0xe5fc);
					this.Add(0xe640, 0xe67e);
					this.Add(0xe680, 0xe6fc);
					this.Add(0xe740, 0xe77e);
					this.Add(0xe780, 0xe7fc);
					this.Add(0xe840, 0xe87e);
					this.Add(0xe880, 0xe8fc);
					this.Add(0xe940, 0xe97e);
					this.Add(0xe980, 0xe9fc);
					this.Add(0xea40, 0xea7e);
					this.Add(0xea80, 0xeaa4);
					this.Add(0xed40, 0xed7e);
					this.Add(0xed80, 0xedfc);
					this.Add(0xee40, 0xee7e);
					this.Add(0xee80, 0xeeec);
					this.Add(0xeeef, 0xeefc);
					this.Add(0xfa40, 0xfa7e);
					this.Add(0xfa80, 0xfafc);
					this.Add(0xfb40, 0xfb7e);
					this.Add(0xfb80, 0xfbfc);
					this.Add(0xfc40, 0xfc4b);
				}

				#endregion

				private void Add(UInt16 bgn, UInt16 end)
				{
					for (UInt16 chr = bgn; chr <= end; chr++)
					{
						this.Add(chr);
					}
				}

				private void Add(UInt16 chr)
				{
					this.ChrMap[chr / 64] |= (UInt64)1 << (chr % 64);
				}

				public bool Contains(byte lead, byte trail)
				{
					UInt16 chr = lead;

					chr <<= 8;
					chr |= trail;

					return Contains(chr);
				}

				public bool Contains(UInt16 chr)
				{
					return (this.ChrMap[chr / 64] & (UInt64)1 << (chr % 64)) != (UInt64)0;
				}
			}

#if false // 削除予定
			public static RandomUnit CRandom = new RandomUnit(new S_CSPRandomNumberGenerator());

			private class S_CSPRandomNumberGenerator : RandomUnit.IRandomNumberGenerator
			{
				private RandomNumberGenerator Rng = new RNGCryptoServiceProvider();
				private byte[] Cache = new byte[4096];

				public byte[] GetBlock()
				{
					this.Rng.GetBytes(this.Cache);
					return this.Cache;
				}

				public void Dispose()
				{
					if (this.Rng != null)
					{
						this.Rng.Dispose();
						this.Rng = null;
					}
				}
			}

			public static byte[] GetSHA512(byte[] src)
			{
				using (SHA512 sha512 = SHA512.Create())
				{
					return sha512.ComputeHash(src);
				}
			}
#endif

			public static class Hex
			{
				public static string ToString(byte[] src)
				{
					StringBuilder buff = new StringBuilder(src.Length * 2);

					foreach (byte chr in src)
					{
						buff.Append(hexadecimal[chr >> 4]);
						buff.Append(hexadecimal[chr & 0x0f]);
					}
					return buff.ToString();
				}

				public static byte[] ToBytes(string src)
				{
					if (src.Length % 2 != 0)
						throw new ArgumentException("入力文字列の長さに問題があります。");

					byte[] dest = new byte[src.Length / 2];

					for (int index = 0; index < dest.Length; index++)
					{
						int hi = To4Bit(src[index * 2 + 0]);
						int lw = To4Bit(src[index * 2 + 1]);

						dest[index] = (byte)((hi << 4) | lw);
					}
					return dest;
				}

				private static int To4Bit(char chr)
				{
					int ret = hexadecimal.IndexOf(char.ToLower(chr));

					if (ret == -1)
						throw new ArgumentException("入力文字列に含まれる文字に問題があります。");

					return ret;
				}
			}

			public static string[] EMPTY_STRINGS = new string[0];

			public static EncodingShiftJIS ENCODING_SJIS = new EncodingShiftJIS();

			public static string BINADECIMAL = "01";
			public static string OCTODECIMAL = "012234567";
			public static string DECIMAL = "0123456789";
			public static string HEXADECIMAL = "0123456789ABCDEF";
			public static string hexadecimal = "0123456789abcdef";

			public static string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			public static string alpha = "abcdefghijklmnopqrstuvwxyz";
			public static string PUNCT =
				S_GetString_SJISHalfCodeRange(0x21, 0x2f) +
				S_GetString_SJISHalfCodeRange(0x3a, 0x40) +
				S_GetString_SJISHalfCodeRange(0x5b, 0x60) +
				S_GetString_SJISHalfCodeRange(0x7b, 0x7e);

			public static string ASCII = DECIMAL + ALPHA + alpha + PUNCT; // == GetString_SJISHalfCodeRange(0x21, 0x7e)
			public static string KANA = S_GetString_SJISHalfCodeRange(0xa1, 0xdf);

			public static string HALF = ASCII + KANA;

			private static string S_GetString_SJISHalfCodeRange(int codeMin, int codeMax)
			{
				byte[] buff = new byte[codeMax - codeMin + 1];

				for (int code = codeMin; code <= codeMax; code++)
				{
					buff[code - codeMin] = (byte)code;
				}
				return ENCODING_SJIS.GetString(buff);
			}

			public static string MBC_DECIMAL = S_GetString_SJISCodeRange(0x82, 0x4f, 0x58);
			public static string MBC_ALPHA = S_GetString_SJISCodeRange(0x82, 0x60, 0x79);
			public static string mbc_alpha = S_GetString_SJISCodeRange(0x82, 0x81, 0x9a);
			public static string MBC_SPACE = S_GetString_SJISCodeRange(0x81, 0x40, 0x40);
			public static string MBC_PUNCT =
				S_GetString_SJISCodeRange(0x81, 0x41, 0x7e) +
				S_GetString_SJISCodeRange(0x81, 0x80, 0xac) +
				S_GetString_SJISCodeRange(0x81, 0xb8, 0xbf) + // 集合
				S_GetString_SJISCodeRange(0x81, 0xc8, 0xce) + // 論理
				S_GetString_SJISCodeRange(0x81, 0xda, 0xe8) + // 数学
				S_GetString_SJISCodeRange(0x81, 0xf0, 0xf7) +
				S_GetString_SJISCodeRange(0x81, 0xfc, 0xfc) +
				S_GetString_SJISCodeRange(0x83, 0x9f, 0xb6) + // ギリシャ語大文字
				S_GetString_SJISCodeRange(0x83, 0xbf, 0xd6) + // ギリシャ語小文字
				S_GetString_SJISCodeRange(0x84, 0x40, 0x60) + // キリル文字大文字
				S_GetString_SJISCodeRange(0x84, 0x70, 0x7e) + // キリル文字小文字(1)
				S_GetString_SJISCodeRange(0x84, 0x80, 0x91) + // キリル文字小文字(2)
				S_GetString_SJISCodeRange(0x84, 0x9f, 0xbe) + // 枠線
				S_GetString_SJISCodeRange(0x87, 0x40, 0x5d) + // 機種依存文字(1)
				S_GetString_SJISCodeRange(0x87, 0x5f, 0x75) + // 機種依存文字(2)
				S_GetString_SJISCodeRange(0x87, 0x7e, 0x7e) + // 機種依存文字(3)
				S_GetString_SJISCodeRange(0x87, 0x80, 0x9c) + // 機種依存文字(4)
				S_GetString_SJISCodeRange(0xee, 0xef, 0xfc); // 機種依存文字(5)

			public static string MBC_CHOUONPU = S_GetString_SJISCodeRange(0x81, 0x5b, 0x5b); // 815b == 長音符 -- ひらがなとカタカナの長音符は同じ文字

			public static string MBC_HIRA = S_GetString_SJISCodeRange(0x82, 0x9f, 0xf1);
			public static string MBC_KANA =
				S_GetString_SJISCodeRange(0x83, 0x40, 0x7e) +
				S_GetString_SJISCodeRange(0x83, 0x80, 0x96) + MBC_CHOUONPU;

			private static string S_GetString_SJISCodeRange(int lead, int trailMin, int trailMax)
			{
				byte[] buff = new byte[(trailMax - trailMin + 1) * 2];

				for (int trail = trailMin; trail <= trailMax; trail++)
				{
					buff[(trail - trailMin) * 2 + 0] = (byte)lead;
					buff[(trail - trailMin) * 2 + 1] = (byte)trail;
				}
				return ENCODING_SJIS.GetString(buff);
			}

			public static int Comp(string a, string b)
			{
				// MEMO: a.CompareTo(b) -- 三すくみの一件以来今でも信用出来ないので使わない。

				return Comp(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
			}

			public static int CompIgnoreCase(string a, string b)
			{
				return Comp(a.ToLower(), b.ToLower());
			}

			public class IECompString : IEqualityComparer<string>
			{
				public bool Equals(string a, string b)
				{
					return a == b;
				}

				public int GetHashCode(string a)
				{
					return a.GetHashCode();
				}
			}

			public class IECompStringIgnoreCase : IEqualityComparer<string>
			{
				public bool Equals(string a, string b)
				{
					return a.ToLower() == b.ToLower();
				}

				public int GetHashCode(string a)
				{
					return a.ToLower().GetHashCode();
				}
			}

			public static bool EqualsIgnoreCase(string a, string b)
			{
				return a.ToLower() == b.ToLower();
			}

			public static bool StartsWithIgnoreCase(string str, string ptn)
			{
				return str.ToLower().StartsWith(ptn.ToLower());
			}

			public static bool EndsWithIgnoreCase(string str, string ptn)
			{
				return str.ToLower().EndsWith(ptn.ToLower());
			}

			public static bool ContainsIgnoreCase(string str, string ptn)
			{
				return str.ToLower().Contains(ptn.ToLower());
			}

			public static int IndexOfIgnoreCase(string str, string ptn)
			{
				return str.ToLower().IndexOf(ptn.ToLower());
			}

			public static int IndexOfIgnoreCase(string str, char chr)
			{
				return str.ToLower().IndexOf(char.ToLower(chr));
			}

			public static int IndexOf(IList<string> strs, string str)
			{
				for (int index = 0; index < strs.Count; index++)
					if (strs[index] == str)
						return index;

				return -1; // not found
			}

			public static int IndexOfIgnoreCase(IList<string> strs, string str)
			{
				string lStr = str.ToLower();

				for (int index = 0; index < strs.Count; index++)
					if (strs[index].ToLower() == lStr)
						return index;

				return -1; // not found
			}

			/// <summary>
			/// 文字列を区切り文字で分割する。
			/// </summary>
			/// <param name="str">文字列</param>
			/// <param name="delimiters">区切り文字の集合</param>
			/// <param name="meaningFlag">区切り文字(delimiters)以外を区切り文字とするか</param>
			/// <param name="ignoreEmpty">空文字列のトークンを除去するか</param>
			/// <param name="limit">最大トークン数(1～), 0 == 無制限</param>
			/// <returns>トークン配列</returns>
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
							tokens.Add(buff.ToString());

						buff = new StringBuilder();
					}
				}
				if (!ignoreEmpty || buff.Length != 0)
					tokens.Add(buff.ToString());

				return tokens.ToArray();
			}

			public static bool HasSameChar(string str)
			{
				for (int r = 1; r < str.Length; r++)
					for (int l = 0; l < r; l++)
						if (str[l] == str[r])
							return true;

				return false;
			}

			public static string[] ParseIsland(string text, string singleTag, bool ignoreCase = false)
			{
				int start;

				if (ignoreCase)
					start = text.ToLower().IndexOf(singleTag.ToLower());
				else
					start = text.IndexOf(singleTag);

				if (start == -1)
					return null;

				int end = start + singleTag.Length;

				return new string[]
				{
				text.Substring(0, start),
				text.Substring(start, end - start),
				text.Substring(end),
				};
			}

			public static string[] ParseEnclosed(string text, string openTag, string closeTag, bool ignoreCase = false)
			{
				string[] starts = ParseIsland(text, openTag, ignoreCase);

				if (starts == null)
					return null;

				string[] ends = ParseIsland(starts[2], closeTag, ignoreCase);

				if (ends == null)
					return null;

				return new string[]
				{
				starts[0],
				starts[1],
				ends[0],
				ends[1],
				ends[2],
				};
			}

#if false // 削除予定
			public static byte[] Compress(byte[] src)
			{
				using (MemoryStream reader = new MemoryStream(src))
				using (MemoryStream writer = new MemoryStream())
				{
					Compress(reader, writer);
					return writer.ToArray();
				}
			}

			public static byte[] Decompress(byte[] src, int limit = -1)
			{
				using (MemoryStream reader = new MemoryStream(src))
				using (MemoryStream writer = new MemoryStream())
				{
					Decompress(reader, writer, (long)limit);
					return writer.ToArray();
				}
			}

			public static void Compress(string rFile, string wFile)
			{
				using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
				using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
				{
					Compress(reader, writer);
				}
			}

			public static void Decompress(string rFile, string wFile, long limit = -1L)
			{
				using (FileStream reader = new FileStream(rFile, FileMode.Open, FileAccess.Read))
				using (FileStream writer = new FileStream(wFile, FileMode.Create, FileAccess.Write))
				{
					Decompress(reader, writer, limit);
				}
			}

			public static void Compress(Stream reader, Stream writer)
			{
				using (GZipStream gz = new GZipStream(writer, CompressionMode.Compress))
				{
					reader.CopyTo(gz);
				}
			}

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
						ReadToEnd(gz.Read, GetLimitedWriter(writer.Write, limit));
					}
				}
			}

			public static Write_d GetLimitedWriter(Write_d writer, long remaining)
			{
				return (buff, offset, count) =>
				{
					if (remaining < (long)count)
						throw new Exception("ストリームに書き込めるバイト数の上限を超えようとしました。");

					remaining -= (long)count;
					writer(buff, offset, count);
				};
			}

			public static string[] Batch(IList<string> commands, string workingDir = "", StartProcessWindowStyle_e winStyle = StartProcessWindowStyle_e.INVISIBLE)
			{
				using (WorkingDir wd = new WorkingDir())
				{
					string batFile = wd.GetPath("Run.bat");
					string outFile = wd.GetPath("Run.out");
					string callBatFile = wd.GetPath("Call.bat");

					File.WriteAllLines(batFile, commands, ENCODING_SJIS);
					File.WriteAllText(callBatFile, "> " + outFile + " CALL " + batFile, ENCODING_SJIS);

					StartProcess("cmd", "/c " + callBatFile, workingDir, winStyle).WaitForExit();

					// batFile 終了待ち
					// -- どうやら MSBuild が終わる前に WaitForExit() が制御を返しているっぽい。@ 2020.11.10 -- t20201110_ConfuserElsa
					{
						//int millis = 0;
						int millis = 100;

						for (; ; )
						{
							try
							{
								File.ReadAllBytes(outFile); // 読み込みテスト
								break;
							}
							catch
							{ }

							Console.WriteLine("プロセス終了待ち");

							if (millis < 2000)
								//millis++;
								millis += 100;

							Thread.Sleep(millis);
						}
					}

					return File.ReadAllLines(outFile, ENCODING_SJIS);
				}
			}

			public enum StartProcessWindowStyle_e
			{
				INVISIBLE = 1,
				MINIMIZED,
				NORMAL,
			};

			public static Process StartProcess(string file, string args, string workingDir = "", StartProcessWindowStyle_e winStyle = StartProcessWindowStyle_e.INVISIBLE)
			{
				ProcessStartInfo psi = new ProcessStartInfo();

				psi.FileName = file;
				psi.Arguments = args;
				psi.WorkingDirectory = workingDir; // 既定値 == ""

				switch (winStyle)
				{
					case StartProcessWindowStyle_e.INVISIBLE:
						psi.CreateNoWindow = true;
						psi.UseShellExecute = false;
						break;

					case StartProcessWindowStyle_e.MINIMIZED:
						psi.CreateNoWindow = false;
						psi.UseShellExecute = true;
						psi.WindowStyle = ProcessWindowStyle.Minimized;
						break;

					case StartProcessWindowStyle_e.NORMAL:
						break;

					default:
						throw null;
				}
				return Process.Start(psi);
			}
#endif

			#region Base64

			public class Base64
			{
				private static Base64 _i = null;

				public static Base64 I
				{
					get
					{
						if (_i == null)
							_i = new Base64();

						return _i;
					}
				}

				private char[] Chars;
				private byte[] CharMap;

				private const char CHAR_PADDING = '=';

				private Base64()
				{
					this.Chars = (SCommon.ALPHA + SCommon.alpha + SCommon.DECIMAL + "+/").ToArray();
					this.CharMap = new byte[(int)char.MaxValue + 1];

					for (int index = 0; index < 64; index++)
						this.CharMap[this.Chars[index]] = (byte)index;
				}

				public string Encode(byte[] src)
				{
					char[] dest = new char[((src.Length + 2) / 3) * 4];
					int writer = 0;
					int index = 0;
					int chr;

					while (index + 3 <= src.Length)
					{
						chr = (src[index++] & 0xff) << 16;
						chr |= (src[index++] & 0xff) << 8;
						chr |= src[index++] & 0xff;
						dest[writer++] = this.Chars[chr >> 18];
						dest[writer++] = this.Chars[(chr >> 12) & 0x3f];
						dest[writer++] = this.Chars[(chr >> 6) & 0x3f];
						dest[writer++] = this.Chars[chr & 0x3f];
					}
					if (index + 2 == src.Length)
					{
						chr = (src[index++] & 0xff) << 8;
						chr |= src[index++] & 0xff;
						dest[writer++] = this.Chars[chr >> 10];
						dest[writer++] = this.Chars[(chr >> 4) & 0x3f];
						dest[writer++] = this.Chars[(chr << 2) & 0x3c];
						dest[writer++] = CHAR_PADDING;
					}
					else if (index + 1 == src.Length)
					{
						chr = src[index++] & 0xff;
						dest[writer++] = this.Chars[chr >> 2];
						dest[writer++] = this.Chars[(chr << 4) & 0x30];
						dest[writer++] = CHAR_PADDING;
						dest[writer++] = CHAR_PADDING;
					}
					return new string(dest);
				}

				public byte[] Decode(string src)
				{
					int destSize = (src.Length / 4) * 3;

					if (destSize != 0)
					{
						if (src[src.Length - 2] == CHAR_PADDING)
						{
							destSize -= 2;
						}
						else if (src[src.Length - 1] == CHAR_PADDING)
						{
							destSize--;
						}
					}
					byte[] dest = new byte[destSize];
					int writer = 0;
					int index = 0;
					int chr;

					while (writer + 3 <= destSize)
					{
						chr = (this.CharMap[src[index++]] & 0x3f) << 18;
						chr |= (this.CharMap[src[index++]] & 0x3f) << 12;
						chr |= (this.CharMap[src[index++]] & 0x3f) << 6;
						chr |= this.CharMap[src[index++]] & 0x3f;
						dest[writer++] = (byte)(chr >> 16);
						dest[writer++] = (byte)((chr >> 8) & 0xff);
						dest[writer++] = (byte)(chr & 0xff);
					}
					if (writer + 2 == destSize)
					{
						chr = (this.CharMap[src[index++]] & 0x3f) << 10;
						chr |= (this.CharMap[src[index++]] & 0x3f) << 4;
						chr |= (this.CharMap[src[index++]] & 0x3c) >> 2;
						dest[writer++] = (byte)(chr >> 8);
						dest[writer++] = (byte)(chr & 0xff);
					}
					else if (writer + 1 == destSize)
					{
						chr = (this.CharMap[src[index++]] & 0x3f) << 2;
						chr |= (this.CharMap[src[index++]] & 0x30) >> 4;
						dest[writer++] = (byte)chr;
					}
					return dest;
				}
			}

			#endregion

			#region TimeStampToSec

			/// <summary>
			/// 日時を 1/1/1 00:00:00 からの経過秒数に変換およびその逆を行います。
			/// 日時のフォーマット
			/// -- YMMDDhhmmss
			/// -- YYMMDDhhmmss
			/// -- YYYMMDDhhmmss
			/// -- YYYYMMDDhhmmss
			/// -- YYYYYMMDDhhmmss
			/// -- YYYYYYMMDDhhmmss
			/// -- YYYYYYYMMDDhhmmss
			/// -- YYYYYYYYMMDDhhmmss
			/// -- YYYYYYYYYMMDDhhmmss -- 但し YYYYYYYYY == 100000000 ～ 922337203
			/// ---- 年の桁数は 1 ～ 9 桁
			/// 日時の範囲
			/// -- 最小 1/1/1 00:00:00
			/// -- 最大 922337203/12/31 23:59:59
			/// </summary>
			public static class TimeStampToSec
			{
				private const int YEAR_MIN = 1;
				private const int YEAR_MAX = 922337203;

				private const long TIME_STAMP_MIN = 10101000000L;
				private const long TIME_STAMP_MAX = 9223372031231235959L;

				public static long ToSec(long timeStamp)
				{
					if (timeStamp < TIME_STAMP_MIN || TIME_STAMP_MAX < timeStamp)
						return 0L;

					int s = (int)(timeStamp % 100L);
					timeStamp /= 100L;
					int i = (int)(timeStamp % 100L);
					timeStamp /= 100L;
					int h = (int)(timeStamp % 100L);
					timeStamp /= 100L;
					int d = (int)(timeStamp % 100L);
					timeStamp /= 100L;
					int m = (int)(timeStamp % 100L);
					int y = (int)(timeStamp / 100L);

					if (
						//y < YEAR_MIN || YEAR_MAX < y ||
						m < 1 || 12 < m ||
						d < 1 || 31 < d ||
						h < 0 || 23 < h ||
						i < 0 || 59 < i ||
						s < 0 || 59 < s
						)
						return 0L;

					if (m <= 2)
						y--;

					long ret = y / 400;
					ret *= 365 * 400 + 97;

					y %= 400;

					ret += y * 365;
					ret += y / 4;
					ret -= y / 100;

					if (2 < m)
					{
						ret -= 31 * 10 - 4;
						m -= 3;
						ret += (m / 5) * (31 * 5 - 2);
						m %= 5;
						ret += (m / 2) * (31 * 2 - 1);
						m %= 2;
						ret += m * 31;
					}
					else
						ret += (m - 1) * 31;

					ret += d - 1;
					ret *= 24;
					ret += h;
					ret *= 60;
					ret += i;
					ret *= 60;
					ret += s;

					return ret;
				}

				public static long ToTimeStamp(long sec)
				{
					if (sec < 0L)
						return TIME_STAMP_MIN;

					int s = (int)(sec % 60L);
					sec /= 60L;
					int i = (int)(sec % 60L);
					sec /= 60L;
					int h = (int)(sec % 24L);
					sec /= 24L;

					int day = (int)(sec % 146097);
					sec /= 146097;
					sec *= 400;
					sec++;

					if (YEAR_MAX < sec)
						return TIME_STAMP_MAX;

					int y = (int)sec;
					int m = 1;
					int d;

					day += Math.Min((day + 306) / 36524, 3);
					y += (day / 1461) * 4;
					day %= 1461;

					day += Math.Min((day + 306) / 365, 3);
					y += day / 366;
					day %= 366;

					if (60 <= day)
					{
						m += 2;
						day -= 60;
						m += (day / 153) * 5;
						day %= 153;
						m += (day / 61) * 2;
						day %= 61;
					}
					m += day / 31;
					day %= 31;
					d = day + 1;

					if (y < YEAR_MIN)
						return TIME_STAMP_MIN;

					if (YEAR_MAX < y)
						return TIME_STAMP_MAX;

					if (
						//y < YEAR_MIN || YEAR_MAX < y ||
						m < 1 || 12 < m ||
						d < 1 || 31 < d ||
						h < 0 || 23 < h ||
						m < 0 || 59 < m ||
						s < 0 || 59 < s
						)
						throw null; // never

					return
						y * 10000000000L +
						m * 100000000L +
						d * 1000000L +
						h * 10000L +
						i * 100L +
						s;
				}

				public static long ToSec(DateTime dateTime)
				{
					return ToSec(ToTimeStamp(dateTime));
				}

				public static long ToTimeStamp(DateTime dateTime)
				{
					return
						10000000000L * dateTime.Year +
						100000000L * dateTime.Month +
						1000000L * dateTime.Day +
						10000L * dateTime.Hour +
						100L * dateTime.Minute +
						dateTime.Second;
				}
			}

			#endregion

			#region SimpleDateTime

			/// <summary>
			/// 日時の範囲：1/1/1 00:00:00 ～ 922337203/12/31 23:59:59
			/// </summary>
			public struct SimpleDateTime
			{
				public readonly int Year;
				public readonly int Month;
				public readonly int Day;
				public readonly int Hour;
				public readonly int Minute;
				public readonly int Second;
				public readonly string Weekday;

				public static SimpleDateTime Now()
				{
					return new SimpleDateTime(DateTime.Now);
				}

				public static SimpleDateTime FromTimeStamp(long timeStamp)
				{
					return new SimpleDateTime(TimeStampToSec.ToSec(timeStamp));
				}

				public SimpleDateTime(DateTime dateTime)
					: this(TimeStampToSec.ToSec(dateTime))
				{ }

				public SimpleDateTime(long sec)
				{
					long timeStamp = TimeStampToSec.ToTimeStamp(sec);
					long t = timeStamp;

					this.Second = (int)(t % 100L);
					t /= 100L;
					this.Minute = (int)(t % 100L);
					t /= 100L;
					this.Hour = (int)(t % 100L);
					t /= 100L;
					this.Day = (int)(t % 100L);
					t /= 100L;
					this.Month = (int)(t % 100L);
					this.Year = (int)(t / 100L);

					this.Weekday = new string(new char[] { "月火水木金土日"[(int)(TimeStampToSec.ToSec(timeStamp) / 86400 % 7)] });
				}

				public override string ToString()
				{
					return this.ToString("{0}/{1:D2}/{2:D2} ({3}) {4:D2}:{5:D2}:{6:D2}");
				}

				public string ToString(string format)
				{
					return string.Format(format, this.Year, this.Month, this.Day, this.Weekday, this.Hour, this.Minute, this.Second);
				}

				public DateTime ToDateTime()
				{
					return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second);
				}

				public long ToTimeStamp()
				{
					return
						10000000000L * this.Year +
						100000000L * this.Month +
						1000000L * this.Day +
						10000L * this.Hour +
						100L * this.Minute +
						this.Second;
				}

				public long ToSec()
				{
					return TimeStampToSec.ToSec(this.ToTimeStamp());
				}

				public static SimpleDateTime operator +(SimpleDateTime instance, long sec)
				{
					return new SimpleDateTime(instance.ToSec() + sec);
				}

				public static SimpleDateTime operator +(long sec, SimpleDateTime instance)
				{
					return new SimpleDateTime(instance.ToSec() + sec);
				}

				public static SimpleDateTime operator -(SimpleDateTime instance, long sec)
				{
					return new SimpleDateTime(instance.ToSec() - sec);
				}

				public static long operator -(SimpleDateTime a, SimpleDateTime b)
				{
					return a.ToSec() - b.ToSec();
				}

				private long GetValueForCompare()
				{
					return this.ToTimeStamp();
				}

				public static bool operator ==(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() == b.GetValueForCompare();
				}

				public static bool operator !=(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() != b.GetValueForCompare();
				}

				public override bool Equals(object other)
				{
					return other is SimpleDateTime && this == (SimpleDateTime)other;
				}

				public override int GetHashCode()
				{
					return this.GetValueForCompare().GetHashCode();
				}

				public static bool operator <(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() < b.GetValueForCompare();
				}

				public static bool operator >(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() > b.GetValueForCompare();
				}

				public static bool operator <=(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() <= b.GetValueForCompare();
				}

				public static bool operator >=(SimpleDateTime a, SimpleDateTime b)
				{
					return a.GetValueForCompare() >= b.GetValueForCompare();
				}
			}

			#endregion

			public class EncodingShiftJIS
			{
				public EncodingShiftJIS()
				{
					for (UInt16 code = 0x01; code <= 0x7e; code++) // ASCII + (制御コード - NUL)
						this.Add(code, code);

					for (UInt16 code = 0xa1; code <= 0xdf; code++) // 半角カナ
						this.Add(code, (UInt16)(code + 65216));

					#region SJIS_UNICODE_PAIRS_RESOURCE

					string SJIS_UNICODE_PAIRS_RESOURCE = @"

8140300081413001814230028143ff0c8144ff0e814530fb8146ff1a8147ff1b8148ff1f8149ff01814a309b814b309c814c00b4814dff40814e00a8814fff3e8150ffe38151ff3f815230fd815330fe8154309d8155309e8156300381574edd8158300581593006815a3007815b30fc815c2015815d2010815eff0f815fff3c
8160ff5e816122258162ff5c816320268164202581652018816620198167201c8168201d8169ff08816aff09816b3014816c3015816dff3b816eff3d816fff5b8170ff5d81713008817230098173300a8174300b8175300c8176300d8177300e8178300f81793010817a3011817bff0b817cff0d817d00b1817e00d7818000f7
8181ff1d818222608183ff1c8184ff1e81852266818622678187221e8188223481892642818a2640818b00b0818c2032818d2033818e2103818fffe58190ff048191ffe08192ffe18193ff058194ff038195ff068196ff0a8197ff20819800a781992606819a2605819b25cb819c25cf819d25ce819e25c7819f25c681a025a1
81a125a081a225b381a325b281a425bd81a525bc81a6203b81a7301281a8219281a9219081aa219181ab219381ac301381b8220881b9220b81ba228681bb228781bc228281bd228381be222a81bf222981c8222781c9222881caffe281cb21d281cc21d481cd220081ce220381da222081db22a581dc231281dd220281de2207
81df226181e0225281e1226a81e2226b81e3221a81e4223d81e5221d81e6223581e7222b81e8222c81f0212b81f1203081f2266f81f3266d81f4266a81f5202081f6202181f700b681fc25ef824fff108250ff118251ff128252ff138253ff148254ff158255ff168256ff178257ff188258ff198260ff218261ff228262ff23
8263ff248264ff258265ff268266ff278267ff288268ff298269ff2a826aff2b826bff2c826cff2d826dff2e826eff2f826fff308270ff318271ff328272ff338273ff348274ff358275ff368276ff378277ff388278ff398279ff3a8281ff418282ff428283ff438284ff448285ff458286ff468287ff478288ff488289ff49
828aff4a828bff4b828cff4c828dff4d828eff4e828fff4f8290ff508291ff518292ff528293ff538294ff548295ff558296ff568297ff578298ff588299ff59829aff5a829f304182a0304282a1304382a2304482a3304582a4304682a5304782a6304882a7304982a8304a82a9304b82aa304c82ab304d82ac304e82ad304f
82ae305082af305182b0305282b1305382b2305482b3305582b4305682b5305782b6305882b7305982b8305a82b9305b82ba305c82bb305d82bc305e82bd305f82be306082bf306182c0306282c1306382c2306482c3306582c4306682c5306782c6306882c7306982c8306a82c9306b82ca306c82cb306d82cc306e82cd306f
82ce307082cf307182d0307282d1307382d2307482d3307582d4307682d5307782d6307882d7307982d8307a82d9307b82da307c82db307d82dc307e82dd307f82de308082df308182e0308282e1308382e2308482e3308582e4308682e5308782e6308882e7308982e8308a82e9308b82ea308c82eb308d82ec308e82ed308f
82ee309082ef309182f0309282f13093834030a1834130a2834230a3834330a4834430a5834530a6834630a7834730a8834830a9834930aa834a30ab834b30ac834c30ad834d30ae834e30af834f30b0835030b1835130b2835230b3835330b4835430b5835530b6835630b7835730b8835830b9835930ba835a30bb835b30bc
835c30bd835d30be835e30bf835f30c0836030c1836130c2836230c3836330c4836430c5836530c6836630c7836730c8836830c9836930ca836a30cb836b30cc836c30cd836d30ce836e30cf836f30d0837030d1837130d2837230d3837330d4837430d5837530d6837630d7837730d8837830d9837930da837a30db837b30dc
837c30dd837d30de837e30df838030e0838130e1838230e2838330e3838430e4838530e5838630e6838730e7838830e8838930e9838a30ea838b30eb838c30ec838d30ed838e30ee838f30ef839030f0839130f1839230f2839330f3839430f4839530f5839630f6839f039183a0039283a1039383a2039483a3039583a40396
83a5039783a6039883a7039983a8039a83a9039b83aa039c83ab039d83ac039e83ad039f83ae03a083af03a183b003a383b103a483b203a583b303a683b403a783b503a883b603a983bf03b183c003b283c103b383c203b483c303b583c403b683c503b783c603b883c703b983c803ba83c903bb83ca03bc83cb03bd83cc03be
83cd03bf83ce03c083cf03c183d003c383d103c483d203c583d303c683d403c783d503c883d603c984400410844104118442041284430413844404148445041584460401844704168448041784490418844a0419844b041a844c041b844d041c844e041d844f041e8450041f8451042084520421845304228454042384550424
84560425845704268458042784590428845a0429845b042a845c042b845d042c845e042d845f042e8460042f84700430847104318472043284730433847404348475043584760451847704368478043784790438847a0439847b043a847c043b847d043c847e043d8480043e8481043f84820440848304418484044284850443
84860444848704458488044684890447848a0448848b0449848c044a848d044b848e044c848f044d8490044e8491044f849f250084a0250284a1250c84a2251084a3251884a4251484a5251c84a6252c84a7252484a8253484a9253c84aa250184ab250384ac250f84ad251384ae251b84af251784b0252384b1253384b2252b
84b3253b84b4254b84b5252084b6252f84b7252884b8253784b9253f84ba251d84bb253084bc252584bd253884be254287402460874124618742246287432463874424648745246587462466874724678748246887492469874a246a874b246b874c246c874d246d874e246e874f246f87502470875124718752247287532473
875421608755216187562162875721638758216487592165875a2166875b2167875c2168875d2169875f334987603314876133228762334d8763331887643327876533038766333687673351876833578769330d876a3326876b3323876c332b876d334a876e333b876f339c8770339d8771339e8772338e8773338f877433c4
877533a1877e337b8780301d8781301f87822116878333cd87842121878532a4878632a5878732a6878832a7878932a8878a3231878b3232878c3239878d337e878e337d878f337c87902252879122618792222b8793222e879422118795221a879622a5879722208798221f879922bf879a2235879b2229879c222a889f4e9c
88a0551688a15a0388a2963f88a354c088a4611b88a5632888a659f688a7902288a8847588a9831c88aa7a5088ab60aa88ac63e188ad6e2588ae65ed88af846688b082a688b19bf588b2689388b3572788b465a188b5627188b65b9b88b759d088b8867b88b998f488ba7d6288bb7dbe88bc9b8e88bd621688be7c9f88bf88b7
88c05b8988c15eb588c2630988c3669788c4684888c595c788c6978d88c7674f88c84ee588c94f0a88ca4f4d88cb4f9d88cc504988cd56f288ce593788cf59d488d05a0188d15c0988d260df88d3610f88d4617088d5661388d6690588d770ba88d8754f88d9757088da79fb88db7dad88dc7def88dd80c388de840e88df8863
88e08b0288e1905588e2907a88e3533b88e44e9588e54ea588e657df88e780b288e890c188e978ef88ea4e0088eb58f188ec6ea288ed903888ee7a3288ef832888f0828b88f19c2f88f2514188f3537088f454bd88f554e188f656e088f759fb88f85f1588f998f288fa6deb88fb80e488fc852d8940966289419670894296a0
894397fb8944540b894553f389465b87894770cf89487fbd89498fc2894a96e8894b536f894c9d5c894d7aba894e4e11894f7893895081fc89516e26895256188953550489546b1d8955851a89569c3b895759e5895853a989596d66895a74dc895b958f895c5642895d4e91895e904b895f96f28960834f8961990c896253e1
896355b689645b3089655f7189666620896766f38968680489696c38896a6cf3896b6d29896c745b896d76c8896e7a4e896f9834897082f18971885b89728a60897392ed89746db2897575ab897676ca897799c5897860a689798b01897a8d8a897b95b2897c698e897d53ad897e518689805712898158308982594489835bb4
89845ef689856028898663a9898763f489886cbf89896f14898a708e898b7114898c7159898d71d5898e733f898f7e0189908276899182d189928597899390608994925b89959d1b89965869899765bc89986c5a89997525899a51f9899b592e899c5965899d5f80899e5fdc899f62bc89a065fa89a16a2a89a26b2789a36bb4
89a4738b89a57fc189a6895689a79d2c89a89d0e89a99ec489aa5ca189ab6c9689ac837b89ad510489ae5c4b89af61b689b081c689b1687689b2726189b34e5989b44ffa89b5537889b6606989b76e2989b87a4f89b997f389ba4e0b89bb531689bc4eee89bd4f5589be4f3d89bf4fa189c04f7389c152a089c253ef89c35609
89c4590f89c55ac189c65bb689c75be189c879d189c9668789ca679c89cb67b689cc6b4c89cd6cb389ce706b89cf73c289d0798d89d179be89d27a3c89d37b8789d482b189d582db89d6830489d7837789d883ef89d983d389da876689db8ab289dc562989dd8ca889de8fe689df904e89e0971e89e1868a89e24fc489e35ce8
89e4621189e5725989e6753b89e781e589e882bd89e986fe89ea8cc089eb96c589ec991389ed99d589ee4ecb89ef4f1a89f089e389f156de89f2584a89f358ca89f45efb89f55feb89f6602a89f7609489f8606289f961d089fa621289fb62d089fc65398a409b418a4166668a4268b08a436d778a4470708a45754c8a467686
8a477d758a4882a58a4987f98a4a958b8a4b968e8a4c8c9d8a4d51f18a4e52be8a4f59168a5054b38a515bb38a525d168a5361688a5469828a556daf8a56788d8a5784cb8a5888578a598a728a5a93a78a5b9ab88a5c6d6c8a5d99a88a5e86d98a5f57a38a6067ff8a6186ce8a62920e8a6352838a6456878a6554048a665ed3
8a6762e18a6864b98a69683c8a6a68388a6b6bbb8a6c73728a6d78ba8a6e7a6b8a6f899a8a7089d28a718d6b8a728f038a7390ed8a7495a38a7596948a7697698a775b668a785cb38a79697d8a7a984d8a7b984e8a7c639b8a7d7b208a7e6a2b8a806a7f8a8168b68a829c0d8a836f5f8a8452728a85559d8a8660708a8762ec
8a886d3b8a896e078a8a6ed18a8b845b8a8c89108a8d8f448a8e4e148a8f9c398a9053f68a91691b8a926a3a8a9397848a94682a8a95515c8a967ac38a9784b28a9891dc8a99938c8a9a565b8a9b9d288a9c68228a9d83058a9e84318a9f7ca58aa052088aa182c58aa274e68aa34e7e8aa44f838aa551a08aa65bd28aa7520a
8aa852d88aa952e78aaa5dfb8aab559a8aac582a8aad59e68aae5b8c8aaf5b988ab05bdb8ab15e728ab25e798ab360a38ab4611f8ab561638ab661be8ab763db8ab865628ab967d18aba68538abb68fa8abc6b3e8abd6b538abe6c578abf6f228ac06f978ac16f458ac274b08ac375188ac476e38ac5770b8ac67aff8ac77ba1
8ac87c218ac97de98aca7f368acb7ff08acc809d8acd82668ace839e8acf89b38ad08acc8ad18cab8ad290848ad394518ad495938ad595918ad695a28ad796658ad897d38ad999288ada82188adb4e388adc542b8add5cb88ade5dcc8adf73a98ae0764c8ae1773c8ae25ca98ae37feb8ae48d0b8ae596c18ae698118ae79854
8ae898588ae94f018aea4f0e8aeb53718aec559c8aed56688aee57fa8aef59478af05b098af15bc48af25c908af35e0c8af45e7e8af55fcc8af663ee8af7673a8af865d78af965e28afa671f8afb68cb8afc68c48b406a5f8b415e308b426bc58b436c178b446c7d8b45757f8b4679488b475b638b487a008b497d008b4a5fbd
8b4b898f8b4c8a188b4d8cb48b4e8d778b4f8ecc8b508f1d8b5198e28b529a0e8b539b3c8b544e808b55507d8b5651008b5759938b585b9c8b59622f8b5a62808b5b64ec8b5c6b3a8b5d72a08b5e75918b5f79478b607fa98b6187fb8b628abc8b638b708b6463ac8b6583ca8b6697a08b6754098b6854038b6955ab8b6a6854
8b6b6a588b6c8a708b6d78278b6e67758b6f9ecd8b7053748b715ba28b72811a8b7386508b7490068b754e188b764e458b774ec78b784f118b7953ca8b7a54388b7b5bae8b7c5f138b7d60258b7e65518b80673d8b816c428b826c728b836ce38b8470788b8574038b867a768b877aae8b887b088b897d1a8b8a7cfe8b8b7d66
8b8c65e78b8d725b8b8e53bb8b8f5c458b905de88b9162d28b9262e08b9363198b946e208b95865a8b968a318b978ddd8b9892f88b996f018b9a79a68b9b9b5a8b9c4ea88b9d4eab8b9e4eac8b9f4f9b8ba04fa08ba150d18ba251478ba37af68ba451718ba551f68ba653548ba753218ba8537f8ba953eb8baa55ac8bab5883
8bac5ce18bad5f378bae5f4a8baf602f8bb060508bb1606d8bb2631f8bb365598bb46a4b8bb56cc18bb672c28bb772ed8bb877ef8bb980f88bba81058bbb82088bbc854e8bbd90f78bbe93e18bbf97ff8bc099578bc19a5a8bc24ef08bc351dd8bc45c2d8bc566818bc6696d8bc75c408bc866f28bc969758bca73898bcb6850
8bcc7c818bcd50c58bce52e48bcf57478bd05dfe8bd193268bd265a48bd36b238bd46b3d8bd574348bd679818bd779bd8bd87b4b8bd97dca8bda82b98bdb83cc8bdc887f8bdd895f8bde8b398bdf8fd18be091d18be1541f8be292808be34e5d8be450368be553e58be6533a8be772d78be873968be977e98bea82e68beb8eaf
8bec99c68bed99c88bee99d28bef51778bf0611a8bf1865e8bf255b08bf37a7a8bf450768bf55bd38bf690478bf796858bf84e328bf96adb8bfa91e78bfb5c518bfc5c488c4063988c417a9f8c426c938c4397748c448f618c457aaa8c46718a8c4796888c487c828c4968178c4a7e708c4b68518c4c936c8c4d52f28c4e541b
8c4f85ab8c508a138c517fa48c528ecd8c5390e18c5453668c5588888c5679418c574fc28c5850be8c5952118c5a51448c5b55538c5c572d8c5d73ea8c5e578b8c5f59518c605f628c615f848c6260758c6361768c6461678c6561a98c6663b28c67643a8c68656c8c69666f8c6a68428c6b6e138c6c75668c6d7a3d8c6e7cfb
8c6f7d4c8c707d998c717e4b8c727f6b8c73830e8c74834a8c7586cd8c768a088c778a638c788b668c798efd8c7a981a8c7b9d8f8c7c82b88c7d8fce8c7e9be88c8052878c81621f8c8264838c836fc08c8496998c8568418c8650918c876b208c886c7a8c896f548c8a7a748c8b7d508c8c88408c8d8a238c8e67088c8f4ef6
8c9050398c9150268c9250658c93517c8c9452388c9552638c9655a78c97570f8c9858058c995acc8c9a5efa8c9b61b28c9c61f88c9d62f38c9e63728c9f691c8ca06a298ca1727d8ca272ac8ca3732e8ca478148ca5786f8ca67d798ca7770c8ca880a98ca9898b8caa8b198cab8ce28cac8ed28cad90638cae93758caf967a
8cb098558cb19a138cb29e788cb351438cb4539f8cb553b38cb65e7b8cb75f268cb86e1b8cb96e908cba73848cbb73fe8cbc7d438cbd82378cbe8a008cbf8afa8cc096508cc14e4e8cc2500b8cc353e48cc4547c8cc556fa8cc659d18cc75b648cc85df18cc95eab8cca5f278ccb62388ccc65458ccd67af8cce6e568ccf72d0
8cd07cca8cd188b48cd280a18cd380e18cd483f08cd5864e8cd68a878cd78de88cd892378cd996c78cda98678cdb9f138cdc4e948cdd4e928cde4f0d8cdf53488ce054498ce1543e8ce25a2f8ce35f8c8ce45fa18ce5609f8ce668a78ce76a8e8ce8745a8ce978818cea8a9e8ceb8aa48cec8b778ced91908cee4e5e8cef9bc9
8cf04ea48cf14f7c8cf24faf8cf350198cf450168cf551498cf6516c8cf7529f8cf852b98cf952fe8cfa539a8cfb53e38cfc54118d40540e8d4155898d4257518d4357a28d44597d8d455b548d465b5d8d475b8f8d485de58d495de78d4a5df78d4b5e788d4c5e838d4d5e9a8d4e5eb78d4f5f188d5060528d51614c8d526297
8d5362d88d5463a78d55653b8d5666028d5766438d5866f48d59676d8d5a68218d5b68978d5c69cb8d5d6c5f8d5e6d2a8d5f6d698d606e2f8d616e9d8d6275328d6376878d64786c8d657a3f8d667ce08d677d058d687d188d697d5e8d6a7db18d6b80158d6c80038d6d80af8d6e80b18d6f81548d70818f8d71822a8d728352
8d73884c8d7488618d758b1b8d768ca28d778cfc8d7890ca8d7991758d7a92718d7b783f8d7c92fc8d7d95a48d7e964d8d8098058d8199998d829ad88d839d3b8d84525b8d8552ab8d8653f78d8754088d8858d58d8962f78d8a6fe08d8b8c6a8d8c8f5f8d8d9eb98d8e514b8d8f523b8d90544a8d9156fd8d927a408d939177
8d949d608d959ed28d9673448d976f098d9881708d9975118d9a5ffd8d9b60da8d9c9aa88d9d72db8d9e8fbc8d9f6b648da098038da14eca8da256f08da357648da458be8da55a5a8da660688da761c78da8660f8da966068daa68398dab68b18dac6df78dad75d58dae7d3a8daf826e8db09b428db14e9b8db24f508db353c9
8db455068db55d6f8db65de68db75dee8db867fb8db96c998dba74738dbb78028dbc8a508dbd93968dbe88df8dbf57508dc05ea78dc1632b8dc250b58dc350ac8dc4518d8dc567008dc654c98dc7585e8dc859bb8dc95bb08dca5f698dcb624d8dcc63a18dcd683d8dce6b738dcf6e088dd0707d8dd191c78dd272808dd37815
8dd478268dd5796d8dd6658e8dd77d308dd883dc8dd988c18dda8f098ddb969b8ddc52648ddd57288dde67508ddf7f6a8de08ca18de151b48de257428de3962a8de4583a8de5698a8de680b48de754b28de85d0e8de957fc8dea78958deb9dfa8dec4f5c8ded524a8dee548b8def643e8df066288df167148df267f58df37a84
8df47b568df57d228df6932f8df7685c8df89bad8df97b398dfa53198dfb518a8dfc52378e405bdf8e4162f68e4264ae8e4364e68e44672d8e456bba8e4685a98e4796d18e4876908e499bd68e4a634c8e4b93068e4c9bab8e4d76bf8e4e66528e4f4e098e5050988e5153c28e525c718e5360e88e5464928e5565638e56685f
8e5771e68e5873ca8e5975238e5a7b978e5b7e828e5c86958e5d8b838e5e8cdb8e5f91788e6099108e6165ac8e6266ab8e636b8b8e644ed58e654ed48e664f3a8e674f7f8e68523a8e6953f88e6a53f28e6b55e38e6c56db8e6d58eb8e6e59cb8e6f59c98e7059ff8e715b508e725c4d8e735e028e745e2b8e755fd78e76601d
8e7763078e78652f8e795b5c8e7a65af8e7b65bd8e7c65e88e7d679d8e7e6b628e806b7b8e816c0f8e8273458e8379498e8479c18e857cf88e867d198e877d2b8e8880a28e8981028e8a81f38e8b89968e8c8a5e8e8d8a698e8e8a668e8f8a8c8e908aee8e918cc78e928cdc8e9396cc8e9498fc8e956b6f8e964e8b8e974f3c
8e984f8d8e9951508e9a5b578e9b5bfa8e9c61488e9d63018e9e66428e9f6b218ea06ecb8ea16cbb8ea2723e8ea374bd8ea475d48ea578c18ea6793a8ea7800c8ea880338ea981ea8eaa84948eab8f9e8eac6c508ead9e7f8eae5f0f8eaf8b588eb09d2b8eb17afa8eb28ef88eb35b8d8eb496eb8eb54e038eb653f18eb757f7
8eb859318eb95ac98eba5ba48ebb60898ebc6e7f8ebd6f068ebe75be8ebf8cea8ec05b9f8ec185008ec27be08ec350728ec467f48ec5829d8ec65c618ec7854a8ec87e1e8ec9820e8eca51998ecb5c048ecc63688ecd8d668ece659c8ecf716e8ed0793e8ed17d178ed280058ed38b1d8ed48eca8ed5906e8ed686c78ed790aa
8ed8501f8ed952fa8eda5c3a8edb67538edc707c8edd72358ede914c8edf91c88ee0932b8ee182e58ee25bc28ee35f318ee460f98ee54e3b8ee653d68ee75b888ee8624b8ee967318eea6b8a8eeb72e98eec73e08eed7a2e8eee816b8eef8da38ef091528ef199968ef251128ef353d78ef4546a8ef55bff8ef663888ef76a39
8ef87dac8ef997008efa56da8efb53ce8efc54688f405b978f415c318f425dde8f434fee8f4461018f4562fe8f466d328f4779c08f4879cb8f497d428f4a7e4d8f4b7fd28f4c81ed8f4d821f8f4e84908f4f88468f5089728f518b908f528e748f538f2f8f5490318f55914b8f56916c8f5796c68f58919c8f594ec08f5a4f4f
8f5b51458f5c53418f5d5f938f5e620e8f5f67d48f606c418f616e0b8f6273638f637e268f6491cd8f6592838f6653d48f6759198f685bbf8f696dd18f6a795d8f6b7e2e8f6c7c9b8f6d587e8f6e719f8f6f51fa8f7088538f718ff08f724fca8f735cfb8f7466258f7577ac8f767ae38f77821c8f7899ff8f7951c68f7a5faa
8f7b65ec8f7c696f8f7d6b898f7e6df38f806e968f816f648f8276fe8f837d148f845de18f8590758f8691878f8798068f8851e68f89521d8f8a62408f8b66918f8c66d98f8d6e1a8f8e5eb68f8f7dd28f907f728f9166f88f9285af8f9385f78f948af88f9552a98f9653d98f9759738f985e8f8f995f908f9a60558f9b92e4
8f9c96648f9d50b78f9e511f8f9f52dd8fa053208fa153478fa253ec8fa354e88fa455468fa555318fa656178fa759688fa859be8fa95a3c8faa5bb58fab5c068fac5c0f8fad5c118fae5c1a8faf5e848fb05e8a8fb15ee08fb25f708fb3627f8fb462848fb562db8fb6638c8fb763778fb866078fb9660c8fba662d8fbb6676
8fbc677e8fbd68a28fbe6a1f8fbf6a358fc06cbc8fc16d888fc26e098fc36e588fc4713c8fc571268fc671678fc775c78fc877018fc9785d8fca79018fcb79658fcc79f08fcd7ae08fce7b118fcf7ca78fd07d398fd180968fd283d68fd3848b8fd485498fd5885d8fd688f38fd78a1f8fd88a3c8fd98a548fda8a738fdb8c61
8fdc8cde8fdd91a48fde92668fdf937e8fe094188fe1969c8fe297988fe34e0a8fe44e088fe54e1e8fe64e578fe751978fe852708fe957ce8fea58348feb58cc8fec5b228fed5e388fee60c58fef64fe8ff067618ff167568ff26d448ff372b68ff475738ff57a638ff684b88ff78b728ff891b88ff993208ffa56318ffb57f4
8ffc98fe904062ed9041690d90426b96904371ed90447e549045807790468272904789e6904898df90498755904a8fb1904b5c3b904c4f38904d4fe1904e4fb5904f550790505a2090515bdd90525be990535fc39054614e9055632f905665b09057664b905868ee9059699b905a6d78905b6df1905c7533905d75b9905e771f
905f795e906079e690617d33906281e3906382af906485aa906589aa90668a3a90678eab90688f9b90699032906a91dd906b9707906c4eba906d4ec1906e5203906f5875907058ec90715c0b9072751a90735c3d9074814e90758a0a90768fc5907796639078976d90797b25907a8acf907b9808907c9162907d56f3907e53a8
90809017908154399082578290835e25908463a890856c349086708a9087776190887c8b90897fe0908a8870908b9042908c9154908d9310908e9318908f968f9090745e90919ac490925d0790935d6990946570909567a290968da8909796db9098636e90996749909a6919909b83c5909c9817909d96c0909e88fe909f6f84
90a0647a90a15bf890a24e1690a3702c90a4755d90a5662f90a651c490a7523690a852e290a959d390aa5f8190ab602790ac621090ad653f90ae657490af661f90b0667490b168f290b2681690b36b6390b46e0590b5727290b6751f90b776db90b87cbe90b9805690ba58f090bb88fd90bc897f90bd8aa090be8a9390bf8acb
90c0901d90c1919290c2975290c3975990c4658990c57a0e90c6810690c796bb90c85e2d90c960dc90ca621a90cb65a590cc661490cd679090ce77f390cf7a4d90d07c4d90d17e3e90d2810a90d38cac90d48d6490d58de190d68e5f90d778a990d8520790d962d990da63a590db644290dc629890dd8a2d90de7a8390df7bc0
90e08aac90e196ea90e27d7690e3820c90e4874990e54ed990e6514890e7534390e8536090e95ba390ea5c0290eb5c1690ec5ddd90ed622690ee624790ef64b090f0681390f1683490f26cc990f36d4590f46d1790f567d390f66f5c90f7714e90f8717d90f965cb90fa7a7f90fb7bad90fc7dda91407e4a91417fa89142817a
9143821b91448239914585a691468a6e91478cce91488df591499078914a9077914b92ad914c9291914d9583914e9bae914f524d9150558491516f3891527136915351689154798591557e55915681b391577cce9158564c91595851915a5ca8915b63aa915c66fe915d66fd915e695a915f72d99160758f9161758e9162790e
91637956916479df91657c9791667d2091677d449168860791698a34916a963b916b9061916c9f20916d50e7916e5275916f53cc917053e291715009917255aa917358ee9174594f9175723d91765b8b91775c649178531d917960e3917a60f3917b635c917c6383917d633f917e63bb918064cd918165e9918266f991835de3
918469cd918569fd91866f15918771e591884e89918975e9918a76f8918b7a93918c7cdf918d7dcf918e7d9c918f806191908349919183589192846c919384bc919485fb919588c591968d70919790019198906d91999397919a971c919b9a12919c50cf919d5897919e618e919f81d391a0853591a18d0891a2902091a34fc3
91a4507491a5524791a6537391a7606f91a8634991a9675f91aa6e2c91ab8db391ac901f91ad4fd791ae5c5e91af8cca91b065cf91b17d9a91b2535291b3889691b4517691b563c391b65b5891b75b6b91b85c0a91b9640d91ba675191bb905c91bc4ed691bd591a91be592a91bf6c7091c08a5191c1553e91c2581591c359a5
91c460f091c5625391c667c191c7823591c8695591c9964091ca99c491cb9a2891cc4f5391cd580691ce5bfe91cf801091d05cb191d15e2f91d25f8591d3602091d4614b91d5623491d666ff91d76cf091d86ede91d980ce91da817f91db82d491dc888b91dd8cb891de900091df902e91e0968a91e19edb91e29bdb91e34ee3
91e453f091e5592791e67b2c91e7918d91e8984c91e99df991ea6edd91eb702791ec535391ed554491ee5b8591ef625891f0629e91f162d391f26ca291f36fef91f4742291f58a1791f6943891f76fc191f88afe91f9833891fa51e791fb86f891fc53ea924053e992414f469242905492438fb09244596a9245813192465dfd
92477aea92488fbf924968da924a8c37924b72f8924c9c48924d6a3d924e8ab0924f4e39925053589251560692525766925362c5925463a2925565e692566b4e92576de192586e5b925970ad925a77ed925b7aef925c7baa925d7dbb925e803d925f80c6926086cb92618a959262935b926356e3926458c792655f3e926665ad
9267669692686a8092696bb5926a7537926b8ac7926c5024926d77e5926e5730926f5f1b927060659271667a92726c60927375f492747a1a92757f6e927681f49277871892789045927999b3927a7bc9927b755c927c7af9927d7b51927e84c492809010928179e992827a929283833692845ae19285774092864e2d92874ef2
92885b9992895fe0928a62bd928b663c928c67f1928d6ce8928e866b928f887792908a3b9291914e929292f3929399d092946a17929570269296732a929782e79298845792998caf929a4e01929b5146929c51cb929d558b929e5bf5929f5e1692a05e3392a15e8192a25f1492a35f3592a45f6b92a55fb492a661f292a76311
92a866a292a9671d92aa6f6e92ab725292ac753a92ad773a92ae807492af813992b0817892b1877692b28abf92b38adc92b48d8592b58df392b6929a92b7957792b8980292b99ce592ba52c592bb635792bc76f492bd671592be6c8892bf73cd92c08cc392c193ae92c2967392c36d2592c4589c92c5690e92c669cc92c78ffd
92c8939a92c975db92ca901a92cb585a92cc680292cd63b492ce69fb92cf4f4392d06f2c92d167d892d28fbb92d3852692d47db492d5935492d6693f92d76f7092d8576a92d958f792da5b2c92db7d2c92dc722a92dd540a92de91e392df9db492e04ead92e14f4e92e2505c92e3507592e4524392e58c9e92e6544892e75824
92e85b9a92e95e1d92ea5e9592eb5ead92ec5ef792ed5f1f92ee608c92ef62b592f0633a92f163d092f268af92f36c4092f4788792f5798e92f67a0b92f77de092f8824792f98a0292fa8ae692fb8e4492fc9013934090b89341912d934291d893439f0e93446ce593456458934664e29347657593486ef493497684934a7b1b
934b9069934c93d1934d6eba934e54f2934f5fb9935064a493518f4d93528fed93539244935451789355586b9356592993575c5593585e9793596dfb935a7e8f935b751c935c8cbc935d8ee2935e985b935f70b993604f1d93616bbf93626fb193637530936496fb9365514e936654109367583593685857936959ac936a5c60
936b5f92936c6597936d675c936e6e21936f767b937083df93718ced93729014937390fd9374934d937578259376783a937752aa93785ea69379571f937a5974937b6012937c5012937d515a937e51ac938051cd938152009382551093835854938458589385595793865b9593875cf693885d8b938960bc938a6295938b642d
938c6771938d6843938e68bc938f68df939076d793916dd893926e6f93936d9b9394706f939571c893965f53939775d89398797793997b49939a7b54939b7b52939c7cd6939d7d71939e5230939f846393a0856993a185e493a28a0e93a38b0493a48c4693a58e0f93a6900393a7900f93a8941993a9967693aa982d93ab9a30
93ac95d893ad50cd93ae52d593af540c93b0580293b15c0e93b261a793b3649e93b46d1e93b577b393b67ae593b780f493b8840493b9905393ba928593bb5ce093bc9d0793bd533f93be5f9793bf5fb393c06d9c93c1727993c2776393c379bf93c47be493c56bd293c672ec93c78aad93c8680393c96a6193ca51f893cb7a81
93cc693493cd5c4a93ce9cf693cf82eb93d05bc593d1914993d2701e93d3567893d45c6f93d560c793d6656693d76c8c93d88c5a93d9904193da981393db545193dc66c793dd920d93de594893df90a393e0518593e14e4d93e251ea93e3859993e48b0e93e5705893e6637a93e7934b93e8696293e999b493ea7e0493eb7577
93ec535793ed696093ee8edf93ef96e393f06c5d93f14e8c93f25c3c93f35f1093f48fe993f5530293f68cd193f7808993f8867993f95eff93fa65e593fb4e7393fc51659440598294415c3f944297ee94434efb9444598a94455fcd94468a8d94476fe1944879b094497962944a5be7944b8471944c732b944d71b1944e5e74
944f5ff59450637b9451649a945271c394537c9894544e4394555efc94564e4b945757dc945856a2945960a9945a6fc3945b7d0d945c80fd945d8133945e81bf945f8fb294608997946186a494625df49463628a946464ad946589879466677794676ce294686d3e94697436946a7834946b5a46946c7f75946d82ad946e99ac
946f4ff394705ec3947162dd94726392947365579474676f947576c39476724c947780cc947880ba94798f29947a914d947b500d947c57f9947d5a92947e68859480697394817164948272fd94838cb7948458f294858ce09486966a948790199488877f948979e4948a77e7948b8429948c4f2f948d5265948e535a948f62cd
949067cf94916cca9492767d94937b9494947c95949582369496858494978feb949866dd94996f20949a7206949b7e1b949c83ab949d99c1949e9ea6949f51fd94a07bb194a1787294a27bb894a3808794a47b4894a56ae894a65e6194a7808c94a8755194a9756094aa516b94ab926294ac6e8c94ad767a94ae919794af9aea
94b04f1094b17f7094b2629c94b37b4f94b495a594b59ce994b6567a94b7585994b886e494b996bc94ba4f3494bb522494bc534a94bd53cd94be53db94bf5e0694c0642c94c1659194c2677f94c36c3e94c46c4e94c5724894c672af94c773ed94c8755494c97e4194ca822c94cb85e994cc8ca994cd7bc494ce91c694cf7169
94d0981294d198ef94d2633d94d3666994d4756a94d576e494d678d094d7854394d886ee94d9532a94da535194db542694dc598394dd5e8794de5f7c94df60b294e0624994e1627994e262ab94e3659094e46bd494e56ccc94e675b294e776ae94e8789194e979d894ea7dcb94eb7f7794ec80a594ed88ab94ee8ab994ef8cbb
94f0907f94f1975e94f298db94f36a0b94f47c3894f5509994f65c3e94f75fae94f8678794f96bd894fa743594fb770994fc7f8e95409f3b954167ca95427a17954353399544758b95459aed95465f669547819d954883f195498098954a5f3c954b5fc5954c7562954d7b46954e903c954f6867955059eb95515a9b95527d10
9553767e95548b2c95554ff595565f6a95576a1995586c3795596f02955a74e2955b7968955c8868955d8a55955e8c79955f5edf956063cf956175c5956279d2956382d795649328956592f29566849c956786ed95689c2d956954c1956a5f6c956b658c956c6d5c956d7015956e8ca7956f8cd39570983b9571654f957274f6
95734e0d95744ed8957557e09576592b95775a6695785bcc957951a8957a5e03957b5e9c957c6016957d6276957e6577958065a79581666e95826d6e9583723695847b26958581509586819a9587829995888b5c95898ca0958a8ce6958b8d74958c961c958d9644958e4fae958f64ab95906b669591821e959284619593856a
959490e895955c0195966953959798a89598847a95998557959a4f0f959b526f959c5fa9959d5e45959e670d959f798f95a0817995a1890795a2898695a36df595a45f1795a5625595a66cb895a74ecf95a8726995a99b9295aa520695ab543b95ac567495ad58b395ae61a495af626e95b0711a95b1596e95b27c8995b37cde
95b47d1b95b596f095b6658795b7805e95b84e1995b94f7595ba517595bb584095bc5e6395bd5e7395be5f0a95bf67c495c04e2695c1853d95c2958995c3965b95c47c7395c5980195c650fb95c758c195c8765695c978a795ca522595cb77a595cc851195cd7b8695ce504f95cf590995d0724795d17bc795d27de895d38fba
95d48fd495d5904d95d64fbf95d752c995d85a2995d95f0195da97ad95db4fdd95dc821795dd92ea95de570395df635595e06b6995e1752b95e288dc95e38f1495e47a4295e552df95e6589395e7615595e8620a95e966ae95ea6bcd95eb7c3f95ec83e995ed502395ee4ff895ef530595f0544695f1583195f2594995f35b9d
95f45cf095f55cef95f65d2995f75e9695f862b195f9636795fa653e95fb65b995fc670b96406cd596416ce1964270f99643783296447e2b964580de964682b39647840c964884ec96498702964a8912964b8a2a964c8c4a964d90a6964e92d2964f98fd96509cf396519d6c96524e4f96534ea19654508d965552569656574a
965759a896585e3d96595fd8965a5fd9965b623f965c66b4965d671b965e67d0965f68d29660519296617d21966280aa966381a896648b0096658c8c96668cbf9667927e9668963296695420966a982c966b5317966c50d5966d535c966e58a8966f64b296706734967172679672776696737a46967491e6967552c396766ca1
96776b869678580096795e4c967a5954967b672c967c7ffb967d51e1967e76c696806469968178e896829b5496839ebb968457cb968559b9968666279687679a96886bce968954e9968a69d9968b5e55968c819c968d6795968e9baa968f67fe96909c529691685d96924ea696934fe3969453c8969562b99696672b96976cab
96988fc496994fad969a7e6d969b9ebf969c4e07969d6162969e6e80969f6f2b96a0851396a1547396a2672a96a39b4596a45df396a57b9596a65cac96a75bc696a8871c96a96e4a96aa84d196ab7a1496ac810896ad599996ae7c8d96af6c1196b0772096b152d996b2592296b3712196b4725f96b577db96b6972796b79d61
96b8690b96b95a7f96ba5a1896bb51a596bc540d96bd547d96be660e96bf76df96c08ff796c1929896c29cf496c359ea96c4725d96c56ec596c6514d96c768c996c87dbf96c97dec96ca976296cb9eba96cc647896cd6a2196ce830296cf598496d05b5f96d16bdb96d2731b96d376f296d47db296d5801796d6849996d75132
96d8672896d99ed996da76ee96db676296dc52ff96dd990596de5c2496df623b96e07c7e96e18cb096e2554f96e360b696e47d0b96e5958096e6530196e74e5f96e851b696e9591c96ea723a96eb803696ec91ce96ed5f2596ee77e296ef538496f05f7996f17d0496f285ac96f38a3396f48e8d96f5975696f667f396f785ae
96f8945396f9610996fa610896fb6cb996fc765297408aed97418f389742552f97434f519744512a974552c7974653cb97475ba597485e7d974960a0974a6182974b63d6974c6709974d67da974e6e67974f6d8c97507336975173379752753197537950975488d597558a989756904a97579091975890f5975996c4975a878d
975b5915975c4e88975d4f59975e4e0e975f8a8997608f3f97619810976250ad97635e7c9764599697655bb997665eb8976763da976863fa976964c1976a66dc976b694a976c69d8976d6d0b976e6eb6976f71949770752897717aaf97727f8a9773800097748449977584c99776898197778b2197788e0a97799065977a967d
977b990a977c617e977d6291977e6b3297806c8397816d7497827fcc97837ffc97846dc097857f85978687ba978788f897886765978983b1978a983c978b96f7978c6d1b978d7d61978e843d978f916a97904e719791537597925d5097936b0497946feb979585cd9796862d979789a7979852299799540f979a5c65979b674e
979c68a8979d7406979e7483979f75e297a088cf97a188e197a291cc97a396e297a4967897a55f8b97a6738797a77acb97a8844e97a963a097aa756597ab528997ac6d4197ad6e9c97ae740997af755997b0786b97b17c9297b2968697b37adc97b49f8d97b54fb697b6616e97b765c597b8865c97b94e8697ba4eae97bb50da
97bc4e2197bd51cc97be5bee97bf659997c0688197c16dbc97c2731f97c3764297c477ad97c57a1c97c67ce797c7826f97c88ad297c9907c97ca91cf97cb967597cc981897cd529b97ce7dd197cf502b97d0539897d1679797d26dcb97d371d097d4743397d581e897d68f2a97d796a397d89c5797d99e9f97da746097db5841
97dc6d9997dd7d2f97de985e97df4ee497e04f3697e14f8b97e251b797e352b197e45dba97e5601c97e673b297e7793c97e882d397e9923497ea96b797eb96f697ec970a97ed9e9797ee9f6297ef66a697f06b7497f1521797f252a397f370c897f488c297f55ec997f6604b97f7619097f86f2397f9714997fa7c3e97fb7df4
97fc806f984084ee984190239842932c9843544298449b6f98456ad39846708998478cc298488def98499732984a52b4984b5a41984c5eca984d5f04984e6717984f697c9850699498516d6a98526f0f98537262985472fc98557bed985680019857807e9858874b985990ce985a516d985b9e93985c7984985d808b985e9332
985f8ad69860502d9861548c98628a7198636b6a98648cc498658107986660d1986767a098689df298694e99986a4e98986b9c10986c8a6b986d85c1986e8568986f690098706e7e9871789798728155989f5f0c98a04e1098a14e1598a24e2a98a34e3198a44e3698a54e3c98a64e3f98a74e4298a84e5698a94e5898aa4e82
98ab4e8598ac8c6b98ad4e8a98ae821298af5f0d98b04e8e98b14e9e98b24e9f98b34ea098b44ea298b54eb098b64eb398b74eb698b84ece98b94ecd98ba4ec498bb4ec698bc4ec298bd4ed798be4ede98bf4eed98c04edf98c14ef798c24f0998c34f5a98c44f3098c54f5b98c64f5d98c74f5798c84f4798c94f7698ca4f88
98cb4f8f98cc4f9898cd4f7b98ce4f6998cf4f7098d04f9198d14f6f98d24f8698d34f9698d4511898d54fd498d64fdf98d74fce98d84fd898d94fdb98da4fd198db4fda98dc4fd098dd4fe498de4fe598df501a98e0502898e1501498e2502a98e3502598e4500598e54f1c98e64ff698e7502198e8502998e9502c98ea4ffe
98eb4fef98ec501198ed500698ee504398ef504798f0670398f1505598f2505098f3504898f4505a98f5505698f6506c98f7507898f8508098f9509a98fa508598fb50b498fc50b2994050c9994150ca994250b3994350c2994450d6994550de994650e5994750ed994850e3994950ee994a50f9994b50f5994c5109994d5101
994e5102994f511699505115995151149952511a995351219954513a995551379956513c9957513b9958513f99595140995a5152995b514c995c5154995d5162995e7af8995f51699960516a9961516e9962518099635182996456d89965518c996651899967518f9968519199695193996a5195996b5196996c51a4996d51a6
996e51a2996f51a9997051aa997151ab997251b3997351b1997451b2997551b0997651b5997751bd997851c5997951c9997a51db997b51e0997c8655997d51e9997e51ed998051f0998151f5998251fe998352049984520b998552149986520e998752279988522a9989522e998a5233998b5239998c524f998d5244998e524b
998f524c9990525e999152549992526a9993527499945269999552739996527f9997527d9998528d99995294999a5292999b5271999c5288999d5291999e8fa8999f8fa799a052ac99a152ad99a252bc99a352b599a452c199a552cd99a652d799a752de99a852e399a952e699aa98ed99ab52e099ac52f399ad52f599ae52f8
99af52f999b0530699b1530899b2753899b3530d99b4531099b5530f99b6531599b7531a99b8532399b9532f99ba533199bb533399bc533899bd534099be534699bf534599c04e1799c1534999c2534d99c351d699c4535e99c5536999c6536e99c7591899c8537b99c9537799ca538299cb539699cc53a099cd53a699ce53a5
99cf53ae99d053b099d153b699d253c399d37c1299d496d999d553df99d666fc99d771ee99d853ee99d953e899da53ed99db53fa99dc540199dd543d99de544099df542c99e0542d99e1543c99e2542e99e3543699e4542999e5541d99e6544e99e7548f99e8547599e9548e99ea545f99eb547199ec547799ed547099ee5492
99ef547b99f0548099f1547699f2548499f3549099f4548699f554c799f654a299f754b899f854a599f954ac99fa54c499fb54c899fc54a89a4054ab9a4154c29a4254a49a4354be9a4454bc9a4554d89a4654e59a4754e69a48550f9a4955149a4a54fd9a4b54ee9a4c54ed9a4d54fa9a4e54e29a4f55399a5055409a515563
9a52554c9a53552e9a54555c9a5555459a5655569a5755579a5855389a5955339a5a555d9a5b55999a5c55809a5d54af9a5e558a9a5f559f9a60557b9a61557e9a6255989a63559e9a6455ae9a65557c9a6655839a6755a99a6855879a6955a89a6a55da9a6b55c59a6c55df9a6d55c49a6e55dc9a6f55e49a7055d49a715614
9a7255f79a7356169a7455fe9a7555fd9a76561b9a7755f99a78564e9a7956509a7a71df9a7b56349a7c56369a7d56329a7e56389a80566b9a8156649a82562f9a83566c9a84566a9a8556869a8656809a87568a9a8856a09a8956949a8a568f9a8b56a59a8c56ae9a8d56b69a8e56b49a8f56c29a9056bc9a9156c19a9256c3
9a9356c09a9456c89a9556ce9a9656d19a9756d39a9856d79a9956ee9a9a56f99a9b57009a9c56ff9a9d57049a9e57099a9f57089aa0570b9aa1570d9aa257139aa357189aa457169aa555c79aa6571c9aa757269aa857379aa957389aaa574e9aab573b9aac57409aad574f9aae57699aaf57c09ab057889ab157619ab2577f
9ab357899ab457939ab557a09ab657b39ab757a49ab857aa9ab957b09aba57c39abb57c69abc57d49abd57d29abe57d39abf580a9ac057d69ac157e39ac2580b9ac358199ac4581d9ac558729ac658219ac758629ac8584b9ac958709aca6bc09acb58529acc583d9acd58799ace58859acf58b99ad0589f9ad158ab9ad258ba
9ad358de9ad458bb9ad558b89ad658ae9ad758c59ad858d39ad958d19ada58d79adb58d99adc58d89add58e59ade58dc9adf58e49ae058df9ae158ef9ae258fa9ae358f99ae458fb9ae558fc9ae658fd9ae759029ae8590a9ae959109aea591b9aeb68a69aec59259aed592c9aee592d9aef59329af059389af1593e9af27ad2
9af359559af459509af5594e9af6595a9af759589af859629af959609afa59679afb596c9afc59699b4059789b4159819b42599d9b434f5e9b444fab9b4559a39b4659b29b4759c69b4859e89b4959dc9b4a598d9b4b59d99b4c59da9b4d5a259b4e5a1f9b4f5a119b505a1c9b515a099b525a1a9b535a409b545a6c9b555a49
9b565a359b575a369b585a629b595a6a9b5a5a9a9b5b5abc9b5c5abe9b5d5acb9b5e5ac29b5f5abd9b605ae39b615ad79b625ae69b635ae99b645ad69b655afa9b665afb9b675b0c9b685b0b9b695b169b6a5b329b6b5ad09b6c5b2a9b6d5b369b6e5b3e9b6f5b439b705b459b715b409b725b519b735b559b745b5a9b755b5b
9b765b659b775b699b785b709b795b739b7a5b759b7b5b789b7c65889b7d5b7a9b7e5b809b805b839b815ba69b825bb89b835bc39b845bc79b855bc99b865bd49b875bd09b885be49b895be69b8a5be29b8b5bde9b8c5be59b8d5beb9b8e5bf09b8f5bf69b905bf39b915c059b925c079b935c089b945c0d9b955c139b965c20
9b975c229b985c289b995c389b9a5c399b9b5c419b9c5c469b9d5c4e9b9e5c539b9f5c509ba05c4f9ba15b719ba25c6c9ba35c6e9ba44e629ba55c769ba65c799ba75c8c9ba85c919ba95c949baa599b9bab5cab9bac5cbb9bad5cb69bae5cbc9baf5cb79bb05cc59bb15cbe9bb25cc79bb35cd99bb45ce99bb55cfd9bb65cfa
9bb75ced9bb85d8c9bb95cea9bba5d0b9bbb5d159bbc5d179bbd5d5c9bbe5d1f9bbf5d1b9bc05d119bc15d149bc25d229bc35d1a9bc45d199bc55d189bc65d4c9bc75d529bc85d4e9bc95d4b9bca5d6c9bcb5d739bcc5d769bcd5d879bce5d849bcf5d829bd05da29bd15d9d9bd25dac9bd35dae9bd45dbd9bd55d909bd65db7
9bd75dbc9bd85dc99bd95dcd9bda5dd39bdb5dd29bdc5dd69bdd5ddb9bde5deb9bdf5df29be05df59be15e0b9be25e1a9be35e199be45e119be55e1b9be65e369be75e379be85e449be95e439bea5e409beb5e4e9bec5e579bed5e549bee5e5f9bef5e629bf05e649bf15e479bf25e759bf35e769bf45e7a9bf59ebc9bf65e7f
9bf75ea09bf85ec19bf95ec29bfa5ec89bfb5ed09bfc5ecf9c405ed69c415ee39c425edd9c435eda9c445edb9c455ee29c465ee19c475ee89c485ee99c495eec9c4a5ef19c4b5ef39c4c5ef09c4d5ef49c4e5ef89c4f5efe9c505f039c515f099c525f5d9c535f5c9c545f0b9c555f119c565f169c575f299c585f2d9c595f38
9c5a5f419c5b5f489c5c5f4c9c5d5f4e9c5e5f2f9c5f5f519c605f569c615f579c625f599c635f619c645f6d9c655f739c665f779c675f839c685f829c695f7f9c6a5f8a9c6b5f889c6c5f919c6d5f879c6e5f9e9c6f5f999c705f989c715fa09c725fa89c735fad9c745fbc9c755fd69c765ffb9c775fe49c785ff89c795ff1
9c7a5fdd9c7b60b39c7c5fff9c7d60219c7e60609c8060199c8160109c8260299c83600e9c8460319c85601b9c8660159c87602b9c8860269c89600f9c8a603a9c8b605a9c8c60419c8d606a9c8e60779c8f605f9c90604a9c9160469c92604d9c9360639c9460439c9560649c9660429c97606c9c98606b9c9960599c9a6081
9c9b608d9c9c60e79c9d60839c9e609a9c9f60849ca0609b9ca160969ca260979ca360929ca460a79ca5608b9ca660e19ca760b89ca860e09ca960d39caa60b49cab5ff09cac60bd9cad60c69cae60b59caf60d89cb0614d9cb161159cb261069cb360f69cb460f79cb561009cb660f49cb760fa9cb861039cb961219cba60fb
9cbb60f19cbc610d9cbd610e9cbe61479cbf613e9cc061289cc161279cc2614a9cc3613f9cc4613c9cc5612c9cc661349cc7613d9cc861429cc961449cca61739ccb61779ccc61589ccd61599cce615a9ccf616b9cd061749cd1616f9cd261659cd361719cd4615f9cd5615d9cd661539cd761759cd861999cd961969cda6187
9cdb61ac9cdc61949cdd619a9cde618a9cdf61919ce061ab9ce161ae9ce261cc9ce361ca9ce461c99ce561f79ce661c89ce761c39ce861c69ce961ba9cea61cb9ceb7f799cec61cd9ced61e69cee61e39cef61f69cf061fa9cf161f49cf261ff9cf361fd9cf461fc9cf561fe9cf662009cf762089cf862099cf9620d9cfa620c
9cfb62149cfc621b9d40621e9d4162219d42622a9d43622e9d4462309d4562329d4662339d4762419d48624e9d49625e9d4a62639d4b625b9d4c62609d4d62689d4e627c9d4f62829d5062899d51627e9d5262929d5362939d5462969d5562d49d5662839d5762949d5862d79d5962d19d5a62bb9d5b62cf9d5c62ff9d5d62c6
9d5e64d49d5f62c89d6062dc9d6162cc9d6262ca9d6362c29d6462c79d65629b9d6662c99d67630c9d6862ee9d6962f19d6a63279d6b63029d6c63089d6d62ef9d6e62f59d6f63509d70633e9d71634d9d72641c9d73634f9d7463969d75638e9d7663809d7763ab9d7863769d7963a39d7a638f9d7b63899d7c639f9d7d63b5
9d7e636b9d8063699d8163be9d8263e99d8363c09d8463c69d8563e39d8663c99d8763d29d8863f69d8963c49d8a64169d8b64349d8c64069d8d64139d8e64269d8f64369d90651d9d9164179d9264289d93640f9d9464679d95646f9d9664769d97644e9d98652a9d9964959d9a64939d9b64a59d9c64a99d9d64889d9e64bc
9d9f64da9da064d29da164c59da264c79da364bb9da464d89da564c29da664f19da764e79da882099da964e09daa64e19dab62ac9dac64e39dad64ef9dae652c9daf64f69db064f49db164f29db264fa9db365009db464fd9db565189db6651c9db765059db865249db965239dba652b9dbb65349dbc65359dbd65379dbe6536
9dbf65389dc0754b9dc165489dc265569dc365559dc4654d9dc565589dc6655e9dc7655d9dc865729dc965789dca65829dcb65839dcc8b8a9dcd659b9dce659f9dcf65ab9dd065b79dd165c39dd265c69dd365c19dd465c49dd565cc9dd665d29dd765db9dd865d99dd965e09dda65e19ddb65f19ddc67729ddd660a9dde6603
9ddf65fb9de067739de166359de266369de366349de4661c9de5664f9de666449de766499de866419de9665e9dea665d9deb66649dec66679ded66689dee665f9def66629df066709df166839df266889df3668e9df466899df566849df666989df7669d9df866c19df966b99dfa66c99dfb66be9dfc66bc9e4066c49e4166b8
9e4266d69e4366da9e4466e09e45663f9e4666e69e4766e99e4866f09e4966f59e4a66f79e4b670f9e4c67169e4d671e9e4e67269e4f67279e5097389e51672e9e52673f9e5367369e5467419e5567389e5667379e5767469e58675e9e5967609e5a67599e5b67639e5c67649e5d67899e5e67709e5f67a99e60677c9e61676a
9e62678c9e63678b9e6467a69e6567a19e6667859e6767b79e6867ef9e6967b49e6a67ec9e6b67b39e6c67e99e6d67b89e6e67e49e6f67de9e7067dd9e7167e29e7267ee9e7367b99e7467ce9e7567c69e7667e79e776a9c9e78681e9e7968469e7a68299e7b68409e7c684d9e7d68329e7e684e9e8068b39e81682b9e826859
9e8368639e8468779e85687f9e86689f9e87688f9e8868ad9e8968949e8a689d9e8b689b9e8c68839e8d6aae9e8e68b99e8f68749e9068b59e9168a09e9268ba9e93690f9e94688d9e95687e9e9669019e9768ca9e9869089e9968d89e9a69229e9b69269e9c68e19e9d690c9e9e68cd9e9f68d49ea068e79ea168d59ea26936
9ea369129ea469049ea568d79ea668e39ea769259ea868f99ea968e09eaa68ef9eab69289eac692a9ead691a9eae69239eaf69219eb068c69eb169799eb269779eb3695c9eb469789eb5696b9eb669549eb7697e9eb8696e9eb969399eba69749ebb693d9ebc69599ebd69309ebe69619ebf695e9ec0695d9ec169819ec2696a
9ec369b29ec469ae9ec569d09ec669bf9ec769c19ec869d39ec969be9eca69ce9ecb5be89ecc69ca9ecd69dd9ece69bb9ecf69c39ed069a79ed16a2e9ed269919ed369a09ed4699c9ed569959ed669b49ed769de9ed869e89ed96a029eda6a1b9edb69ff9edc6b0a9edd69f99ede69f29edf69e79ee06a059ee169b19ee26a1e
9ee369ed9ee46a149ee569eb9ee66a0a9ee76a129ee86ac19ee96a239eea6a139eeb6a449eec6a0c9eed6a729eee6a369eef6a789ef06a479ef16a629ef26a599ef36a669ef46a489ef56a389ef66a229ef76a909ef86a8d9ef96aa09efa6a849efb6aa29efc6aa39f406a979f4186179f426abb9f436ac39f446ac29f456ab8
9f466ab39f476aac9f486ade9f496ad19f4a6adf9f4b6aaa9f4c6ada9f4d6aea9f4e6afb9f4f6b059f5086169f516afa9f526b129f536b169f549b319f556b1f9f566b389f576b379f5876dc9f596b399f5a98ee9f5b6b479f5c6b439f5d6b499f5e6b509f5f6b599f606b549f616b5b9f626b5f9f636b619f646b789f656b79
9f666b7f9f676b809f686b849f696b839f6a6b8d9f6b6b989f6c6b959f6d6b9e9f6e6ba49f6f6baa9f706bab9f716baf9f726bb29f736bb19f746bb39f756bb79f766bbc9f776bc69f786bcb9f796bd39f7a6bdf9f7b6bec9f7c6beb9f7d6bf39f7e6bef9f809ebe9f816c089f826c139f836c149f846c1b9f856c249f866c23
9f876c5e9f886c559f896c629f8a6c6a9f8b6c829f8c6c8d9f8d6c9a9f8e6c819f8f6c9b9f906c7e9f916c689f926c739f936c929f946c909f956cc49f966cf19f976cd39f986cbd9f996cd79f9a6cc59f9b6cdd9f9c6cae9f9d6cb19f9e6cbe9f9f6cba9fa06cdb9fa16cef9fa26cd99fa36cea9fa46d1f9fa5884d9fa66d36
9fa76d2b9fa86d3d9fa96d389faa6d199fab6d359fac6d339fad6d129fae6d0c9faf6d639fb06d939fb16d649fb26d5a9fb36d799fb46d599fb56d8e9fb66d959fb76fe49fb86d859fb96df99fba6e159fbb6e0a9fbc6db59fbd6dc79fbe6de69fbf6db89fc06dc69fc16dec9fc26dde9fc36dcc9fc46de89fc56dd29fc66dc5
9fc76dfa9fc86dd99fc96de49fca6dd59fcb6dea9fcc6dee9fcd6e2d9fce6e6e9fcf6e2e9fd06e199fd16e729fd26e5f9fd36e3e9fd46e239fd56e6b9fd66e2b9fd76e769fd86e4d9fd96e1f9fda6e439fdb6e3a9fdc6e4e9fdd6e249fde6eff9fdf6e1d9fe06e389fe16e829fe26eaa9fe36e989fe46ec99fe56eb79fe66ed3
9fe76ebd9fe86eaf9fe96ec49fea6eb29feb6ed49fec6ed59fed6e8f9fee6ea59fef6ec29ff06e9f9ff16f419ff26f119ff3704c9ff46eec9ff56ef89ff66efe9ff76f3f9ff86ef29ff96f319ffa6eef9ffb6f329ffc6ecce0406f3ee0416f13e0426ef7e0436f86e0446f7ae0456f78e0466f81e0476f80e0486f6fe0496f5b
e04a6ff3e04b6f6de04c6f82e04d6f7ce04e6f58e04f6f8ee0506f91e0516fc2e0526f66e0536fb3e0546fa3e0556fa1e0566fa4e0576fb9e0586fc6e0596faae05a6fdfe05b6fd5e05c6fece05d6fd4e05e6fd8e05f6ff1e0606feee0616fdbe0627009e063700be0646ffae0657011e0667001e067700fe0686ffee069701b
e06a701ae06b6f74e06c701de06d7018e06e701fe06f7030e070703ee0717032e0727051e0737063e0747099e0757092e07670afe07770f1e07870ace07970b8e07a70b3e07b70aee07c70dfe07d70cbe07e70dde08070d9e0817109e08270fde083711ce0847119e0857165e0867155e0877188e0887166e0897162e08a714c
e08b7156e08c716ce08d718fe08e71fbe08f7184e0907195e09171a8e09271ace09371d7e09471b9e09571bee09671d2e09771c9e09871d4e09971cee09a71e0e09b71ece09c71e7e09d71f5e09e71fce09f71f9e0a071ffe0a1720de0a27210e0a3721be0a47228e0a5722de0a6722ce0a77230e0a87232e0a9723be0aa723c
e0ab723fe0ac7240e0ad7246e0ae724be0af7258e0b07274e0b1727ee0b27282e0b37281e0b47287e0b57292e0b67296e0b772a2e0b872a7e0b972b9e0ba72b2e0bb72c3e0bc72c6e0bd72c4e0be72cee0bf72d2e0c072e2e0c172e0e0c272e1e0c372f9e0c472f7e0c5500fe0c67317e0c7730ae0c8731ce0c97316e0ca731d
e0cb7334e0cc732fe0cd7329e0ce7325e0cf733ee0d0734ee0d1734fe0d29ed8e0d37357e0d4736ae0d57368e0d67370e0d77378e0d87375e0d9737be0da737ae0db73c8e0dc73b3e0dd73cee0de73bbe0df73c0e0e073e5e0e173eee0e273dee0e374a2e0e47405e0e5746fe0e67425e0e773f8e0e87432e0e9743ae0ea7455
e0eb743fe0ec745fe0ed7459e0ee7441e0ef745ce0f07469e0f17470e0f27463e0f3746ae0f47476e0f5747ee0f6748be0f7749ee0f874a7e0f974cae0fa74cfe0fb74d4e0fc73f1e14074e0e14174e3e14274e7e14374e9e14474eee14574f2e14674f0e14774f1e14874f8e14974f7e14a7504e14b7503e14c7505e14d750c
e14e750ee14f750de1507515e1517513e152751ee1537526e154752ce155753ce1567544e157754de158754ae1597549e15a755be15b7546e15c755ae15d7569e15e7564e15f7567e160756be161756de1627578e1637576e1647586e1657587e1667574e167758ae1687589e1697582e16a7594e16b759ae16c759de16d75a5
e16e75a3e16f75c2e17075b3e17175c3e17275b5e17375bde17475b8e17575bce17675b1e17775cde17875cae17975d2e17a75d9e17b75e3e17c75dee17d75fee17e75ffe18075fce1817601e18275f0e18375fae18475f2e18575f3e186760be187760de1887609e189761fe18a7627e18b7620e18c7621e18d7622e18e7624
e18f7634e1907630e191763be1927647e1937648e1947646e195765ce1967658e1977661e1987662e1997668e19a7669e19b766ae19c7667e19d766ce19e7670e19f7672e1a07676e1a17678e1a2767ce1a37680e1a47683e1a57688e1a6768be1a7768ee1a87696e1a97693e1aa7699e1ab769ae1ac76b0e1ad76b4e1ae76b8
e1af76b9e1b076bae1b176c2e1b276cde1b376d6e1b476d2e1b576dee1b676e1e1b776e5e1b876e7e1b976eae1ba862fe1bb76fbe1bc7708e1bd7707e1be7704e1bf7729e1c07724e1c1771ee1c27725e1c37726e1c4771be1c57737e1c67738e1c77747e1c8775ae1c97768e1ca776be1cb775be1cc7765e1cd777fe1ce777e
e1cf7779e1d0778ee1d1778be1d27791e1d377a0e1d4779ee1d577b0e1d677b6e1d777b9e1d877bfe1d977bce1da77bde1db77bbe1dc77c7e1dd77cde1de77d7e1df77dae1e077dce1e177e3e1e277eee1e377fce1e4780ce1e57812e1e67926e1e77820e1e8792ae1e97845e1ea788ee1eb7874e1ec7886e1ed787ce1ee789a
e1ef788ce1f078a3e1f178b5e1f278aae1f378afe1f478d1e1f578c6e1f678cbe1f778d4e1f878bee1f978bce1fa78c5e1fb78cae1fc78ece24078e7e24178dae24278fde24378f4e2447907e2457912e2467911e2477919e248792ce249792be24a7940e24b7960e24c7957e24d795fe24e795ae24f7955e2507953e251797a
e252797fe253798ae254799de25579a7e2569f4be25779aae25879aee25979b3e25a79b9e25b79bae25c79c9e25d79d5e25e79e7e25f79ece26079e1e26179e3e2627a08e2637a0de2647a18e2657a19e2667a20e2677a1fe2687980e2697a31e26a7a3be26b7a3ee26c7a37e26d7a43e26e7a57e26f7a49e2707a61e2717a62
e2727a69e2739f9de2747a70e2757a79e2767a7de2777a88e2787a97e2797a95e27a7a98e27b7a96e27c7aa9e27d7ac8e27e7ab0e2807ab6e2817ac5e2827ac4e2837abfe2849083e2857ac7e2867acae2877acde2887acfe2897ad5e28a7ad3e28b7ad9e28c7adae28d7adde28e7ae1e28f7ae2e2907ae6e2917aede2927af0
e2937b02e2947b0fe2957b0ae2967b06e2977b33e2987b18e2997b19e29a7b1ee29b7b35e29c7b28e29d7b36e29e7b50e29f7b7ae2a07b04e2a17b4de2a27b0be2a37b4ce2a47b45e2a57b75e2a67b65e2a77b74e2a87b67e2a97b70e2aa7b71e2ab7b6ce2ac7b6ee2ad7b9de2ae7b98e2af7b9fe2b07b8de2b17b9ce2b27b9a
e2b37b8be2b47b92e2b57b8fe2b67b5de2b77b99e2b87bcbe2b97bc1e2ba7bcce2bb7bcfe2bc7bb4e2bd7bc6e2be7bdde2bf7be9e2c07c11e2c17c14e2c27be6e2c37be5e2c47c60e2c57c00e2c67c07e2c77c13e2c87bf3e2c97bf7e2ca7c17e2cb7c0de2cc7bf6e2cd7c23e2ce7c27e2cf7c2ae2d07c1fe2d17c37e2d27c2b
e2d37c3de2d47c4ce2d57c43e2d67c54e2d77c4fe2d87c40e2d97c50e2da7c58e2db7c5fe2dc7c64e2dd7c56e2de7c65e2df7c6ce2e07c75e2e17c83e2e27c90e2e37ca4e2e47cade2e57ca2e2e67cabe2e77ca1e2e87ca8e2e97cb3e2ea7cb2e2eb7cb1e2ec7caee2ed7cb9e2ee7cbde2ef7cc0e2f07cc5e2f17cc2e2f27cd8
e2f37cd2e2f47cdce2f57ce2e2f69b3be2f77cefe2f87cf2e2f97cf4e2fa7cf6e2fb7cfae2fc7d06e3407d02e3417d1ce3427d15e3437d0ae3447d45e3457d4be3467d2ee3477d32e3487d3fe3497d35e34a7d46e34b7d73e34c7d56e34d7d4ee34e7d72e34f7d68e3507d6ee3517d4fe3527d63e3537d93e3547d89e3557d5b
e3567d8fe3577d7de3587d9be3597dbae35a7daee35b7da3e35c7db5e35d7dc7e35e7dbde35f7dabe3607e3de3617da2e3627dafe3637ddce3647db8e3657d9fe3667db0e3677dd8e3687ddde3697de4e36a7ddee36b7dfbe36c7df2e36d7de1e36e7e05e36f7e0ae3707e23e3717e21e3727e12e3737e31e3747e1fe3757e09
e3767e0be3777e22e3787e46e3797e66e37a7e3be37b7e35e37c7e39e37d7e43e37e7e37e3807e32e3817e3ae3827e67e3837e5de3847e56e3857e5ee3867e59e3877e5ae3887e79e3897e6ae38a7e69e38b7e7ce38c7e7be38d7e83e38e7dd5e38f7e7de3908faee3917e7fe3927e88e3937e89e3947e8ce3957e92e3967e90
e3977e93e3987e94e3997e96e39a7e8ee39b7e9be39c7e9ce39d7f38e39e7f3ae39f7f45e3a07f4ce3a17f4de3a27f4ee3a37f50e3a47f51e3a57f55e3a67f54e3a77f58e3a87f5fe3a97f60e3aa7f68e3ab7f69e3ac7f67e3ad7f78e3ae7f82e3af7f86e3b07f83e3b17f88e3b27f87e3b37f8ce3b47f94e3b57f9ee3b67f9d
e3b77f9ae3b87fa3e3b97fafe3ba7fb2e3bb7fb9e3bc7faee3bd7fb6e3be7fb8e3bf8b71e3c07fc5e3c17fc6e3c27fcae3c37fd5e3c47fd4e3c57fe1e3c67fe6e3c77fe9e3c87ff3e3c97ff9e3ca98dce3cb8006e3cc8004e3cd800be3ce8012e3cf8018e3d08019e3d1801ce3d28021e3d38028e3d4803fe3d5803be3d6804a
e3d78046e3d88052e3d98058e3da805ae3db805fe3dc8062e3dd8068e3de8073e3df8072e3e08070e3e18076e3e28079e3e3807de3e4807fe3e58084e3e68086e3e78085e3e8809be3e98093e3ea809ae3eb80ade3ec5190e3ed80ace3ee80dbe3ef80e5e3f080d9e3f180dde3f280c4e3f380dae3f480d6e3f58109e3f680ef
e3f780f1e3f8811be3f98129e3fa8123e3fb812fe3fc814be440968be4418146e442813ee4438153e4448151e44580fce4468171e447816ee4488165e4498166e44a8174e44b8183e44c8188e44d818ae44e8180e44f8182e45081a0e4518195e45281a4e45381a3e454815fe4558193e45681a9e45781b0e45881b5e45981be
e45a81b8e45b81bde45c81c0e45d81c2e45e81bae45f81c9e46081cde46181d1e46281d9e46381d8e46481c8e46581dae46681dfe46781e0e46881e7e46981fae46a81fbe46b81fee46c8201e46d8202e46e8205e46f8207e470820ae471820de4728210e4738216e4748229e475822be4768238e4778233e4788240e4798259
e47a8258e47b825de47c825ae47d825fe47e8264e4808262e4818268e482826ae483826be484822ee4858271e4868277e4878278e488827ee489828de48a8292e48b82abe48c829fe48d82bbe48e82ace48f82e1e49082e3e49182dfe49282d2e49382f4e49482f3e49582fae4968393e4978303e49882fbe49982f9e49a82de
e49b8306e49c82dce49d8309e49e82d9e49f8335e4a08334e4a18316e4a28332e4a38331e4a48340e4a58339e4a68350e4a78345e4a8832fe4a9832be4aa8317e4ab8318e4ac8385e4ad839ae4ae83aae4af839fe4b083a2e4b18396e4b28323e4b3838ee4b48387e4b5838ae4b6837ce4b783b5e4b88373e4b98375e4ba83a0
e4bb8389e4bc83a8e4bd83f4e4be8413e4bf83ebe4c083cee4c183fde4c28403e4c383d8e4c4840be4c583c1e4c683f7e4c78407e4c883e0e4c983f2e4ca840de4cb8422e4cc8420e4cd83bde4ce8438e4cf8506e4d083fbe4d1846de4d2842ae4d3843ce4d4855ae4d58484e4d68477e4d7846be4d884ade4d9846ee4da8482
e4db8469e4dc8446e4dd842ce4de846fe4df8479e4e08435e4e184cae4e28462e4e384b9e4e484bfe4e5849fe4e684d9e4e784cde4e884bbe4e984dae4ea84d0e4eb84c1e4ec84c6e4ed84d6e4ee84a1e4ef8521e4f084ffe4f184f4e4f28517e4f38518e4f4852ce4f5851fe4f68515e4f78514e4f884fce4f98540e4fa8563
e4fb8558e4fc8548e5408541e5418602e542854be5438555e5448580e54585a4e5468588e5478591e548858ae54985a8e54a856de54b8594e54c859be54d85eae54e8587e54f859ce5508577e551857ee5528590e55385c9e55485bae55585cfe55685b9e55785d0e55885d5e55985dde55a85e5e55b85dce55c85f9e55d860a
e55e8613e55f860be56085fee56185fae5628606e5638622e564861ae5658630e566863fe567864de5684e55e5698654e56a865fe56b8667e56c8671e56d8693e56e86a3e56f86a9e57086aae571868be572868ce57386b6e57486afe57586c4e57686c6e57786b0e57886c9e5798823e57a86abe57b86d4e57c86dee57d86e9
e57e86ece58086dfe58186dbe58286efe5838712e5848706e5858708e5868700e5878703e58886fbe5898711e58a8709e58b870de58c86f9e58d870ae58e8734e58f873fe5908737e591873be5928725e5938729e594871ae5958760e596875fe5978778e598874ce599874ee59a8774e59b8757e59c8768e59d876ee59e8759
e59f8753e5a08763e5a1876ae5a28805e5a387a2e5a4879fe5a58782e5a687afe5a787cbe5a887bde5a987c0e5aa87d0e5ab96d6e5ac87abe5ad87c4e5ae87b3e5af87c7e5b087c6e5b187bbe5b287efe5b387f2e5b487e0e5b5880fe5b6880de5b787fee5b887f6e5b987f7e5ba880ee5bb87d2e5bc8811e5bd8816e5be8815
e5bf8822e5c08821e5c18831e5c28836e5c38839e5c48827e5c5883be5c68844e5c78842e5c88852e5c98859e5ca885ee5cb8862e5cc886be5cd8881e5ce887ee5cf889ee5d08875e5d1887de5d288b5e5d38872e5d48882e5d58897e5d68892e5d788aee5d88899e5d988a2e5da888de5db88a4e5dc88b0e5dd88bfe5de88b1
e5df88c3e5e088c4e5e188d4e5e288d8e5e388d9e5e488dde5e588f9e5e68902e5e788fce5e888f4e5e988e8e5ea88f2e5eb8904e5ec890ce5ed890ae5ee8913e5ef8943e5f0891ee5f18925e5f2892ae5f3892be5f48941e5f58944e5f6893be5f78936e5f88938e5f9894ce5fa891de5fb8960e5fc895ee6408966e6418964
e642896de643896ae644896fe6458974e6468977e647897ee6488983e6498988e64a898ae64b8993e64c8998e64d89a1e64e89a9e64f89a6e65089ace65189afe65289b2e65389bae65489bde65589bfe65689c0e65789dae65889dce65989dde65a89e7e65b89f4e65c89f8e65d8a03e65e8a16e65f8a10e6608a0ce6618a1b
e6628a1de6638a25e6648a36e6658a41e6668a5be6678a52e6688a46e6698a48e66a8a7ce66b8a6de66c8a6ce66d8a62e66e8a85e66f8a82e6708a84e6718aa8e6728aa1e6738a91e6748aa5e6758aa6e6768a9ae6778aa3e6788ac4e6798acde67a8ac2e67b8adae67c8aebe67d8af3e67e8ae7e6808ae4e6818af1e6828b14
e6838ae0e6848ae2e6858af7e6868adee6878adbe6888b0ce6898b07e68a8b1ae68b8ae1e68c8b16e68d8b10e68e8b17e68f8b20e6908b33e69197abe6928b26e6938b2be6948b3ee6958b28e6968b41e6978b4ce6988b4fe6998b4ee69a8b49e69b8b56e69c8b5be69d8b5ae69e8b6be69f8b5fe6a08b6ce6a18b6fe6a28b74
e6a38b7de6a48b80e6a58b8ce6a68b8ee6a78b92e6a88b93e6a98b96e6aa8b99e6ab8b9ae6ac8c3ae6ad8c41e6ae8c3fe6af8c48e6b08c4ce6b18c4ee6b28c50e6b38c55e6b48c62e6b58c6ce6b68c78e6b78c7ae6b88c82e6b98c89e6ba8c85e6bb8c8ae6bc8c8de6bd8c8ee6be8c94e6bf8c7ce6c08c98e6c1621de6c28cad
e6c38caae6c48cbde6c58cb2e6c68cb3e6c78caee6c88cb6e6c98cc8e6ca8cc1e6cb8ce4e6cc8ce3e6cd8cdae6ce8cfde6cf8cfae6d08cfbe6d18d04e6d28d05e6d38d0ae6d48d07e6d58d0fe6d68d0de6d78d10e6d89f4ee6d98d13e6da8ccde6db8d14e6dc8d16e6dd8d67e6de8d6de6df8d71e6e08d73e6e18d81e6e28d99
e6e38dc2e6e48dbee6e58dbae6e68dcfe6e78ddae6e88dd6e6e98dcce6ea8ddbe6eb8dcbe6ec8deae6ed8debe6ee8ddfe6ef8de3e6f08dfce6f18e08e6f28e09e6f38dffe6f48e1de6f58e1ee6f68e10e6f78e1fe6f88e42e6f98e35e6fa8e30e6fb8e34e6fc8e4ae7408e47e7418e49e7428e4ce7438e50e7448e48e7458e59
e7468e64e7478e60e7488e2ae7498e63e74a8e55e74b8e76e74c8e72e74d8e7ce74e8e81e74f8e87e7508e85e7518e84e7528e8be7538e8ae7548e93e7558e91e7568e94e7578e99e7588eaae7598ea1e75a8eace75b8eb0e75c8ec6e75d8eb1e75e8ebee75f8ec5e7608ec8e7618ecbe7628edbe7638ee3e7648efce7658efb
e7668eebe7678efee7688f0ae7698f05e76a8f15e76b8f12e76c8f19e76d8f13e76e8f1ce76f8f1fe7708f1be7718f0ce7728f26e7738f33e7748f3be7758f39e7768f45e7778f42e7788f3ee7798f4ce77a8f49e77b8f46e77c8f4ee77d8f57e77e8f5ce7808f62e7818f63e7828f64e7838f9ce7848f9fe7858fa3e7868fad
e7878fafe7888fb7e7898fdae78a8fe5e78b8fe2e78c8feae78d8fefe78e9087e78f8ff4e7909005e7918ff9e7928ffae7939011e7949015e7959021e796900de797901ee7989016e799900be79a9027e79b9036e79c9035e79d9039e79e8ff8e79f904fe7a09050e7a19051e7a29052e7a3900ee7a49049e7a5903ee7a69056
e7a79058e7a8905ee7a99068e7aa906fe7ab9076e7ac96a8e7ad9072e7ae9082e7af907de7b09081e7b19080e7b2908ae7b39089e7b4908fe7b590a8e7b690afe7b790b1e7b890b5e7b990e2e7ba90e4e7bb6248e7bc90dbe7bd9102e7be9112e7bf9119e7c09132e7c19130e7c2914ae7c39156e7c49158e7c59163e7c69165
e7c79169e7c89173e7c99172e7ca918be7cb9189e7cc9182e7cd91a2e7ce91abe7cf91afe7d091aae7d191b5e7d291b4e7d391bae7d491c0e7d591c1e7d691c9e7d791cbe7d891d0e7d991d6e7da91dfe7db91e1e7dc91dbe7dd91fce7de91f5e7df91f6e7e0921ee7e191ffe7e29214e7e3922ce7e49215e7e59211e7e6925e
e7e79257e7e89245e7e99249e7ea9264e7eb9248e7ec9295e7ed923fe7ee924be7ef9250e7f0929ce7f19296e7f29293e7f3929be7f4925ae7f592cfe7f692b9e7f792b7e7f892e9e7f9930fe7fa92fae7fb9344e7fc932ee8409319e8419322e842931ae8439323e844933ae8459335e846933be847935ce8489360e849937c
e84a936ee84b9356e84c93b0e84d93ace84e93ade84f9394e85093b9e85193d6e85293d7e85393e8e85493e5e85593d8e85693c3e85793dde85893d0e85993c8e85a93e4e85b941ae85c9414e85d9413e85e9403e85f9407e8609410e8619436e862942be8639435e8649421e865943ae8669441e8679452e8689444e869945b
e86a9460e86b9462e86c945ee86d946ae86e9229e86f9470e8709475e8719477e872947de873945ae874947ce875947ee8769481e877947fe8789582e8799587e87a958ae87b9594e87c9596e87d9598e87e9599e88095a0e88195a8e88295a7e88395ade88495bce88595bbe88695b9e88795bee88895cae8896ff6e88a95c3
e88b95cde88c95cce88d95d5e88e95d4e88f95d6e89095dce89195e1e89295e5e89395e2e8949621e8959628e896962ee897962fe8989642e899964ce89a964fe89b964be89c9677e89d965ce89e965ee89f965de8a0965fe8a19666e8a29672e8a3966ce8a4968de8a59698e8a69695e8a79697e8a896aae8a996a7e8aa96b1
e8ab96b2e8ac96b0e8ad96b4e8ae96b6e8af96b8e8b096b9e8b196cee8b296cbe8b396c9e8b496cde8b5894de8b696dce8b7970de8b896d5e8b996f9e8ba9704e8bb9706e8bc9708e8bd9713e8be970ee8bf9711e8c0970fe8c19716e8c29719e8c39724e8c4972ae8c59730e8c69739e8c7973de8c8973ee8c99744e8ca9746
e8cb9748e8cc9742e8cd9749e8ce975ce8cf9760e8d09764e8d19766e8d29768e8d352d2e8d4976be8d59771e8d69779e8d79785e8d8977ce8d99781e8da977ae8db9786e8dc978be8dd978fe8de9790e8df979ce8e097a8e8e197a6e8e297a3e8e397b3e8e497b4e8e597c3e8e697c6e8e797c8e8e897cbe8e997dce8ea97ed
e8eb9f4fe8ec97f2e8ed7adfe8ee97f6e8ef97f5e8f0980fe8f1980ce8f29838e8f39824e8f49821e8f59837e8f6983de8f79846e8f8984fe8f9984be8fa986be8fb986fe8fc9870e9409871e9419874e9429873e94398aae94498afe94598b1e94698b6e94798c4e94898c3e94998c6e94a98e9e94b98ebe94c9903e94d9909
e94e9912e94f9914e9509918e9519921e952991de953991ee9549924e9559920e956992ce957992ee958993de959993ee95a9942e95b9949e95c9945e95d9950e95e994be95f9951e9609952e961994ce9629955e9639997e9649998e96599a5e96699ade96799aee96899bce96999dfe96a99dbe96b99dde96c99d8e96d99d1
e96e99ede96f99eee97099f1e97199f2e97299fbe97399f8e9749a01e9759a0fe9769a05e97799e2e9789a19e9799a2be97a9a37e97b9a45e97c9a42e97d9a40e97e9a43e9809a3ee9819a55e9829a4de9839a5be9849a57e9859a5fe9869a62e9879a65e9889a64e9899a69e98a9a6be98b9a6ae98c9aade98d9ab0e98e9abc
e98f9ac0e9909acfe9919ad1e9929ad3e9939ad4e9949adee9959adfe9969ae2e9979ae3e9989ae6e9999aefe99a9aebe99b9aeee99c9af4e99d9af1e99e9af7e99f9afbe9a09b06e9a19b18e9a29b1ae9a39b1fe9a49b22e9a59b23e9a69b25e9a79b27e9a89b28e9a99b29e9aa9b2ae9ab9b2ee9ac9b2fe9ad9b32e9ae9b44
e9af9b43e9b09b4fe9b19b4de9b29b4ee9b39b51e9b49b58e9b59b74e9b69b93e9b79b83e9b89b91e9b99b96e9ba9b97e9bb9b9fe9bc9ba0e9bd9ba8e9be9bb4e9bf9bc0e9c09bcae9c19bb9e9c29bc6e9c39bcfe9c49bd1e9c59bd2e9c69be3e9c79be2e9c89be4e9c99bd4e9ca9be1e9cb9c3ae9cc9bf2e9cd9bf1e9ce9bf0
e9cf9c15e9d09c14e9d19c09e9d29c13e9d39c0ce9d49c06e9d59c08e9d69c12e9d79c0ae9d89c04e9d99c2ee9da9c1be9db9c25e9dc9c24e9dd9c21e9de9c30e9df9c47e9e09c32e9e19c46e9e29c3ee9e39c5ae9e49c60e9e59c67e9e69c76e9e79c78e9e89ce7e9e99cece9ea9cf0e9eb9d09e9ec9d08e9ed9cebe9ee9d03
e9ef9d06e9f09d2ae9f19d26e9f29dafe9f39d23e9f49d1fe9f59d44e9f69d15e9f79d12e9f89d41e9f99d3fe9fa9d3ee9fb9d46e9fc9d48ea409d5dea419d5eea429d64ea439d51ea449d50ea459d59ea469d72ea479d89ea489d87ea499dabea4a9d6fea4b9d7aea4c9d9aea4d9da4ea4e9da9ea4f9db2ea509dc4ea519dc1
ea529dbbea539db8ea549dbaea559dc6ea569dcfea579dc2ea589dd9ea599dd3ea5a9df8ea5b9de6ea5c9dedea5d9defea5e9dfdea5f9e1aea609e1bea619e1eea629e75ea639e79ea649e7dea659e81ea669e88ea679e8bea689e8cea699e92ea6a9e95ea6b9e91ea6c9e9dea6d9ea5ea6e9ea9ea6f9eb8ea709eaaea719ead
ea729761ea739eccea749eceea759ecfea769ed0ea779ed4ea789edcea799edeea7a9eddea7b9ee0ea7c9ee5ea7d9ee8ea7e9eefea809ef4ea819ef6ea829ef7ea839ef9ea849efbea859efcea869efdea879f07ea889f08ea8976b7ea8a9f15ea8b9f21ea8c9f2cea8d9f3eea8e9f4aea8f9f52ea909f54ea919f63ea929f5f
ea939f60ea949f61ea959f66ea969f67ea979f6cea989f6aea999f77ea9a9f72ea9b9f76ea9c9f95ea9d9f9cea9e9fa0ea9f582feaa069c7eaa19059eaa27464eaa351dceaa47199ed407e8aed41891ced429348ed439288ed4484dced454fc9ed4670bbed476631ed4868c8ed4992f9ed4a66fbed4b5f45ed4c4e28ed4d4ee1
ed4e4efced4f4f00ed504f03ed514f39ed524f56ed534f92ed544f8aed554f9aed564f94ed574fcded585040ed595022ed5a4fffed5b501eed5c5046ed5d5070ed5e5042ed5f5094ed6050f4ed6150d8ed62514aed635164ed64519ded6551beed6651eced675215ed68529ced6952a6ed6a52c0ed6b52dbed6c5300ed6d5307
ed6e5324ed6f5372ed705393ed7153b2ed7253dded73fa0eed74549ced75548aed7654a9ed7754ffed785586ed795759ed7a5765ed7b57aced7c57c8ed7d57c7ed7efa0fed80fa10ed81589eed8258b2ed83590bed845953ed85595bed86595ded875963ed8859a4ed8959baed8a5b56ed8b5bc0ed8c752fed8d5bd8ed8e5bec
ed8f5c1eed905ca6ed915cbaed925cf5ed935d27ed945d53ed95fa11ed965d42ed975d6ded985db8ed995db9ed9a5dd0ed9b5f21ed9c5f34ed9d5f67ed9e5fb7ed9f5fdeeda0605deda16085eda2608aeda360deeda460d5eda56120eda660f2eda76111eda86137eda96130edaa6198edab6213edac62a6edad63f5edae6460
edaf649dedb064ceedb1654eedb26600edb36615edb4663bedb56609edb6662eedb7661eedb86624edb96665edba6657edbb6659edbcfa12edbd6673edbe6699edbf66a0edc066b2edc166bfedc266faedc3670eedc4f929edc56766edc667bbedc76852edc867c0edc96801edca6844edcb68cfedccfa13edcd6968edcefa14
edcf6998edd069e2edd16a30edd26a6bedd36a46edd46a73edd56a7eedd66ae2edd76ae4edd86bd6edd96c3fedda6c5ceddb6c86eddc6c6feddd6cdaedde6d04eddf6d87ede06d6fede16d96ede26dacede36dcfede46df8ede56df2ede66dfcede76e39ede86e5cede96e27edea6e3cedeb6ebfedec6f88eded6fb5edee6ff5
edef7005edf07007edf17028edf27085edf370abedf4710fedf57104edf6715cedf77146edf87147edf9fa15edfa71c1edfb71feedfc72b1ee4072beee417324ee42fa16ee437377ee4473bdee4573c9ee4673d6ee4773e3ee4873d2ee497407ee4a73f5ee4b7426ee4c742aee4d7429ee4e742eee4f7462ee507489ee51749f
ee527501ee53756fee547682ee55769cee56769eee57769bee5876a6ee59fa17ee5a7746ee5b52afee5c7821ee5d784eee5e7864ee5f787aee607930ee61fa18ee62fa19ee63fa1aee647994ee65fa1bee66799bee677ad1ee687ae7ee69fa1cee6a7aebee6b7b9eee6cfa1dee6d7d48ee6e7d5cee6f7db7ee707da0ee717dd6
ee727e52ee737f47ee747fa1ee75fa1eee768301ee778362ee78837fee7983c7ee7a83f6ee7b8448ee7c84b4ee7d8553ee7e8559ee80856bee81fa1fee8285b0ee83fa20ee84fa21ee858807ee8688f5ee878a12ee888a37ee898a79ee8a8aa7ee8b8abeee8c8adfee8dfa22ee8e8af6ee8f8b53ee908b7fee918cf0ee928cf4
ee938d12ee948d76ee95fa23ee968ecfee97fa24ee98fa25ee999067ee9a90deee9bfa26ee9c9115ee9d9127ee9e91daee9f91d7eea091deeea191edeea291eeeea391e4eea491e5eea59206eea69210eea7920aeea8923aeea99240eeaa923ceeab924eeeac9259eead9251eeae9239eeaf9267eeb092a7eeb19277eeb29278
eeb392e7eeb492d7eeb592d9eeb692d0eeb7fa27eeb892d5eeb992e0eeba92d3eebb9325eebc9321eebd92fbeebefa28eebf931eeec092ffeec1931deec29302eec39370eec49357eec593a4eec693c6eec793deeec893f8eec99431eeca9445eecb9448eecc9592eecdf9dceecefa29eecf969deed096afeed19733eed2973b
eed39743eed4974deed5974feed69751eed79755eed89857eed99865eedafa2aeedbfa2beedc9927eeddfa2ceede999eeedf9a4eeee09ad9eee19adceee29b75eee39b72eee49b8feee59bb1eee69bbbeee79c00eee89d70eee99d6beeeafa2deeeb9e19eeec9ed1eeef2170eef02171eef12172eef22173eef32174eef42175
eef52176eef62177eef72178eef82179eef9ffe2eefaffe4eefbff07eefcff02fa402170fa412171fa422172fa432173fa442174fa452175fa462176fa472177fa482178fa492179fa4a2160fa4b2161fa4c2162fa4d2163fa4e2164fa4f2165fa502166fa512167fa522168fa532169fa54ffe2fa55ffe4fa56ff07fa57ff02
fa583231fa592116fa5a2121fa5b2235fa5c7e8afa5d891cfa5e9348fa5f9288fa6084dcfa614fc9fa6270bbfa636631fa6468c8fa6592f9fa6666fbfa675f45fa684e28fa694ee1fa6a4efcfa6b4f00fa6c4f03fa6d4f39fa6e4f56fa6f4f92fa704f8afa714f9afa724f94fa734fcdfa745040fa755022fa764ffffa77501e
fa785046fa795070fa7a5042fa7b5094fa7c50f4fa7d50d8fa7e514afa805164fa81519dfa8251befa8351ecfa845215fa85529cfa8652a6fa8752c0fa8852dbfa895300fa8a5307fa8b5324fa8c5372fa8d5393fa8e53b2fa8f53ddfa90fa0efa91549cfa92548afa9354a9fa9454fffa955586fa965759fa975765fa9857ac
fa9957c8fa9a57c7fa9bfa0ffa9cfa10fa9d589efa9e58b2fa9f590bfaa05953faa1595bfaa2595dfaa35963faa459a4faa559bafaa65b56faa75bc0faa8752ffaa95bd8faaa5becfaab5c1efaac5ca6faad5cbafaae5cf5faaf5d27fab05d53fab1fa11fab25d42fab35d6dfab45db8fab55db9fab65dd0fab75f21fab85f34
fab95f67faba5fb7fabb5fdefabc605dfabd6085fabe608afabf60defac060d5fac16120fac260f2fac36111fac46137fac56130fac66198fac76213fac862a6fac963f5faca6460facb649dfacc64cefacd654eface6600facf6615fad0663bfad16609fad2662efad3661efad46624fad56665fad66657fad76659fad8fa12
fad96673fada6699fadb66a0fadc66b2fadd66bffade66fafadf670efae0f929fae16766fae267bbfae36852fae467c0fae56801fae66844fae768cffae8fa13fae96968faeafa14faeb6998faec69e2faed6a30faee6a6bfaef6a46faf06a73faf16a7efaf26ae2faf36ae4faf46bd6faf56c3ffaf66c5cfaf76c86faf86c6f
faf96cdafafa6d04fafb6d87fafc6d6ffb406d96fb416dacfb426dcffb436df8fb446df2fb456dfcfb466e39fb476e5cfb486e27fb496e3cfb4a6ebffb4b6f88fb4c6fb5fb4d6ff5fb4e7005fb4f7007fb507028fb517085fb5270abfb53710ffb547104fb55715cfb567146fb577147fb58fa15fb5971c1fb5a71fefb5b72b1
fb5c72befb5d7324fb5efa16fb5f7377fb6073bdfb6173c9fb6273d6fb6373e3fb6473d2fb657407fb6673f5fb677426fb68742afb697429fb6a742efb6b7462fb6c7489fb6d749ffb6e7501fb6f756ffb707682fb71769cfb72769efb73769bfb7476a6fb75fa17fb767746fb7752affb787821fb79784efb7a7864fb7b787a
fb7c7930fb7dfa18fb7efa19fb80fa1afb817994fb82fa1bfb83799bfb847ad1fb857ae7fb86fa1cfb877aebfb887b9efb89fa1dfb8a7d48fb8b7d5cfb8c7db7fb8d7da0fb8e7dd6fb8f7e52fb907f47fb917fa1fb92fa1efb938301fb948362fb95837ffb9683c7fb9783f6fb988448fb9984b4fb9a8553fb9b8559fb9c856b
fb9dfa1ffb9e85b0fb9ffa20fba0fa21fba18807fba288f5fba38a12fba48a37fba58a79fba68aa7fba78abefba88adffba9fa22fbaa8af6fbab8b53fbac8b7ffbad8cf0fbae8cf4fbaf8d12fbb08d76fbb1fa23fbb28ecffbb3fa24fbb4fa25fbb59067fbb690defbb7fa26fbb89115fbb99127fbba91dafbbb91d7fbbc91de
fbbd91edfbbe91eefbbf91e4fbc091e5fbc19206fbc29210fbc3920afbc4923afbc59240fbc6923cfbc7924efbc89259fbc99251fbca9239fbcb9267fbcc92a7fbcd9277fbce9278fbcf92e7fbd092d7fbd192d9fbd292d0fbd3fa27fbd492d5fbd592e0fbd692d3fbd79325fbd89321fbd992fbfbdafa28fbdb931efbdc92ff
fbdd931dfbde9302fbdf9370fbe09357fbe193a4fbe293c6fbe393defbe493f8fbe59431fbe69445fbe79448fbe89592fbe9f9dcfbeafa29fbeb969dfbec96affbed9733fbee973bfbef9743fbf0974dfbf1974ffbf29751fbf39755fbf49857fbf59865fbf6fa2afbf7fa2bfbf89927fbf9fa2cfbfa999efbfb9a4efbfc9ad9
fc409adcfc419b75fc429b72fc439b8ffc449bb1fc459bbbfc469c00fc479d70fc489d6bfc49fa2dfc4a9e19fc4b9ed1

";

					#endregion

					SJIS_UNICODE_PAIRS_RESOURCE = new string(SJIS_UNICODE_PAIRS_RESOURCE.Where(chr => ' ' < chr).ToArray());

					for (int index = 0; index < SJIS_UNICODE_PAIRS_RESOURCE.Length; index += 8)
					{
						this.Add(
							(UInt16)Convert.ToInt32(SJIS_UNICODE_PAIRS_RESOURCE.Substring(index, 4), 16),
							(UInt16)Convert.ToInt32(SJIS_UNICODE_PAIRS_RESOURCE.Substring(index + 4, 4), 16)
							);
					}
				}

				private void Add(UInt16 codeSJIS, UInt16 codeUnicode)
				{
					this.SJIS2Unicode[codeSJIS] = codeUnicode;
					this.Unicode2SJIS[codeUnicode] = codeSJIS;
				}

				private UInt16[] SJIS2Unicode = new UInt16[1 << 16];
				private UInt16[] Unicode2SJIS = new UInt16[1 << 16];

				public byte[] GetBytes(string str)
				{
					List<byte> buff = new List<byte>();

					foreach (char chr in str)
					{
						UInt16 code = this.Unicode2SJIS[(int)chr];

						if (code == 0)
							code = (UInt16)'?'; // 不明な文字

						if (code < 256) // ? 1-バイト文字
						{
							buff.Add((byte)code);
						}
						else // ? 2-バイト文字
						{
							buff.Add((byte)(code >> 8));
							buff.Add((byte)(code & 0xff));
						}
					}
					return buff.ToArray();
				}

				public string GetString(byte[] bytes)
				{
					StringBuilder buff = new StringBuilder();

					for (int index = 0; index < bytes.Length; index++)
					{
						int code = (int)bytes[index];
						UInt16 chr = this.SJIS2Unicode[code];

						if (chr == 0 && index + 1 < bytes.Length)
						{
							code <<= 8;
							code |= bytes[index + 1];
							chr = this.SJIS2Unicode[code];

							if (chr == 0)
								chr = (UInt16)'?'; // 不明な文字
							else
								index++;
						}
						buff.Append((char)chr);
					}
					return buff.ToString();
				}
			}
		}

		public class WorkingDir : IDisposable
		{
			public static RootInfo Root = null;

			public class RootInfo
			{
				private string Dir;
				private bool Created = false;

				public RootInfo(string dir)
				{
					this.Dir = dir;
				}

				public string GetDir()
				{
					if (!this.Created)
					{
						SCommon.DeletePath(this.Dir);
						SCommon.CreateDir(this.Dir);

						this.Created = true;
					}
					return this.Dir;
				}

				public void Delete()
				{
					if (this.Created)
					{
						SCommon.DeletePath(this.Dir);

						this.Created = false;
					}
				}
			}

			public static RootInfo CreateProcessRoot()
			{
				// 環境変数 TMP のパスは ProcMain.CheckLogonUserAndTmp() で検査している。

				// 環境変数 TMP のフォルダの配下は定期的に削除される。-> プロセス終了時の削除漏れはケアしない。

				return new RootInfo(Path.Combine(Environment.GetEnvironmentVariable("TMP"), ProcMain.APP_IDENT + "_" + Process.GetCurrentProcess().Id));
			}

			private static long CtorCounter = 0L;

			private string Dir = null;

			private string GetDir()
			{
				if (this.Dir == null)
				{
					if (Root == null)
						throw new Exception("Root is null");

					//this.Dir = Path.Combine(Root.GetDir(), Guid.NewGuid().ToString("B"));
					//this.Dir = Path.Combine(Root.GetDir(), SecurityTools.MakePassword_9A());
					this.Dir = Path.Combine(Root.GetDir(), "$" + CtorCounter++);

					SCommon.CreateDir(this.Dir);
				}
				return this.Dir;
			}

			public string GetPath(string localName)
			{
				return Path.Combine(this.GetDir(), localName);
			}

			private long PathCounter = 0L;

			public string MakePath()
			{
				//return this.GetPath(Guid.NewGuid().ToString("B"));
				//return this.GetPath(SecurityTools.MakePassword_9A());
				return this.GetPath("$" + this.PathCounter++);
			}

			public void Dispose()
			{
				if (this.Dir != null)
				{
					try
					{
						Directory.Delete(this.Dir, true);
					}
					catch (Exception e)
					{
						ProcMain.WriteLog(e);
					}

					this.Dir = null;
				}
			}
		}

		public static class ProcMain
		{
			public static string APP_IDENT = "{4f15ebac-66b4-4544-999e-b0c44490b8f1}";

			public static Action<object> WriteLog = message => Console.WriteLine(message);
		}
	}
}
