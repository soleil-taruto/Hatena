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
					for (UInt16 code = 0xa1; code <= 0xdf; code++) // 半角カナ
						this.Add(code, (UInt16)(code + 65216));

					#region Add SJIS-Unicode Pairs

					this.Add(33088, 12288); this.Add(33089, 12289); this.Add(33090, 12290); this.Add(33091, 65292); this.Add(33092, 65294); this.Add(33093, 12539); this.Add(33094, 65306); this.Add(33095, 65307);
					this.Add(33096, 65311); this.Add(33097, 65281); this.Add(33098, 12443); this.Add(33099, 12444); this.Add(33100, 180); this.Add(33101, 65344); this.Add(33102, 168); this.Add(33103, 65342);
					this.Add(33104, 65507); this.Add(33105, 65343); this.Add(33106, 12541); this.Add(33107, 12542); this.Add(33108, 12445); this.Add(33109, 12446); this.Add(33110, 12291); this.Add(33111, 20189);
					this.Add(33112, 12293); this.Add(33113, 12294); this.Add(33114, 12295); this.Add(33115, 12540); this.Add(33116, 8213); this.Add(33117, 8208); this.Add(33118, 65295); this.Add(33119, 65340);
					this.Add(33120, 65374); this.Add(33121, 8741); this.Add(33122, 65372); this.Add(33123, 8230); this.Add(33124, 8229); this.Add(33125, 8216); this.Add(33126, 8217); this.Add(33127, 8220);
					this.Add(33128, 8221); this.Add(33129, 65288); this.Add(33130, 65289); this.Add(33131, 12308); this.Add(33132, 12309); this.Add(33133, 65339); this.Add(33134, 65341); this.Add(33135, 65371);
					this.Add(33136, 65373); this.Add(33137, 12296); this.Add(33138, 12297); this.Add(33139, 12298); this.Add(33140, 12299); this.Add(33141, 12300); this.Add(33142, 12301); this.Add(33143, 12302);
					this.Add(33144, 12303); this.Add(33145, 12304); this.Add(33146, 12305); this.Add(33147, 65291); this.Add(33148, 65293); this.Add(33149, 177); this.Add(33150, 215); this.Add(33152, 247);
					this.Add(33153, 65309); this.Add(33154, 8800); this.Add(33155, 65308); this.Add(33156, 65310); this.Add(33157, 8806); this.Add(33158, 8807); this.Add(33159, 8734); this.Add(33160, 8756);
					this.Add(33161, 9794); this.Add(33162, 9792); this.Add(33163, 176); this.Add(33164, 8242); this.Add(33165, 8243); this.Add(33166, 8451); this.Add(33167, 65509); this.Add(33168, 65284);
					this.Add(33169, 65504); this.Add(33170, 65505); this.Add(33171, 65285); this.Add(33172, 65283); this.Add(33173, 65286); this.Add(33174, 65290); this.Add(33175, 65312); this.Add(33176, 167);
					this.Add(33177, 9734); this.Add(33178, 9733); this.Add(33179, 9675); this.Add(33180, 9679); this.Add(33181, 9678); this.Add(33182, 9671); this.Add(33183, 9670); this.Add(33184, 9633);
					this.Add(33185, 9632); this.Add(33186, 9651); this.Add(33187, 9650); this.Add(33188, 9661); this.Add(33189, 9660); this.Add(33190, 8251); this.Add(33191, 12306); this.Add(33192, 8594);
					this.Add(33193, 8592); this.Add(33194, 8593); this.Add(33195, 8595); this.Add(33196, 12307); this.Add(33208, 8712); this.Add(33209, 8715); this.Add(33210, 8838); this.Add(33211, 8839);
					this.Add(33212, 8834); this.Add(33213, 8835); this.Add(33214, 8746); this.Add(33215, 8745); this.Add(33224, 8743); this.Add(33225, 8744); this.Add(33226, 65506); this.Add(33227, 8658);
					this.Add(33228, 8660); this.Add(33229, 8704); this.Add(33230, 8707); this.Add(33242, 8736); this.Add(33243, 8869); this.Add(33244, 8978); this.Add(33245, 8706); this.Add(33246, 8711);
					this.Add(33247, 8801); this.Add(33248, 8786); this.Add(33249, 8810); this.Add(33250, 8811); this.Add(33251, 8730); this.Add(33252, 8765); this.Add(33253, 8733); this.Add(33254, 8757);
					this.Add(33255, 8747); this.Add(33256, 8748); this.Add(33264, 8491); this.Add(33265, 8240); this.Add(33266, 9839); this.Add(33267, 9837); this.Add(33268, 9834); this.Add(33269, 8224);
					this.Add(33270, 8225); this.Add(33271, 182); this.Add(33276, 9711); this.Add(33359, 65296); this.Add(33360, 65297); this.Add(33361, 65298); this.Add(33362, 65299); this.Add(33363, 65300);
					this.Add(33364, 65301); this.Add(33365, 65302); this.Add(33366, 65303); this.Add(33367, 65304); this.Add(33368, 65305); this.Add(33376, 65313); this.Add(33377, 65314); this.Add(33378, 65315);
					this.Add(33379, 65316); this.Add(33380, 65317); this.Add(33381, 65318); this.Add(33382, 65319); this.Add(33383, 65320); this.Add(33384, 65321); this.Add(33385, 65322); this.Add(33386, 65323);
					this.Add(33387, 65324); this.Add(33388, 65325); this.Add(33389, 65326); this.Add(33390, 65327); this.Add(33391, 65328); this.Add(33392, 65329); this.Add(33393, 65330); this.Add(33394, 65331);
					this.Add(33395, 65332); this.Add(33396, 65333); this.Add(33397, 65334); this.Add(33398, 65335); this.Add(33399, 65336); this.Add(33400, 65337); this.Add(33401, 65338); this.Add(33409, 65345);
					this.Add(33410, 65346); this.Add(33411, 65347); this.Add(33412, 65348); this.Add(33413, 65349); this.Add(33414, 65350); this.Add(33415, 65351); this.Add(33416, 65352); this.Add(33417, 65353);
					this.Add(33418, 65354); this.Add(33419, 65355); this.Add(33420, 65356); this.Add(33421, 65357); this.Add(33422, 65358); this.Add(33423, 65359); this.Add(33424, 65360); this.Add(33425, 65361);
					this.Add(33426, 65362); this.Add(33427, 65363); this.Add(33428, 65364); this.Add(33429, 65365); this.Add(33430, 65366); this.Add(33431, 65367); this.Add(33432, 65368); this.Add(33433, 65369);
					this.Add(33434, 65370); this.Add(33439, 12353); this.Add(33440, 12354); this.Add(33441, 12355); this.Add(33442, 12356); this.Add(33443, 12357); this.Add(33444, 12358); this.Add(33445, 12359);
					this.Add(33446, 12360); this.Add(33447, 12361); this.Add(33448, 12362); this.Add(33449, 12363); this.Add(33450, 12364); this.Add(33451, 12365); this.Add(33452, 12366); this.Add(33453, 12367);
					this.Add(33454, 12368); this.Add(33455, 12369); this.Add(33456, 12370); this.Add(33457, 12371); this.Add(33458, 12372); this.Add(33459, 12373); this.Add(33460, 12374); this.Add(33461, 12375);
					this.Add(33462, 12376); this.Add(33463, 12377); this.Add(33464, 12378); this.Add(33465, 12379); this.Add(33466, 12380); this.Add(33467, 12381); this.Add(33468, 12382); this.Add(33469, 12383);
					this.Add(33470, 12384); this.Add(33471, 12385); this.Add(33472, 12386); this.Add(33473, 12387); this.Add(33474, 12388); this.Add(33475, 12389); this.Add(33476, 12390); this.Add(33477, 12391);
					this.Add(33478, 12392); this.Add(33479, 12393); this.Add(33480, 12394); this.Add(33481, 12395); this.Add(33482, 12396); this.Add(33483, 12397); this.Add(33484, 12398); this.Add(33485, 12399);
					this.Add(33486, 12400); this.Add(33487, 12401); this.Add(33488, 12402); this.Add(33489, 12403); this.Add(33490, 12404); this.Add(33491, 12405); this.Add(33492, 12406); this.Add(33493, 12407);
					this.Add(33494, 12408); this.Add(33495, 12409); this.Add(33496, 12410); this.Add(33497, 12411); this.Add(33498, 12412); this.Add(33499, 12413); this.Add(33500, 12414); this.Add(33501, 12415);
					this.Add(33502, 12416); this.Add(33503, 12417); this.Add(33504, 12418); this.Add(33505, 12419); this.Add(33506, 12420); this.Add(33507, 12421); this.Add(33508, 12422); this.Add(33509, 12423);
					this.Add(33510, 12424); this.Add(33511, 12425); this.Add(33512, 12426); this.Add(33513, 12427); this.Add(33514, 12428); this.Add(33515, 12429); this.Add(33516, 12430); this.Add(33517, 12431);
					this.Add(33518, 12432); this.Add(33519, 12433); this.Add(33520, 12434); this.Add(33521, 12435); this.Add(33600, 12449); this.Add(33601, 12450); this.Add(33602, 12451); this.Add(33603, 12452);
					this.Add(33604, 12453); this.Add(33605, 12454); this.Add(33606, 12455); this.Add(33607, 12456); this.Add(33608, 12457); this.Add(33609, 12458); this.Add(33610, 12459); this.Add(33611, 12460);
					this.Add(33612, 12461); this.Add(33613, 12462); this.Add(33614, 12463); this.Add(33615, 12464); this.Add(33616, 12465); this.Add(33617, 12466); this.Add(33618, 12467); this.Add(33619, 12468);
					this.Add(33620, 12469); this.Add(33621, 12470); this.Add(33622, 12471); this.Add(33623, 12472); this.Add(33624, 12473); this.Add(33625, 12474); this.Add(33626, 12475); this.Add(33627, 12476);
					this.Add(33628, 12477); this.Add(33629, 12478); this.Add(33630, 12479); this.Add(33631, 12480); this.Add(33632, 12481); this.Add(33633, 12482); this.Add(33634, 12483); this.Add(33635, 12484);
					this.Add(33636, 12485); this.Add(33637, 12486); this.Add(33638, 12487); this.Add(33639, 12488); this.Add(33640, 12489); this.Add(33641, 12490); this.Add(33642, 12491); this.Add(33643, 12492);
					this.Add(33644, 12493); this.Add(33645, 12494); this.Add(33646, 12495); this.Add(33647, 12496); this.Add(33648, 12497); this.Add(33649, 12498); this.Add(33650, 12499); this.Add(33651, 12500);
					this.Add(33652, 12501); this.Add(33653, 12502); this.Add(33654, 12503); this.Add(33655, 12504); this.Add(33656, 12505); this.Add(33657, 12506); this.Add(33658, 12507); this.Add(33659, 12508);
					this.Add(33660, 12509); this.Add(33661, 12510); this.Add(33662, 12511); this.Add(33664, 12512); this.Add(33665, 12513); this.Add(33666, 12514); this.Add(33667, 12515); this.Add(33668, 12516);
					this.Add(33669, 12517); this.Add(33670, 12518); this.Add(33671, 12519); this.Add(33672, 12520); this.Add(33673, 12521); this.Add(33674, 12522); this.Add(33675, 12523); this.Add(33676, 12524);
					this.Add(33677, 12525); this.Add(33678, 12526); this.Add(33679, 12527); this.Add(33680, 12528); this.Add(33681, 12529); this.Add(33682, 12530); this.Add(33683, 12531); this.Add(33684, 12532);
					this.Add(33685, 12533); this.Add(33686, 12534); this.Add(33695, 913); this.Add(33696, 914); this.Add(33697, 915); this.Add(33698, 916); this.Add(33699, 917); this.Add(33700, 918);
					this.Add(33701, 919); this.Add(33702, 920); this.Add(33703, 921); this.Add(33704, 922); this.Add(33705, 923); this.Add(33706, 924); this.Add(33707, 925); this.Add(33708, 926);
					this.Add(33709, 927); this.Add(33710, 928); this.Add(33711, 929); this.Add(33712, 931); this.Add(33713, 932); this.Add(33714, 933); this.Add(33715, 934); this.Add(33716, 935);
					this.Add(33717, 936); this.Add(33718, 937); this.Add(33727, 945); this.Add(33728, 946); this.Add(33729, 947); this.Add(33730, 948); this.Add(33731, 949); this.Add(33732, 950);
					this.Add(33733, 951); this.Add(33734, 952); this.Add(33735, 953); this.Add(33736, 954); this.Add(33737, 955); this.Add(33738, 956); this.Add(33739, 957); this.Add(33740, 958);
					this.Add(33741, 959); this.Add(33742, 960); this.Add(33743, 961); this.Add(33744, 963); this.Add(33745, 964); this.Add(33746, 965); this.Add(33747, 966); this.Add(33748, 967);
					this.Add(33749, 968); this.Add(33750, 969); this.Add(33856, 1040); this.Add(33857, 1041); this.Add(33858, 1042); this.Add(33859, 1043); this.Add(33860, 1044); this.Add(33861, 1045);
					this.Add(33862, 1025); this.Add(33863, 1046); this.Add(33864, 1047); this.Add(33865, 1048); this.Add(33866, 1049); this.Add(33867, 1050); this.Add(33868, 1051); this.Add(33869, 1052);
					this.Add(33870, 1053); this.Add(33871, 1054); this.Add(33872, 1055); this.Add(33873, 1056); this.Add(33874, 1057); this.Add(33875, 1058); this.Add(33876, 1059); this.Add(33877, 1060);
					this.Add(33878, 1061); this.Add(33879, 1062); this.Add(33880, 1063); this.Add(33881, 1064); this.Add(33882, 1065); this.Add(33883, 1066); this.Add(33884, 1067); this.Add(33885, 1068);
					this.Add(33886, 1069); this.Add(33887, 1070); this.Add(33888, 1071); this.Add(33904, 1072); this.Add(33905, 1073); this.Add(33906, 1074); this.Add(33907, 1075); this.Add(33908, 1076);
					this.Add(33909, 1077); this.Add(33910, 1105); this.Add(33911, 1078); this.Add(33912, 1079); this.Add(33913, 1080); this.Add(33914, 1081); this.Add(33915, 1082); this.Add(33916, 1083);
					this.Add(33917, 1084); this.Add(33918, 1085); this.Add(33920, 1086); this.Add(33921, 1087); this.Add(33922, 1088); this.Add(33923, 1089); this.Add(33924, 1090); this.Add(33925, 1091);
					this.Add(33926, 1092); this.Add(33927, 1093); this.Add(33928, 1094); this.Add(33929, 1095); this.Add(33930, 1096); this.Add(33931, 1097); this.Add(33932, 1098); this.Add(33933, 1099);
					this.Add(33934, 1100); this.Add(33935, 1101); this.Add(33936, 1102); this.Add(33937, 1103); this.Add(33951, 9472); this.Add(33952, 9474); this.Add(33953, 9484); this.Add(33954, 9488);
					this.Add(33955, 9496); this.Add(33956, 9492); this.Add(33957, 9500); this.Add(33958, 9516); this.Add(33959, 9508); this.Add(33960, 9524); this.Add(33961, 9532); this.Add(33962, 9473);
					this.Add(33963, 9475); this.Add(33964, 9487); this.Add(33965, 9491); this.Add(33966, 9499); this.Add(33967, 9495); this.Add(33968, 9507); this.Add(33969, 9523); this.Add(33970, 9515);
					this.Add(33971, 9531); this.Add(33972, 9547); this.Add(33973, 9504); this.Add(33974, 9519); this.Add(33975, 9512); this.Add(33976, 9527); this.Add(33977, 9535); this.Add(33978, 9501);
					this.Add(33979, 9520); this.Add(33980, 9509); this.Add(33981, 9528); this.Add(33982, 9538); this.Add(34624, 9312); this.Add(34625, 9313); this.Add(34626, 9314); this.Add(34627, 9315);
					this.Add(34628, 9316); this.Add(34629, 9317); this.Add(34630, 9318); this.Add(34631, 9319); this.Add(34632, 9320); this.Add(34633, 9321); this.Add(34634, 9322); this.Add(34635, 9323);
					this.Add(34636, 9324); this.Add(34637, 9325); this.Add(34638, 9326); this.Add(34639, 9327); this.Add(34640, 9328); this.Add(34641, 9329); this.Add(34642, 9330); this.Add(34643, 9331);
					this.Add(34644, 8544); this.Add(34645, 8545); this.Add(34646, 8546); this.Add(34647, 8547); this.Add(34648, 8548); this.Add(34649, 8549); this.Add(34650, 8550); this.Add(34651, 8551);
					this.Add(34652, 8552); this.Add(34653, 8553); this.Add(34655, 13129); this.Add(34656, 13076); this.Add(34657, 13090); this.Add(34658, 13133); this.Add(34659, 13080); this.Add(34660, 13095);
					this.Add(34661, 13059); this.Add(34662, 13110); this.Add(34663, 13137); this.Add(34664, 13143); this.Add(34665, 13069); this.Add(34666, 13094); this.Add(34667, 13091); this.Add(34668, 13099);
					this.Add(34669, 13130); this.Add(34670, 13115); this.Add(34671, 13212); this.Add(34672, 13213); this.Add(34673, 13214); this.Add(34674, 13198); this.Add(34675, 13199); this.Add(34676, 13252);
					this.Add(34677, 13217); this.Add(34686, 13179); this.Add(34688, 12317); this.Add(34689, 12319); this.Add(34690, 8470); this.Add(34691, 13261); this.Add(34692, 8481); this.Add(34693, 12964);
					this.Add(34694, 12965); this.Add(34695, 12966); this.Add(34696, 12967); this.Add(34697, 12968); this.Add(34698, 12849); this.Add(34699, 12850); this.Add(34700, 12857); this.Add(34701, 13182);
					this.Add(34702, 13181); this.Add(34703, 13180); this.Add(34704, 8786); this.Add(34705, 8801); this.Add(34706, 8747); this.Add(34707, 8750); this.Add(34708, 8721); this.Add(34709, 8730);
					this.Add(34710, 8869); this.Add(34711, 8736); this.Add(34712, 8735); this.Add(34713, 8895); this.Add(34714, 8757); this.Add(34715, 8745); this.Add(34716, 8746); this.Add(34975, 20124);
					this.Add(34976, 21782); this.Add(34977, 23043); this.Add(34978, 38463); this.Add(34979, 21696); this.Add(34980, 24859); this.Add(34981, 25384); this.Add(34982, 23030); this.Add(34983, 36898);
					this.Add(34984, 33909); this.Add(34985, 33564); this.Add(34986, 31312); this.Add(34987, 24746); this.Add(34988, 25569); this.Add(34989, 28197); this.Add(34990, 26093); this.Add(34991, 33894);
					this.Add(34992, 33446); this.Add(34993, 39925); this.Add(34994, 26771); this.Add(34995, 22311); this.Add(34996, 26017); this.Add(34997, 25201); this.Add(34998, 23451); this.Add(34999, 22992);
					this.Add(35000, 34427); this.Add(35001, 39156); this.Add(35002, 32098); this.Add(35003, 32190); this.Add(35004, 39822); this.Add(35005, 25110); this.Add(35006, 31903); this.Add(35007, 34999);
					this.Add(35008, 23433); this.Add(35009, 24245); this.Add(35010, 25353); this.Add(35011, 26263); this.Add(35012, 26696); this.Add(35013, 38343); this.Add(35014, 38797); this.Add(35015, 26447);
					this.Add(35016, 20197); this.Add(35017, 20234); this.Add(35018, 20301); this.Add(35019, 20381); this.Add(35020, 20553); this.Add(35021, 22258); this.Add(35022, 22839); this.Add(35023, 22996);
					this.Add(35024, 23041); this.Add(35025, 23561); this.Add(35026, 24799); this.Add(35027, 24847); this.Add(35028, 24944); this.Add(35029, 26131); this.Add(35030, 26885); this.Add(35031, 28858);
					this.Add(35032, 30031); this.Add(35033, 30064); this.Add(35034, 31227); this.Add(35035, 32173); this.Add(35036, 32239); this.Add(35037, 32963); this.Add(35038, 33806); this.Add(35039, 34915);
					this.Add(35040, 35586); this.Add(35041, 36949); this.Add(35042, 36986); this.Add(35043, 21307); this.Add(35044, 20117); this.Add(35045, 20133); this.Add(35046, 22495); this.Add(35047, 32946);
					this.Add(35048, 37057); this.Add(35049, 30959); this.Add(35050, 19968); this.Add(35051, 22769); this.Add(35052, 28322); this.Add(35053, 36920); this.Add(35054, 31282); this.Add(35055, 33576);
					this.Add(35056, 33419); this.Add(35057, 39983); this.Add(35058, 20801); this.Add(35059, 21360); this.Add(35060, 21693); this.Add(35061, 21729); this.Add(35062, 22240); this.Add(35063, 23035);
					this.Add(35064, 24341); this.Add(35065, 39154); this.Add(35066, 28139); this.Add(35067, 32996); this.Add(35068, 34093); this.Add(35136, 38498); this.Add(35137, 38512); this.Add(35138, 38560);
					this.Add(35139, 38907); this.Add(35140, 21515); this.Add(35141, 21491); this.Add(35142, 23431); this.Add(35143, 28879); this.Add(35144, 32701); this.Add(35145, 36802); this.Add(35146, 38632);
					this.Add(35147, 21359); this.Add(35148, 40284); this.Add(35149, 31418); this.Add(35150, 19985); this.Add(35151, 30867); this.Add(35152, 33276); this.Add(35153, 28198); this.Add(35154, 22040);
					this.Add(35155, 21764); this.Add(35156, 27421); this.Add(35157, 34074); this.Add(35158, 39995); this.Add(35159, 23013); this.Add(35160, 21417); this.Add(35161, 28006); this.Add(35162, 29916);
					this.Add(35163, 38287); this.Add(35164, 22082); this.Add(35165, 20113); this.Add(35166, 36939); this.Add(35167, 38642); this.Add(35168, 33615); this.Add(35169, 39180); this.Add(35170, 21473);
					this.Add(35171, 21942); this.Add(35172, 23344); this.Add(35173, 24433); this.Add(35174, 26144); this.Add(35175, 26355); this.Add(35176, 26628); this.Add(35177, 27704); this.Add(35178, 27891);
					this.Add(35179, 27945); this.Add(35180, 29787); this.Add(35181, 30408); this.Add(35182, 31310); this.Add(35183, 38964); this.Add(35184, 33521); this.Add(35185, 34907); this.Add(35186, 35424);
					this.Add(35187, 37613); this.Add(35188, 28082); this.Add(35189, 30123); this.Add(35190, 30410); this.Add(35191, 39365); this.Add(35192, 24742); this.Add(35193, 35585); this.Add(35194, 36234);
					this.Add(35195, 38322); this.Add(35196, 27022); this.Add(35197, 21421); this.Add(35198, 20870); this.Add(35200, 22290); this.Add(35201, 22576); this.Add(35202, 22852); this.Add(35203, 23476);
					this.Add(35204, 24310); this.Add(35205, 24616); this.Add(35206, 25513); this.Add(35207, 25588); this.Add(35208, 27839); this.Add(35209, 28436); this.Add(35210, 28814); this.Add(35211, 28948);
					this.Add(35212, 29017); this.Add(35213, 29141); this.Add(35214, 29503); this.Add(35215, 32257); this.Add(35216, 33398); this.Add(35217, 33489); this.Add(35218, 34199); this.Add(35219, 36960);
					this.Add(35220, 37467); this.Add(35221, 40219); this.Add(35222, 22633); this.Add(35223, 26044); this.Add(35224, 27738); this.Add(35225, 29989); this.Add(35226, 20985); this.Add(35227, 22830);
					this.Add(35228, 22885); this.Add(35229, 24448); this.Add(35230, 24540); this.Add(35231, 25276); this.Add(35232, 26106); this.Add(35233, 27178); this.Add(35234, 27431); this.Add(35235, 27572);
					this.Add(35236, 29579); this.Add(35237, 32705); this.Add(35238, 35158); this.Add(35239, 40236); this.Add(35240, 40206); this.Add(35241, 40644); this.Add(35242, 23713); this.Add(35243, 27798);
					this.Add(35244, 33659); this.Add(35245, 20740); this.Add(35246, 23627); this.Add(35247, 25014); this.Add(35248, 33222); this.Add(35249, 26742); this.Add(35250, 29281); this.Add(35251, 20057);
					this.Add(35252, 20474); this.Add(35253, 21368); this.Add(35254, 24681); this.Add(35255, 28201); this.Add(35256, 31311); this.Add(35257, 38899); this.Add(35258, 19979); this.Add(35259, 21270);
					this.Add(35260, 20206); this.Add(35261, 20309); this.Add(35262, 20285); this.Add(35263, 20385); this.Add(35264, 20339); this.Add(35265, 21152); this.Add(35266, 21487); this.Add(35267, 22025);
					this.Add(35268, 22799); this.Add(35269, 23233); this.Add(35270, 23478); this.Add(35271, 23521); this.Add(35272, 31185); this.Add(35273, 26247); this.Add(35274, 26524); this.Add(35275, 26550);
					this.Add(35276, 27468); this.Add(35277, 27827); this.Add(35278, 28779); this.Add(35279, 29634); this.Add(35280, 31117); this.Add(35281, 31166); this.Add(35282, 31292); this.Add(35283, 31623);
					this.Add(35284, 33457); this.Add(35285, 33499); this.Add(35286, 33540); this.Add(35287, 33655); this.Add(35288, 33775); this.Add(35289, 33747); this.Add(35290, 34662); this.Add(35291, 35506);
					this.Add(35292, 22057); this.Add(35293, 36008); this.Add(35294, 36838); this.Add(35295, 36942); this.Add(35296, 38686); this.Add(35297, 34442); this.Add(35298, 20420); this.Add(35299, 23784);
					this.Add(35300, 25105); this.Add(35301, 29273); this.Add(35302, 30011); this.Add(35303, 33253); this.Add(35304, 33469); this.Add(35305, 34558); this.Add(35306, 36032); this.Add(35307, 38597);
					this.Add(35308, 39187); this.Add(35309, 39381); this.Add(35310, 20171); this.Add(35311, 20250); this.Add(35312, 35299); this.Add(35313, 22238); this.Add(35314, 22602); this.Add(35315, 22730);
					this.Add(35316, 24315); this.Add(35317, 24555); this.Add(35318, 24618); this.Add(35319, 24724); this.Add(35320, 24674); this.Add(35321, 25040); this.Add(35322, 25106); this.Add(35323, 25296);
					this.Add(35324, 25913); this.Add(35392, 39745); this.Add(35393, 26214); this.Add(35394, 26800); this.Add(35395, 28023); this.Add(35396, 28784); this.Add(35397, 30028); this.Add(35398, 30342);
					this.Add(35399, 32117); this.Add(35400, 33445); this.Add(35401, 34809); this.Add(35402, 38283); this.Add(35403, 38542); this.Add(35404, 35997); this.Add(35405, 20977); this.Add(35406, 21182);
					this.Add(35407, 22806); this.Add(35408, 21683); this.Add(35409, 23475); this.Add(35410, 23830); this.Add(35411, 24936); this.Add(35412, 27010); this.Add(35413, 28079); this.Add(35414, 30861);
					this.Add(35415, 33995); this.Add(35416, 34903); this.Add(35417, 35442); this.Add(35418, 37799); this.Add(35419, 39608); this.Add(35420, 28012); this.Add(35421, 39336); this.Add(35422, 34521);
					this.Add(35423, 22435); this.Add(35424, 26623); this.Add(35425, 34510); this.Add(35426, 37390); this.Add(35427, 21123); this.Add(35428, 22151); this.Add(35429, 21508); this.Add(35430, 24275);
					this.Add(35431, 25313); this.Add(35432, 25785); this.Add(35433, 26684); this.Add(35434, 26680); this.Add(35435, 27579); this.Add(35436, 29554); this.Add(35437, 30906); this.Add(35438, 31339);
					this.Add(35439, 35226); this.Add(35440, 35282); this.Add(35441, 36203); this.Add(35442, 36611); this.Add(35443, 37101); this.Add(35444, 38307); this.Add(35445, 38548); this.Add(35446, 38761);
					this.Add(35447, 23398); this.Add(35448, 23731); this.Add(35449, 27005); this.Add(35450, 38989); this.Add(35451, 38990); this.Add(35452, 25499); this.Add(35453, 31520); this.Add(35454, 27179);
					this.Add(35456, 27263); this.Add(35457, 26806); this.Add(35458, 39949); this.Add(35459, 28511); this.Add(35460, 21106); this.Add(35461, 21917); this.Add(35462, 24688); this.Add(35463, 25324);
					this.Add(35464, 27963); this.Add(35465, 28167); this.Add(35466, 28369); this.Add(35467, 33883); this.Add(35468, 35088); this.Add(35469, 36676); this.Add(35470, 19988); this.Add(35471, 39993);
					this.Add(35472, 21494); this.Add(35473, 26907); this.Add(35474, 27194); this.Add(35475, 38788); this.Add(35476, 26666); this.Add(35477, 20828); this.Add(35478, 31427); this.Add(35479, 33970);
					this.Add(35480, 37340); this.Add(35481, 37772); this.Add(35482, 22107); this.Add(35483, 40232); this.Add(35484, 26658); this.Add(35485, 33541); this.Add(35486, 33841); this.Add(35487, 31909);
					this.Add(35488, 21000); this.Add(35489, 33477); this.Add(35490, 29926); this.Add(35491, 20094); this.Add(35492, 20355); this.Add(35493, 20896); this.Add(35494, 23506); this.Add(35495, 21002);
					this.Add(35496, 21208); this.Add(35497, 21223); this.Add(35498, 24059); this.Add(35499, 21914); this.Add(35500, 22570); this.Add(35501, 23014); this.Add(35502, 23436); this.Add(35503, 23448);
					this.Add(35504, 23515); this.Add(35505, 24178); this.Add(35506, 24185); this.Add(35507, 24739); this.Add(35508, 24863); this.Add(35509, 24931); this.Add(35510, 25022); this.Add(35511, 25563);
					this.Add(35512, 25954); this.Add(35513, 26577); this.Add(35514, 26707); this.Add(35515, 26874); this.Add(35516, 27454); this.Add(35517, 27475); this.Add(35518, 27735); this.Add(35519, 28450);
					this.Add(35520, 28567); this.Add(35521, 28485); this.Add(35522, 29872); this.Add(35523, 29976); this.Add(35524, 30435); this.Add(35525, 30475); this.Add(35526, 31487); this.Add(35527, 31649);
					this.Add(35528, 31777); this.Add(35529, 32233); this.Add(35530, 32566); this.Add(35531, 32752); this.Add(35532, 32925); this.Add(35533, 33382); this.Add(35534, 33694); this.Add(35535, 35251);
					this.Add(35536, 35532); this.Add(35537, 36011); this.Add(35538, 36996); this.Add(35539, 37969); this.Add(35540, 38291); this.Add(35541, 38289); this.Add(35542, 38306); this.Add(35543, 38501);
					this.Add(35544, 38867); this.Add(35545, 39208); this.Add(35546, 33304); this.Add(35547, 20024); this.Add(35548, 21547); this.Add(35549, 23736); this.Add(35550, 24012); this.Add(35551, 29609);
					this.Add(35552, 30284); this.Add(35553, 30524); this.Add(35554, 23721); this.Add(35555, 32747); this.Add(35556, 36107); this.Add(35557, 38593); this.Add(35558, 38929); this.Add(35559, 38996);
					this.Add(35560, 39000); this.Add(35561, 20225); this.Add(35562, 20238); this.Add(35563, 21361); this.Add(35564, 21916); this.Add(35565, 22120); this.Add(35566, 22522); this.Add(35567, 22855);
					this.Add(35568, 23305); this.Add(35569, 23492); this.Add(35570, 23696); this.Add(35571, 24076); this.Add(35572, 24190); this.Add(35573, 24524); this.Add(35574, 25582); this.Add(35575, 26426);
					this.Add(35576, 26071); this.Add(35577, 26082); this.Add(35578, 26399); this.Add(35579, 26827); this.Add(35580, 26820); this.Add(35648, 27231); this.Add(35649, 24112); this.Add(35650, 27589);
					this.Add(35651, 27671); this.Add(35652, 27773); this.Add(35653, 30079); this.Add(35654, 31048); this.Add(35655, 23395); this.Add(35656, 31232); this.Add(35657, 32000); this.Add(35658, 24509);
					this.Add(35659, 35215); this.Add(35660, 35352); this.Add(35661, 36020); this.Add(35662, 36215); this.Add(35663, 36556); this.Add(35664, 36637); this.Add(35665, 39138); this.Add(35666, 39438);
					this.Add(35667, 39740); this.Add(35668, 20096); this.Add(35669, 20605); this.Add(35670, 20736); this.Add(35671, 22931); this.Add(35672, 23452); this.Add(35673, 25135); this.Add(35674, 25216);
					this.Add(35675, 25836); this.Add(35676, 27450); this.Add(35677, 29344); this.Add(35678, 30097); this.Add(35679, 31047); this.Add(35680, 32681); this.Add(35681, 34811); this.Add(35682, 35516);
					this.Add(35683, 35696); this.Add(35684, 25516); this.Add(35685, 33738); this.Add(35686, 38816); this.Add(35687, 21513); this.Add(35688, 21507); this.Add(35689, 21931); this.Add(35690, 26708);
					this.Add(35691, 27224); this.Add(35692, 35440); this.Add(35693, 30759); this.Add(35694, 26485); this.Add(35695, 40653); this.Add(35696, 21364); this.Add(35697, 23458); this.Add(35698, 33050);
					this.Add(35699, 34384); this.Add(35700, 36870); this.Add(35701, 19992); this.Add(35702, 20037); this.Add(35703, 20167); this.Add(35704, 20241); this.Add(35705, 21450); this.Add(35706, 21560);
					this.Add(35707, 23470); this.Add(35708, 24339); this.Add(35709, 24613); this.Add(35710, 25937); this.Add(35712, 26429); this.Add(35713, 27714); this.Add(35714, 27762); this.Add(35715, 27875);
					this.Add(35716, 28792); this.Add(35717, 29699); this.Add(35718, 31350); this.Add(35719, 31406); this.Add(35720, 31496); this.Add(35721, 32026); this.Add(35722, 31998); this.Add(35723, 32102);
					this.Add(35724, 26087); this.Add(35725, 29275); this.Add(35726, 21435); this.Add(35727, 23621); this.Add(35728, 24040); this.Add(35729, 25298); this.Add(35730, 25312); this.Add(35731, 25369);
					this.Add(35732, 28192); this.Add(35733, 34394); this.Add(35734, 35377); this.Add(35735, 36317); this.Add(35736, 37624); this.Add(35737, 28417); this.Add(35738, 31142); this.Add(35739, 39770);
					this.Add(35740, 20136); this.Add(35741, 20139); this.Add(35742, 20140); this.Add(35743, 20379); this.Add(35744, 20384); this.Add(35745, 20689); this.Add(35746, 20807); this.Add(35747, 31478);
					this.Add(35748, 20849); this.Add(35749, 20982); this.Add(35750, 21332); this.Add(35751, 21281); this.Add(35752, 21375); this.Add(35753, 21483); this.Add(35754, 21932); this.Add(35755, 22659);
					this.Add(35756, 23777); this.Add(35757, 24375); this.Add(35758, 24394); this.Add(35759, 24623); this.Add(35760, 24656); this.Add(35761, 24685); this.Add(35762, 25375); this.Add(35763, 25945);
					this.Add(35764, 27211); this.Add(35765, 27841); this.Add(35766, 29378); this.Add(35767, 29421); this.Add(35768, 30703); this.Add(35769, 33016); this.Add(35770, 33029); this.Add(35771, 33288);
					this.Add(35772, 34126); this.Add(35773, 37111); this.Add(35774, 37857); this.Add(35775, 38911); this.Add(35776, 39255); this.Add(35777, 39514); this.Add(35778, 20208); this.Add(35779, 20957);
					this.Add(35780, 23597); this.Add(35781, 26241); this.Add(35782, 26989); this.Add(35783, 23616); this.Add(35784, 26354); this.Add(35785, 26997); this.Add(35786, 29577); this.Add(35787, 26704);
					this.Add(35788, 31873); this.Add(35789, 20677); this.Add(35790, 21220); this.Add(35791, 22343); this.Add(35792, 24062); this.Add(35793, 37670); this.Add(35794, 26020); this.Add(35795, 27427);
					this.Add(35796, 27453); this.Add(35797, 29748); this.Add(35798, 31105); this.Add(35799, 31165); this.Add(35800, 31563); this.Add(35801, 32202); this.Add(35802, 33465); this.Add(35803, 33740);
					this.Add(35804, 34943); this.Add(35805, 35167); this.Add(35806, 35641); this.Add(35807, 36817); this.Add(35808, 37329); this.Add(35809, 21535); this.Add(35810, 37504); this.Add(35811, 20061);
					this.Add(35812, 20534); this.Add(35813, 21477); this.Add(35814, 21306); this.Add(35815, 29399); this.Add(35816, 29590); this.Add(35817, 30697); this.Add(35818, 33510); this.Add(35819, 36527);
					this.Add(35820, 39366); this.Add(35821, 39368); this.Add(35822, 39378); this.Add(35823, 20855); this.Add(35824, 24858); this.Add(35825, 34398); this.Add(35826, 21936); this.Add(35827, 31354);
					this.Add(35828, 20598); this.Add(35829, 23507); this.Add(35830, 36935); this.Add(35831, 38533); this.Add(35832, 20018); this.Add(35833, 27355); this.Add(35834, 37351); this.Add(35835, 23633);
					this.Add(35836, 23624); this.Add(35904, 25496); this.Add(35905, 31391); this.Add(35906, 27795); this.Add(35907, 38772); this.Add(35908, 36705); this.Add(35909, 31402); this.Add(35910, 29066);
					this.Add(35911, 38536); this.Add(35912, 31874); this.Add(35913, 26647); this.Add(35914, 32368); this.Add(35915, 26705); this.Add(35916, 37740); this.Add(35917, 21234); this.Add(35918, 21531);
					this.Add(35919, 34219); this.Add(35920, 35347); this.Add(35921, 32676); this.Add(35922, 36557); this.Add(35923, 37089); this.Add(35924, 21350); this.Add(35925, 34952); this.Add(35926, 31041);
					this.Add(35927, 20418); this.Add(35928, 20670); this.Add(35929, 21009); this.Add(35930, 20804); this.Add(35931, 21843); this.Add(35932, 22317); this.Add(35933, 29674); this.Add(35934, 22411);
					this.Add(35935, 22865); this.Add(35936, 24418); this.Add(35937, 24452); this.Add(35938, 24693); this.Add(35939, 24950); this.Add(35940, 24935); this.Add(35941, 25001); this.Add(35942, 25522);
					this.Add(35943, 25658); this.Add(35944, 25964); this.Add(35945, 26223); this.Add(35946, 26690); this.Add(35947, 28179); this.Add(35948, 30054); this.Add(35949, 31293); this.Add(35950, 31995);
					this.Add(35951, 32076); this.Add(35952, 32153); this.Add(35953, 32331); this.Add(35954, 32619); this.Add(35955, 33550); this.Add(35956, 33610); this.Add(35957, 34509); this.Add(35958, 35336);
					this.Add(35959, 35427); this.Add(35960, 35686); this.Add(35961, 36605); this.Add(35962, 38938); this.Add(35963, 40335); this.Add(35964, 33464); this.Add(35965, 36814); this.Add(35966, 39912);
					this.Add(35968, 21127); this.Add(35969, 25119); this.Add(35970, 25731); this.Add(35971, 28608); this.Add(35972, 38553); this.Add(35973, 26689); this.Add(35974, 20625); this.Add(35975, 27424);
					this.Add(35976, 27770); this.Add(35977, 28500); this.Add(35978, 31348); this.Add(35979, 32080); this.Add(35980, 34880); this.Add(35981, 35363); this.Add(35982, 26376); this.Add(35983, 20214);
					this.Add(35984, 20537); this.Add(35985, 20518); this.Add(35986, 20581); this.Add(35987, 20860); this.Add(35988, 21048); this.Add(35989, 21091); this.Add(35990, 21927); this.Add(35991, 22287);
					this.Add(35992, 22533); this.Add(35993, 23244); this.Add(35994, 24314); this.Add(35995, 25010); this.Add(35996, 25080); this.Add(35997, 25331); this.Add(35998, 25458); this.Add(35999, 26908);
					this.Add(36000, 27177); this.Add(36001, 29309); this.Add(36002, 29356); this.Add(36003, 29486); this.Add(36004, 30740); this.Add(36005, 30831); this.Add(36006, 32121); this.Add(36007, 30476);
					this.Add(36008, 32937); this.Add(36009, 35211); this.Add(36010, 35609); this.Add(36011, 36066); this.Add(36012, 36562); this.Add(36013, 36963); this.Add(36014, 37749); this.Add(36015, 38522);
					this.Add(36016, 38997); this.Add(36017, 39443); this.Add(36018, 40568); this.Add(36019, 20803); this.Add(36020, 21407); this.Add(36021, 21427); this.Add(36022, 24187); this.Add(36023, 24358);
					this.Add(36024, 28187); this.Add(36025, 28304); this.Add(36026, 29572); this.Add(36027, 29694); this.Add(36028, 32067); this.Add(36029, 33335); this.Add(36030, 35328); this.Add(36031, 35578);
					this.Add(36032, 38480); this.Add(36033, 20046); this.Add(36034, 20491); this.Add(36035, 21476); this.Add(36036, 21628); this.Add(36037, 22266); this.Add(36038, 22993); this.Add(36039, 23396);
					this.Add(36040, 24049); this.Add(36041, 24235); this.Add(36042, 24359); this.Add(36043, 25144); this.Add(36044, 25925); this.Add(36045, 26543); this.Add(36046, 28246); this.Add(36047, 29392);
					this.Add(36048, 31946); this.Add(36049, 34996); this.Add(36050, 32929); this.Add(36051, 32993); this.Add(36052, 33776); this.Add(36053, 34382); this.Add(36054, 35463); this.Add(36055, 36328);
					this.Add(36056, 37431); this.Add(36057, 38599); this.Add(36058, 39015); this.Add(36059, 40723); this.Add(36060, 20116); this.Add(36061, 20114); this.Add(36062, 20237); this.Add(36063, 21320);
					this.Add(36064, 21577); this.Add(36065, 21566); this.Add(36066, 23087); this.Add(36067, 24460); this.Add(36068, 24481); this.Add(36069, 24735); this.Add(36070, 26791); this.Add(36071, 27278);
					this.Add(36072, 29786); this.Add(36073, 30849); this.Add(36074, 35486); this.Add(36075, 35492); this.Add(36076, 35703); this.Add(36077, 37264); this.Add(36078, 20062); this.Add(36079, 39881);
					this.Add(36080, 20132); this.Add(36081, 20348); this.Add(36082, 20399); this.Add(36083, 20505); this.Add(36084, 20502); this.Add(36085, 20809); this.Add(36086, 20844); this.Add(36087, 21151);
					this.Add(36088, 21177); this.Add(36089, 21246); this.Add(36090, 21402); this.Add(36091, 21475); this.Add(36092, 21521); this.Add(36160, 21518); this.Add(36161, 21897); this.Add(36162, 22353);
					this.Add(36163, 22434); this.Add(36164, 22909); this.Add(36165, 23380); this.Add(36166, 23389); this.Add(36167, 23439); this.Add(36168, 24037); this.Add(36169, 24039); this.Add(36170, 24055);
					this.Add(36171, 24184); this.Add(36172, 24195); this.Add(36173, 24218); this.Add(36174, 24247); this.Add(36175, 24344); this.Add(36176, 24658); this.Add(36177, 24908); this.Add(36178, 25239);
					this.Add(36179, 25304); this.Add(36180, 25511); this.Add(36181, 25915); this.Add(36182, 26114); this.Add(36183, 26179); this.Add(36184, 26356); this.Add(36185, 26477); this.Add(36186, 26657);
					this.Add(36187, 26775); this.Add(36188, 27083); this.Add(36189, 27743); this.Add(36190, 27946); this.Add(36191, 28009); this.Add(36192, 28207); this.Add(36193, 28317); this.Add(36194, 30002);
					this.Add(36195, 30343); this.Add(36196, 30828); this.Add(36197, 31295); this.Add(36198, 31968); this.Add(36199, 32005); this.Add(36200, 32024); this.Add(36201, 32094); this.Add(36202, 32177);
					this.Add(36203, 32789); this.Add(36204, 32771); this.Add(36205, 32943); this.Add(36206, 32945); this.Add(36207, 33108); this.Add(36208, 33167); this.Add(36209, 33322); this.Add(36210, 33618);
					this.Add(36211, 34892); this.Add(36212, 34913); this.Add(36213, 35611); this.Add(36214, 36002); this.Add(36215, 36092); this.Add(36216, 37066); this.Add(36217, 37237); this.Add(36218, 37489);
					this.Add(36219, 30783); this.Add(36220, 37628); this.Add(36221, 38308); this.Add(36222, 38477); this.Add(36224, 38917); this.Add(36225, 39321); this.Add(36226, 39640); this.Add(36227, 40251);
					this.Add(36228, 21083); this.Add(36229, 21163); this.Add(36230, 21495); this.Add(36231, 21512); this.Add(36232, 22741); this.Add(36233, 25335); this.Add(36234, 28640); this.Add(36235, 35946);
					this.Add(36236, 36703); this.Add(36237, 40633); this.Add(36238, 20811); this.Add(36239, 21051); this.Add(36240, 21578); this.Add(36241, 22269); this.Add(36242, 31296); this.Add(36243, 37239);
					this.Add(36244, 40288); this.Add(36245, 40658); this.Add(36246, 29508); this.Add(36247, 28425); this.Add(36248, 33136); this.Add(36249, 29969); this.Add(36250, 24573); this.Add(36251, 24794);
					this.Add(36252, 39592); this.Add(36253, 29403); this.Add(36254, 36796); this.Add(36255, 27492); this.Add(36256, 38915); this.Add(36257, 20170); this.Add(36258, 22256); this.Add(36259, 22372);
					this.Add(36260, 22718); this.Add(36261, 23130); this.Add(36262, 24680); this.Add(36263, 25031); this.Add(36264, 26127); this.Add(36265, 26118); this.Add(36266, 26681); this.Add(36267, 26801);
					this.Add(36268, 28151); this.Add(36269, 30165); this.Add(36270, 32058); this.Add(36271, 33390); this.Add(36272, 39746); this.Add(36273, 20123); this.Add(36274, 20304); this.Add(36275, 21449);
					this.Add(36276, 21766); this.Add(36277, 23919); this.Add(36278, 24038); this.Add(36279, 24046); this.Add(36280, 26619); this.Add(36281, 27801); this.Add(36282, 29811); this.Add(36283, 30722);
					this.Add(36284, 35408); this.Add(36285, 37782); this.Add(36286, 35039); this.Add(36287, 22352); this.Add(36288, 24231); this.Add(36289, 25387); this.Add(36290, 20661); this.Add(36291, 20652);
					this.Add(36292, 20877); this.Add(36293, 26368); this.Add(36294, 21705); this.Add(36295, 22622); this.Add(36296, 22971); this.Add(36297, 23472); this.Add(36298, 24425); this.Add(36299, 25165);
					this.Add(36300, 25505); this.Add(36301, 26685); this.Add(36302, 27507); this.Add(36303, 28168); this.Add(36304, 28797); this.Add(36305, 37319); this.Add(36306, 29312); this.Add(36307, 30741);
					this.Add(36308, 30758); this.Add(36309, 31085); this.Add(36310, 25998); this.Add(36311, 32048); this.Add(36312, 33756); this.Add(36313, 35009); this.Add(36314, 36617); this.Add(36315, 38555);
					this.Add(36316, 21092); this.Add(36317, 22312); this.Add(36318, 26448); this.Add(36319, 32618); this.Add(36320, 36001); this.Add(36321, 20916); this.Add(36322, 22338); this.Add(36323, 38442);
					this.Add(36324, 22586); this.Add(36325, 27018); this.Add(36326, 32948); this.Add(36327, 21682); this.Add(36328, 23822); this.Add(36329, 22524); this.Add(36330, 30869); this.Add(36331, 40442);
					this.Add(36332, 20316); this.Add(36333, 21066); this.Add(36334, 21643); this.Add(36335, 25662); this.Add(36336, 26152); this.Add(36337, 26388); this.Add(36338, 26613); this.Add(36339, 31364);
					this.Add(36340, 31574); this.Add(36341, 32034); this.Add(36342, 37679); this.Add(36343, 26716); this.Add(36344, 39853); this.Add(36345, 31545); this.Add(36346, 21273); this.Add(36347, 20874);
					this.Add(36348, 21047); this.Add(36416, 23519); this.Add(36417, 25334); this.Add(36418, 25774); this.Add(36419, 25830); this.Add(36420, 26413); this.Add(36421, 27578); this.Add(36422, 34217);
					this.Add(36423, 38609); this.Add(36424, 30352); this.Add(36425, 39894); this.Add(36426, 25420); this.Add(36427, 37638); this.Add(36428, 39851); this.Add(36429, 30399); this.Add(36430, 26194);
					this.Add(36431, 19977); this.Add(36432, 20632); this.Add(36433, 21442); this.Add(36434, 23665); this.Add(36435, 24808); this.Add(36436, 25746); this.Add(36437, 25955); this.Add(36438, 26719);
					this.Add(36439, 29158); this.Add(36440, 29642); this.Add(36441, 29987); this.Add(36442, 31639); this.Add(36443, 32386); this.Add(36444, 34453); this.Add(36445, 35715); this.Add(36446, 36059);
					this.Add(36447, 37240); this.Add(36448, 39184); this.Add(36449, 26028); this.Add(36450, 26283); this.Add(36451, 27531); this.Add(36452, 20181); this.Add(36453, 20180); this.Add(36454, 20282);
					this.Add(36455, 20351); this.Add(36456, 21050); this.Add(36457, 21496); this.Add(36458, 21490); this.Add(36459, 21987); this.Add(36460, 22235); this.Add(36461, 22763); this.Add(36462, 22987);
					this.Add(36463, 22985); this.Add(36464, 23039); this.Add(36465, 23376); this.Add(36466, 23629); this.Add(36467, 24066); this.Add(36468, 24107); this.Add(36469, 24535); this.Add(36470, 24605);
					this.Add(36471, 25351); this.Add(36472, 25903); this.Add(36473, 23388); this.Add(36474, 26031); this.Add(36475, 26045); this.Add(36476, 26088); this.Add(36477, 26525); this.Add(36478, 27490);
					this.Add(36480, 27515); this.Add(36481, 27663); this.Add(36482, 29509); this.Add(36483, 31049); this.Add(36484, 31169); this.Add(36485, 31992); this.Add(36486, 32025); this.Add(36487, 32043);
					this.Add(36488, 32930); this.Add(36489, 33026); this.Add(36490, 33267); this.Add(36491, 35222); this.Add(36492, 35422); this.Add(36493, 35433); this.Add(36494, 35430); this.Add(36495, 35468);
					this.Add(36496, 35566); this.Add(36497, 36039); this.Add(36498, 36060); this.Add(36499, 38604); this.Add(36500, 39164); this.Add(36501, 27503); this.Add(36502, 20107); this.Add(36503, 20284);
					this.Add(36504, 20365); this.Add(36505, 20816); this.Add(36506, 23383); this.Add(36507, 23546); this.Add(36508, 24904); this.Add(36509, 25345); this.Add(36510, 26178); this.Add(36511, 27425);
					this.Add(36512, 28363); this.Add(36513, 27835); this.Add(36514, 29246); this.Add(36515, 29885); this.Add(36516, 30164); this.Add(36517, 30913); this.Add(36518, 31034); this.Add(36519, 32780);
					this.Add(36520, 32819); this.Add(36521, 33258); this.Add(36522, 33940); this.Add(36523, 36766); this.Add(36524, 27728); this.Add(36525, 40575); this.Add(36526, 24335); this.Add(36527, 35672);
					this.Add(36528, 40235); this.Add(36529, 31482); this.Add(36530, 36600); this.Add(36531, 23437); this.Add(36532, 38635); this.Add(36533, 19971); this.Add(36534, 21489); this.Add(36535, 22519);
					this.Add(36536, 22833); this.Add(36537, 23241); this.Add(36538, 23460); this.Add(36539, 24713); this.Add(36540, 28287); this.Add(36541, 28422); this.Add(36542, 30142); this.Add(36543, 36074);
					this.Add(36544, 23455); this.Add(36545, 34048); this.Add(36546, 31712); this.Add(36547, 20594); this.Add(36548, 26612); this.Add(36549, 33437); this.Add(36550, 23649); this.Add(36551, 34122);
					this.Add(36552, 32286); this.Add(36553, 33294); this.Add(36554, 20889); this.Add(36555, 23556); this.Add(36556, 25448); this.Add(36557, 36198); this.Add(36558, 26012); this.Add(36559, 29038);
					this.Add(36560, 31038); this.Add(36561, 32023); this.Add(36562, 32773); this.Add(36563, 35613); this.Add(36564, 36554); this.Add(36565, 36974); this.Add(36566, 34503); this.Add(36567, 37034);
					this.Add(36568, 20511); this.Add(36569, 21242); this.Add(36570, 23610); this.Add(36571, 26451); this.Add(36572, 28796); this.Add(36573, 29237); this.Add(36574, 37196); this.Add(36575, 37320);
					this.Add(36576, 37675); this.Add(36577, 33509); this.Add(36578, 23490); this.Add(36579, 24369); this.Add(36580, 24825); this.Add(36581, 20027); this.Add(36582, 21462); this.Add(36583, 23432);
					this.Add(36584, 25163); this.Add(36585, 26417); this.Add(36586, 27530); this.Add(36587, 29417); this.Add(36588, 29664); this.Add(36589, 31278); this.Add(36590, 33131); this.Add(36591, 36259);
					this.Add(36592, 37202); this.Add(36593, 39318); this.Add(36594, 20754); this.Add(36595, 21463); this.Add(36596, 21610); this.Add(36597, 23551); this.Add(36598, 25480); this.Add(36599, 27193);
					this.Add(36600, 32172); this.Add(36601, 38656); this.Add(36602, 22234); this.Add(36603, 21454); this.Add(36604, 21608); this.Add(36672, 23447); this.Add(36673, 23601); this.Add(36674, 24030);
					this.Add(36675, 20462); this.Add(36676, 24833); this.Add(36677, 25342); this.Add(36678, 27954); this.Add(36679, 31168); this.Add(36680, 31179); this.Add(36681, 32066); this.Add(36682, 32333);
					this.Add(36683, 32722); this.Add(36684, 33261); this.Add(36685, 33311); this.Add(36686, 33936); this.Add(36687, 34886); this.Add(36688, 35186); this.Add(36689, 35728); this.Add(36690, 36468);
					this.Add(36691, 36655); this.Add(36692, 36913); this.Add(36693, 37195); this.Add(36694, 37228); this.Add(36695, 38598); this.Add(36696, 37276); this.Add(36697, 20160); this.Add(36698, 20303);
					this.Add(36699, 20805); this.Add(36700, 21313); this.Add(36701, 24467); this.Add(36702, 25102); this.Add(36703, 26580); this.Add(36704, 27713); this.Add(36705, 28171); this.Add(36706, 29539);
					this.Add(36707, 32294); this.Add(36708, 37325); this.Add(36709, 37507); this.Add(36710, 21460); this.Add(36711, 22809); this.Add(36712, 23487); this.Add(36713, 28113); this.Add(36714, 31069);
					this.Add(36715, 32302); this.Add(36716, 31899); this.Add(36717, 22654); this.Add(36718, 29087); this.Add(36719, 20986); this.Add(36720, 34899); this.Add(36721, 36848); this.Add(36722, 20426);
					this.Add(36723, 23803); this.Add(36724, 26149); this.Add(36725, 30636); this.Add(36726, 31459); this.Add(36727, 33308); this.Add(36728, 39423); this.Add(36729, 20934); this.Add(36730, 24490);
					this.Add(36731, 26092); this.Add(36732, 26991); this.Add(36733, 27529); this.Add(36734, 28147); this.Add(36736, 28310); this.Add(36737, 28516); this.Add(36738, 30462); this.Add(36739, 32020);
					this.Add(36740, 24033); this.Add(36741, 36981); this.Add(36742, 37255); this.Add(36743, 38918); this.Add(36744, 20966); this.Add(36745, 21021); this.Add(36746, 25152); this.Add(36747, 26257);
					this.Add(36748, 26329); this.Add(36749, 28186); this.Add(36750, 24246); this.Add(36751, 32210); this.Add(36752, 32626); this.Add(36753, 26360); this.Add(36754, 34223); this.Add(36755, 34295);
					this.Add(36756, 35576); this.Add(36757, 21161); this.Add(36758, 21465); this.Add(36759, 22899); this.Add(36760, 24207); this.Add(36761, 24464); this.Add(36762, 24661); this.Add(36763, 37604);
					this.Add(36764, 38500); this.Add(36765, 20663); this.Add(36766, 20767); this.Add(36767, 21213); this.Add(36768, 21280); this.Add(36769, 21319); this.Add(36770, 21484); this.Add(36771, 21736);
					this.Add(36772, 21830); this.Add(36773, 21809); this.Add(36774, 22039); this.Add(36775, 22888); this.Add(36776, 22974); this.Add(36777, 23100); this.Add(36778, 23477); this.Add(36779, 23558);
					this.Add(36780, 23567); this.Add(36781, 23569); this.Add(36782, 23578); this.Add(36783, 24196); this.Add(36784, 24202); this.Add(36785, 24288); this.Add(36786, 24432); this.Add(36787, 25215);
					this.Add(36788, 25220); this.Add(36789, 25307); this.Add(36790, 25484); this.Add(36791, 25463); this.Add(36792, 26119); this.Add(36793, 26124); this.Add(36794, 26157); this.Add(36795, 26230);
					this.Add(36796, 26494); this.Add(36797, 26786); this.Add(36798, 27167); this.Add(36799, 27189); this.Add(36800, 27836); this.Add(36801, 28040); this.Add(36802, 28169); this.Add(36803, 28248);
					this.Add(36804, 28988); this.Add(36805, 28966); this.Add(36806, 29031); this.Add(36807, 30151); this.Add(36808, 30465); this.Add(36809, 30813); this.Add(36810, 30977); this.Add(36811, 31077);
					this.Add(36812, 31216); this.Add(36813, 31456); this.Add(36814, 31505); this.Add(36815, 31911); this.Add(36816, 32057); this.Add(36817, 32918); this.Add(36818, 33750); this.Add(36819, 33931);
					this.Add(36820, 34121); this.Add(36821, 34909); this.Add(36822, 35059); this.Add(36823, 35359); this.Add(36824, 35388); this.Add(36825, 35412); this.Add(36826, 35443); this.Add(36827, 35937);
					this.Add(36828, 36062); this.Add(36829, 37284); this.Add(36830, 37478); this.Add(36831, 37758); this.Add(36832, 37912); this.Add(36833, 38556); this.Add(36834, 38808); this.Add(36835, 19978);
					this.Add(36836, 19976); this.Add(36837, 19998); this.Add(36838, 20055); this.Add(36839, 20887); this.Add(36840, 21104); this.Add(36841, 22478); this.Add(36842, 22580); this.Add(36843, 22732);
					this.Add(36844, 23330); this.Add(36845, 24120); this.Add(36846, 24773); this.Add(36847, 25854); this.Add(36848, 26465); this.Add(36849, 26454); this.Add(36850, 27972); this.Add(36851, 29366);
					this.Add(36852, 30067); this.Add(36853, 31331); this.Add(36854, 33976); this.Add(36855, 35698); this.Add(36856, 37304); this.Add(36857, 37664); this.Add(36858, 22065); this.Add(36859, 22516);
					this.Add(36860, 39166); this.Add(36928, 25325); this.Add(36929, 26893); this.Add(36930, 27542); this.Add(36931, 29165); this.Add(36932, 32340); this.Add(36933, 32887); this.Add(36934, 33394);
					this.Add(36935, 35302); this.Add(36936, 39135); this.Add(36937, 34645); this.Add(36938, 36785); this.Add(36939, 23611); this.Add(36940, 20280); this.Add(36941, 20449); this.Add(36942, 20405);
					this.Add(36943, 21767); this.Add(36944, 23072); this.Add(36945, 23517); this.Add(36946, 23529); this.Add(36947, 24515); this.Add(36948, 24910); this.Add(36949, 25391); this.Add(36950, 26032);
					this.Add(36951, 26187); this.Add(36952, 26862); this.Add(36953, 27035); this.Add(36954, 28024); this.Add(36955, 28145); this.Add(36956, 30003); this.Add(36957, 30137); this.Add(36958, 30495);
					this.Add(36959, 31070); this.Add(36960, 31206); this.Add(36961, 32051); this.Add(36962, 33251); this.Add(36963, 33455); this.Add(36964, 34218); this.Add(36965, 35242); this.Add(36966, 35386);
					this.Add(36967, 36523); this.Add(36968, 36763); this.Add(36969, 36914); this.Add(36970, 37341); this.Add(36971, 38663); this.Add(36972, 20154); this.Add(36973, 20161); this.Add(36974, 20995);
					this.Add(36975, 22645); this.Add(36976, 22764); this.Add(36977, 23563); this.Add(36978, 29978); this.Add(36979, 23613); this.Add(36980, 33102); this.Add(36981, 35338); this.Add(36982, 36805);
					this.Add(36983, 38499); this.Add(36984, 38765); this.Add(36985, 31525); this.Add(36986, 35535); this.Add(36987, 38920); this.Add(36988, 37218); this.Add(36989, 22259); this.Add(36990, 21416);
					this.Add(36992, 36887); this.Add(36993, 21561); this.Add(36994, 22402); this.Add(36995, 24101); this.Add(36996, 25512); this.Add(36997, 27700); this.Add(36998, 28810); this.Add(36999, 30561);
					this.Add(37000, 31883); this.Add(37001, 32736); this.Add(37002, 34928); this.Add(37003, 36930); this.Add(37004, 37204); this.Add(37005, 37648); this.Add(37006, 37656); this.Add(37007, 38543);
					this.Add(37008, 29790); this.Add(37009, 39620); this.Add(37010, 23815); this.Add(37011, 23913); this.Add(37012, 25968); this.Add(37013, 26530); this.Add(37014, 36264); this.Add(37015, 38619);
					this.Add(37016, 25454); this.Add(37017, 26441); this.Add(37018, 26905); this.Add(37019, 33733); this.Add(37020, 38935); this.Add(37021, 38592); this.Add(37022, 35070); this.Add(37023, 28548);
					this.Add(37024, 25722); this.Add(37025, 23544); this.Add(37026, 19990); this.Add(37027, 28716); this.Add(37028, 30045); this.Add(37029, 26159); this.Add(37030, 20932); this.Add(37031, 21046);
					this.Add(37032, 21218); this.Add(37033, 22995); this.Add(37034, 24449); this.Add(37035, 24615); this.Add(37036, 25104); this.Add(37037, 25919); this.Add(37038, 25972); this.Add(37039, 26143);
					this.Add(37040, 26228); this.Add(37041, 26866); this.Add(37042, 26646); this.Add(37043, 27491); this.Add(37044, 28165); this.Add(37045, 29298); this.Add(37046, 29983); this.Add(37047, 30427);
					this.Add(37048, 31934); this.Add(37049, 32854); this.Add(37050, 22768); this.Add(37051, 35069); this.Add(37052, 35199); this.Add(37053, 35488); this.Add(37054, 35475); this.Add(37055, 35531);
					this.Add(37056, 36893); this.Add(37057, 37266); this.Add(37058, 38738); this.Add(37059, 38745); this.Add(37060, 25993); this.Add(37061, 31246); this.Add(37062, 33030); this.Add(37063, 38587);
					this.Add(37064, 24109); this.Add(37065, 24796); this.Add(37066, 25114); this.Add(37067, 26021); this.Add(37068, 26132); this.Add(37069, 26512); this.Add(37070, 30707); this.Add(37071, 31309);
					this.Add(37072, 31821); this.Add(37073, 32318); this.Add(37074, 33034); this.Add(37075, 36012); this.Add(37076, 36196); this.Add(37077, 36321); this.Add(37078, 36447); this.Add(37079, 30889);
					this.Add(37080, 20999); this.Add(37081, 25305); this.Add(37082, 25509); this.Add(37083, 25666); this.Add(37084, 25240); this.Add(37085, 35373); this.Add(37086, 31363); this.Add(37087, 31680);
					this.Add(37088, 35500); this.Add(37089, 38634); this.Add(37090, 32118); this.Add(37091, 33292); this.Add(37092, 34633); this.Add(37093, 20185); this.Add(37094, 20808); this.Add(37095, 21315);
					this.Add(37096, 21344); this.Add(37097, 23459); this.Add(37098, 23554); this.Add(37099, 23574); this.Add(37100, 24029); this.Add(37101, 25126); this.Add(37102, 25159); this.Add(37103, 25776);
					this.Add(37104, 26643); this.Add(37105, 26676); this.Add(37106, 27849); this.Add(37107, 27973); this.Add(37108, 27927); this.Add(37109, 26579); this.Add(37110, 28508); this.Add(37111, 29006);
					this.Add(37112, 29053); this.Add(37113, 26059); this.Add(37114, 31359); this.Add(37115, 31661); this.Add(37116, 32218); this.Add(37184, 32330); this.Add(37185, 32680); this.Add(37186, 33146);
					this.Add(37187, 33307); this.Add(37188, 33337); this.Add(37189, 34214); this.Add(37190, 35438); this.Add(37191, 36046); this.Add(37192, 36341); this.Add(37193, 36984); this.Add(37194, 36983);
					this.Add(37195, 37549); this.Add(37196, 37521); this.Add(37197, 38275); this.Add(37198, 39854); this.Add(37199, 21069); this.Add(37200, 21892); this.Add(37201, 28472); this.Add(37202, 28982);
					this.Add(37203, 20840); this.Add(37204, 31109); this.Add(37205, 32341); this.Add(37206, 33203); this.Add(37207, 31950); this.Add(37208, 22092); this.Add(37209, 22609); this.Add(37210, 23720);
					this.Add(37211, 25514); this.Add(37212, 26366); this.Add(37213, 26365); this.Add(37214, 26970); this.Add(37215, 29401); this.Add(37216, 30095); this.Add(37217, 30094); this.Add(37218, 30990);
					this.Add(37219, 31062); this.Add(37220, 31199); this.Add(37221, 31895); this.Add(37222, 32032); this.Add(37223, 32068); this.Add(37224, 34311); this.Add(37225, 35380); this.Add(37226, 38459);
					this.Add(37227, 36961); this.Add(37228, 40736); this.Add(37229, 20711); this.Add(37230, 21109); this.Add(37231, 21452); this.Add(37232, 21474); this.Add(37233, 20489); this.Add(37234, 21930);
					this.Add(37235, 22766); this.Add(37236, 22863); this.Add(37237, 29245); this.Add(37238, 23435); this.Add(37239, 23652); this.Add(37240, 21277); this.Add(37241, 24803); this.Add(37242, 24819);
					this.Add(37243, 25436); this.Add(37244, 25475); this.Add(37245, 25407); this.Add(37246, 25531); this.Add(37248, 25805); this.Add(37249, 26089); this.Add(37250, 26361); this.Add(37251, 24035);
					this.Add(37252, 27085); this.Add(37253, 27133); this.Add(37254, 28437); this.Add(37255, 29157); this.Add(37256, 20105); this.Add(37257, 30185); this.Add(37258, 30456); this.Add(37259, 31379);
					this.Add(37260, 31967); this.Add(37261, 32207); this.Add(37262, 32156); this.Add(37263, 32865); this.Add(37264, 33609); this.Add(37265, 33624); this.Add(37266, 33900); this.Add(37267, 33980);
					this.Add(37268, 34299); this.Add(37269, 35013); this.Add(37270, 36208); this.Add(37271, 36865); this.Add(37272, 36973); this.Add(37273, 37783); this.Add(37274, 38684); this.Add(37275, 39442);
					this.Add(37276, 20687); this.Add(37277, 22679); this.Add(37278, 24974); this.Add(37279, 33235); this.Add(37280, 34101); this.Add(37281, 36104); this.Add(37282, 36896); this.Add(37283, 20419);
					this.Add(37284, 20596); this.Add(37285, 21063); this.Add(37286, 21363); this.Add(37287, 24687); this.Add(37288, 25417); this.Add(37289, 26463); this.Add(37290, 28204); this.Add(37291, 36275);
					this.Add(37292, 36895); this.Add(37293, 20439); this.Add(37294, 23646); this.Add(37295, 36042); this.Add(37296, 26063); this.Add(37297, 32154); this.Add(37298, 21330); this.Add(37299, 34966);
					this.Add(37300, 20854); this.Add(37301, 25539); this.Add(37302, 23384); this.Add(37303, 23403); this.Add(37304, 23562); this.Add(37305, 25613); this.Add(37306, 26449); this.Add(37307, 36956);
					this.Add(37308, 20182); this.Add(37309, 22810); this.Add(37310, 22826); this.Add(37311, 27760); this.Add(37312, 35409); this.Add(37313, 21822); this.Add(37314, 22549); this.Add(37315, 22949);
					this.Add(37316, 24816); this.Add(37317, 25171); this.Add(37318, 26561); this.Add(37319, 33333); this.Add(37320, 26965); this.Add(37321, 38464); this.Add(37322, 39364); this.Add(37323, 39464);
					this.Add(37324, 20307); this.Add(37325, 22534); this.Add(37326, 23550); this.Add(37327, 32784); this.Add(37328, 23729); this.Add(37329, 24111); this.Add(37330, 24453); this.Add(37331, 24608);
					this.Add(37332, 24907); this.Add(37333, 25140); this.Add(37334, 26367); this.Add(37335, 27888); this.Add(37336, 28382); this.Add(37337, 32974); this.Add(37338, 33151); this.Add(37339, 33492);
					this.Add(37340, 34955); this.Add(37341, 36024); this.Add(37342, 36864); this.Add(37343, 36910); this.Add(37344, 38538); this.Add(37345, 40667); this.Add(37346, 39899); this.Add(37347, 20195);
					this.Add(37348, 21488); this.Add(37349, 22823); this.Add(37350, 31532); this.Add(37351, 37261); this.Add(37352, 38988); this.Add(37353, 40441); this.Add(37354, 28381); this.Add(37355, 28711);
					this.Add(37356, 21331); this.Add(37357, 21828); this.Add(37358, 23429); this.Add(37359, 25176); this.Add(37360, 25246); this.Add(37361, 25299); this.Add(37362, 27810); this.Add(37363, 28655);
					this.Add(37364, 29730); this.Add(37365, 35351); this.Add(37366, 37944); this.Add(37367, 28609); this.Add(37368, 35582); this.Add(37369, 33592); this.Add(37370, 20967); this.Add(37371, 34552);
					this.Add(37372, 21482); this.Add(37440, 21481); this.Add(37441, 20294); this.Add(37442, 36948); this.Add(37443, 36784); this.Add(37444, 22890); this.Add(37445, 33073); this.Add(37446, 24061);
					this.Add(37447, 31466); this.Add(37448, 36799); this.Add(37449, 26842); this.Add(37450, 35895); this.Add(37451, 29432); this.Add(37452, 40008); this.Add(37453, 27197); this.Add(37454, 35504);
					this.Add(37455, 20025); this.Add(37456, 21336); this.Add(37457, 22022); this.Add(37458, 22374); this.Add(37459, 25285); this.Add(37460, 25506); this.Add(37461, 26086); this.Add(37462, 27470);
					this.Add(37463, 28129); this.Add(37464, 28251); this.Add(37465, 28845); this.Add(37466, 30701); this.Add(37467, 31471); this.Add(37468, 31658); this.Add(37469, 32187); this.Add(37470, 32829);
					this.Add(37471, 32966); this.Add(37472, 34507); this.Add(37473, 35477); this.Add(37474, 37723); this.Add(37475, 22243); this.Add(37476, 22727); this.Add(37477, 24382); this.Add(37478, 26029);
					this.Add(37479, 26262); this.Add(37480, 27264); this.Add(37481, 27573); this.Add(37482, 30007); this.Add(37483, 35527); this.Add(37484, 20516); this.Add(37485, 30693); this.Add(37486, 22320);
					this.Add(37487, 24347); this.Add(37488, 24677); this.Add(37489, 26234); this.Add(37490, 27744); this.Add(37491, 30196); this.Add(37492, 31258); this.Add(37493, 32622); this.Add(37494, 33268);
					this.Add(37495, 34584); this.Add(37496, 36933); this.Add(37497, 39347); this.Add(37498, 31689); this.Add(37499, 30044); this.Add(37500, 31481); this.Add(37501, 31569); this.Add(37502, 33988);
					this.Add(37504, 36880); this.Add(37505, 31209); this.Add(37506, 31378); this.Add(37507, 33590); this.Add(37508, 23265); this.Add(37509, 30528); this.Add(37510, 20013); this.Add(37511, 20210);
					this.Add(37512, 23449); this.Add(37513, 24544); this.Add(37514, 25277); this.Add(37515, 26172); this.Add(37516, 26609); this.Add(37517, 27880); this.Add(37518, 34411); this.Add(37519, 34935);
					this.Add(37520, 35387); this.Add(37521, 37198); this.Add(37522, 37619); this.Add(37523, 39376); this.Add(37524, 27159); this.Add(37525, 28710); this.Add(37526, 29482); this.Add(37527, 33511);
					this.Add(37528, 33879); this.Add(37529, 36015); this.Add(37530, 19969); this.Add(37531, 20806); this.Add(37532, 20939); this.Add(37533, 21899); this.Add(37534, 23541); this.Add(37535, 24086);
					this.Add(37536, 24115); this.Add(37537, 24193); this.Add(37538, 24340); this.Add(37539, 24373); this.Add(37540, 24427); this.Add(37541, 24500); this.Add(37542, 25074); this.Add(37543, 25361);
					this.Add(37544, 26274); this.Add(37545, 26397); this.Add(37546, 28526); this.Add(37547, 29266); this.Add(37548, 30010); this.Add(37549, 30522); this.Add(37550, 32884); this.Add(37551, 33081);
					this.Add(37552, 33144); this.Add(37553, 34678); this.Add(37554, 35519); this.Add(37555, 35548); this.Add(37556, 36229); this.Add(37557, 36339); this.Add(37558, 37530); this.Add(37559, 38263);
					this.Add(37560, 38914); this.Add(37561, 40165); this.Add(37562, 21189); this.Add(37563, 25431); this.Add(37564, 30452); this.Add(37565, 26389); this.Add(37566, 27784); this.Add(37567, 29645);
					this.Add(37568, 36035); this.Add(37569, 37806); this.Add(37570, 38515); this.Add(37571, 27941); this.Add(37572, 22684); this.Add(37573, 26894); this.Add(37574, 27084); this.Add(37575, 36861);
					this.Add(37576, 37786); this.Add(37577, 30171); this.Add(37578, 36890); this.Add(37579, 22618); this.Add(37580, 26626); this.Add(37581, 25524); this.Add(37582, 27131); this.Add(37583, 20291);
					this.Add(37584, 28460); this.Add(37585, 26584); this.Add(37586, 36795); this.Add(37587, 34086); this.Add(37588, 32180); this.Add(37589, 37716); this.Add(37590, 26943); this.Add(37591, 28528);
					this.Add(37592, 22378); this.Add(37593, 22775); this.Add(37594, 23340); this.Add(37595, 32044); this.Add(37596, 29226); this.Add(37597, 21514); this.Add(37598, 37347); this.Add(37599, 40372);
					this.Add(37600, 20141); this.Add(37601, 20302); this.Add(37602, 20572); this.Add(37603, 20597); this.Add(37604, 21059); this.Add(37605, 35998); this.Add(37606, 21576); this.Add(37607, 22564);
					this.Add(37608, 23450); this.Add(37609, 24093); this.Add(37610, 24213); this.Add(37611, 24237); this.Add(37612, 24311); this.Add(37613, 24351); this.Add(37614, 24716); this.Add(37615, 25269);
					this.Add(37616, 25402); this.Add(37617, 25552); this.Add(37618, 26799); this.Add(37619, 27712); this.Add(37620, 30855); this.Add(37621, 31118); this.Add(37622, 31243); this.Add(37623, 32224);
					this.Add(37624, 33351); this.Add(37625, 35330); this.Add(37626, 35558); this.Add(37627, 36420); this.Add(37628, 36883); this.Add(37696, 37048); this.Add(37697, 37165); this.Add(37698, 37336);
					this.Add(37699, 40718); this.Add(37700, 27877); this.Add(37701, 25688); this.Add(37702, 25826); this.Add(37703, 25973); this.Add(37704, 28404); this.Add(37705, 30340); this.Add(37706, 31515);
					this.Add(37707, 36969); this.Add(37708, 37841); this.Add(37709, 28346); this.Add(37710, 21746); this.Add(37711, 24505); this.Add(37712, 25764); this.Add(37713, 36685); this.Add(37714, 36845);
					this.Add(37715, 37444); this.Add(37716, 20856); this.Add(37717, 22635); this.Add(37718, 22825); this.Add(37719, 23637); this.Add(37720, 24215); this.Add(37721, 28155); this.Add(37722, 32399);
					this.Add(37723, 29980); this.Add(37724, 36028); this.Add(37725, 36578); this.Add(37726, 39003); this.Add(37727, 28857); this.Add(37728, 20253); this.Add(37729, 27583); this.Add(37730, 28593);
					this.Add(37731, 30000); this.Add(37732, 38651); this.Add(37733, 20814); this.Add(37734, 21520); this.Add(37735, 22581); this.Add(37736, 22615); this.Add(37737, 22956); this.Add(37738, 23648);
					this.Add(37739, 24466); this.Add(37740, 26007); this.Add(37741, 26460); this.Add(37742, 28193); this.Add(37743, 30331); this.Add(37744, 33759); this.Add(37745, 36077); this.Add(37746, 36884);
					this.Add(37747, 37117); this.Add(37748, 37709); this.Add(37749, 30757); this.Add(37750, 30778); this.Add(37751, 21162); this.Add(37752, 24230); this.Add(37753, 22303); this.Add(37754, 22900);
					this.Add(37755, 24594); this.Add(37756, 20498); this.Add(37757, 20826); this.Add(37758, 20908); this.Add(37760, 20941); this.Add(37761, 20992); this.Add(37762, 21776); this.Add(37763, 22612);
					this.Add(37764, 22616); this.Add(37765, 22871); this.Add(37766, 23445); this.Add(37767, 23798); this.Add(37768, 23947); this.Add(37769, 24764); this.Add(37770, 25237); this.Add(37771, 25645);
					this.Add(37772, 26481); this.Add(37773, 26691); this.Add(37774, 26812); this.Add(37775, 26847); this.Add(37776, 30423); this.Add(37777, 28120); this.Add(37778, 28271); this.Add(37779, 28059);
					this.Add(37780, 28783); this.Add(37781, 29128); this.Add(37782, 24403); this.Add(37783, 30168); this.Add(37784, 31095); this.Add(37785, 31561); this.Add(37786, 31572); this.Add(37787, 31570);
					this.Add(37788, 31958); this.Add(37789, 32113); this.Add(37790, 21040); this.Add(37791, 33891); this.Add(37792, 34153); this.Add(37793, 34276); this.Add(37794, 35342); this.Add(37795, 35588);
					this.Add(37796, 35910); this.Add(37797, 36367); this.Add(37798, 36867); this.Add(37799, 36879); this.Add(37800, 37913); this.Add(37801, 38518); this.Add(37802, 38957); this.Add(37803, 39472);
					this.Add(37804, 38360); this.Add(37805, 20685); this.Add(37806, 21205); this.Add(37807, 21516); this.Add(37808, 22530); this.Add(37809, 23566); this.Add(37810, 24999); this.Add(37811, 25758);
					this.Add(37812, 27934); this.Add(37813, 30643); this.Add(37814, 31461); this.Add(37815, 33012); this.Add(37816, 33796); this.Add(37817, 36947); this.Add(37818, 37509); this.Add(37819, 23776);
					this.Add(37820, 40199); this.Add(37821, 21311); this.Add(37822, 24471); this.Add(37823, 24499); this.Add(37824, 28060); this.Add(37825, 29305); this.Add(37826, 30563); this.Add(37827, 31167);
					this.Add(37828, 31716); this.Add(37829, 27602); this.Add(37830, 29420); this.Add(37831, 35501); this.Add(37832, 26627); this.Add(37833, 27233); this.Add(37834, 20984); this.Add(37835, 31361);
					this.Add(37836, 26932); this.Add(37837, 23626); this.Add(37838, 40182); this.Add(37839, 33515); this.Add(37840, 23493); this.Add(37841, 37193); this.Add(37842, 28702); this.Add(37843, 22136);
					this.Add(37844, 23663); this.Add(37845, 24775); this.Add(37846, 25958); this.Add(37847, 27788); this.Add(37848, 35930); this.Add(37849, 36929); this.Add(37850, 38931); this.Add(37851, 21585);
					this.Add(37852, 26311); this.Add(37853, 37389); this.Add(37854, 22856); this.Add(37855, 37027); this.Add(37856, 20869); this.Add(37857, 20045); this.Add(37858, 20970); this.Add(37859, 34201);
					this.Add(37860, 35598); this.Add(37861, 28760); this.Add(37862, 25466); this.Add(37863, 37707); this.Add(37864, 26978); this.Add(37865, 39348); this.Add(37866, 32260); this.Add(37867, 30071);
					this.Add(37868, 21335); this.Add(37869, 26976); this.Add(37870, 36575); this.Add(37871, 38627); this.Add(37872, 27741); this.Add(37873, 20108); this.Add(37874, 23612); this.Add(37875, 24336);
					this.Add(37876, 36841); this.Add(37877, 21250); this.Add(37878, 36049); this.Add(37879, 32905); this.Add(37880, 34425); this.Add(37881, 24319); this.Add(37882, 26085); this.Add(37883, 20083);
					this.Add(37884, 20837); this.Add(37952, 22914); this.Add(37953, 23615); this.Add(37954, 38894); this.Add(37955, 20219); this.Add(37956, 22922); this.Add(37957, 24525); this.Add(37958, 35469);
					this.Add(37959, 28641); this.Add(37960, 31152); this.Add(37961, 31074); this.Add(37962, 23527); this.Add(37963, 33905); this.Add(37964, 29483); this.Add(37965, 29105); this.Add(37966, 24180);
					this.Add(37967, 24565); this.Add(37968, 25467); this.Add(37969, 25754); this.Add(37970, 29123); this.Add(37971, 31896); this.Add(37972, 20035); this.Add(37973, 24316); this.Add(37974, 20043);
					this.Add(37975, 22492); this.Add(37976, 22178); this.Add(37977, 24745); this.Add(37978, 28611); this.Add(37979, 32013); this.Add(37980, 33021); this.Add(37981, 33075); this.Add(37982, 33215);
					this.Add(37983, 36786); this.Add(37984, 35223); this.Add(37985, 34468); this.Add(37986, 24052); this.Add(37987, 25226); this.Add(37988, 25773); this.Add(37989, 35207); this.Add(37990, 26487);
					this.Add(37991, 27874); this.Add(37992, 27966); this.Add(37993, 29750); this.Add(37994, 30772); this.Add(37995, 23110); this.Add(37996, 32629); this.Add(37997, 33453); this.Add(37998, 39340);
					this.Add(37999, 20467); this.Add(38000, 24259); this.Add(38001, 25309); this.Add(38002, 25490); this.Add(38003, 25943); this.Add(38004, 26479); this.Add(38005, 30403); this.Add(38006, 29260);
					this.Add(38007, 32972); this.Add(38008, 32954); this.Add(38009, 36649); this.Add(38010, 37197); this.Add(38011, 20493); this.Add(38012, 22521); this.Add(38013, 23186); this.Add(38014, 26757);
					this.Add(38016, 26995); this.Add(38017, 29028); this.Add(38018, 29437); this.Add(38019, 36023); this.Add(38020, 22770); this.Add(38021, 36064); this.Add(38022, 38506); this.Add(38023, 36889);
					this.Add(38024, 34687); this.Add(38025, 31204); this.Add(38026, 30695); this.Add(38027, 33833); this.Add(38028, 20271); this.Add(38029, 21093); this.Add(38030, 21338); this.Add(38031, 25293);
					this.Add(38032, 26575); this.Add(38033, 27850); this.Add(38034, 30333); this.Add(38035, 31636); this.Add(38036, 31893); this.Add(38037, 33334); this.Add(38038, 34180); this.Add(38039, 36843);
					this.Add(38040, 26333); this.Add(38041, 28448); this.Add(38042, 29190); this.Add(38043, 32283); this.Add(38044, 33707); this.Add(38045, 39361); this.Add(38046, 40614); this.Add(38047, 20989);
					this.Add(38048, 31665); this.Add(38049, 30834); this.Add(38050, 31672); this.Add(38051, 32903); this.Add(38052, 31560); this.Add(38053, 27368); this.Add(38054, 24161); this.Add(38055, 32908);
					this.Add(38056, 30033); this.Add(38057, 30048); this.Add(38058, 20843); this.Add(38059, 37474); this.Add(38060, 28300); this.Add(38061, 30330); this.Add(38062, 37271); this.Add(38063, 39658);
					this.Add(38064, 20240); this.Add(38065, 32624); this.Add(38066, 25244); this.Add(38067, 31567); this.Add(38068, 38309); this.Add(38069, 40169); this.Add(38070, 22138); this.Add(38071, 22617);
					this.Add(38072, 34532); this.Add(38073, 38588); this.Add(38074, 20276); this.Add(38075, 21028); this.Add(38076, 21322); this.Add(38077, 21453); this.Add(38078, 21467); this.Add(38079, 24070);
					this.Add(38080, 25644); this.Add(38081, 26001); this.Add(38082, 26495); this.Add(38083, 27710); this.Add(38084, 27726); this.Add(38085, 29256); this.Add(38086, 29359); this.Add(38087, 29677);
					this.Add(38088, 30036); this.Add(38089, 32321); this.Add(38090, 33324); this.Add(38091, 34281); this.Add(38092, 36009); this.Add(38093, 31684); this.Add(38094, 37318); this.Add(38095, 29033);
					this.Add(38096, 38930); this.Add(38097, 39151); this.Add(38098, 25405); this.Add(38099, 26217); this.Add(38100, 30058); this.Add(38101, 30436); this.Add(38102, 30928); this.Add(38103, 34115);
					this.Add(38104, 34542); this.Add(38105, 21290); this.Add(38106, 21329); this.Add(38107, 21542); this.Add(38108, 22915); this.Add(38109, 24199); this.Add(38110, 24444); this.Add(38111, 24754);
					this.Add(38112, 25161); this.Add(38113, 25209); this.Add(38114, 25259); this.Add(38115, 26000); this.Add(38116, 27604); this.Add(38117, 27852); this.Add(38118, 30130); this.Add(38119, 30382);
					this.Add(38120, 30865); this.Add(38121, 31192); this.Add(38122, 32203); this.Add(38123, 32631); this.Add(38124, 32933); this.Add(38125, 34987); this.Add(38126, 35513); this.Add(38127, 36027);
					this.Add(38128, 36991); this.Add(38129, 38750); this.Add(38130, 39131); this.Add(38131, 27147); this.Add(38132, 31800); this.Add(38133, 20633); this.Add(38134, 23614); this.Add(38135, 24494);
					this.Add(38136, 26503); this.Add(38137, 27608); this.Add(38138, 29749); this.Add(38139, 30473); this.Add(38140, 32654); this.Add(38208, 40763); this.Add(38209, 26570); this.Add(38210, 31255);
					this.Add(38211, 21305); this.Add(38212, 30091); this.Add(38213, 39661); this.Add(38214, 24422); this.Add(38215, 33181); this.Add(38216, 33777); this.Add(38217, 32920); this.Add(38218, 24380);
					this.Add(38219, 24517); this.Add(38220, 30050); this.Add(38221, 31558); this.Add(38222, 36924); this.Add(38223, 26727); this.Add(38224, 23019); this.Add(38225, 23195); this.Add(38226, 32016);
					this.Add(38227, 30334); this.Add(38228, 35628); this.Add(38229, 20469); this.Add(38230, 24426); this.Add(38231, 27161); this.Add(38232, 27703); this.Add(38233, 28418); this.Add(38234, 29922);
					this.Add(38235, 31080); this.Add(38236, 34920); this.Add(38237, 35413); this.Add(38238, 35961); this.Add(38239, 24287); this.Add(38240, 25551); this.Add(38241, 30149); this.Add(38242, 31186);
					this.Add(38243, 33495); this.Add(38244, 37672); this.Add(38245, 37618); this.Add(38246, 33948); this.Add(38247, 34541); this.Add(38248, 39981); this.Add(38249, 21697); this.Add(38250, 24428);
					this.Add(38251, 25996); this.Add(38252, 27996); this.Add(38253, 28693); this.Add(38254, 36007); this.Add(38255, 36051); this.Add(38256, 38971); this.Add(38257, 25935); this.Add(38258, 29942);
					this.Add(38259, 19981); this.Add(38260, 20184); this.Add(38261, 22496); this.Add(38262, 22827); this.Add(38263, 23142); this.Add(38264, 23500); this.Add(38265, 20904); this.Add(38266, 24067);
					this.Add(38267, 24220); this.Add(38268, 24598); this.Add(38269, 25206); this.Add(38270, 25975); this.Add(38272, 26023); this.Add(38273, 26222); this.Add(38274, 28014); this.Add(38275, 29238);
					this.Add(38276, 31526); this.Add(38277, 33104); this.Add(38278, 33178); this.Add(38279, 33433); this.Add(38280, 35676); this.Add(38281, 36000); this.Add(38282, 36070); this.Add(38283, 36212);
					this.Add(38284, 38428); this.Add(38285, 38468); this.Add(38286, 20398); this.Add(38287, 25771); this.Add(38288, 27494); this.Add(38289, 33310); this.Add(38290, 33889); this.Add(38291, 34154);
					this.Add(38292, 37096); this.Add(38293, 23553); this.Add(38294, 26963); this.Add(38295, 39080); this.Add(38296, 33914); this.Add(38297, 34135); this.Add(38298, 20239); this.Add(38299, 21103);
					this.Add(38300, 24489); this.Add(38301, 24133); this.Add(38302, 26381); this.Add(38303, 31119); this.Add(38304, 33145); this.Add(38305, 35079); this.Add(38306, 35206); this.Add(38307, 28149);
					this.Add(38308, 24343); this.Add(38309, 25173); this.Add(38310, 27832); this.Add(38311, 20175); this.Add(38312, 29289); this.Add(38313, 39826); this.Add(38314, 20998); this.Add(38315, 21563);
					this.Add(38316, 22132); this.Add(38317, 22707); this.Add(38318, 24996); this.Add(38319, 25198); this.Add(38320, 28954); this.Add(38321, 22894); this.Add(38322, 31881); this.Add(38323, 31966);
					this.Add(38324, 32027); this.Add(38325, 38640); this.Add(38326, 25991); this.Add(38327, 32862); this.Add(38328, 19993); this.Add(38329, 20341); this.Add(38330, 20853); this.Add(38331, 22592);
					this.Add(38332, 24163); this.Add(38333, 24179); this.Add(38334, 24330); this.Add(38335, 26564); this.Add(38336, 20006); this.Add(38337, 34109); this.Add(38338, 38281); this.Add(38339, 38491);
					this.Add(38340, 31859); this.Add(38341, 38913); this.Add(38342, 20731); this.Add(38343, 22721); this.Add(38344, 30294); this.Add(38345, 30887); this.Add(38346, 21029); this.Add(38347, 30629);
					this.Add(38348, 34065); this.Add(38349, 31622); this.Add(38350, 20559); this.Add(38351, 22793); this.Add(38352, 29255); this.Add(38353, 31687); this.Add(38354, 32232); this.Add(38355, 36794);
					this.Add(38356, 36820); this.Add(38357, 36941); this.Add(38358, 20415); this.Add(38359, 21193); this.Add(38360, 23081); this.Add(38361, 24321); this.Add(38362, 38829); this.Add(38363, 20445);
					this.Add(38364, 33303); this.Add(38365, 37610); this.Add(38366, 22275); this.Add(38367, 25429); this.Add(38368, 27497); this.Add(38369, 29995); this.Add(38370, 35036); this.Add(38371, 36628);
					this.Add(38372, 31298); this.Add(38373, 21215); this.Add(38374, 22675); this.Add(38375, 24917); this.Add(38376, 25098); this.Add(38377, 26286); this.Add(38378, 27597); this.Add(38379, 31807);
					this.Add(38380, 33769); this.Add(38381, 20515); this.Add(38382, 20472); this.Add(38383, 21253); this.Add(38384, 21574); this.Add(38385, 22577); this.Add(38386, 22857); this.Add(38387, 23453);
					this.Add(38388, 23792); this.Add(38389, 23791); this.Add(38390, 23849); this.Add(38391, 24214); this.Add(38392, 25265); this.Add(38393, 25447); this.Add(38394, 25918); this.Add(38395, 26041);
					this.Add(38396, 26379); this.Add(38464, 27861); this.Add(38465, 27873); this.Add(38466, 28921); this.Add(38467, 30770); this.Add(38468, 32299); this.Add(38469, 32990); this.Add(38470, 33459);
					this.Add(38471, 33804); this.Add(38472, 34028); this.Add(38473, 34562); this.Add(38474, 35090); this.Add(38475, 35370); this.Add(38476, 35914); this.Add(38477, 37030); this.Add(38478, 37586);
					this.Add(38479, 39165); this.Add(38480, 40179); this.Add(38481, 40300); this.Add(38482, 20047); this.Add(38483, 20129); this.Add(38484, 20621); this.Add(38485, 21078); this.Add(38486, 22346);
					this.Add(38487, 22952); this.Add(38488, 24125); this.Add(38489, 24536); this.Add(38490, 24537); this.Add(38491, 25151); this.Add(38492, 26292); this.Add(38493, 26395); this.Add(38494, 26576);
					this.Add(38495, 26834); this.Add(38496, 20882); this.Add(38497, 32033); this.Add(38498, 32938); this.Add(38499, 33192); this.Add(38500, 35584); this.Add(38501, 35980); this.Add(38502, 36031);
					this.Add(38503, 37502); this.Add(38504, 38450); this.Add(38505, 21536); this.Add(38506, 38956); this.Add(38507, 21271); this.Add(38508, 20693); this.Add(38509, 21340); this.Add(38510, 22696);
					this.Add(38511, 25778); this.Add(38512, 26420); this.Add(38513, 29287); this.Add(38514, 30566); this.Add(38515, 31302); this.Add(38516, 37350); this.Add(38517, 21187); this.Add(38518, 27809);
					this.Add(38519, 27526); this.Add(38520, 22528); this.Add(38521, 24140); this.Add(38522, 22868); this.Add(38523, 26412); this.Add(38524, 32763); this.Add(38525, 20961); this.Add(38526, 30406);
					this.Add(38528, 25705); this.Add(38529, 30952); this.Add(38530, 39764); this.Add(38531, 40635); this.Add(38532, 22475); this.Add(38533, 22969); this.Add(38534, 26151); this.Add(38535, 26522);
					this.Add(38536, 27598); this.Add(38537, 21737); this.Add(38538, 27097); this.Add(38539, 24149); this.Add(38540, 33180); this.Add(38541, 26517); this.Add(38542, 39850); this.Add(38543, 26622);
					this.Add(38544, 40018); this.Add(38545, 26717); this.Add(38546, 20134); this.Add(38547, 20451); this.Add(38548, 21448); this.Add(38549, 25273); this.Add(38550, 26411); this.Add(38551, 27819);
					this.Add(38552, 36804); this.Add(38553, 20397); this.Add(38554, 32365); this.Add(38555, 40639); this.Add(38556, 19975); this.Add(38557, 24930); this.Add(38558, 28288); this.Add(38559, 28459);
					this.Add(38560, 34067); this.Add(38561, 21619); this.Add(38562, 26410); this.Add(38563, 39749); this.Add(38564, 24051); this.Add(38565, 31637); this.Add(38566, 23724); this.Add(38567, 23494);
					this.Add(38568, 34588); this.Add(38569, 28234); this.Add(38570, 34001); this.Add(38571, 31252); this.Add(38572, 33032); this.Add(38573, 22937); this.Add(38574, 31885); this.Add(38575, 27665);
					this.Add(38576, 30496); this.Add(38577, 21209); this.Add(38578, 22818); this.Add(38579, 28961); this.Add(38580, 29279); this.Add(38581, 30683); this.Add(38582, 38695); this.Add(38583, 40289);
					this.Add(38584, 26891); this.Add(38585, 23167); this.Add(38586, 23064); this.Add(38587, 20901); this.Add(38588, 21517); this.Add(38589, 21629); this.Add(38590, 26126); this.Add(38591, 30431);
					this.Add(38592, 36855); this.Add(38593, 37528); this.Add(38594, 40180); this.Add(38595, 23018); this.Add(38596, 29277); this.Add(38597, 28357); this.Add(38598, 20813); this.Add(38599, 26825);
					this.Add(38600, 32191); this.Add(38601, 32236); this.Add(38602, 38754); this.Add(38603, 40634); this.Add(38604, 25720); this.Add(38605, 27169); this.Add(38606, 33538); this.Add(38607, 22916);
					this.Add(38608, 23391); this.Add(38609, 27611); this.Add(38610, 29467); this.Add(38611, 30450); this.Add(38612, 32178); this.Add(38613, 32791); this.Add(38614, 33945); this.Add(38615, 20786);
					this.Add(38616, 26408); this.Add(38617, 40665); this.Add(38618, 30446); this.Add(38619, 26466); this.Add(38620, 21247); this.Add(38621, 39173); this.Add(38622, 23588); this.Add(38623, 25147);
					this.Add(38624, 31870); this.Add(38625, 36016); this.Add(38626, 21839); this.Add(38627, 24758); this.Add(38628, 32011); this.Add(38629, 38272); this.Add(38630, 21249); this.Add(38631, 20063);
					this.Add(38632, 20918); this.Add(38633, 22812); this.Add(38634, 29242); this.Add(38635, 32822); this.Add(38636, 37326); this.Add(38637, 24357); this.Add(38638, 30690); this.Add(38639, 21380);
					this.Add(38640, 24441); this.Add(38641, 32004); this.Add(38642, 34220); this.Add(38643, 35379); this.Add(38644, 36493); this.Add(38645, 38742); this.Add(38646, 26611); this.Add(38647, 34222);
					this.Add(38648, 37971); this.Add(38649, 24841); this.Add(38650, 24840); this.Add(38651, 27833); this.Add(38652, 30290); this.Add(38720, 35565); this.Add(38721, 36664); this.Add(38722, 21807);
					this.Add(38723, 20305); this.Add(38724, 20778); this.Add(38725, 21191); this.Add(38726, 21451); this.Add(38727, 23461); this.Add(38728, 24189); this.Add(38729, 24736); this.Add(38730, 24962);
					this.Add(38731, 25558); this.Add(38732, 26377); this.Add(38733, 26586); this.Add(38734, 28263); this.Add(38735, 28044); this.Add(38736, 29494); this.Add(38737, 29495); this.Add(38738, 30001);
					this.Add(38739, 31056); this.Add(38740, 35029); this.Add(38741, 35480); this.Add(38742, 36938); this.Add(38743, 37009); this.Add(38744, 37109); this.Add(38745, 38596); this.Add(38746, 34701);
					this.Add(38747, 22805); this.Add(38748, 20104); this.Add(38749, 20313); this.Add(38750, 19982); this.Add(38751, 35465); this.Add(38752, 36671); this.Add(38753, 38928); this.Add(38754, 20653);
					this.Add(38755, 24188); this.Add(38756, 22934); this.Add(38757, 23481); this.Add(38758, 24248); this.Add(38759, 25562); this.Add(38760, 25594); this.Add(38761, 25793); this.Add(38762, 26332);
					this.Add(38763, 26954); this.Add(38764, 27096); this.Add(38765, 27915); this.Add(38766, 28342); this.Add(38767, 29076); this.Add(38768, 29992); this.Add(38769, 31407); this.Add(38770, 32650);
					this.Add(38771, 32768); this.Add(38772, 33865); this.Add(38773, 33993); this.Add(38774, 35201); this.Add(38775, 35617); this.Add(38776, 36362); this.Add(38777, 36965); this.Add(38778, 38525);
					this.Add(38779, 39178); this.Add(38780, 24958); this.Add(38781, 25233); this.Add(38782, 27442); this.Add(38784, 27779); this.Add(38785, 28020); this.Add(38786, 32716); this.Add(38787, 32764);
					this.Add(38788, 28096); this.Add(38789, 32645); this.Add(38790, 34746); this.Add(38791, 35064); this.Add(38792, 26469); this.Add(38793, 33713); this.Add(38794, 38972); this.Add(38795, 38647);
					this.Add(38796, 27931); this.Add(38797, 32097); this.Add(38798, 33853); this.Add(38799, 37226); this.Add(38800, 20081); this.Add(38801, 21365); this.Add(38802, 23888); this.Add(38803, 27396);
					this.Add(38804, 28651); this.Add(38805, 34253); this.Add(38806, 34349); this.Add(38807, 35239); this.Add(38808, 21033); this.Add(38809, 21519); this.Add(38810, 23653); this.Add(38811, 26446);
					this.Add(38812, 26792); this.Add(38813, 29702); this.Add(38814, 29827); this.Add(38815, 30178); this.Add(38816, 35023); this.Add(38817, 35041); this.Add(38818, 37324); this.Add(38819, 38626);
					this.Add(38820, 38520); this.Add(38821, 24459); this.Add(38822, 29575); this.Add(38823, 31435); this.Add(38824, 33870); this.Add(38825, 25504); this.Add(38826, 30053); this.Add(38827, 21129);
					this.Add(38828, 27969); this.Add(38829, 28316); this.Add(38830, 29705); this.Add(38831, 30041); this.Add(38832, 30827); this.Add(38833, 31890); this.Add(38834, 38534); this.Add(38835, 31452);
					this.Add(38836, 40845); this.Add(38837, 20406); this.Add(38838, 24942); this.Add(38839, 26053); this.Add(38840, 34396); this.Add(38841, 20102); this.Add(38842, 20142); this.Add(38843, 20698);
					this.Add(38844, 20001); this.Add(38845, 20940); this.Add(38846, 23534); this.Add(38847, 26009); this.Add(38848, 26753); this.Add(38849, 28092); this.Add(38850, 29471); this.Add(38851, 30274);
					this.Add(38852, 30637); this.Add(38853, 31260); this.Add(38854, 31975); this.Add(38855, 33391); this.Add(38856, 35538); this.Add(38857, 36988); this.Add(38858, 37327); this.Add(38859, 38517);
					this.Add(38860, 38936); this.Add(38861, 21147); this.Add(38862, 32209); this.Add(38863, 20523); this.Add(38864, 21400); this.Add(38865, 26519); this.Add(38866, 28107); this.Add(38867, 29136);
					this.Add(38868, 29747); this.Add(38869, 33256); this.Add(38870, 36650); this.Add(38871, 38563); this.Add(38872, 40023); this.Add(38873, 40607); this.Add(38874, 29792); this.Add(38875, 22593);
					this.Add(38876, 28057); this.Add(38877, 32047); this.Add(38878, 39006); this.Add(38879, 20196); this.Add(38880, 20278); this.Add(38881, 20363); this.Add(38882, 20919); this.Add(38883, 21169);
					this.Add(38884, 23994); this.Add(38885, 24604); this.Add(38886, 29618); this.Add(38887, 31036); this.Add(38888, 33491); this.Add(38889, 37428); this.Add(38890, 38583); this.Add(38891, 38646);
					this.Add(38892, 38666); this.Add(38893, 40599); this.Add(38894, 40802); this.Add(38895, 26278); this.Add(38896, 27508); this.Add(38897, 21015); this.Add(38898, 21155); this.Add(38899, 28872);
					this.Add(38900, 35010); this.Add(38901, 24265); this.Add(38902, 24651); this.Add(38903, 24976); this.Add(38904, 28451); this.Add(38905, 29001); this.Add(38906, 31806); this.Add(38907, 32244);
					this.Add(38908, 32879); this.Add(38976, 34030); this.Add(38977, 36899); this.Add(38978, 37676); this.Add(38979, 21570); this.Add(38980, 39791); this.Add(38981, 27347); this.Add(38982, 28809);
					this.Add(38983, 36034); this.Add(38984, 36335); this.Add(38985, 38706); this.Add(38986, 21172); this.Add(38987, 23105); this.Add(38988, 24266); this.Add(38989, 24324); this.Add(38990, 26391);
					this.Add(38991, 27004); this.Add(38992, 27028); this.Add(38993, 28010); this.Add(38994, 28431); this.Add(38995, 29282); this.Add(38996, 29436); this.Add(38997, 31725); this.Add(38998, 32769);
					this.Add(38999, 32894); this.Add(39000, 34635); this.Add(39001, 37070); this.Add(39002, 20845); this.Add(39003, 40595); this.Add(39004, 31108); this.Add(39005, 32907); this.Add(39006, 37682);
					this.Add(39007, 35542); this.Add(39008, 20525); this.Add(39009, 21644); this.Add(39010, 35441); this.Add(39011, 27498); this.Add(39012, 36036); this.Add(39013, 33031); this.Add(39014, 24785);
					this.Add(39015, 26528); this.Add(39016, 40434); this.Add(39017, 20121); this.Add(39018, 20120); this.Add(39019, 39952); this.Add(39020, 35435); this.Add(39021, 34241); this.Add(39022, 34152);
					this.Add(39023, 26880); this.Add(39024, 28286); this.Add(39025, 30871); this.Add(39026, 33109); this.Add(39071, 24332); this.Add(39072, 19984); this.Add(39073, 19989); this.Add(39074, 20010);
					this.Add(39075, 20017); this.Add(39076, 20022); this.Add(39077, 20028); this.Add(39078, 20031); this.Add(39079, 20034); this.Add(39080, 20054); this.Add(39081, 20056); this.Add(39082, 20098);
					this.Add(39083, 20101); this.Add(39084, 35947); this.Add(39085, 20106); this.Add(39086, 33298); this.Add(39087, 24333); this.Add(39088, 20110); this.Add(39089, 20126); this.Add(39090, 20127);
					this.Add(39091, 20128); this.Add(39092, 20130); this.Add(39093, 20144); this.Add(39094, 20147); this.Add(39095, 20150); this.Add(39096, 20174); this.Add(39097, 20173); this.Add(39098, 20164);
					this.Add(39099, 20166); this.Add(39100, 20162); this.Add(39101, 20183); this.Add(39102, 20190); this.Add(39103, 20205); this.Add(39104, 20191); this.Add(39105, 20215); this.Add(39106, 20233);
					this.Add(39107, 20314); this.Add(39108, 20272); this.Add(39109, 20315); this.Add(39110, 20317); this.Add(39111, 20311); this.Add(39112, 20295); this.Add(39113, 20342); this.Add(39114, 20360);
					this.Add(39115, 20367); this.Add(39116, 20376); this.Add(39117, 20347); this.Add(39118, 20329); this.Add(39119, 20336); this.Add(39120, 20369); this.Add(39121, 20335); this.Add(39122, 20358);
					this.Add(39123, 20374); this.Add(39124, 20760); this.Add(39125, 20436); this.Add(39126, 20447); this.Add(39127, 20430); this.Add(39128, 20440); this.Add(39129, 20443); this.Add(39130, 20433);
					this.Add(39131, 20442); this.Add(39132, 20432); this.Add(39133, 20452); this.Add(39134, 20453); this.Add(39135, 20506); this.Add(39136, 20520); this.Add(39137, 20500); this.Add(39138, 20522);
					this.Add(39139, 20517); this.Add(39140, 20485); this.Add(39141, 20252); this.Add(39142, 20470); this.Add(39143, 20513); this.Add(39144, 20521); this.Add(39145, 20524); this.Add(39146, 20478);
					this.Add(39147, 20463); this.Add(39148, 20497); this.Add(39149, 20486); this.Add(39150, 20547); this.Add(39151, 20551); this.Add(39152, 26371); this.Add(39153, 20565); this.Add(39154, 20560);
					this.Add(39155, 20552); this.Add(39156, 20570); this.Add(39157, 20566); this.Add(39158, 20588); this.Add(39159, 20600); this.Add(39160, 20608); this.Add(39161, 20634); this.Add(39162, 20613);
					this.Add(39163, 20660); this.Add(39164, 20658); this.Add(39232, 20681); this.Add(39233, 20682); this.Add(39234, 20659); this.Add(39235, 20674); this.Add(39236, 20694); this.Add(39237, 20702);
					this.Add(39238, 20709); this.Add(39239, 20717); this.Add(39240, 20707); this.Add(39241, 20718); this.Add(39242, 20729); this.Add(39243, 20725); this.Add(39244, 20745); this.Add(39245, 20737);
					this.Add(39246, 20738); this.Add(39247, 20758); this.Add(39248, 20757); this.Add(39249, 20756); this.Add(39250, 20762); this.Add(39251, 20769); this.Add(39252, 20794); this.Add(39253, 20791);
					this.Add(39254, 20796); this.Add(39255, 20795); this.Add(39256, 20799); this.Add(39257, 20800); this.Add(39258, 20818); this.Add(39259, 20812); this.Add(39260, 20820); this.Add(39261, 20834);
					this.Add(39262, 31480); this.Add(39263, 20841); this.Add(39264, 20842); this.Add(39265, 20846); this.Add(39266, 20864); this.Add(39267, 20866); this.Add(39268, 22232); this.Add(39269, 20876);
					this.Add(39270, 20873); this.Add(39271, 20879); this.Add(39272, 20881); this.Add(39273, 20883); this.Add(39274, 20885); this.Add(39275, 20886); this.Add(39276, 20900); this.Add(39277, 20902);
					this.Add(39278, 20898); this.Add(39279, 20905); this.Add(39280, 20906); this.Add(39281, 20907); this.Add(39282, 20915); this.Add(39283, 20913); this.Add(39284, 20914); this.Add(39285, 20912);
					this.Add(39286, 20917); this.Add(39287, 20925); this.Add(39288, 20933); this.Add(39289, 20937); this.Add(39290, 20955); this.Add(39291, 20960); this.Add(39292, 34389); this.Add(39293, 20969);
					this.Add(39294, 20973); this.Add(39296, 20976); this.Add(39297, 20981); this.Add(39298, 20990); this.Add(39299, 20996); this.Add(39300, 21003); this.Add(39301, 21012); this.Add(39302, 21006);
					this.Add(39303, 21031); this.Add(39304, 21034); this.Add(39305, 21038); this.Add(39306, 21043); this.Add(39307, 21049); this.Add(39308, 21071); this.Add(39309, 21060); this.Add(39310, 21067);
					this.Add(39311, 21068); this.Add(39312, 21086); this.Add(39313, 21076); this.Add(39314, 21098); this.Add(39315, 21108); this.Add(39316, 21097); this.Add(39317, 21107); this.Add(39318, 21119);
					this.Add(39319, 21117); this.Add(39320, 21133); this.Add(39321, 21140); this.Add(39322, 21138); this.Add(39323, 21105); this.Add(39324, 21128); this.Add(39325, 21137); this.Add(39326, 36776);
					this.Add(39327, 36775); this.Add(39328, 21164); this.Add(39329, 21165); this.Add(39330, 21180); this.Add(39331, 21173); this.Add(39332, 21185); this.Add(39333, 21197); this.Add(39334, 21207);
					this.Add(39335, 21214); this.Add(39336, 21219); this.Add(39337, 21222); this.Add(39338, 39149); this.Add(39339, 21216); this.Add(39340, 21235); this.Add(39341, 21237); this.Add(39342, 21240);
					this.Add(39343, 21241); this.Add(39344, 21254); this.Add(39345, 21256); this.Add(39346, 30008); this.Add(39347, 21261); this.Add(39348, 21264); this.Add(39349, 21263); this.Add(39350, 21269);
					this.Add(39351, 21274); this.Add(39352, 21283); this.Add(39353, 21295); this.Add(39354, 21297); this.Add(39355, 21299); this.Add(39356, 21304); this.Add(39357, 21312); this.Add(39358, 21318);
					this.Add(39359, 21317); this.Add(39360, 19991); this.Add(39361, 21321); this.Add(39362, 21325); this.Add(39363, 20950); this.Add(39364, 21342); this.Add(39365, 21353); this.Add(39366, 21358);
					this.Add(39367, 22808); this.Add(39368, 21371); this.Add(39369, 21367); this.Add(39370, 21378); this.Add(39371, 21398); this.Add(39372, 21408); this.Add(39373, 21414); this.Add(39374, 21413);
					this.Add(39375, 21422); this.Add(39376, 21424); this.Add(39377, 21430); this.Add(39378, 21443); this.Add(39379, 31762); this.Add(39380, 38617); this.Add(39381, 21471); this.Add(39382, 26364);
					this.Add(39383, 29166); this.Add(39384, 21486); this.Add(39385, 21480); this.Add(39386, 21485); this.Add(39387, 21498); this.Add(39388, 21505); this.Add(39389, 21565); this.Add(39390, 21568);
					this.Add(39391, 21548); this.Add(39392, 21549); this.Add(39393, 21564); this.Add(39394, 21550); this.Add(39395, 21558); this.Add(39396, 21545); this.Add(39397, 21533); this.Add(39398, 21582);
					this.Add(39399, 21647); this.Add(39400, 21621); this.Add(39401, 21646); this.Add(39402, 21599); this.Add(39403, 21617); this.Add(39404, 21623); this.Add(39405, 21616); this.Add(39406, 21650);
					this.Add(39407, 21627); this.Add(39408, 21632); this.Add(39409, 21622); this.Add(39410, 21636); this.Add(39411, 21648); this.Add(39412, 21638); this.Add(39413, 21703); this.Add(39414, 21666);
					this.Add(39415, 21688); this.Add(39416, 21669); this.Add(39417, 21676); this.Add(39418, 21700); this.Add(39419, 21704); this.Add(39420, 21672); this.Add(39488, 21675); this.Add(39489, 21698);
					this.Add(39490, 21668); this.Add(39491, 21694); this.Add(39492, 21692); this.Add(39493, 21720); this.Add(39494, 21733); this.Add(39495, 21734); this.Add(39496, 21775); this.Add(39497, 21780);
					this.Add(39498, 21757); this.Add(39499, 21742); this.Add(39500, 21741); this.Add(39501, 21754); this.Add(39502, 21730); this.Add(39503, 21817); this.Add(39504, 21824); this.Add(39505, 21859);
					this.Add(39506, 21836); this.Add(39507, 21806); this.Add(39508, 21852); this.Add(39509, 21829); this.Add(39510, 21846); this.Add(39511, 21847); this.Add(39512, 21816); this.Add(39513, 21811);
					this.Add(39514, 21853); this.Add(39515, 21913); this.Add(39516, 21888); this.Add(39517, 21679); this.Add(39518, 21898); this.Add(39519, 21919); this.Add(39520, 21883); this.Add(39521, 21886);
					this.Add(39522, 21912); this.Add(39523, 21918); this.Add(39524, 21934); this.Add(39525, 21884); this.Add(39526, 21891); this.Add(39527, 21929); this.Add(39528, 21895); this.Add(39529, 21928);
					this.Add(39530, 21978); this.Add(39531, 21957); this.Add(39532, 21983); this.Add(39533, 21956); this.Add(39534, 21980); this.Add(39535, 21988); this.Add(39536, 21972); this.Add(39537, 22036);
					this.Add(39538, 22007); this.Add(39539, 22038); this.Add(39540, 22014); this.Add(39541, 22013); this.Add(39542, 22043); this.Add(39543, 22009); this.Add(39544, 22094); this.Add(39545, 22096);
					this.Add(39546, 29151); this.Add(39547, 22068); this.Add(39548, 22070); this.Add(39549, 22066); this.Add(39550, 22072); this.Add(39552, 22123); this.Add(39553, 22116); this.Add(39554, 22063);
					this.Add(39555, 22124); this.Add(39556, 22122); this.Add(39557, 22150); this.Add(39558, 22144); this.Add(39559, 22154); this.Add(39560, 22176); this.Add(39561, 22164); this.Add(39562, 22159);
					this.Add(39563, 22181); this.Add(39564, 22190); this.Add(39565, 22198); this.Add(39566, 22196); this.Add(39567, 22210); this.Add(39568, 22204); this.Add(39569, 22209); this.Add(39570, 22211);
					this.Add(39571, 22208); this.Add(39572, 22216); this.Add(39573, 22222); this.Add(39574, 22225); this.Add(39575, 22227); this.Add(39576, 22231); this.Add(39577, 22254); this.Add(39578, 22265);
					this.Add(39579, 22272); this.Add(39580, 22271); this.Add(39581, 22276); this.Add(39582, 22281); this.Add(39583, 22280); this.Add(39584, 22283); this.Add(39585, 22285); this.Add(39586, 22291);
					this.Add(39587, 22296); this.Add(39588, 22294); this.Add(39589, 21959); this.Add(39590, 22300); this.Add(39591, 22310); this.Add(39592, 22327); this.Add(39593, 22328); this.Add(39594, 22350);
					this.Add(39595, 22331); this.Add(39596, 22336); this.Add(39597, 22351); this.Add(39598, 22377); this.Add(39599, 22464); this.Add(39600, 22408); this.Add(39601, 22369); this.Add(39602, 22399);
					this.Add(39603, 22409); this.Add(39604, 22419); this.Add(39605, 22432); this.Add(39606, 22451); this.Add(39607, 22436); this.Add(39608, 22442); this.Add(39609, 22448); this.Add(39610, 22467);
					this.Add(39611, 22470); this.Add(39612, 22484); this.Add(39613, 22482); this.Add(39614, 22483); this.Add(39615, 22538); this.Add(39616, 22486); this.Add(39617, 22499); this.Add(39618, 22539);
					this.Add(39619, 22553); this.Add(39620, 22557); this.Add(39621, 22642); this.Add(39622, 22561); this.Add(39623, 22626); this.Add(39624, 22603); this.Add(39625, 22640); this.Add(39626, 27584);
					this.Add(39627, 22610); this.Add(39628, 22589); this.Add(39629, 22649); this.Add(39630, 22661); this.Add(39631, 22713); this.Add(39632, 22687); this.Add(39633, 22699); this.Add(39634, 22714);
					this.Add(39635, 22750); this.Add(39636, 22715); this.Add(39637, 22712); this.Add(39638, 22702); this.Add(39639, 22725); this.Add(39640, 22739); this.Add(39641, 22737); this.Add(39642, 22743);
					this.Add(39643, 22745); this.Add(39644, 22744); this.Add(39645, 22757); this.Add(39646, 22748); this.Add(39647, 22756); this.Add(39648, 22751); this.Add(39649, 22767); this.Add(39650, 22778);
					this.Add(39651, 22777); this.Add(39652, 22779); this.Add(39653, 22780); this.Add(39654, 22781); this.Add(39655, 22786); this.Add(39656, 22794); this.Add(39657, 22800); this.Add(39658, 22811);
					this.Add(39659, 26790); this.Add(39660, 22821); this.Add(39661, 22828); this.Add(39662, 22829); this.Add(39663, 22834); this.Add(39664, 22840); this.Add(39665, 22846); this.Add(39666, 31442);
					this.Add(39667, 22869); this.Add(39668, 22864); this.Add(39669, 22862); this.Add(39670, 22874); this.Add(39671, 22872); this.Add(39672, 22882); this.Add(39673, 22880); this.Add(39674, 22887);
					this.Add(39675, 22892); this.Add(39676, 22889); this.Add(39744, 22904); this.Add(39745, 22913); this.Add(39746, 22941); this.Add(39747, 20318); this.Add(39748, 20395); this.Add(39749, 22947);
					this.Add(39750, 22962); this.Add(39751, 22982); this.Add(39752, 23016); this.Add(39753, 23004); this.Add(39754, 22925); this.Add(39755, 23001); this.Add(39756, 23002); this.Add(39757, 23077);
					this.Add(39758, 23071); this.Add(39759, 23057); this.Add(39760, 23068); this.Add(39761, 23049); this.Add(39762, 23066); this.Add(39763, 23104); this.Add(39764, 23148); this.Add(39765, 23113);
					this.Add(39766, 23093); this.Add(39767, 23094); this.Add(39768, 23138); this.Add(39769, 23146); this.Add(39770, 23194); this.Add(39771, 23228); this.Add(39772, 23230); this.Add(39773, 23243);
					this.Add(39774, 23234); this.Add(39775, 23229); this.Add(39776, 23267); this.Add(39777, 23255); this.Add(39778, 23270); this.Add(39779, 23273); this.Add(39780, 23254); this.Add(39781, 23290);
					this.Add(39782, 23291); this.Add(39783, 23308); this.Add(39784, 23307); this.Add(39785, 23318); this.Add(39786, 23346); this.Add(39787, 23248); this.Add(39788, 23338); this.Add(39789, 23350);
					this.Add(39790, 23358); this.Add(39791, 23363); this.Add(39792, 23365); this.Add(39793, 23360); this.Add(39794, 23377); this.Add(39795, 23381); this.Add(39796, 23386); this.Add(39797, 23387);
					this.Add(39798, 23397); this.Add(39799, 23401); this.Add(39800, 23408); this.Add(39801, 23411); this.Add(39802, 23413); this.Add(39803, 23416); this.Add(39804, 25992); this.Add(39805, 23418);
					this.Add(39806, 23424); this.Add(39808, 23427); this.Add(39809, 23462); this.Add(39810, 23480); this.Add(39811, 23491); this.Add(39812, 23495); this.Add(39813, 23497); this.Add(39814, 23508);
					this.Add(39815, 23504); this.Add(39816, 23524); this.Add(39817, 23526); this.Add(39818, 23522); this.Add(39819, 23518); this.Add(39820, 23525); this.Add(39821, 23531); this.Add(39822, 23536);
					this.Add(39823, 23542); this.Add(39824, 23539); this.Add(39825, 23557); this.Add(39826, 23559); this.Add(39827, 23560); this.Add(39828, 23565); this.Add(39829, 23571); this.Add(39830, 23584);
					this.Add(39831, 23586); this.Add(39832, 23592); this.Add(39833, 23608); this.Add(39834, 23609); this.Add(39835, 23617); this.Add(39836, 23622); this.Add(39837, 23630); this.Add(39838, 23635);
					this.Add(39839, 23632); this.Add(39840, 23631); this.Add(39841, 23409); this.Add(39842, 23660); this.Add(39843, 23662); this.Add(39844, 20066); this.Add(39845, 23670); this.Add(39846, 23673);
					this.Add(39847, 23692); this.Add(39848, 23697); this.Add(39849, 23700); this.Add(39850, 22939); this.Add(39851, 23723); this.Add(39852, 23739); this.Add(39853, 23734); this.Add(39854, 23740);
					this.Add(39855, 23735); this.Add(39856, 23749); this.Add(39857, 23742); this.Add(39858, 23751); this.Add(39859, 23769); this.Add(39860, 23785); this.Add(39861, 23805); this.Add(39862, 23802);
					this.Add(39863, 23789); this.Add(39864, 23948); this.Add(39865, 23786); this.Add(39866, 23819); this.Add(39867, 23829); this.Add(39868, 23831); this.Add(39869, 23900); this.Add(39870, 23839);
					this.Add(39871, 23835); this.Add(39872, 23825); this.Add(39873, 23828); this.Add(39874, 23842); this.Add(39875, 23834); this.Add(39876, 23833); this.Add(39877, 23832); this.Add(39878, 23884);
					this.Add(39879, 23890); this.Add(39880, 23886); this.Add(39881, 23883); this.Add(39882, 23916); this.Add(39883, 23923); this.Add(39884, 23926); this.Add(39885, 23943); this.Add(39886, 23940);
					this.Add(39887, 23938); this.Add(39888, 23970); this.Add(39889, 23965); this.Add(39890, 23980); this.Add(39891, 23982); this.Add(39892, 23997); this.Add(39893, 23952); this.Add(39894, 23991);
					this.Add(39895, 23996); this.Add(39896, 24009); this.Add(39897, 24013); this.Add(39898, 24019); this.Add(39899, 24018); this.Add(39900, 24022); this.Add(39901, 24027); this.Add(39902, 24043);
					this.Add(39903, 24050); this.Add(39904, 24053); this.Add(39905, 24075); this.Add(39906, 24090); this.Add(39907, 24089); this.Add(39908, 24081); this.Add(39909, 24091); this.Add(39910, 24118);
					this.Add(39911, 24119); this.Add(39912, 24132); this.Add(39913, 24131); this.Add(39914, 24128); this.Add(39915, 24142); this.Add(39916, 24151); this.Add(39917, 24148); this.Add(39918, 24159);
					this.Add(39919, 24162); this.Add(39920, 24164); this.Add(39921, 24135); this.Add(39922, 24181); this.Add(39923, 24182); this.Add(39924, 24186); this.Add(39925, 40636); this.Add(39926, 24191);
					this.Add(39927, 24224); this.Add(39928, 24257); this.Add(39929, 24258); this.Add(39930, 24264); this.Add(39931, 24272); this.Add(39932, 24271); this.Add(40000, 24278); this.Add(40001, 24291);
					this.Add(40002, 24285); this.Add(40003, 24282); this.Add(40004, 24283); this.Add(40005, 24290); this.Add(40006, 24289); this.Add(40007, 24296); this.Add(40008, 24297); this.Add(40009, 24300);
					this.Add(40010, 24305); this.Add(40011, 24307); this.Add(40012, 24304); this.Add(40013, 24308); this.Add(40014, 24312); this.Add(40015, 24318); this.Add(40016, 24323); this.Add(40017, 24329);
					this.Add(40018, 24413); this.Add(40019, 24412); this.Add(40020, 24331); this.Add(40021, 24337); this.Add(40022, 24342); this.Add(40023, 24361); this.Add(40024, 24365); this.Add(40025, 24376);
					this.Add(40026, 24385); this.Add(40027, 24392); this.Add(40028, 24396); this.Add(40029, 24398); this.Add(40030, 24367); this.Add(40031, 24401); this.Add(40032, 24406); this.Add(40033, 24407);
					this.Add(40034, 24409); this.Add(40035, 24417); this.Add(40036, 24429); this.Add(40037, 24435); this.Add(40038, 24439); this.Add(40039, 24451); this.Add(40040, 24450); this.Add(40041, 24447);
					this.Add(40042, 24458); this.Add(40043, 24456); this.Add(40044, 24465); this.Add(40045, 24455); this.Add(40046, 24478); this.Add(40047, 24473); this.Add(40048, 24472); this.Add(40049, 24480);
					this.Add(40050, 24488); this.Add(40051, 24493); this.Add(40052, 24508); this.Add(40053, 24534); this.Add(40054, 24571); this.Add(40055, 24548); this.Add(40056, 24568); this.Add(40057, 24561);
					this.Add(40058, 24541); this.Add(40059, 24755); this.Add(40060, 24575); this.Add(40061, 24609); this.Add(40062, 24672); this.Add(40064, 24601); this.Add(40065, 24592); this.Add(40066, 24617);
					this.Add(40067, 24590); this.Add(40068, 24625); this.Add(40069, 24603); this.Add(40070, 24597); this.Add(40071, 24619); this.Add(40072, 24614); this.Add(40073, 24591); this.Add(40074, 24634);
					this.Add(40075, 24666); this.Add(40076, 24641); this.Add(40077, 24682); this.Add(40078, 24695); this.Add(40079, 24671); this.Add(40080, 24650); this.Add(40081, 24646); this.Add(40082, 24653);
					this.Add(40083, 24675); this.Add(40084, 24643); this.Add(40085, 24676); this.Add(40086, 24642); this.Add(40087, 24684); this.Add(40088, 24683); this.Add(40089, 24665); this.Add(40090, 24705);
					this.Add(40091, 24717); this.Add(40092, 24807); this.Add(40093, 24707); this.Add(40094, 24730); this.Add(40095, 24708); this.Add(40096, 24731); this.Add(40097, 24726); this.Add(40098, 24727);
					this.Add(40099, 24722); this.Add(40100, 24743); this.Add(40101, 24715); this.Add(40102, 24801); this.Add(40103, 24760); this.Add(40104, 24800); this.Add(40105, 24787); this.Add(40106, 24756);
					this.Add(40107, 24560); this.Add(40108, 24765); this.Add(40109, 24774); this.Add(40110, 24757); this.Add(40111, 24792); this.Add(40112, 24909); this.Add(40113, 24853); this.Add(40114, 24838);
					this.Add(40115, 24822); this.Add(40116, 24823); this.Add(40117, 24832); this.Add(40118, 24820); this.Add(40119, 24826); this.Add(40120, 24835); this.Add(40121, 24865); this.Add(40122, 24827);
					this.Add(40123, 24817); this.Add(40124, 24845); this.Add(40125, 24846); this.Add(40126, 24903); this.Add(40127, 24894); this.Add(40128, 24872); this.Add(40129, 24871); this.Add(40130, 24906);
					this.Add(40131, 24895); this.Add(40132, 24892); this.Add(40133, 24876); this.Add(40134, 24884); this.Add(40135, 24893); this.Add(40136, 24898); this.Add(40137, 24900); this.Add(40138, 24947);
					this.Add(40139, 24951); this.Add(40140, 24920); this.Add(40141, 24921); this.Add(40142, 24922); this.Add(40143, 24939); this.Add(40144, 24948); this.Add(40145, 24943); this.Add(40146, 24933);
					this.Add(40147, 24945); this.Add(40148, 24927); this.Add(40149, 24925); this.Add(40150, 24915); this.Add(40151, 24949); this.Add(40152, 24985); this.Add(40153, 24982); this.Add(40154, 24967);
					this.Add(40155, 25004); this.Add(40156, 24980); this.Add(40157, 24986); this.Add(40158, 24970); this.Add(40159, 24977); this.Add(40160, 25003); this.Add(40161, 25006); this.Add(40162, 25036);
					this.Add(40163, 25034); this.Add(40164, 25033); this.Add(40165, 25079); this.Add(40166, 25032); this.Add(40167, 25027); this.Add(40168, 25030); this.Add(40169, 25018); this.Add(40170, 25035);
					this.Add(40171, 32633); this.Add(40172, 25037); this.Add(40173, 25062); this.Add(40174, 25059); this.Add(40175, 25078); this.Add(40176, 25082); this.Add(40177, 25076); this.Add(40178, 25087);
					this.Add(40179, 25085); this.Add(40180, 25084); this.Add(40181, 25086); this.Add(40182, 25088); this.Add(40183, 25096); this.Add(40184, 25097); this.Add(40185, 25101); this.Add(40186, 25100);
					this.Add(40187, 25108); this.Add(40188, 25115); this.Add(40256, 25118); this.Add(40257, 25121); this.Add(40258, 25130); this.Add(40259, 25134); this.Add(40260, 25136); this.Add(40261, 25138);
					this.Add(40262, 25139); this.Add(40263, 25153); this.Add(40264, 25166); this.Add(40265, 25182); this.Add(40266, 25187); this.Add(40267, 25179); this.Add(40268, 25184); this.Add(40269, 25192);
					this.Add(40270, 25212); this.Add(40271, 25218); this.Add(40272, 25225); this.Add(40273, 25214); this.Add(40274, 25234); this.Add(40275, 25235); this.Add(40276, 25238); this.Add(40277, 25300);
					this.Add(40278, 25219); this.Add(40279, 25236); this.Add(40280, 25303); this.Add(40281, 25297); this.Add(40282, 25275); this.Add(40283, 25295); this.Add(40284, 25343); this.Add(40285, 25286);
					this.Add(40286, 25812); this.Add(40287, 25288); this.Add(40288, 25308); this.Add(40289, 25292); this.Add(40290, 25290); this.Add(40291, 25282); this.Add(40292, 25287); this.Add(40293, 25243);
					this.Add(40294, 25289); this.Add(40295, 25356); this.Add(40296, 25326); this.Add(40297, 25329); this.Add(40298, 25383); this.Add(40299, 25346); this.Add(40300, 25352); this.Add(40301, 25327);
					this.Add(40302, 25333); this.Add(40303, 25424); this.Add(40304, 25406); this.Add(40305, 25421); this.Add(40306, 25628); this.Add(40307, 25423); this.Add(40308, 25494); this.Add(40309, 25486);
					this.Add(40310, 25472); this.Add(40311, 25515); this.Add(40312, 25462); this.Add(40313, 25507); this.Add(40314, 25487); this.Add(40315, 25481); this.Add(40316, 25503); this.Add(40317, 25525);
					this.Add(40318, 25451); this.Add(40320, 25449); this.Add(40321, 25534); this.Add(40322, 25577); this.Add(40323, 25536); this.Add(40324, 25542); this.Add(40325, 25571); this.Add(40326, 25545);
					this.Add(40327, 25554); this.Add(40328, 25590); this.Add(40329, 25540); this.Add(40330, 25622); this.Add(40331, 25652); this.Add(40332, 25606); this.Add(40333, 25619); this.Add(40334, 25638);
					this.Add(40335, 25654); this.Add(40336, 25885); this.Add(40337, 25623); this.Add(40338, 25640); this.Add(40339, 25615); this.Add(40340, 25703); this.Add(40341, 25711); this.Add(40342, 25718);
					this.Add(40343, 25678); this.Add(40344, 25898); this.Add(40345, 25749); this.Add(40346, 25747); this.Add(40347, 25765); this.Add(40348, 25769); this.Add(40349, 25736); this.Add(40350, 25788);
					this.Add(40351, 25818); this.Add(40352, 25810); this.Add(40353, 25797); this.Add(40354, 25799); this.Add(40355, 25787); this.Add(40356, 25816); this.Add(40357, 25794); this.Add(40358, 25841);
					this.Add(40359, 25831); this.Add(40360, 33289); this.Add(40361, 25824); this.Add(40362, 25825); this.Add(40363, 25260); this.Add(40364, 25827); this.Add(40365, 25839); this.Add(40366, 25900);
					this.Add(40367, 25846); this.Add(40368, 25844); this.Add(40369, 25842); this.Add(40370, 25850); this.Add(40371, 25856); this.Add(40372, 25853); this.Add(40373, 25880); this.Add(40374, 25884);
					this.Add(40375, 25861); this.Add(40376, 25892); this.Add(40377, 25891); this.Add(40378, 25899); this.Add(40379, 25908); this.Add(40380, 25909); this.Add(40381, 25911); this.Add(40382, 25910);
					this.Add(40383, 25912); this.Add(40384, 30027); this.Add(40385, 25928); this.Add(40386, 25942); this.Add(40387, 25941); this.Add(40388, 25933); this.Add(40389, 25944); this.Add(40390, 25950);
					this.Add(40391, 25949); this.Add(40392, 25970); this.Add(40393, 25976); this.Add(40394, 25986); this.Add(40395, 25987); this.Add(40396, 35722); this.Add(40397, 26011); this.Add(40398, 26015);
					this.Add(40399, 26027); this.Add(40400, 26039); this.Add(40401, 26051); this.Add(40402, 26054); this.Add(40403, 26049); this.Add(40404, 26052); this.Add(40405, 26060); this.Add(40406, 26066);
					this.Add(40407, 26075); this.Add(40408, 26073); this.Add(40409, 26080); this.Add(40410, 26081); this.Add(40411, 26097); this.Add(40412, 26482); this.Add(40413, 26122); this.Add(40414, 26115);
					this.Add(40415, 26107); this.Add(40416, 26483); this.Add(40417, 26165); this.Add(40418, 26166); this.Add(40419, 26164); this.Add(40420, 26140); this.Add(40421, 26191); this.Add(40422, 26180);
					this.Add(40423, 26185); this.Add(40424, 26177); this.Add(40425, 26206); this.Add(40426, 26205); this.Add(40427, 26212); this.Add(40428, 26215); this.Add(40429, 26216); this.Add(40430, 26207);
					this.Add(40431, 26210); this.Add(40432, 26224); this.Add(40433, 26243); this.Add(40434, 26248); this.Add(40435, 26254); this.Add(40436, 26249); this.Add(40437, 26244); this.Add(40438, 26264);
					this.Add(40439, 26269); this.Add(40440, 26305); this.Add(40441, 26297); this.Add(40442, 26313); this.Add(40443, 26302); this.Add(40444, 26300); this.Add(40512, 26308); this.Add(40513, 26296);
					this.Add(40514, 26326); this.Add(40515, 26330); this.Add(40516, 26336); this.Add(40517, 26175); this.Add(40518, 26342); this.Add(40519, 26345); this.Add(40520, 26352); this.Add(40521, 26357);
					this.Add(40522, 26359); this.Add(40523, 26383); this.Add(40524, 26390); this.Add(40525, 26398); this.Add(40526, 26406); this.Add(40527, 26407); this.Add(40528, 38712); this.Add(40529, 26414);
					this.Add(40530, 26431); this.Add(40531, 26422); this.Add(40532, 26433); this.Add(40533, 26424); this.Add(40534, 26423); this.Add(40535, 26438); this.Add(40536, 26462); this.Add(40537, 26464);
					this.Add(40538, 26457); this.Add(40539, 26467); this.Add(40540, 26468); this.Add(40541, 26505); this.Add(40542, 26480); this.Add(40543, 26537); this.Add(40544, 26492); this.Add(40545, 26474);
					this.Add(40546, 26508); this.Add(40547, 26507); this.Add(40548, 26534); this.Add(40549, 26529); this.Add(40550, 26501); this.Add(40551, 26551); this.Add(40552, 26607); this.Add(40553, 26548);
					this.Add(40554, 26604); this.Add(40555, 26547); this.Add(40556, 26601); this.Add(40557, 26552); this.Add(40558, 26596); this.Add(40559, 26590); this.Add(40560, 26589); this.Add(40561, 26594);
					this.Add(40562, 26606); this.Add(40563, 26553); this.Add(40564, 26574); this.Add(40565, 26566); this.Add(40566, 26599); this.Add(40567, 27292); this.Add(40568, 26654); this.Add(40569, 26694);
					this.Add(40570, 26665); this.Add(40571, 26688); this.Add(40572, 26701); this.Add(40573, 26674); this.Add(40574, 26702); this.Add(40576, 26803); this.Add(40577, 26667); this.Add(40578, 26713);
					this.Add(40579, 26723); this.Add(40580, 26743); this.Add(40581, 26751); this.Add(40582, 26783); this.Add(40583, 26767); this.Add(40584, 26797); this.Add(40585, 26772); this.Add(40586, 26781);
					this.Add(40587, 26779); this.Add(40588, 26755); this.Add(40589, 27310); this.Add(40590, 26809); this.Add(40591, 26740); this.Add(40592, 26805); this.Add(40593, 26784); this.Add(40594, 26810);
					this.Add(40595, 26895); this.Add(40596, 26765); this.Add(40597, 26750); this.Add(40598, 26881); this.Add(40599, 26826); this.Add(40600, 26888); this.Add(40601, 26840); this.Add(40602, 26914);
					this.Add(40603, 26918); this.Add(40604, 26849); this.Add(40605, 26892); this.Add(40606, 26829); this.Add(40607, 26836); this.Add(40608, 26855); this.Add(40609, 26837); this.Add(40610, 26934);
					this.Add(40611, 26898); this.Add(40612, 26884); this.Add(40613, 26839); this.Add(40614, 26851); this.Add(40615, 26917); this.Add(40616, 26873); this.Add(40617, 26848); this.Add(40618, 26863);
					this.Add(40619, 26920); this.Add(40620, 26922); this.Add(40621, 26906); this.Add(40622, 26915); this.Add(40623, 26913); this.Add(40624, 26822); this.Add(40625, 27001); this.Add(40626, 26999);
					this.Add(40627, 26972); this.Add(40628, 27000); this.Add(40629, 26987); this.Add(40630, 26964); this.Add(40631, 27006); this.Add(40632, 26990); this.Add(40633, 26937); this.Add(40634, 26996);
					this.Add(40635, 26941); this.Add(40636, 26969); this.Add(40637, 26928); this.Add(40638, 26977); this.Add(40639, 26974); this.Add(40640, 26973); this.Add(40641, 27009); this.Add(40642, 26986);
					this.Add(40643, 27058); this.Add(40644, 27054); this.Add(40645, 27088); this.Add(40646, 27071); this.Add(40647, 27073); this.Add(40648, 27091); this.Add(40649, 27070); this.Add(40650, 27086);
					this.Add(40651, 23528); this.Add(40652, 27082); this.Add(40653, 27101); this.Add(40654, 27067); this.Add(40655, 27075); this.Add(40656, 27047); this.Add(40657, 27182); this.Add(40658, 27025);
					this.Add(40659, 27040); this.Add(40660, 27036); this.Add(40661, 27029); this.Add(40662, 27060); this.Add(40663, 27102); this.Add(40664, 27112); this.Add(40665, 27138); this.Add(40666, 27163);
					this.Add(40667, 27135); this.Add(40668, 27402); this.Add(40669, 27129); this.Add(40670, 27122); this.Add(40671, 27111); this.Add(40672, 27141); this.Add(40673, 27057); this.Add(40674, 27166);
					this.Add(40675, 27117); this.Add(40676, 27156); this.Add(40677, 27115); this.Add(40678, 27146); this.Add(40679, 27154); this.Add(40680, 27329); this.Add(40681, 27171); this.Add(40682, 27155);
					this.Add(40683, 27204); this.Add(40684, 27148); this.Add(40685, 27250); this.Add(40686, 27190); this.Add(40687, 27256); this.Add(40688, 27207); this.Add(40689, 27234); this.Add(40690, 27225);
					this.Add(40691, 27238); this.Add(40692, 27208); this.Add(40693, 27192); this.Add(40694, 27170); this.Add(40695, 27280); this.Add(40696, 27277); this.Add(40697, 27296); this.Add(40698, 27268);
					this.Add(40699, 27298); this.Add(40700, 27299); this.Add(40768, 27287); this.Add(40769, 34327); this.Add(40770, 27323); this.Add(40771, 27331); this.Add(40772, 27330); this.Add(40773, 27320);
					this.Add(40774, 27315); this.Add(40775, 27308); this.Add(40776, 27358); this.Add(40777, 27345); this.Add(40778, 27359); this.Add(40779, 27306); this.Add(40780, 27354); this.Add(40781, 27370);
					this.Add(40782, 27387); this.Add(40783, 27397); this.Add(40784, 34326); this.Add(40785, 27386); this.Add(40786, 27410); this.Add(40787, 27414); this.Add(40788, 39729); this.Add(40789, 27423);
					this.Add(40790, 27448); this.Add(40791, 27447); this.Add(40792, 30428); this.Add(40793, 27449); this.Add(40794, 39150); this.Add(40795, 27463); this.Add(40796, 27459); this.Add(40797, 27465);
					this.Add(40798, 27472); this.Add(40799, 27481); this.Add(40800, 27476); this.Add(40801, 27483); this.Add(40802, 27487); this.Add(40803, 27489); this.Add(40804, 27512); this.Add(40805, 27513);
					this.Add(40806, 27519); this.Add(40807, 27520); this.Add(40808, 27524); this.Add(40809, 27523); this.Add(40810, 27533); this.Add(40811, 27544); this.Add(40812, 27541); this.Add(40813, 27550);
					this.Add(40814, 27556); this.Add(40815, 27562); this.Add(40816, 27563); this.Add(40817, 27567); this.Add(40818, 27570); this.Add(40819, 27569); this.Add(40820, 27571); this.Add(40821, 27575);
					this.Add(40822, 27580); this.Add(40823, 27590); this.Add(40824, 27595); this.Add(40825, 27603); this.Add(40826, 27615); this.Add(40827, 27628); this.Add(40828, 27627); this.Add(40829, 27635);
					this.Add(40830, 27631); this.Add(40832, 40638); this.Add(40833, 27656); this.Add(40834, 27667); this.Add(40835, 27668); this.Add(40836, 27675); this.Add(40837, 27684); this.Add(40838, 27683);
					this.Add(40839, 27742); this.Add(40840, 27733); this.Add(40841, 27746); this.Add(40842, 27754); this.Add(40843, 27778); this.Add(40844, 27789); this.Add(40845, 27802); this.Add(40846, 27777);
					this.Add(40847, 27803); this.Add(40848, 27774); this.Add(40849, 27752); this.Add(40850, 27763); this.Add(40851, 27794); this.Add(40852, 27792); this.Add(40853, 27844); this.Add(40854, 27889);
					this.Add(40855, 27859); this.Add(40856, 27837); this.Add(40857, 27863); this.Add(40858, 27845); this.Add(40859, 27869); this.Add(40860, 27822); this.Add(40861, 27825); this.Add(40862, 27838);
					this.Add(40863, 27834); this.Add(40864, 27867); this.Add(40865, 27887); this.Add(40866, 27865); this.Add(40867, 27882); this.Add(40868, 27935); this.Add(40869, 34893); this.Add(40870, 27958);
					this.Add(40871, 27947); this.Add(40872, 27965); this.Add(40873, 27960); this.Add(40874, 27929); this.Add(40875, 27957); this.Add(40876, 27955); this.Add(40877, 27922); this.Add(40878, 27916);
					this.Add(40879, 28003); this.Add(40880, 28051); this.Add(40881, 28004); this.Add(40882, 27994); this.Add(40883, 28025); this.Add(40884, 27993); this.Add(40885, 28046); this.Add(40886, 28053);
					this.Add(40887, 28644); this.Add(40888, 28037); this.Add(40889, 28153); this.Add(40890, 28181); this.Add(40891, 28170); this.Add(40892, 28085); this.Add(40893, 28103); this.Add(40894, 28134);
					this.Add(40895, 28088); this.Add(40896, 28102); this.Add(40897, 28140); this.Add(40898, 28126); this.Add(40899, 28108); this.Add(40900, 28136); this.Add(40901, 28114); this.Add(40902, 28101);
					this.Add(40903, 28154); this.Add(40904, 28121); this.Add(40905, 28132); this.Add(40906, 28117); this.Add(40907, 28138); this.Add(40908, 28142); this.Add(40909, 28205); this.Add(40910, 28270);
					this.Add(40911, 28206); this.Add(40912, 28185); this.Add(40913, 28274); this.Add(40914, 28255); this.Add(40915, 28222); this.Add(40916, 28195); this.Add(40917, 28267); this.Add(40918, 28203);
					this.Add(40919, 28278); this.Add(40920, 28237); this.Add(40921, 28191); this.Add(40922, 28227); this.Add(40923, 28218); this.Add(40924, 28238); this.Add(40925, 28196); this.Add(40926, 28415);
					this.Add(40927, 28189); this.Add(40928, 28216); this.Add(40929, 28290); this.Add(40930, 28330); this.Add(40931, 28312); this.Add(40932, 28361); this.Add(40933, 28343); this.Add(40934, 28371);
					this.Add(40935, 28349); this.Add(40936, 28335); this.Add(40937, 28356); this.Add(40938, 28338); this.Add(40939, 28372); this.Add(40940, 28373); this.Add(40941, 28303); this.Add(40942, 28325);
					this.Add(40943, 28354); this.Add(40944, 28319); this.Add(40945, 28481); this.Add(40946, 28433); this.Add(40947, 28748); this.Add(40948, 28396); this.Add(40949, 28408); this.Add(40950, 28414);
					this.Add(40951, 28479); this.Add(40952, 28402); this.Add(40953, 28465); this.Add(40954, 28399); this.Add(40955, 28466); this.Add(40956, 28364); this.Add(57408, 28478); this.Add(57409, 28435);
					this.Add(57410, 28407); this.Add(57411, 28550); this.Add(57412, 28538); this.Add(57413, 28536); this.Add(57414, 28545); this.Add(57415, 28544); this.Add(57416, 28527); this.Add(57417, 28507);
					this.Add(57418, 28659); this.Add(57419, 28525); this.Add(57420, 28546); this.Add(57421, 28540); this.Add(57422, 28504); this.Add(57423, 28558); this.Add(57424, 28561); this.Add(57425, 28610);
					this.Add(57426, 28518); this.Add(57427, 28595); this.Add(57428, 28579); this.Add(57429, 28577); this.Add(57430, 28580); this.Add(57431, 28601); this.Add(57432, 28614); this.Add(57433, 28586);
					this.Add(57434, 28639); this.Add(57435, 28629); this.Add(57436, 28652); this.Add(57437, 28628); this.Add(57438, 28632); this.Add(57439, 28657); this.Add(57440, 28654); this.Add(57441, 28635);
					this.Add(57442, 28681); this.Add(57443, 28683); this.Add(57444, 28666); this.Add(57445, 28689); this.Add(57446, 28673); this.Add(57447, 28687); this.Add(57448, 28670); this.Add(57449, 28699);
					this.Add(57450, 28698); this.Add(57451, 28532); this.Add(57452, 28701); this.Add(57453, 28696); this.Add(57454, 28703); this.Add(57455, 28720); this.Add(57456, 28734); this.Add(57457, 28722);
					this.Add(57458, 28753); this.Add(57459, 28771); this.Add(57460, 28825); this.Add(57461, 28818); this.Add(57462, 28847); this.Add(57463, 28913); this.Add(57464, 28844); this.Add(57465, 28856);
					this.Add(57466, 28851); this.Add(57467, 28846); this.Add(57468, 28895); this.Add(57469, 28875); this.Add(57470, 28893); this.Add(57472, 28889); this.Add(57473, 28937); this.Add(57474, 28925);
					this.Add(57475, 28956); this.Add(57476, 28953); this.Add(57477, 29029); this.Add(57478, 29013); this.Add(57479, 29064); this.Add(57480, 29030); this.Add(57481, 29026); this.Add(57482, 29004);
					this.Add(57483, 29014); this.Add(57484, 29036); this.Add(57485, 29071); this.Add(57486, 29179); this.Add(57487, 29060); this.Add(57488, 29077); this.Add(57489, 29096); this.Add(57490, 29100);
					this.Add(57491, 29143); this.Add(57492, 29113); this.Add(57493, 29118); this.Add(57494, 29138); this.Add(57495, 29129); this.Add(57496, 29140); this.Add(57497, 29134); this.Add(57498, 29152);
					this.Add(57499, 29164); this.Add(57500, 29159); this.Add(57501, 29173); this.Add(57502, 29180); this.Add(57503, 29177); this.Add(57504, 29183); this.Add(57505, 29197); this.Add(57506, 29200);
					this.Add(57507, 29211); this.Add(57508, 29224); this.Add(57509, 29229); this.Add(57510, 29228); this.Add(57511, 29232); this.Add(57512, 29234); this.Add(57513, 29243); this.Add(57514, 29244);
					this.Add(57515, 29247); this.Add(57516, 29248); this.Add(57517, 29254); this.Add(57518, 29259); this.Add(57519, 29272); this.Add(57520, 29300); this.Add(57521, 29310); this.Add(57522, 29314);
					this.Add(57523, 29313); this.Add(57524, 29319); this.Add(57525, 29330); this.Add(57526, 29334); this.Add(57527, 29346); this.Add(57528, 29351); this.Add(57529, 29369); this.Add(57530, 29362);
					this.Add(57531, 29379); this.Add(57532, 29382); this.Add(57533, 29380); this.Add(57534, 29390); this.Add(57535, 29394); this.Add(57536, 29410); this.Add(57537, 29408); this.Add(57538, 29409);
					this.Add(57539, 29433); this.Add(57540, 29431); this.Add(57541, 20495); this.Add(57542, 29463); this.Add(57543, 29450); this.Add(57544, 29468); this.Add(57545, 29462); this.Add(57546, 29469);
					this.Add(57547, 29492); this.Add(57548, 29487); this.Add(57549, 29481); this.Add(57550, 29477); this.Add(57551, 29502); this.Add(57552, 29518); this.Add(57553, 29519); this.Add(57554, 40664);
					this.Add(57555, 29527); this.Add(57556, 29546); this.Add(57557, 29544); this.Add(57558, 29552); this.Add(57559, 29560); this.Add(57560, 29557); this.Add(57561, 29563); this.Add(57562, 29562);
					this.Add(57563, 29640); this.Add(57564, 29619); this.Add(57565, 29646); this.Add(57566, 29627); this.Add(57567, 29632); this.Add(57568, 29669); this.Add(57569, 29678); this.Add(57570, 29662);
					this.Add(57571, 29858); this.Add(57572, 29701); this.Add(57573, 29807); this.Add(57574, 29733); this.Add(57575, 29688); this.Add(57576, 29746); this.Add(57577, 29754); this.Add(57578, 29781);
					this.Add(57579, 29759); this.Add(57580, 29791); this.Add(57581, 29785); this.Add(57582, 29761); this.Add(57583, 29788); this.Add(57584, 29801); this.Add(57585, 29808); this.Add(57586, 29795);
					this.Add(57587, 29802); this.Add(57588, 29814); this.Add(57589, 29822); this.Add(57590, 29835); this.Add(57591, 29854); this.Add(57592, 29863); this.Add(57593, 29898); this.Add(57594, 29903);
					this.Add(57595, 29908); this.Add(57596, 29681); this.Add(57664, 29920); this.Add(57665, 29923); this.Add(57666, 29927); this.Add(57667, 29929); this.Add(57668, 29934); this.Add(57669, 29938);
					this.Add(57670, 29936); this.Add(57671, 29937); this.Add(57672, 29944); this.Add(57673, 29943); this.Add(57674, 29956); this.Add(57675, 29955); this.Add(57676, 29957); this.Add(57677, 29964);
					this.Add(57678, 29966); this.Add(57679, 29965); this.Add(57680, 29973); this.Add(57681, 29971); this.Add(57682, 29982); this.Add(57683, 29990); this.Add(57684, 29996); this.Add(57685, 30012);
					this.Add(57686, 30020); this.Add(57687, 30029); this.Add(57688, 30026); this.Add(57689, 30025); this.Add(57690, 30043); this.Add(57691, 30022); this.Add(57692, 30042); this.Add(57693, 30057);
					this.Add(57694, 30052); this.Add(57695, 30055); this.Add(57696, 30059); this.Add(57697, 30061); this.Add(57698, 30072); this.Add(57699, 30070); this.Add(57700, 30086); this.Add(57701, 30087);
					this.Add(57702, 30068); this.Add(57703, 30090); this.Add(57704, 30089); this.Add(57705, 30082); this.Add(57706, 30100); this.Add(57707, 30106); this.Add(57708, 30109); this.Add(57709, 30117);
					this.Add(57710, 30115); this.Add(57711, 30146); this.Add(57712, 30131); this.Add(57713, 30147); this.Add(57714, 30133); this.Add(57715, 30141); this.Add(57716, 30136); this.Add(57717, 30140);
					this.Add(57718, 30129); this.Add(57719, 30157); this.Add(57720, 30154); this.Add(57721, 30162); this.Add(57722, 30169); this.Add(57723, 30179); this.Add(57724, 30174); this.Add(57725, 30206);
					this.Add(57726, 30207); this.Add(57728, 30204); this.Add(57729, 30209); this.Add(57730, 30192); this.Add(57731, 30202); this.Add(57732, 30194); this.Add(57733, 30195); this.Add(57734, 30219);
					this.Add(57735, 30221); this.Add(57736, 30217); this.Add(57737, 30239); this.Add(57738, 30247); this.Add(57739, 30240); this.Add(57740, 30241); this.Add(57741, 30242); this.Add(57742, 30244);
					this.Add(57743, 30260); this.Add(57744, 30256); this.Add(57745, 30267); this.Add(57746, 30279); this.Add(57747, 30280); this.Add(57748, 30278); this.Add(57749, 30300); this.Add(57750, 30296);
					this.Add(57751, 30305); this.Add(57752, 30306); this.Add(57753, 30312); this.Add(57754, 30313); this.Add(57755, 30314); this.Add(57756, 30311); this.Add(57757, 30316); this.Add(57758, 30320);
					this.Add(57759, 30322); this.Add(57760, 30326); this.Add(57761, 30328); this.Add(57762, 30332); this.Add(57763, 30336); this.Add(57764, 30339); this.Add(57765, 30344); this.Add(57766, 30347);
					this.Add(57767, 30350); this.Add(57768, 30358); this.Add(57769, 30355); this.Add(57770, 30361); this.Add(57771, 30362); this.Add(57772, 30384); this.Add(57773, 30388); this.Add(57774, 30392);
					this.Add(57775, 30393); this.Add(57776, 30394); this.Add(57777, 30402); this.Add(57778, 30413); this.Add(57779, 30422); this.Add(57780, 30418); this.Add(57781, 30430); this.Add(57782, 30433);
					this.Add(57783, 30437); this.Add(57784, 30439); this.Add(57785, 30442); this.Add(57786, 34351); this.Add(57787, 30459); this.Add(57788, 30472); this.Add(57789, 30471); this.Add(57790, 30468);
					this.Add(57791, 30505); this.Add(57792, 30500); this.Add(57793, 30494); this.Add(57794, 30501); this.Add(57795, 30502); this.Add(57796, 30491); this.Add(57797, 30519); this.Add(57798, 30520);
					this.Add(57799, 30535); this.Add(57800, 30554); this.Add(57801, 30568); this.Add(57802, 30571); this.Add(57803, 30555); this.Add(57804, 30565); this.Add(57805, 30591); this.Add(57806, 30590);
					this.Add(57807, 30585); this.Add(57808, 30606); this.Add(57809, 30603); this.Add(57810, 30609); this.Add(57811, 30624); this.Add(57812, 30622); this.Add(57813, 30640); this.Add(57814, 30646);
					this.Add(57815, 30649); this.Add(57816, 30655); this.Add(57817, 30652); this.Add(57818, 30653); this.Add(57819, 30651); this.Add(57820, 30663); this.Add(57821, 30669); this.Add(57822, 30679);
					this.Add(57823, 30682); this.Add(57824, 30684); this.Add(57825, 30691); this.Add(57826, 30702); this.Add(57827, 30716); this.Add(57828, 30732); this.Add(57829, 30738); this.Add(57830, 31014);
					this.Add(57831, 30752); this.Add(57832, 31018); this.Add(57833, 30789); this.Add(57834, 30862); this.Add(57835, 30836); this.Add(57836, 30854); this.Add(57837, 30844); this.Add(57838, 30874);
					this.Add(57839, 30860); this.Add(57840, 30883); this.Add(57841, 30901); this.Add(57842, 30890); this.Add(57843, 30895); this.Add(57844, 30929); this.Add(57845, 30918); this.Add(57846, 30923);
					this.Add(57847, 30932); this.Add(57848, 30910); this.Add(57849, 30908); this.Add(57850, 30917); this.Add(57851, 30922); this.Add(57852, 30956); this.Add(57920, 30951); this.Add(57921, 30938);
					this.Add(57922, 30973); this.Add(57923, 30964); this.Add(57924, 30983); this.Add(57925, 30994); this.Add(57926, 30993); this.Add(57927, 31001); this.Add(57928, 31020); this.Add(57929, 31019);
					this.Add(57930, 31040); this.Add(57931, 31072); this.Add(57932, 31063); this.Add(57933, 31071); this.Add(57934, 31066); this.Add(57935, 31061); this.Add(57936, 31059); this.Add(57937, 31098);
					this.Add(57938, 31103); this.Add(57939, 31114); this.Add(57940, 31133); this.Add(57941, 31143); this.Add(57942, 40779); this.Add(57943, 31146); this.Add(57944, 31150); this.Add(57945, 31155);
					this.Add(57946, 31161); this.Add(57947, 31162); this.Add(57948, 31177); this.Add(57949, 31189); this.Add(57950, 31207); this.Add(57951, 31212); this.Add(57952, 31201); this.Add(57953, 31203);
					this.Add(57954, 31240); this.Add(57955, 31245); this.Add(57956, 31256); this.Add(57957, 31257); this.Add(57958, 31264); this.Add(57959, 31263); this.Add(57960, 31104); this.Add(57961, 31281);
					this.Add(57962, 31291); this.Add(57963, 31294); this.Add(57964, 31287); this.Add(57965, 31299); this.Add(57966, 31319); this.Add(57967, 31305); this.Add(57968, 31329); this.Add(57969, 31330);
					this.Add(57970, 31337); this.Add(57971, 40861); this.Add(57972, 31344); this.Add(57973, 31353); this.Add(57974, 31357); this.Add(57975, 31368); this.Add(57976, 31383); this.Add(57977, 31381);
					this.Add(57978, 31384); this.Add(57979, 31382); this.Add(57980, 31401); this.Add(57981, 31432); this.Add(57982, 31408); this.Add(57984, 31414); this.Add(57985, 31429); this.Add(57986, 31428);
					this.Add(57987, 31423); this.Add(57988, 36995); this.Add(57989, 31431); this.Add(57990, 31434); this.Add(57991, 31437); this.Add(57992, 31439); this.Add(57993, 31445); this.Add(57994, 31443);
					this.Add(57995, 31449); this.Add(57996, 31450); this.Add(57997, 31453); this.Add(57998, 31457); this.Add(57999, 31458); this.Add(58000, 31462); this.Add(58001, 31469); this.Add(58002, 31472);
					this.Add(58003, 31490); this.Add(58004, 31503); this.Add(58005, 31498); this.Add(58006, 31494); this.Add(58007, 31539); this.Add(58008, 31512); this.Add(58009, 31513); this.Add(58010, 31518);
					this.Add(58011, 31541); this.Add(58012, 31528); this.Add(58013, 31542); this.Add(58014, 31568); this.Add(58015, 31610); this.Add(58016, 31492); this.Add(58017, 31565); this.Add(58018, 31499);
					this.Add(58019, 31564); this.Add(58020, 31557); this.Add(58021, 31605); this.Add(58022, 31589); this.Add(58023, 31604); this.Add(58024, 31591); this.Add(58025, 31600); this.Add(58026, 31601);
					this.Add(58027, 31596); this.Add(58028, 31598); this.Add(58029, 31645); this.Add(58030, 31640); this.Add(58031, 31647); this.Add(58032, 31629); this.Add(58033, 31644); this.Add(58034, 31642);
					this.Add(58035, 31627); this.Add(58036, 31634); this.Add(58037, 31631); this.Add(58038, 31581); this.Add(58039, 31641); this.Add(58040, 31691); this.Add(58041, 31681); this.Add(58042, 31692);
					this.Add(58043, 31695); this.Add(58044, 31668); this.Add(58045, 31686); this.Add(58046, 31709); this.Add(58047, 31721); this.Add(58048, 31761); this.Add(58049, 31764); this.Add(58050, 31718);
					this.Add(58051, 31717); this.Add(58052, 31840); this.Add(58053, 31744); this.Add(58054, 31751); this.Add(58055, 31763); this.Add(58056, 31731); this.Add(58057, 31735); this.Add(58058, 31767);
					this.Add(58059, 31757); this.Add(58060, 31734); this.Add(58061, 31779); this.Add(58062, 31783); this.Add(58063, 31786); this.Add(58064, 31775); this.Add(58065, 31799); this.Add(58066, 31787);
					this.Add(58067, 31805); this.Add(58068, 31820); this.Add(58069, 31811); this.Add(58070, 31828); this.Add(58071, 31823); this.Add(58072, 31808); this.Add(58073, 31824); this.Add(58074, 31832);
					this.Add(58075, 31839); this.Add(58076, 31844); this.Add(58077, 31830); this.Add(58078, 31845); this.Add(58079, 31852); this.Add(58080, 31861); this.Add(58081, 31875); this.Add(58082, 31888);
					this.Add(58083, 31908); this.Add(58084, 31917); this.Add(58085, 31906); this.Add(58086, 31915); this.Add(58087, 31905); this.Add(58088, 31912); this.Add(58089, 31923); this.Add(58090, 31922);
					this.Add(58091, 31921); this.Add(58092, 31918); this.Add(58093, 31929); this.Add(58094, 31933); this.Add(58095, 31936); this.Add(58096, 31941); this.Add(58097, 31938); this.Add(58098, 31960);
					this.Add(58099, 31954); this.Add(58100, 31964); this.Add(58101, 31970); this.Add(58102, 39739); this.Add(58103, 31983); this.Add(58104, 31986); this.Add(58105, 31988); this.Add(58106, 31990);
					this.Add(58107, 31994); this.Add(58108, 32006); this.Add(58176, 32002); this.Add(58177, 32028); this.Add(58178, 32021); this.Add(58179, 32010); this.Add(58180, 32069); this.Add(58181, 32075);
					this.Add(58182, 32046); this.Add(58183, 32050); this.Add(58184, 32063); this.Add(58185, 32053); this.Add(58186, 32070); this.Add(58187, 32115); this.Add(58188, 32086); this.Add(58189, 32078);
					this.Add(58190, 32114); this.Add(58191, 32104); this.Add(58192, 32110); this.Add(58193, 32079); this.Add(58194, 32099); this.Add(58195, 32147); this.Add(58196, 32137); this.Add(58197, 32091);
					this.Add(58198, 32143); this.Add(58199, 32125); this.Add(58200, 32155); this.Add(58201, 32186); this.Add(58202, 32174); this.Add(58203, 32163); this.Add(58204, 32181); this.Add(58205, 32199);
					this.Add(58206, 32189); this.Add(58207, 32171); this.Add(58208, 32317); this.Add(58209, 32162); this.Add(58210, 32175); this.Add(58211, 32220); this.Add(58212, 32184); this.Add(58213, 32159);
					this.Add(58214, 32176); this.Add(58215, 32216); this.Add(58216, 32221); this.Add(58217, 32228); this.Add(58218, 32222); this.Add(58219, 32251); this.Add(58220, 32242); this.Add(58221, 32225);
					this.Add(58222, 32261); this.Add(58223, 32266); this.Add(58224, 32291); this.Add(58225, 32289); this.Add(58226, 32274); this.Add(58227, 32305); this.Add(58228, 32287); this.Add(58229, 32265);
					this.Add(58230, 32267); this.Add(58231, 32290); this.Add(58232, 32326); this.Add(58233, 32358); this.Add(58234, 32315); this.Add(58235, 32309); this.Add(58236, 32313); this.Add(58237, 32323);
					this.Add(58238, 32311); this.Add(58240, 32306); this.Add(58241, 32314); this.Add(58242, 32359); this.Add(58243, 32349); this.Add(58244, 32342); this.Add(58245, 32350); this.Add(58246, 32345);
					this.Add(58247, 32346); this.Add(58248, 32377); this.Add(58249, 32362); this.Add(58250, 32361); this.Add(58251, 32380); this.Add(58252, 32379); this.Add(58253, 32387); this.Add(58254, 32213);
					this.Add(58255, 32381); this.Add(58256, 36782); this.Add(58257, 32383); this.Add(58258, 32392); this.Add(58259, 32393); this.Add(58260, 32396); this.Add(58261, 32402); this.Add(58262, 32400);
					this.Add(58263, 32403); this.Add(58264, 32404); this.Add(58265, 32406); this.Add(58266, 32398); this.Add(58267, 32411); this.Add(58268, 32412); this.Add(58269, 32568); this.Add(58270, 32570);
					this.Add(58271, 32581); this.Add(58272, 32588); this.Add(58273, 32589); this.Add(58274, 32590); this.Add(58275, 32592); this.Add(58276, 32593); this.Add(58277, 32597); this.Add(58278, 32596);
					this.Add(58279, 32600); this.Add(58280, 32607); this.Add(58281, 32608); this.Add(58282, 32616); this.Add(58283, 32617); this.Add(58284, 32615); this.Add(58285, 32632); this.Add(58286, 32642);
					this.Add(58287, 32646); this.Add(58288, 32643); this.Add(58289, 32648); this.Add(58290, 32647); this.Add(58291, 32652); this.Add(58292, 32660); this.Add(58293, 32670); this.Add(58294, 32669);
					this.Add(58295, 32666); this.Add(58296, 32675); this.Add(58297, 32687); this.Add(58298, 32690); this.Add(58299, 32697); this.Add(58300, 32686); this.Add(58301, 32694); this.Add(58302, 32696);
					this.Add(58303, 35697); this.Add(58304, 32709); this.Add(58305, 32710); this.Add(58306, 32714); this.Add(58307, 32725); this.Add(58308, 32724); this.Add(58309, 32737); this.Add(58310, 32742);
					this.Add(58311, 32745); this.Add(58312, 32755); this.Add(58313, 32761); this.Add(58314, 39132); this.Add(58315, 32774); this.Add(58316, 32772); this.Add(58317, 32779); this.Add(58318, 32786);
					this.Add(58319, 32792); this.Add(58320, 32793); this.Add(58321, 32796); this.Add(58322, 32801); this.Add(58323, 32808); this.Add(58324, 32831); this.Add(58325, 32827); this.Add(58326, 32842);
					this.Add(58327, 32838); this.Add(58328, 32850); this.Add(58329, 32856); this.Add(58330, 32858); this.Add(58331, 32863); this.Add(58332, 32866); this.Add(58333, 32872); this.Add(58334, 32883);
					this.Add(58335, 32882); this.Add(58336, 32880); this.Add(58337, 32886); this.Add(58338, 32889); this.Add(58339, 32893); this.Add(58340, 32895); this.Add(58341, 32900); this.Add(58342, 32902);
					this.Add(58343, 32901); this.Add(58344, 32923); this.Add(58345, 32915); this.Add(58346, 32922); this.Add(58347, 32941); this.Add(58348, 20880); this.Add(58349, 32940); this.Add(58350, 32987);
					this.Add(58351, 32997); this.Add(58352, 32985); this.Add(58353, 32989); this.Add(58354, 32964); this.Add(58355, 32986); this.Add(58356, 32982); this.Add(58357, 33033); this.Add(58358, 33007);
					this.Add(58359, 33009); this.Add(58360, 33051); this.Add(58361, 33065); this.Add(58362, 33059); this.Add(58363, 33071); this.Add(58364, 33099); this.Add(58432, 38539); this.Add(58433, 33094);
					this.Add(58434, 33086); this.Add(58435, 33107); this.Add(58436, 33105); this.Add(58437, 33020); this.Add(58438, 33137); this.Add(58439, 33134); this.Add(58440, 33125); this.Add(58441, 33126);
					this.Add(58442, 33140); this.Add(58443, 33155); this.Add(58444, 33160); this.Add(58445, 33162); this.Add(58446, 33152); this.Add(58447, 33154); this.Add(58448, 33184); this.Add(58449, 33173);
					this.Add(58450, 33188); this.Add(58451, 33187); this.Add(58452, 33119); this.Add(58453, 33171); this.Add(58454, 33193); this.Add(58455, 33200); this.Add(58456, 33205); this.Add(58457, 33214);
					this.Add(58458, 33208); this.Add(58459, 33213); this.Add(58460, 33216); this.Add(58461, 33218); this.Add(58462, 33210); this.Add(58463, 33225); this.Add(58464, 33229); this.Add(58465, 33233);
					this.Add(58466, 33241); this.Add(58467, 33240); this.Add(58468, 33224); this.Add(58469, 33242); this.Add(58470, 33247); this.Add(58471, 33248); this.Add(58472, 33255); this.Add(58473, 33274);
					this.Add(58474, 33275); this.Add(58475, 33278); this.Add(58476, 33281); this.Add(58477, 33282); this.Add(58478, 33285); this.Add(58479, 33287); this.Add(58480, 33290); this.Add(58481, 33293);
					this.Add(58482, 33296); this.Add(58483, 33302); this.Add(58484, 33321); this.Add(58485, 33323); this.Add(58486, 33336); this.Add(58487, 33331); this.Add(58488, 33344); this.Add(58489, 33369);
					this.Add(58490, 33368); this.Add(58491, 33373); this.Add(58492, 33370); this.Add(58493, 33375); this.Add(58494, 33380); this.Add(58496, 33378); this.Add(58497, 33384); this.Add(58498, 33386);
					this.Add(58499, 33387); this.Add(58500, 33326); this.Add(58501, 33393); this.Add(58502, 33399); this.Add(58503, 33400); this.Add(58504, 33406); this.Add(58505, 33421); this.Add(58506, 33426);
					this.Add(58507, 33451); this.Add(58508, 33439); this.Add(58509, 33467); this.Add(58510, 33452); this.Add(58511, 33505); this.Add(58512, 33507); this.Add(58513, 33503); this.Add(58514, 33490);
					this.Add(58515, 33524); this.Add(58516, 33523); this.Add(58517, 33530); this.Add(58518, 33683); this.Add(58519, 33539); this.Add(58520, 33531); this.Add(58521, 33529); this.Add(58522, 33502);
					this.Add(58523, 33542); this.Add(58524, 33500); this.Add(58525, 33545); this.Add(58526, 33497); this.Add(58527, 33589); this.Add(58528, 33588); this.Add(58529, 33558); this.Add(58530, 33586);
					this.Add(58531, 33585); this.Add(58532, 33600); this.Add(58533, 33593); this.Add(58534, 33616); this.Add(58535, 33605); this.Add(58536, 33583); this.Add(58537, 33579); this.Add(58538, 33559);
					this.Add(58539, 33560); this.Add(58540, 33669); this.Add(58541, 33690); this.Add(58542, 33706); this.Add(58543, 33695); this.Add(58544, 33698); this.Add(58545, 33686); this.Add(58546, 33571);
					this.Add(58547, 33678); this.Add(58548, 33671); this.Add(58549, 33674); this.Add(58550, 33660); this.Add(58551, 33717); this.Add(58552, 33651); this.Add(58553, 33653); this.Add(58554, 33696);
					this.Add(58555, 33673); this.Add(58556, 33704); this.Add(58557, 33780); this.Add(58558, 33811); this.Add(58559, 33771); this.Add(58560, 33742); this.Add(58561, 33789); this.Add(58562, 33795);
					this.Add(58563, 33752); this.Add(58564, 33803); this.Add(58565, 33729); this.Add(58566, 33783); this.Add(58567, 33799); this.Add(58568, 33760); this.Add(58569, 33778); this.Add(58570, 33805);
					this.Add(58571, 33826); this.Add(58572, 33824); this.Add(58573, 33725); this.Add(58574, 33848); this.Add(58575, 34054); this.Add(58576, 33787); this.Add(58577, 33901); this.Add(58578, 33834);
					this.Add(58579, 33852); this.Add(58580, 34138); this.Add(58581, 33924); this.Add(58582, 33911); this.Add(58583, 33899); this.Add(58584, 33965); this.Add(58585, 33902); this.Add(58586, 33922);
					this.Add(58587, 33897); this.Add(58588, 33862); this.Add(58589, 33836); this.Add(58590, 33903); this.Add(58591, 33913); this.Add(58592, 33845); this.Add(58593, 33994); this.Add(58594, 33890);
					this.Add(58595, 33977); this.Add(58596, 33983); this.Add(58597, 33951); this.Add(58598, 34009); this.Add(58599, 33997); this.Add(58600, 33979); this.Add(58601, 34010); this.Add(58602, 34000);
					this.Add(58603, 33985); this.Add(58604, 33990); this.Add(58605, 34006); this.Add(58606, 33953); this.Add(58607, 34081); this.Add(58608, 34047); this.Add(58609, 34036); this.Add(58610, 34071);
					this.Add(58611, 34072); this.Add(58612, 34092); this.Add(58613, 34079); this.Add(58614, 34069); this.Add(58615, 34068); this.Add(58616, 34044); this.Add(58617, 34112); this.Add(58618, 34147);
					this.Add(58619, 34136); this.Add(58620, 34120); this.Add(58688, 34113); this.Add(58689, 34306); this.Add(58690, 34123); this.Add(58691, 34133); this.Add(58692, 34176); this.Add(58693, 34212);
					this.Add(58694, 34184); this.Add(58695, 34193); this.Add(58696, 34186); this.Add(58697, 34216); this.Add(58698, 34157); this.Add(58699, 34196); this.Add(58700, 34203); this.Add(58701, 34282);
					this.Add(58702, 34183); this.Add(58703, 34204); this.Add(58704, 34167); this.Add(58705, 34174); this.Add(58706, 34192); this.Add(58707, 34249); this.Add(58708, 34234); this.Add(58709, 34255);
					this.Add(58710, 34233); this.Add(58711, 34256); this.Add(58712, 34261); this.Add(58713, 34269); this.Add(58714, 34277); this.Add(58715, 34268); this.Add(58716, 34297); this.Add(58717, 34314);
					this.Add(58718, 34323); this.Add(58719, 34315); this.Add(58720, 34302); this.Add(58721, 34298); this.Add(58722, 34310); this.Add(58723, 34338); this.Add(58724, 34330); this.Add(58725, 34352);
					this.Add(58726, 34367); this.Add(58727, 34381); this.Add(58728, 20053); this.Add(58729, 34388); this.Add(58730, 34399); this.Add(58731, 34407); this.Add(58732, 34417); this.Add(58733, 34451);
					this.Add(58734, 34467); this.Add(58735, 34473); this.Add(58736, 34474); this.Add(58737, 34443); this.Add(58738, 34444); this.Add(58739, 34486); this.Add(58740, 34479); this.Add(58741, 34500);
					this.Add(58742, 34502); this.Add(58743, 34480); this.Add(58744, 34505); this.Add(58745, 34851); this.Add(58746, 34475); this.Add(58747, 34516); this.Add(58748, 34526); this.Add(58749, 34537);
					this.Add(58750, 34540); this.Add(58752, 34527); this.Add(58753, 34523); this.Add(58754, 34543); this.Add(58755, 34578); this.Add(58756, 34566); this.Add(58757, 34568); this.Add(58758, 34560);
					this.Add(58759, 34563); this.Add(58760, 34555); this.Add(58761, 34577); this.Add(58762, 34569); this.Add(58763, 34573); this.Add(58764, 34553); this.Add(58765, 34570); this.Add(58766, 34612);
					this.Add(58767, 34623); this.Add(58768, 34615); this.Add(58769, 34619); this.Add(58770, 34597); this.Add(58771, 34601); this.Add(58772, 34586); this.Add(58773, 34656); this.Add(58774, 34655);
					this.Add(58775, 34680); this.Add(58776, 34636); this.Add(58777, 34638); this.Add(58778, 34676); this.Add(58779, 34647); this.Add(58780, 34664); this.Add(58781, 34670); this.Add(58782, 34649);
					this.Add(58783, 34643); this.Add(58784, 34659); this.Add(58785, 34666); this.Add(58786, 34821); this.Add(58787, 34722); this.Add(58788, 34719); this.Add(58789, 34690); this.Add(58790, 34735);
					this.Add(58791, 34763); this.Add(58792, 34749); this.Add(58793, 34752); this.Add(58794, 34768); this.Add(58795, 38614); this.Add(58796, 34731); this.Add(58797, 34756); this.Add(58798, 34739);
					this.Add(58799, 34759); this.Add(58800, 34758); this.Add(58801, 34747); this.Add(58802, 34799); this.Add(58803, 34802); this.Add(58804, 34784); this.Add(58805, 34831); this.Add(58806, 34829);
					this.Add(58807, 34814); this.Add(58808, 34806); this.Add(58809, 34807); this.Add(58810, 34830); this.Add(58811, 34770); this.Add(58812, 34833); this.Add(58813, 34838); this.Add(58814, 34837);
					this.Add(58815, 34850); this.Add(58816, 34849); this.Add(58817, 34865); this.Add(58818, 34870); this.Add(58819, 34873); this.Add(58820, 34855); this.Add(58821, 34875); this.Add(58822, 34884);
					this.Add(58823, 34882); this.Add(58824, 34898); this.Add(58825, 34905); this.Add(58826, 34910); this.Add(58827, 34914); this.Add(58828, 34923); this.Add(58829, 34945); this.Add(58830, 34942);
					this.Add(58831, 34974); this.Add(58832, 34933); this.Add(58833, 34941); this.Add(58834, 34997); this.Add(58835, 34930); this.Add(58836, 34946); this.Add(58837, 34967); this.Add(58838, 34962);
					this.Add(58839, 34990); this.Add(58840, 34969); this.Add(58841, 34978); this.Add(58842, 34957); this.Add(58843, 34980); this.Add(58844, 34992); this.Add(58845, 35007); this.Add(58846, 34993);
					this.Add(58847, 35011); this.Add(58848, 35012); this.Add(58849, 35028); this.Add(58850, 35032); this.Add(58851, 35033); this.Add(58852, 35037); this.Add(58853, 35065); this.Add(58854, 35074);
					this.Add(58855, 35068); this.Add(58856, 35060); this.Add(58857, 35048); this.Add(58858, 35058); this.Add(58859, 35076); this.Add(58860, 35084); this.Add(58861, 35082); this.Add(58862, 35091);
					this.Add(58863, 35139); this.Add(58864, 35102); this.Add(58865, 35109); this.Add(58866, 35114); this.Add(58867, 35115); this.Add(58868, 35137); this.Add(58869, 35140); this.Add(58870, 35131);
					this.Add(58871, 35126); this.Add(58872, 35128); this.Add(58873, 35148); this.Add(58874, 35101); this.Add(58875, 35168); this.Add(58876, 35166); this.Add(58944, 35174); this.Add(58945, 35172);
					this.Add(58946, 35181); this.Add(58947, 35178); this.Add(58948, 35183); this.Add(58949, 35188); this.Add(58950, 35191); this.Add(58951, 35198); this.Add(58952, 35203); this.Add(58953, 35208);
					this.Add(58954, 35210); this.Add(58955, 35219); this.Add(58956, 35224); this.Add(58957, 35233); this.Add(58958, 35241); this.Add(58959, 35238); this.Add(58960, 35244); this.Add(58961, 35247);
					this.Add(58962, 35250); this.Add(58963, 35258); this.Add(58964, 35261); this.Add(58965, 35263); this.Add(58966, 35264); this.Add(58967, 35290); this.Add(58968, 35292); this.Add(58969, 35293);
					this.Add(58970, 35303); this.Add(58971, 35316); this.Add(58972, 35320); this.Add(58973, 35331); this.Add(58974, 35350); this.Add(58975, 35344); this.Add(58976, 35340); this.Add(58977, 35355);
					this.Add(58978, 35357); this.Add(58979, 35365); this.Add(58980, 35382); this.Add(58981, 35393); this.Add(58982, 35419); this.Add(58983, 35410); this.Add(58984, 35398); this.Add(58985, 35400);
					this.Add(58986, 35452); this.Add(58987, 35437); this.Add(58988, 35436); this.Add(58989, 35426); this.Add(58990, 35461); this.Add(58991, 35458); this.Add(58992, 35460); this.Add(58993, 35496);
					this.Add(58994, 35489); this.Add(58995, 35473); this.Add(58996, 35493); this.Add(58997, 35494); this.Add(58998, 35482); this.Add(58999, 35491); this.Add(59000, 35524); this.Add(59001, 35533);
					this.Add(59002, 35522); this.Add(59003, 35546); this.Add(59004, 35563); this.Add(59005, 35571); this.Add(59006, 35559); this.Add(59008, 35556); this.Add(59009, 35569); this.Add(59010, 35604);
					this.Add(59011, 35552); this.Add(59012, 35554); this.Add(59013, 35575); this.Add(59014, 35550); this.Add(59015, 35547); this.Add(59016, 35596); this.Add(59017, 35591); this.Add(59018, 35610);
					this.Add(59019, 35553); this.Add(59020, 35606); this.Add(59021, 35600); this.Add(59022, 35607); this.Add(59023, 35616); this.Add(59024, 35635); this.Add(59025, 38827); this.Add(59026, 35622);
					this.Add(59027, 35627); this.Add(59028, 35646); this.Add(59029, 35624); this.Add(59030, 35649); this.Add(59031, 35660); this.Add(59032, 35663); this.Add(59033, 35662); this.Add(59034, 35657);
					this.Add(59035, 35670); this.Add(59036, 35675); this.Add(59037, 35674); this.Add(59038, 35691); this.Add(59039, 35679); this.Add(59040, 35692); this.Add(59041, 35695); this.Add(59042, 35700);
					this.Add(59043, 35709); this.Add(59044, 35712); this.Add(59045, 35724); this.Add(59046, 35726); this.Add(59047, 35730); this.Add(59048, 35731); this.Add(59049, 35734); this.Add(59050, 35737);
					this.Add(59051, 35738); this.Add(59052, 35898); this.Add(59053, 35905); this.Add(59054, 35903); this.Add(59055, 35912); this.Add(59056, 35916); this.Add(59057, 35918); this.Add(59058, 35920);
					this.Add(59059, 35925); this.Add(59060, 35938); this.Add(59061, 35948); this.Add(59062, 35960); this.Add(59063, 35962); this.Add(59064, 35970); this.Add(59065, 35977); this.Add(59066, 35973);
					this.Add(59067, 35978); this.Add(59068, 35981); this.Add(59069, 35982); this.Add(59070, 35988); this.Add(59071, 35964); this.Add(59072, 35992); this.Add(59073, 25117); this.Add(59074, 36013);
					this.Add(59075, 36010); this.Add(59076, 36029); this.Add(59077, 36018); this.Add(59078, 36019); this.Add(59079, 36014); this.Add(59080, 36022); this.Add(59081, 36040); this.Add(59082, 36033);
					this.Add(59083, 36068); this.Add(59084, 36067); this.Add(59085, 36058); this.Add(59086, 36093); this.Add(59087, 36090); this.Add(59088, 36091); this.Add(59089, 36100); this.Add(59090, 36101);
					this.Add(59091, 36106); this.Add(59092, 36103); this.Add(59093, 36111); this.Add(59094, 36109); this.Add(59095, 36112); this.Add(59096, 40782); this.Add(59097, 36115); this.Add(59098, 36045);
					this.Add(59099, 36116); this.Add(59100, 36118); this.Add(59101, 36199); this.Add(59102, 36205); this.Add(59103, 36209); this.Add(59104, 36211); this.Add(59105, 36225); this.Add(59106, 36249);
					this.Add(59107, 36290); this.Add(59108, 36286); this.Add(59109, 36282); this.Add(59110, 36303); this.Add(59111, 36314); this.Add(59112, 36310); this.Add(59113, 36300); this.Add(59114, 36315);
					this.Add(59115, 36299); this.Add(59116, 36330); this.Add(59117, 36331); this.Add(59118, 36319); this.Add(59119, 36323); this.Add(59120, 36348); this.Add(59121, 36360); this.Add(59122, 36361);
					this.Add(59123, 36351); this.Add(59124, 36381); this.Add(59125, 36382); this.Add(59126, 36368); this.Add(59127, 36383); this.Add(59128, 36418); this.Add(59129, 36405); this.Add(59130, 36400);
					this.Add(59131, 36404); this.Add(59132, 36426); this.Add(59200, 36423); this.Add(59201, 36425); this.Add(59202, 36428); this.Add(59203, 36432); this.Add(59204, 36424); this.Add(59205, 36441);
					this.Add(59206, 36452); this.Add(59207, 36448); this.Add(59208, 36394); this.Add(59209, 36451); this.Add(59210, 36437); this.Add(59211, 36470); this.Add(59212, 36466); this.Add(59213, 36476);
					this.Add(59214, 36481); this.Add(59215, 36487); this.Add(59216, 36485); this.Add(59217, 36484); this.Add(59218, 36491); this.Add(59219, 36490); this.Add(59220, 36499); this.Add(59221, 36497);
					this.Add(59222, 36500); this.Add(59223, 36505); this.Add(59224, 36522); this.Add(59225, 36513); this.Add(59226, 36524); this.Add(59227, 36528); this.Add(59228, 36550); this.Add(59229, 36529);
					this.Add(59230, 36542); this.Add(59231, 36549); this.Add(59232, 36552); this.Add(59233, 36555); this.Add(59234, 36571); this.Add(59235, 36579); this.Add(59236, 36604); this.Add(59237, 36603);
					this.Add(59238, 36587); this.Add(59239, 36606); this.Add(59240, 36618); this.Add(59241, 36613); this.Add(59242, 36629); this.Add(59243, 36626); this.Add(59244, 36633); this.Add(59245, 36627);
					this.Add(59246, 36636); this.Add(59247, 36639); this.Add(59248, 36635); this.Add(59249, 36620); this.Add(59250, 36646); this.Add(59251, 36659); this.Add(59252, 36667); this.Add(59253, 36665);
					this.Add(59254, 36677); this.Add(59255, 36674); this.Add(59256, 36670); this.Add(59257, 36684); this.Add(59258, 36681); this.Add(59259, 36678); this.Add(59260, 36686); this.Add(59261, 36695);
					this.Add(59262, 36700); this.Add(59264, 36706); this.Add(59265, 36707); this.Add(59266, 36708); this.Add(59267, 36764); this.Add(59268, 36767); this.Add(59269, 36771); this.Add(59270, 36781);
					this.Add(59271, 36783); this.Add(59272, 36791); this.Add(59273, 36826); this.Add(59274, 36837); this.Add(59275, 36834); this.Add(59276, 36842); this.Add(59277, 36847); this.Add(59278, 36999);
					this.Add(59279, 36852); this.Add(59280, 36869); this.Add(59281, 36857); this.Add(59282, 36858); this.Add(59283, 36881); this.Add(59284, 36885); this.Add(59285, 36897); this.Add(59286, 36877);
					this.Add(59287, 36894); this.Add(59288, 36886); this.Add(59289, 36875); this.Add(59290, 36903); this.Add(59291, 36918); this.Add(59292, 36917); this.Add(59293, 36921); this.Add(59294, 36856);
					this.Add(59295, 36943); this.Add(59296, 36944); this.Add(59297, 36945); this.Add(59298, 36946); this.Add(59299, 36878); this.Add(59300, 36937); this.Add(59301, 36926); this.Add(59302, 36950);
					this.Add(59303, 36952); this.Add(59304, 36958); this.Add(59305, 36968); this.Add(59306, 36975); this.Add(59307, 36982); this.Add(59308, 38568); this.Add(59309, 36978); this.Add(59310, 36994);
					this.Add(59311, 36989); this.Add(59312, 36993); this.Add(59313, 36992); this.Add(59314, 37002); this.Add(59315, 37001); this.Add(59316, 37007); this.Add(59317, 37032); this.Add(59318, 37039);
					this.Add(59319, 37041); this.Add(59320, 37045); this.Add(59321, 37090); this.Add(59322, 37092); this.Add(59323, 25160); this.Add(59324, 37083); this.Add(59325, 37122); this.Add(59326, 37138);
					this.Add(59327, 37145); this.Add(59328, 37170); this.Add(59329, 37168); this.Add(59330, 37194); this.Add(59331, 37206); this.Add(59332, 37208); this.Add(59333, 37219); this.Add(59334, 37221);
					this.Add(59335, 37225); this.Add(59336, 37235); this.Add(59337, 37234); this.Add(59338, 37259); this.Add(59339, 37257); this.Add(59340, 37250); this.Add(59341, 37282); this.Add(59342, 37291);
					this.Add(59343, 37295); this.Add(59344, 37290); this.Add(59345, 37301); this.Add(59346, 37300); this.Add(59347, 37306); this.Add(59348, 37312); this.Add(59349, 37313); this.Add(59350, 37321);
					this.Add(59351, 37323); this.Add(59352, 37328); this.Add(59353, 37334); this.Add(59354, 37343); this.Add(59355, 37345); this.Add(59356, 37339); this.Add(59357, 37372); this.Add(59358, 37365);
					this.Add(59359, 37366); this.Add(59360, 37406); this.Add(59361, 37375); this.Add(59362, 37396); this.Add(59363, 37420); this.Add(59364, 37397); this.Add(59365, 37393); this.Add(59366, 37470);
					this.Add(59367, 37463); this.Add(59368, 37445); this.Add(59369, 37449); this.Add(59370, 37476); this.Add(59371, 37448); this.Add(59372, 37525); this.Add(59373, 37439); this.Add(59374, 37451);
					this.Add(59375, 37456); this.Add(59376, 37532); this.Add(59377, 37526); this.Add(59378, 37523); this.Add(59379, 37531); this.Add(59380, 37466); this.Add(59381, 37583); this.Add(59382, 37561);
					this.Add(59383, 37559); this.Add(59384, 37609); this.Add(59385, 37647); this.Add(59386, 37626); this.Add(59387, 37700); this.Add(59388, 37678); this.Add(59456, 37657); this.Add(59457, 37666);
					this.Add(59458, 37658); this.Add(59459, 37667); this.Add(59460, 37690); this.Add(59461, 37685); this.Add(59462, 37691); this.Add(59463, 37724); this.Add(59464, 37728); this.Add(59465, 37756);
					this.Add(59466, 37742); this.Add(59467, 37718); this.Add(59468, 37808); this.Add(59469, 37804); this.Add(59470, 37805); this.Add(59471, 37780); this.Add(59472, 37817); this.Add(59473, 37846);
					this.Add(59474, 37847); this.Add(59475, 37864); this.Add(59476, 37861); this.Add(59477, 37848); this.Add(59478, 37827); this.Add(59479, 37853); this.Add(59480, 37840); this.Add(59481, 37832);
					this.Add(59482, 37860); this.Add(59483, 37914); this.Add(59484, 37908); this.Add(59485, 37907); this.Add(59486, 37891); this.Add(59487, 37895); this.Add(59488, 37904); this.Add(59489, 37942);
					this.Add(59490, 37931); this.Add(59491, 37941); this.Add(59492, 37921); this.Add(59493, 37946); this.Add(59494, 37953); this.Add(59495, 37970); this.Add(59496, 37956); this.Add(59497, 37979);
					this.Add(59498, 37984); this.Add(59499, 37986); this.Add(59500, 37982); this.Add(59501, 37994); this.Add(59502, 37417); this.Add(59503, 38000); this.Add(59504, 38005); this.Add(59505, 38007);
					this.Add(59506, 38013); this.Add(59507, 37978); this.Add(59508, 38012); this.Add(59509, 38014); this.Add(59510, 38017); this.Add(59511, 38015); this.Add(59512, 38274); this.Add(59513, 38279);
					this.Add(59514, 38282); this.Add(59515, 38292); this.Add(59516, 38294); this.Add(59517, 38296); this.Add(59518, 38297); this.Add(59520, 38304); this.Add(59521, 38312); this.Add(59522, 38311);
					this.Add(59523, 38317); this.Add(59524, 38332); this.Add(59525, 38331); this.Add(59526, 38329); this.Add(59527, 38334); this.Add(59528, 38346); this.Add(59529, 28662); this.Add(59530, 38339);
					this.Add(59531, 38349); this.Add(59532, 38348); this.Add(59533, 38357); this.Add(59534, 38356); this.Add(59535, 38358); this.Add(59536, 38364); this.Add(59537, 38369); this.Add(59538, 38373);
					this.Add(59539, 38370); this.Add(59540, 38433); this.Add(59541, 38440); this.Add(59542, 38446); this.Add(59543, 38447); this.Add(59544, 38466); this.Add(59545, 38476); this.Add(59546, 38479);
					this.Add(59547, 38475); this.Add(59548, 38519); this.Add(59549, 38492); this.Add(59550, 38494); this.Add(59551, 38493); this.Add(59552, 38495); this.Add(59553, 38502); this.Add(59554, 38514);
					this.Add(59555, 38508); this.Add(59556, 38541); this.Add(59557, 38552); this.Add(59558, 38549); this.Add(59559, 38551); this.Add(59560, 38570); this.Add(59561, 38567); this.Add(59562, 38577);
					this.Add(59563, 38578); this.Add(59564, 38576); this.Add(59565, 38580); this.Add(59566, 38582); this.Add(59567, 38584); this.Add(59568, 38585); this.Add(59569, 38606); this.Add(59570, 38603);
					this.Add(59571, 38601); this.Add(59572, 38605); this.Add(59573, 35149); this.Add(59574, 38620); this.Add(59575, 38669); this.Add(59576, 38613); this.Add(59577, 38649); this.Add(59578, 38660);
					this.Add(59579, 38662); this.Add(59580, 38664); this.Add(59581, 38675); this.Add(59582, 38670); this.Add(59583, 38673); this.Add(59584, 38671); this.Add(59585, 38678); this.Add(59586, 38681);
					this.Add(59587, 38692); this.Add(59588, 38698); this.Add(59589, 38704); this.Add(59590, 38713); this.Add(59591, 38717); this.Add(59592, 38718); this.Add(59593, 38724); this.Add(59594, 38726);
					this.Add(59595, 38728); this.Add(59596, 38722); this.Add(59597, 38729); this.Add(59598, 38748); this.Add(59599, 38752); this.Add(59600, 38756); this.Add(59601, 38758); this.Add(59602, 38760);
					this.Add(59603, 21202); this.Add(59604, 38763); this.Add(59605, 38769); this.Add(59606, 38777); this.Add(59607, 38789); this.Add(59608, 38780); this.Add(59609, 38785); this.Add(59610, 38778);
					this.Add(59611, 38790); this.Add(59612, 38795); this.Add(59613, 38799); this.Add(59614, 38800); this.Add(59615, 38812); this.Add(59616, 38824); this.Add(59617, 38822); this.Add(59618, 38819);
					this.Add(59619, 38835); this.Add(59620, 38836); this.Add(59621, 38851); this.Add(59622, 38854); this.Add(59623, 38856); this.Add(59624, 38859); this.Add(59625, 38876); this.Add(59626, 38893);
					this.Add(59627, 40783); this.Add(59628, 38898); this.Add(59629, 31455); this.Add(59630, 38902); this.Add(59631, 38901); this.Add(59632, 38927); this.Add(59633, 38924); this.Add(59634, 38968);
					this.Add(59635, 38948); this.Add(59636, 38945); this.Add(59637, 38967); this.Add(59638, 38973); this.Add(59639, 38982); this.Add(59640, 38991); this.Add(59641, 38987); this.Add(59642, 39019);
					this.Add(59643, 39023); this.Add(59644, 39024); this.Add(59712, 39025); this.Add(59713, 39028); this.Add(59714, 39027); this.Add(59715, 39082); this.Add(59716, 39087); this.Add(59717, 39089);
					this.Add(59718, 39094); this.Add(59719, 39108); this.Add(59720, 39107); this.Add(59721, 39110); this.Add(59722, 39145); this.Add(59723, 39147); this.Add(59724, 39171); this.Add(59725, 39177);
					this.Add(59726, 39186); this.Add(59727, 39188); this.Add(59728, 39192); this.Add(59729, 39201); this.Add(59730, 39197); this.Add(59731, 39198); this.Add(59732, 39204); this.Add(59733, 39200);
					this.Add(59734, 39212); this.Add(59735, 39214); this.Add(59736, 39229); this.Add(59737, 39230); this.Add(59738, 39234); this.Add(59739, 39241); this.Add(59740, 39237); this.Add(59741, 39248);
					this.Add(59742, 39243); this.Add(59743, 39249); this.Add(59744, 39250); this.Add(59745, 39244); this.Add(59746, 39253); this.Add(59747, 39319); this.Add(59748, 39320); this.Add(59749, 39333);
					this.Add(59750, 39341); this.Add(59751, 39342); this.Add(59752, 39356); this.Add(59753, 39391); this.Add(59754, 39387); this.Add(59755, 39389); this.Add(59756, 39384); this.Add(59757, 39377);
					this.Add(59758, 39405); this.Add(59759, 39406); this.Add(59760, 39409); this.Add(59761, 39410); this.Add(59762, 39419); this.Add(59763, 39416); this.Add(59764, 39425); this.Add(59765, 39439);
					this.Add(59766, 39429); this.Add(59767, 39394); this.Add(59768, 39449); this.Add(59769, 39467); this.Add(59770, 39479); this.Add(59771, 39493); this.Add(59772, 39490); this.Add(59773, 39488);
					this.Add(59774, 39491); this.Add(59776, 39486); this.Add(59777, 39509); this.Add(59778, 39501); this.Add(59779, 39515); this.Add(59780, 39511); this.Add(59781, 39519); this.Add(59782, 39522);
					this.Add(59783, 39525); this.Add(59784, 39524); this.Add(59785, 39529); this.Add(59786, 39531); this.Add(59787, 39530); this.Add(59788, 39597); this.Add(59789, 39600); this.Add(59790, 39612);
					this.Add(59791, 39616); this.Add(59792, 39631); this.Add(59793, 39633); this.Add(59794, 39635); this.Add(59795, 39636); this.Add(59796, 39646); this.Add(59797, 39647); this.Add(59798, 39650);
					this.Add(59799, 39651); this.Add(59800, 39654); this.Add(59801, 39663); this.Add(59802, 39659); this.Add(59803, 39662); this.Add(59804, 39668); this.Add(59805, 39665); this.Add(59806, 39671);
					this.Add(59807, 39675); this.Add(59808, 39686); this.Add(59809, 39704); this.Add(59810, 39706); this.Add(59811, 39711); this.Add(59812, 39714); this.Add(59813, 39715); this.Add(59814, 39717);
					this.Add(59815, 39719); this.Add(59816, 39720); this.Add(59817, 39721); this.Add(59818, 39722); this.Add(59819, 39726); this.Add(59820, 39727); this.Add(59821, 39730); this.Add(59822, 39748);
					this.Add(59823, 39747); this.Add(59824, 39759); this.Add(59825, 39757); this.Add(59826, 39758); this.Add(59827, 39761); this.Add(59828, 39768); this.Add(59829, 39796); this.Add(59830, 39827);
					this.Add(59831, 39811); this.Add(59832, 39825); this.Add(59833, 39830); this.Add(59834, 39831); this.Add(59835, 39839); this.Add(59836, 39840); this.Add(59837, 39848); this.Add(59838, 39860);
					this.Add(59839, 39872); this.Add(59840, 39882); this.Add(59841, 39865); this.Add(59842, 39878); this.Add(59843, 39887); this.Add(59844, 39889); this.Add(59845, 39890); this.Add(59846, 39907);
					this.Add(59847, 39906); this.Add(59848, 39908); this.Add(59849, 39892); this.Add(59850, 39905); this.Add(59851, 39994); this.Add(59852, 39922); this.Add(59853, 39921); this.Add(59854, 39920);
					this.Add(59855, 39957); this.Add(59856, 39956); this.Add(59857, 39945); this.Add(59858, 39955); this.Add(59859, 39948); this.Add(59860, 39942); this.Add(59861, 39944); this.Add(59862, 39954);
					this.Add(59863, 39946); this.Add(59864, 39940); this.Add(59865, 39982); this.Add(59866, 39963); this.Add(59867, 39973); this.Add(59868, 39972); this.Add(59869, 39969); this.Add(59870, 39984);
					this.Add(59871, 40007); this.Add(59872, 39986); this.Add(59873, 40006); this.Add(59874, 39998); this.Add(59875, 40026); this.Add(59876, 40032); this.Add(59877, 40039); this.Add(59878, 40054);
					this.Add(59879, 40056); this.Add(59880, 40167); this.Add(59881, 40172); this.Add(59882, 40176); this.Add(59883, 40201); this.Add(59884, 40200); this.Add(59885, 40171); this.Add(59886, 40195);
					this.Add(59887, 40198); this.Add(59888, 40234); this.Add(59889, 40230); this.Add(59890, 40367); this.Add(59891, 40227); this.Add(59892, 40223); this.Add(59893, 40260); this.Add(59894, 40213);
					this.Add(59895, 40210); this.Add(59896, 40257); this.Add(59897, 40255); this.Add(59898, 40254); this.Add(59899, 40262); this.Add(59900, 40264); this.Add(59968, 40285); this.Add(59969, 40286);
					this.Add(59970, 40292); this.Add(59971, 40273); this.Add(59972, 40272); this.Add(59973, 40281); this.Add(59974, 40306); this.Add(59975, 40329); this.Add(59976, 40327); this.Add(59977, 40363);
					this.Add(59978, 40303); this.Add(59979, 40314); this.Add(59980, 40346); this.Add(59981, 40356); this.Add(59982, 40361); this.Add(59983, 40370); this.Add(59984, 40388); this.Add(59985, 40385);
					this.Add(59986, 40379); this.Add(59987, 40376); this.Add(59988, 40378); this.Add(59989, 40390); this.Add(59990, 40399); this.Add(59991, 40386); this.Add(59992, 40409); this.Add(59993, 40403);
					this.Add(59994, 40440); this.Add(59995, 40422); this.Add(59996, 40429); this.Add(59997, 40431); this.Add(59998, 40445); this.Add(59999, 40474); this.Add(60000, 40475); this.Add(60001, 40478);
					this.Add(60002, 40565); this.Add(60003, 40569); this.Add(60004, 40573); this.Add(60005, 40577); this.Add(60006, 40584); this.Add(60007, 40587); this.Add(60008, 40588); this.Add(60009, 40594);
					this.Add(60010, 40597); this.Add(60011, 40593); this.Add(60012, 40605); this.Add(60013, 40613); this.Add(60014, 40617); this.Add(60015, 40632); this.Add(60016, 40618); this.Add(60017, 40621);
					this.Add(60018, 38753); this.Add(60019, 40652); this.Add(60020, 40654); this.Add(60021, 40655); this.Add(60022, 40656); this.Add(60023, 40660); this.Add(60024, 40668); this.Add(60025, 40670);
					this.Add(60026, 40669); this.Add(60027, 40672); this.Add(60028, 40677); this.Add(60029, 40680); this.Add(60030, 40687); this.Add(60032, 40692); this.Add(60033, 40694); this.Add(60034, 40695);
					this.Add(60035, 40697); this.Add(60036, 40699); this.Add(60037, 40700); this.Add(60038, 40701); this.Add(60039, 40711); this.Add(60040, 40712); this.Add(60041, 30391); this.Add(60042, 40725);
					this.Add(60043, 40737); this.Add(60044, 40748); this.Add(60045, 40766); this.Add(60046, 40778); this.Add(60047, 40786); this.Add(60048, 40788); this.Add(60049, 40803); this.Add(60050, 40799);
					this.Add(60051, 40800); this.Add(60052, 40801); this.Add(60053, 40806); this.Add(60054, 40807); this.Add(60055, 40812); this.Add(60056, 40810); this.Add(60057, 40823); this.Add(60058, 40818);
					this.Add(60059, 40822); this.Add(60060, 40853); this.Add(60061, 40860); this.Add(60062, 40864); this.Add(60063, 22575); this.Add(60064, 27079); this.Add(60065, 36953); this.Add(60066, 29796);
					this.Add(60067, 20956); this.Add(60068, 29081); this.Add(60736, 32394); this.Add(60737, 35100); this.Add(60738, 37704); this.Add(60739, 37512); this.Add(60740, 34012); this.Add(60741, 20425);
					this.Add(60742, 28859); this.Add(60743, 26161); this.Add(60744, 26824); this.Add(60745, 37625); this.Add(60746, 26363); this.Add(60747, 24389); this.Add(60748, 20008); this.Add(60749, 20193);
					this.Add(60750, 20220); this.Add(60751, 20224); this.Add(60752, 20227); this.Add(60753, 20281); this.Add(60754, 20310); this.Add(60755, 20370); this.Add(60756, 20362); this.Add(60757, 20378);
					this.Add(60758, 20372); this.Add(60759, 20429); this.Add(60760, 20544); this.Add(60761, 20514); this.Add(60762, 20479); this.Add(60763, 20510); this.Add(60764, 20550); this.Add(60765, 20592);
					this.Add(60766, 20546); this.Add(60767, 20628); this.Add(60768, 20724); this.Add(60769, 20696); this.Add(60770, 20810); this.Add(60771, 20836); this.Add(60772, 20893); this.Add(60773, 20926);
					this.Add(60774, 20972); this.Add(60775, 21013); this.Add(60776, 21148); this.Add(60777, 21158); this.Add(60778, 21184); this.Add(60779, 21211); this.Add(60780, 21248); this.Add(60781, 21255);
					this.Add(60782, 21284); this.Add(60783, 21362); this.Add(60784, 21395); this.Add(60785, 21426); this.Add(60786, 21469); this.Add(60787, 64014); this.Add(60788, 21660); this.Add(60789, 21642);
					this.Add(60790, 21673); this.Add(60791, 21759); this.Add(60792, 21894); this.Add(60793, 22361); this.Add(60794, 22373); this.Add(60795, 22444); this.Add(60796, 22472); this.Add(60797, 22471);
					this.Add(60798, 64015); this.Add(60800, 64016); this.Add(60801, 22686); this.Add(60802, 22706); this.Add(60803, 22795); this.Add(60804, 22867); this.Add(60805, 22875); this.Add(60806, 22877);
					this.Add(60807, 22883); this.Add(60808, 22948); this.Add(60809, 22970); this.Add(60810, 23382); this.Add(60811, 23488); this.Add(60812, 29999); this.Add(60813, 23512); this.Add(60814, 23532);
					this.Add(60815, 23582); this.Add(60816, 23718); this.Add(60817, 23738); this.Add(60818, 23797); this.Add(60819, 23847); this.Add(60820, 23891); this.Add(60821, 64017); this.Add(60822, 23874);
					this.Add(60823, 23917); this.Add(60824, 23992); this.Add(60825, 23993); this.Add(60826, 24016); this.Add(60827, 24353); this.Add(60828, 24372); this.Add(60829, 24423); this.Add(60830, 24503);
					this.Add(60831, 24542); this.Add(60832, 24669); this.Add(60833, 24709); this.Add(60834, 24714); this.Add(60835, 24798); this.Add(60836, 24789); this.Add(60837, 24864); this.Add(60838, 24818);
					this.Add(60839, 24849); this.Add(60840, 24887); this.Add(60841, 24880); this.Add(60842, 24984); this.Add(60843, 25107); this.Add(60844, 25254); this.Add(60845, 25589); this.Add(60846, 25696);
					this.Add(60847, 25757); this.Add(60848, 25806); this.Add(60849, 25934); this.Add(60850, 26112); this.Add(60851, 26133); this.Add(60852, 26171); this.Add(60853, 26121); this.Add(60854, 26158);
					this.Add(60855, 26142); this.Add(60856, 26148); this.Add(60857, 26213); this.Add(60858, 26199); this.Add(60859, 26201); this.Add(60860, 64018); this.Add(60861, 26227); this.Add(60862, 26265);
					this.Add(60863, 26272); this.Add(60864, 26290); this.Add(60865, 26303); this.Add(60866, 26362); this.Add(60867, 26382); this.Add(60868, 63785); this.Add(60869, 26470); this.Add(60870, 26555);
					this.Add(60871, 26706); this.Add(60872, 26560); this.Add(60873, 26625); this.Add(60874, 26692); this.Add(60875, 26831); this.Add(60876, 64019); this.Add(60877, 26984); this.Add(60878, 64020);
					this.Add(60879, 27032); this.Add(60880, 27106); this.Add(60881, 27184); this.Add(60882, 27243); this.Add(60883, 27206); this.Add(60884, 27251); this.Add(60885, 27262); this.Add(60886, 27362);
					this.Add(60887, 27364); this.Add(60888, 27606); this.Add(60889, 27711); this.Add(60890, 27740); this.Add(60891, 27782); this.Add(60892, 27759); this.Add(60893, 27866); this.Add(60894, 27908);
					this.Add(60895, 28039); this.Add(60896, 28015); this.Add(60897, 28054); this.Add(60898, 28076); this.Add(60899, 28111); this.Add(60900, 28152); this.Add(60901, 28146); this.Add(60902, 28156);
					this.Add(60903, 28217); this.Add(60904, 28252); this.Add(60905, 28199); this.Add(60906, 28220); this.Add(60907, 28351); this.Add(60908, 28552); this.Add(60909, 28597); this.Add(60910, 28661);
					this.Add(60911, 28677); this.Add(60912, 28679); this.Add(60913, 28712); this.Add(60914, 28805); this.Add(60915, 28843); this.Add(60916, 28943); this.Add(60917, 28932); this.Add(60918, 29020);
					this.Add(60919, 28998); this.Add(60920, 28999); this.Add(60921, 64021); this.Add(60922, 29121); this.Add(60923, 29182); this.Add(60924, 29361); this.Add(60992, 29374); this.Add(60993, 29476);
					this.Add(60994, 64022); this.Add(60995, 29559); this.Add(60996, 29629); this.Add(60997, 29641); this.Add(60998, 29654); this.Add(60999, 29667); this.Add(61000, 29650); this.Add(61001, 29703);
					this.Add(61002, 29685); this.Add(61003, 29734); this.Add(61004, 29738); this.Add(61005, 29737); this.Add(61006, 29742); this.Add(61007, 29794); this.Add(61008, 29833); this.Add(61009, 29855);
					this.Add(61010, 29953); this.Add(61011, 30063); this.Add(61012, 30338); this.Add(61013, 30364); this.Add(61014, 30366); this.Add(61015, 30363); this.Add(61016, 30374); this.Add(61017, 64023);
					this.Add(61018, 30534); this.Add(61019, 21167); this.Add(61020, 30753); this.Add(61021, 30798); this.Add(61022, 30820); this.Add(61023, 30842); this.Add(61024, 31024); this.Add(61025, 64024);
					this.Add(61026, 64025); this.Add(61027, 64026); this.Add(61028, 31124); this.Add(61029, 64027); this.Add(61030, 31131); this.Add(61031, 31441); this.Add(61032, 31463); this.Add(61033, 64028);
					this.Add(61034, 31467); this.Add(61035, 31646); this.Add(61036, 64029); this.Add(61037, 32072); this.Add(61038, 32092); this.Add(61039, 32183); this.Add(61040, 32160); this.Add(61041, 32214);
					this.Add(61042, 32338); this.Add(61043, 32583); this.Add(61044, 32673); this.Add(61045, 64030); this.Add(61046, 33537); this.Add(61047, 33634); this.Add(61048, 33663); this.Add(61049, 33735);
					this.Add(61050, 33782); this.Add(61051, 33864); this.Add(61052, 33972); this.Add(61053, 34131); this.Add(61054, 34137); this.Add(61056, 34155); this.Add(61057, 64031); this.Add(61058, 34224);
					this.Add(61059, 64032); this.Add(61060, 64033); this.Add(61061, 34823); this.Add(61062, 35061); this.Add(61063, 35346); this.Add(61064, 35383); this.Add(61065, 35449); this.Add(61066, 35495);
					this.Add(61067, 35518); this.Add(61068, 35551); this.Add(61069, 64034); this.Add(61070, 35574); this.Add(61071, 35667); this.Add(61072, 35711); this.Add(61073, 36080); this.Add(61074, 36084);
					this.Add(61075, 36114); this.Add(61076, 36214); this.Add(61077, 64035); this.Add(61078, 36559); this.Add(61079, 64036); this.Add(61080, 64037); this.Add(61081, 36967); this.Add(61082, 37086);
					this.Add(61083, 64038); this.Add(61084, 37141); this.Add(61085, 37159); this.Add(61086, 37338); this.Add(61087, 37335); this.Add(61088, 37342); this.Add(61089, 37357); this.Add(61090, 37358);
					this.Add(61091, 37348); this.Add(61092, 37349); this.Add(61093, 37382); this.Add(61094, 37392); this.Add(61095, 37386); this.Add(61096, 37434); this.Add(61097, 37440); this.Add(61098, 37436);
					this.Add(61099, 37454); this.Add(61100, 37465); this.Add(61101, 37457); this.Add(61102, 37433); this.Add(61103, 37479); this.Add(61104, 37543); this.Add(61105, 37495); this.Add(61106, 37496);
					this.Add(61107, 37607); this.Add(61108, 37591); this.Add(61109, 37593); this.Add(61110, 37584); this.Add(61111, 64039); this.Add(61112, 37589); this.Add(61113, 37600); this.Add(61114, 37587);
					this.Add(61115, 37669); this.Add(61116, 37665); this.Add(61117, 37627); this.Add(61118, 64040); this.Add(61119, 37662); this.Add(61120, 37631); this.Add(61121, 37661); this.Add(61122, 37634);
					this.Add(61123, 37744); this.Add(61124, 37719); this.Add(61125, 37796); this.Add(61126, 37830); this.Add(61127, 37854); this.Add(61128, 37880); this.Add(61129, 37937); this.Add(61130, 37957);
					this.Add(61131, 37960); this.Add(61132, 38290); this.Add(61133, 63964); this.Add(61134, 64041); this.Add(61135, 38557); this.Add(61136, 38575); this.Add(61137, 38707); this.Add(61138, 38715);
					this.Add(61139, 38723); this.Add(61140, 38733); this.Add(61141, 38735); this.Add(61142, 38737); this.Add(61143, 38741); this.Add(61144, 38999); this.Add(61145, 39013); this.Add(61146, 64042);
					this.Add(61147, 64043); this.Add(61148, 39207); this.Add(61149, 64044); this.Add(61150, 39326); this.Add(61151, 39502); this.Add(61152, 39641); this.Add(61153, 39644); this.Add(61154, 39797);
					this.Add(61155, 39794); this.Add(61156, 39823); this.Add(61157, 39857); this.Add(61158, 39867); this.Add(61159, 39936); this.Add(61160, 40304); this.Add(61161, 40299); this.Add(61162, 64045);
					this.Add(61163, 40473); this.Add(61164, 40657); this.Add(61167, 8560); this.Add(61168, 8561); this.Add(61169, 8562); this.Add(61170, 8563); this.Add(61171, 8564); this.Add(61172, 8565);
					this.Add(61173, 8566); this.Add(61174, 8567); this.Add(61175, 8568); this.Add(61176, 8569); this.Add(61177, 65506); this.Add(61178, 65508); this.Add(61179, 65287); this.Add(61180, 65282);
					this.Add(64064, 8560); this.Add(64065, 8561); this.Add(64066, 8562); this.Add(64067, 8563); this.Add(64068, 8564); this.Add(64069, 8565); this.Add(64070, 8566); this.Add(64071, 8567);
					this.Add(64072, 8568); this.Add(64073, 8569); this.Add(64074, 8544); this.Add(64075, 8545); this.Add(64076, 8546); this.Add(64077, 8547); this.Add(64078, 8548); this.Add(64079, 8549);
					this.Add(64080, 8550); this.Add(64081, 8551); this.Add(64082, 8552); this.Add(64083, 8553); this.Add(64084, 65506); this.Add(64085, 65508); this.Add(64086, 65287); this.Add(64087, 65282);
					this.Add(64088, 12849); this.Add(64089, 8470); this.Add(64090, 8481); this.Add(64091, 8757); this.Add(64092, 32394); this.Add(64093, 35100); this.Add(64094, 37704); this.Add(64095, 37512);
					this.Add(64096, 34012); this.Add(64097, 20425); this.Add(64098, 28859); this.Add(64099, 26161); this.Add(64100, 26824); this.Add(64101, 37625); this.Add(64102, 26363); this.Add(64103, 24389);
					this.Add(64104, 20008); this.Add(64105, 20193); this.Add(64106, 20220); this.Add(64107, 20224); this.Add(64108, 20227); this.Add(64109, 20281); this.Add(64110, 20310); this.Add(64111, 20370);
					this.Add(64112, 20362); this.Add(64113, 20378); this.Add(64114, 20372); this.Add(64115, 20429); this.Add(64116, 20544); this.Add(64117, 20514); this.Add(64118, 20479); this.Add(64119, 20510);
					this.Add(64120, 20550); this.Add(64121, 20592); this.Add(64122, 20546); this.Add(64123, 20628); this.Add(64124, 20724); this.Add(64125, 20696); this.Add(64126, 20810); this.Add(64128, 20836);
					this.Add(64129, 20893); this.Add(64130, 20926); this.Add(64131, 20972); this.Add(64132, 21013); this.Add(64133, 21148); this.Add(64134, 21158); this.Add(64135, 21184); this.Add(64136, 21211);
					this.Add(64137, 21248); this.Add(64138, 21255); this.Add(64139, 21284); this.Add(64140, 21362); this.Add(64141, 21395); this.Add(64142, 21426); this.Add(64143, 21469); this.Add(64144, 64014);
					this.Add(64145, 21660); this.Add(64146, 21642); this.Add(64147, 21673); this.Add(64148, 21759); this.Add(64149, 21894); this.Add(64150, 22361); this.Add(64151, 22373); this.Add(64152, 22444);
					this.Add(64153, 22472); this.Add(64154, 22471); this.Add(64155, 64015); this.Add(64156, 64016); this.Add(64157, 22686); this.Add(64158, 22706); this.Add(64159, 22795); this.Add(64160, 22867);
					this.Add(64161, 22875); this.Add(64162, 22877); this.Add(64163, 22883); this.Add(64164, 22948); this.Add(64165, 22970); this.Add(64166, 23382); this.Add(64167, 23488); this.Add(64168, 29999);
					this.Add(64169, 23512); this.Add(64170, 23532); this.Add(64171, 23582); this.Add(64172, 23718); this.Add(64173, 23738); this.Add(64174, 23797); this.Add(64175, 23847); this.Add(64176, 23891);
					this.Add(64177, 64017); this.Add(64178, 23874); this.Add(64179, 23917); this.Add(64180, 23992); this.Add(64181, 23993); this.Add(64182, 24016); this.Add(64183, 24353); this.Add(64184, 24372);
					this.Add(64185, 24423); this.Add(64186, 24503); this.Add(64187, 24542); this.Add(64188, 24669); this.Add(64189, 24709); this.Add(64190, 24714); this.Add(64191, 24798); this.Add(64192, 24789);
					this.Add(64193, 24864); this.Add(64194, 24818); this.Add(64195, 24849); this.Add(64196, 24887); this.Add(64197, 24880); this.Add(64198, 24984); this.Add(64199, 25107); this.Add(64200, 25254);
					this.Add(64201, 25589); this.Add(64202, 25696); this.Add(64203, 25757); this.Add(64204, 25806); this.Add(64205, 25934); this.Add(64206, 26112); this.Add(64207, 26133); this.Add(64208, 26171);
					this.Add(64209, 26121); this.Add(64210, 26158); this.Add(64211, 26142); this.Add(64212, 26148); this.Add(64213, 26213); this.Add(64214, 26199); this.Add(64215, 26201); this.Add(64216, 64018);
					this.Add(64217, 26227); this.Add(64218, 26265); this.Add(64219, 26272); this.Add(64220, 26290); this.Add(64221, 26303); this.Add(64222, 26362); this.Add(64223, 26382); this.Add(64224, 63785);
					this.Add(64225, 26470); this.Add(64226, 26555); this.Add(64227, 26706); this.Add(64228, 26560); this.Add(64229, 26625); this.Add(64230, 26692); this.Add(64231, 26831); this.Add(64232, 64019);
					this.Add(64233, 26984); this.Add(64234, 64020); this.Add(64235, 27032); this.Add(64236, 27106); this.Add(64237, 27184); this.Add(64238, 27243); this.Add(64239, 27206); this.Add(64240, 27251);
					this.Add(64241, 27262); this.Add(64242, 27362); this.Add(64243, 27364); this.Add(64244, 27606); this.Add(64245, 27711); this.Add(64246, 27740); this.Add(64247, 27782); this.Add(64248, 27759);
					this.Add(64249, 27866); this.Add(64250, 27908); this.Add(64251, 28039); this.Add(64252, 28015); this.Add(64320, 28054); this.Add(64321, 28076); this.Add(64322, 28111); this.Add(64323, 28152);
					this.Add(64324, 28146); this.Add(64325, 28156); this.Add(64326, 28217); this.Add(64327, 28252); this.Add(64328, 28199); this.Add(64329, 28220); this.Add(64330, 28351); this.Add(64331, 28552);
					this.Add(64332, 28597); this.Add(64333, 28661); this.Add(64334, 28677); this.Add(64335, 28679); this.Add(64336, 28712); this.Add(64337, 28805); this.Add(64338, 28843); this.Add(64339, 28943);
					this.Add(64340, 28932); this.Add(64341, 29020); this.Add(64342, 28998); this.Add(64343, 28999); this.Add(64344, 64021); this.Add(64345, 29121); this.Add(64346, 29182); this.Add(64347, 29361);
					this.Add(64348, 29374); this.Add(64349, 29476); this.Add(64350, 64022); this.Add(64351, 29559); this.Add(64352, 29629); this.Add(64353, 29641); this.Add(64354, 29654); this.Add(64355, 29667);
					this.Add(64356, 29650); this.Add(64357, 29703); this.Add(64358, 29685); this.Add(64359, 29734); this.Add(64360, 29738); this.Add(64361, 29737); this.Add(64362, 29742); this.Add(64363, 29794);
					this.Add(64364, 29833); this.Add(64365, 29855); this.Add(64366, 29953); this.Add(64367, 30063); this.Add(64368, 30338); this.Add(64369, 30364); this.Add(64370, 30366); this.Add(64371, 30363);
					this.Add(64372, 30374); this.Add(64373, 64023); this.Add(64374, 30534); this.Add(64375, 21167); this.Add(64376, 30753); this.Add(64377, 30798); this.Add(64378, 30820); this.Add(64379, 30842);
					this.Add(64380, 31024); this.Add(64381, 64024); this.Add(64382, 64025); this.Add(64384, 64026); this.Add(64385, 31124); this.Add(64386, 64027); this.Add(64387, 31131); this.Add(64388, 31441);
					this.Add(64389, 31463); this.Add(64390, 64028); this.Add(64391, 31467); this.Add(64392, 31646); this.Add(64393, 64029); this.Add(64394, 32072); this.Add(64395, 32092); this.Add(64396, 32183);
					this.Add(64397, 32160); this.Add(64398, 32214); this.Add(64399, 32338); this.Add(64400, 32583); this.Add(64401, 32673); this.Add(64402, 64030); this.Add(64403, 33537); this.Add(64404, 33634);
					this.Add(64405, 33663); this.Add(64406, 33735); this.Add(64407, 33782); this.Add(64408, 33864); this.Add(64409, 33972); this.Add(64410, 34131); this.Add(64411, 34137); this.Add(64412, 34155);
					this.Add(64413, 64031); this.Add(64414, 34224); this.Add(64415, 64032); this.Add(64416, 64033); this.Add(64417, 34823); this.Add(64418, 35061); this.Add(64419, 35346); this.Add(64420, 35383);
					this.Add(64421, 35449); this.Add(64422, 35495); this.Add(64423, 35518); this.Add(64424, 35551); this.Add(64425, 64034); this.Add(64426, 35574); this.Add(64427, 35667); this.Add(64428, 35711);
					this.Add(64429, 36080); this.Add(64430, 36084); this.Add(64431, 36114); this.Add(64432, 36214); this.Add(64433, 64035); this.Add(64434, 36559); this.Add(64435, 64036); this.Add(64436, 64037);
					this.Add(64437, 36967); this.Add(64438, 37086); this.Add(64439, 64038); this.Add(64440, 37141); this.Add(64441, 37159); this.Add(64442, 37338); this.Add(64443, 37335); this.Add(64444, 37342);
					this.Add(64445, 37357); this.Add(64446, 37358); this.Add(64447, 37348); this.Add(64448, 37349); this.Add(64449, 37382); this.Add(64450, 37392); this.Add(64451, 37386); this.Add(64452, 37434);
					this.Add(64453, 37440); this.Add(64454, 37436); this.Add(64455, 37454); this.Add(64456, 37465); this.Add(64457, 37457); this.Add(64458, 37433); this.Add(64459, 37479); this.Add(64460, 37543);
					this.Add(64461, 37495); this.Add(64462, 37496); this.Add(64463, 37607); this.Add(64464, 37591); this.Add(64465, 37593); this.Add(64466, 37584); this.Add(64467, 64039); this.Add(64468, 37589);
					this.Add(64469, 37600); this.Add(64470, 37587); this.Add(64471, 37669); this.Add(64472, 37665); this.Add(64473, 37627); this.Add(64474, 64040); this.Add(64475, 37662); this.Add(64476, 37631);
					this.Add(64477, 37661); this.Add(64478, 37634); this.Add(64479, 37744); this.Add(64480, 37719); this.Add(64481, 37796); this.Add(64482, 37830); this.Add(64483, 37854); this.Add(64484, 37880);
					this.Add(64485, 37937); this.Add(64486, 37957); this.Add(64487, 37960); this.Add(64488, 38290); this.Add(64489, 63964); this.Add(64490, 64041); this.Add(64491, 38557); this.Add(64492, 38575);
					this.Add(64493, 38707); this.Add(64494, 38715); this.Add(64495, 38723); this.Add(64496, 38733); this.Add(64497, 38735); this.Add(64498, 38737); this.Add(64499, 38741); this.Add(64500, 38999);
					this.Add(64501, 39013); this.Add(64502, 64042); this.Add(64503, 64043); this.Add(64504, 39207); this.Add(64505, 64044); this.Add(64506, 39326); this.Add(64507, 39502); this.Add(64508, 39641);
					this.Add(64576, 39644); this.Add(64577, 39797); this.Add(64578, 39794); this.Add(64579, 39823); this.Add(64580, 39857); this.Add(64581, 39867); this.Add(64582, 39936); this.Add(64583, 40304);
					this.Add(64584, 40299); this.Add(64585, 64045); this.Add(64586, 40473); this.Add(64587, 40657);

					#endregion

					for (UInt16 code = 0x01; code <= 0x7e; code++) // ASCII + (制御コード - NUL)
						this.Add(code, code);
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
