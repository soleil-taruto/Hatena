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
				channel.ResHeaderPairs.Add(new string[] { "X-Last-Modified-Time", fileInfo.LastWriteTime.Ticks.ToString() });

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
				if (EqualsIgnoreCase(pair[0], name))
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

			private Dictionary<string, string> Extension2ContentType = CreateDictionaryIgnoreCase<string>();

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

				//str = SCommon.ToJString(SCommon.ENCODING_SJIS.GetBytes(str), true, false, false, true);

				string[] words = str.Split('.');

				for (int index = 0; index < words.Length; index++)
				{
					string word = words[index];

					word = word.Trim();

					if (
						index == 0 &&
						GetReservedWordsForWindowsPath().Any(resWord => EqualsIgnoreCase(resWord, word)) ||
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
							dest.WriteByte((byte)Convert.ToInt32(Encoding.ASCII.GetString(new byte[] { src[index + 1], src[index + 2] }), 16));
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

					if (EqualsIgnoreCase(key, "Content-Length"))
					{
						if (value.Length < 1 || 10 < value.Length)
							throw new Exception("Bad Content-Length value");

						this.ContentLength = int.Parse(value);
					}
					else if (EqualsIgnoreCase(key, "Transfer-Encoding"))
					{
						this.Chunked = ContainsIgnoreCase(value, "chunked");
					}
					else if (EqualsIgnoreCase(key, "Content-Type"))
					{
						this.ContentType = value;
					}
					else if (EqualsIgnoreCase(key, "Expect"))
					{
						this.Expect100Continue = ContainsIgnoreCase(value, "100-continue");
					}
					else if (EqualsIgnoreCase(key, "Connection"))
					{
						this.KeepAlive = ContainsIgnoreCase(value, "keep-alive");
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
			public void Write(byte[] data)
			{
				throw new Exception("REJECT-BODY");
			}

			public int Count
			{
				get
				{
					return 0;
				}
			}

			public byte[] ToByteArray()
			{
				return new byte[0];
			}

			public void Dispose()
			{
				// noop
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
									channel.Connected = Supplier(this.E_Connected(channel));
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
									this.Channels[index] = this.Channels[this.Channels.Count - 1];
									this.Channels.RemoveAt(this.Channels.Count - 1);

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
						this.Add(Encoding.UTF8.GetBytes(new string(new char[] { SJISHanKanaToUnicode(chr) })));

					foreach (char chr in GetUnicodeListOfSJISMBC()) // 2バイト文字
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

		public static Dictionary<string, V> CreateDictionaryIgnoreCase<V>()
		{
			return new Dictionary<string, V>(new IECompStringIgnoreCase());
		}

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

		public static bool ContainsIgnoreCase(string str, string ptn)
		{
			return str.ToLower().Contains(ptn.ToLower());
		}

		public static char SJISHanKanaToUnicode(byte chr)
		{
			return (char)((int)chr + 65216);
		}

		public static IEnumerable<char> GetUnicodeListOfSJISMBC()
		{
			throw null; // TODO
		}

		public static class ProcMain
		{
			public static Action<object> WriteLog = message => Console.WriteLine(message);
		}
	}
}
