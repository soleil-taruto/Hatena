using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleWebServer // ★名前空間は適宜変えて下さい。
{
	public class SimpleWebServer
	{
		/// <summary>
		/// ドキュメントルート
		/// </summary>
		public string DocRoot = @"C:\www\DocRoot";

		/// <summary>
		/// ポート番号
		/// </summary>
		public int PortNo = 80;

		/// <summary>
		/// サーバー処理の合間に呼ばれる処理
		/// 戻り値：
		/// -- サーバーを継続するか
		/// </summary>
		public Func<bool> Interlude = () => !Console.KeyAvailable; // 何かキーを押したらサーバー終了

		// インスタンスの設定ここまで

		/// <summary>
		/// Keep-Alive-タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int KeepAliveTimeoutMillis = 5000;

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

		/// <summary>
		/// リクエストの最初の行のみの無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int FirstLineTimeoutMillis = 2000;

		/// <summary>
		/// リクエストの最初の行以外の(レスポンスも含む)無通信タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int IdleTimeoutMillis = 600000;

		/// <summary>
		/// スレッド占用タイムアウト_ミリ秒
		/// -1 == INFINITE
		/// </summary>
		public static int ThreadTimeoutMillis = 100;

		/// <summary>
		/// リクエストのボディの最大サイズ_バイト数
		/// -1 == INFINITE
		/// </summary>
		public static int BodySizeMax = 300000000;

		/// <summary>
		/// 接続待ちキューの長さ
		/// </summary>
		public static int Backlog = 300;

		/// <summary>
		/// 最大同時接続数
		/// </summary>
		public static int ConnectMax = 100;

		// 設定ここまで

		public static void Run(string docRoot, int portNo)
		{
			try
			{
				if (string.IsNullOrEmpty(docRoot))
					throw new Exception("ドキュメントルートを指定して下さい。");

				if (!Directory.Exists(docRoot))
					throw new Exception("ドキュメントルートは存在しません：" + docRoot);

				if (portNo < 1 || 65535 < portNo)
					throw new Exception("不正なポート番号です：" + portNo);

				new SimpleWebServer()
				{
					DocRoot = docRoot,
					PortNo = portNo,
				}
				.Perform();
			}
			catch (Exception e)
			{
				WriteLog(e);
			}
		}

		public void Perform()
		{
			HTTPServer hs = new HTTPServer();

			hs.HTTPConnected = this.P_Connected;

			hs.Perform(this.PortNo, this.Interlude);
		}

		private void P_Connected(HTTPServerChannel channel)
		{
			WriteLog("Client: " + channel.Channel.Handler.RemoteEndPoint);

			if (7 < channel.Method.Length) // ? 最も長いメソッドより長い。
				throw new Exception("Received method is too long");

			WriteLog("Method: " + channel.Method);

			switch (channel.Method)
			{
				case "GET":
					this.GetOrHead(channel, false);
					break;

				case "HEAD":
					this.GetOrHead(channel, true);
					break;

				case "POST":
					throw new Exception("Unsupported method");

				case "PUT":
					throw new Exception("Unsupported method");

				case "DELETE":
					throw new Exception("Unsupported method");

				case "CONNECT":
					throw new Exception("Unsupported method");

				case "OPTIONS":
					throw new Exception("Unsupported method");

				case "TRACE":
					throw new Exception("Unsupported method");

				case "PATCH":
					throw new Exception("Unsupported method");

				default:
					throw new Exception("Unknown method");
			}
		}

		private void GetOrHead(HTTPServerChannel channel, bool head)
		{
			string urlPath = channel.PathQuery;

			// クエリ除去
			{
				int ques = urlPath.IndexOf('?');

				if (ques != -1)
					urlPath = urlPath.Substring(0, ques);
			}

			if (1000 < urlPath.Length) // rough limit
				throw new Exception("Received path is too long");

			WriteLog("URL-Path：" + urlPath);

			string[] pTkns = urlPath.Split('/').Where(v => v != "").Select(v => ToFairLocalPath(v, 0)).ToArray();
			string path = Path.Combine(new string[] { this.DocRoot }.Concat(pTkns).ToArray());

			WriteLog("RealPath：" + path);

			if (urlPath.EndsWith("/"))
			{
				path = Path.Combine(path, "index.htm");

				if (!File.Exists(path))
					path += "l";
			}
			else if (Directory.Exists(path))
			{
				channel.ResStatus = 301;
				channel.ResHeaderPairs.Add(new string[] { "Location", $"http://{channel.GetHeaderValue("Host")}/{string.Join("", pTkns.Select(v => EncodeUrl(v)))}/" });
				//channel.ResBody = null;

				goto response;
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

		response:
			channel.ResHeaderPairs.Add(new string[] { "Server", "sws" });

			if (head && channel.ResBody != null)
			{
				FileInfo fileInfo = new FileInfo(path);

				channel.ResHeaderPairs.Add(new string[] { "Content-Length", fileInfo.Length.ToString() });
				channel.ResHeaderPairs.Add(new string[] { "X-Last-Modified-Time", fileInfo.LastWriteTime.Ticks.ToString() });

				channel.ResBody = null;
			}

			WriteLog("Res-Status: " + channel.ResStatus);

			foreach (string[] pair in channel.ResHeaderPairs)
				WriteLog($"Res-Header: {pair[0]} = {pair[1]}");

			WriteLog("Res-Body: " + (channel.ResBody != null));
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

				WriteLog($"Read: {offset} {readSize} {fileSize} {(offset * 100.0 / fileSize).ToString("F2")} {((offset + readSize) * 100.0 / fileSize).ToString("F2")}");

				using (FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read))
				{
					reader.Seek(offset, SeekOrigin.Begin);
					reader.Read(buff, 0, readSize);
				}
				yield return buff;

				offset += (long)readSize;
			}
		}

		private class ContentTypeCollection
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

		private class HTTPServer : SockServer
		{
			/// <summary>
			/// サーバーロジック
			/// 引数：
			/// -- channel: 接続チャネル
			/// </summary>
			public Action<HTTPServerChannel> HTTPConnected = channel => { };

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

					HTTPConnected(hsChannel);

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

		private class HTTPServerChannel
		{
			public SockChannel Channel;

			public IEnumerable<int> RecvRequest()
			{
				this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(RequestTimeoutMillis);
				this.Channel.P_IdleTimeoutMillis = FirstLineTimeoutMillis;

				foreach (int relay in this.RecvLine(ret => this.FirstLine = ret))
					yield return relay;

				{
					string[] tokens = this.FirstLine.Split(' ');

					this.Method = tokens[0];
					this.PathQuery = DecodeURL(tokens[1]);
					this.HTTPVersion = tokens[2];
				}

				this.Channel.P_IdleTimeoutMillis = IdleTimeoutMillis;

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

					if (!P_UTF8Check.Check(bytes))
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

					if (line[0] <= ' ') // HACK: ライン・フォルディング対応 -- フォルディングは廃止されたっぽい？
					{
						this.HeaderPairs[this.HeaderPairs.Count - 1][1] += line.Trim();
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

			public string GetHeaderValue(string name)
			{
				string[] pair = this.HeaderPairs.FirstOrDefault(v => EqualsIgnoreCase(v[0], name));

				if (pair == null)
					throw new Exception("No header key: " + name);

				return pair[1];
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

					if (1000 < key.Length || 1000 < value.Length) // rough limit
					{
						WriteLog("Ignore gen-header key and value (too long)");
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
				const int READ_SIZE_MAX = 2000000;

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
							throw new Exception("Bad chunk-size: " + size);

						if (BodySizeMax != -1 && BodySizeMax - buff.Count < size)
							throw new Exception($"Chunk is too big: {buff.Count} + {size}");

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
						throw new Exception("Bad Content-Length: " + this.ContentLength);

					if (BodySizeMax != -1 && BodySizeMax < this.ContentLength)
						throw new Exception("Body is too big: " + this.ContentLength);

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

			public int ResStatus = 200;
			public List<string[]> ResHeaderPairs = new List<string[]>();
			public IEnumerable<byte[]> ResBody = null;

			public IEnumerable<int> SendResponse()
			{
				this.Body = null;
				this.Channel.SessionTimeoutTime = TimeoutMillisToDateTime(ResponseTimeoutMillis);

				foreach (int relay in this.SendLine("HTTP/1.1 " + this.ResStatus + " Hello Happy World"))
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

					if (resBodyIterator.MoveNext())
					{
						byte[] first = resBodyIterator.Current;

						if (resBodyIterator.MoveNext())
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
							while (resBodyIterator.MoveNext());

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

		private class HTTPBodyOutputStream : IDisposable
		{
#if true
			public void Write(byte[] data)
			{
				throw new Exception("Request-Body is not supported");
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
#else
			private static string TMP_Dir = null;
			private string BuffFile;
			private int Size = 0;

			public HTTPBodyOutputStream()
			{
				if (TMP_Dir == null)
				{
					string dir = Environment.GetEnvironmentVariable("TMP");

					if (string.IsNullOrEmpty(dir))
						throw new Exception("TMP is empty");

					if (!Directory.Exists(dir))
						throw new Exception("TMP does not exists");

					TMP_Dir = dir;
				}
				this.BuffFile = Path.Combine(TMP_Dir, $"SimpleWebServer_{Guid.NewGuid().ToString()}.tmp");
				File.WriteAllBytes(this.BuffFile, new byte[0]);
			}

			public void Write(byte[] data)
			{
				using (FileStream writer = new FileStream(this.BuffFile, FileMode.Append, FileAccess.Write))
				{
					writer.Write(data, 0, data.Length);
				}
				this.Size += data.Length;
			}

			public int Count
			{
				get
				{
					return this.Size;
				}
			}

			public byte[] ToByteArray()
			{
				byte[] data = File.ReadAllBytes(this.BuffFile);

				File.WriteAllBytes(this.BuffFile, new byte[0]);
				this.Size = 0;

				return data;
			}

			public void Dispose()
			{
				if (this.BuffFile != null)
				{
					try
					{
						File.Delete(this.BuffFile);
					}
					catch
					{ }

					this.BuffFile = null;
				}
			}
#endif
		}

		private abstract class SockServer
		{
			/// <summary>
			/// サーバーロジック
			/// 通信量：
			/// -- 0 == 通信終了 -- Supplier の最後の要素の次以降 0 (default(int)) になるため
			/// -- 0 未満 == 通信無し
			/// -- 1 以上 == 通信有り
			/// </summary>
			/// <param name="channel">接続チャネル</param>
			/// <returns>通信量</returns>
			protected abstract IEnumerable<int> E_Connected(SockChannel channel);

			private List<SockChannel> Channels = new List<SockChannel>();

			public void Perform(int portNo, Func<bool> interlude)
			{
				WriteLog("SERVER STARTING...");

				try
				{
					using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
					{
						IPEndPoint endPoint = new IPEndPoint(0L, portNo);

						listener.Bind(endPoint);
						listener.Listen(Backlog);
						listener.Blocking = false;

						WriteLog("SERVER STARTED!");

						int waitMillis = 0;

						while (interlude())
						{
							if (waitMillis < 100)
								waitMillis++;

							for (int c = 0; c < 30; c++) // HACK: 繰り返し回数_適当
							{
								Socket handler = this.Channels.Count < ConnectMax ? this.Connect(listener) : null;

								if (handler == null) // ? 接続無し || 最大同時接続数に達している。
									break;

								waitMillis = 0; // reset

								TimeWaitMonitor.Connected();

								{
									SockChannel channel = new SockChannel();

									channel.Handler = handler;
									handler = null; // もう使わない。
									channel.Handler.Blocking = false;
									channel.ID = IDIssuer.Issue();
									channel.Connected = Supplier(this.E_Connected(channel));
									channel.BodyOutputStream = new HTTPBodyOutputStream();

									this.Channels.Add(channel);

									WriteLog("CONN-ST " + channel.ID);
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
									WriteLog(e);
									size = 0;
								}

								if (size == 0) // ? 切断
								{
									WriteLog("CONN-ED " + channel.ID);

									this.Disconnect(channel);
									this.Channels[index] = this.Channels[this.Channels.Count - 1];
									this.Channels.RemoveAt(this.Channels.Count - 1);

									TimeWaitMonitor.Disconnect();
								}
							}

							if (0 < waitMillis)
								Thread.Sleep(waitMillis);
						}

						WriteLog("SERVER ENDING...");

						this.Stop();
					}
				}
				catch (Exception e)
				{
					WriteLog(e);
				}

				WriteLog("SERVER ENDED!");
			}

			private Socket Connect(Socket listener) // ret: null == 接続タイムアウト
			{
				try
				{
					return listener.Accept();
				}
				catch (SocketException e)
				{
					if (e.ErrorCode != 10035)
					{
						throw new Exception("Connection error", e);
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
					WriteLog(e);
				}

				try
				{
					channel.Handler.Close();
				}
				catch (Exception e)
				{
					WriteLog(e);
				}

				channel.BodyOutputStream.Dispose();

				IDIssuer.Discard(channel.ID);
			}
		}

		private class SockChannel
		{
			public Socket Handler;
			public int ID;
			public Func<int> Connected;
			public HTTPBodyOutputStream BodyOutputStream;

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
			/// 無通信タイムアウト_ミリ秒
			/// -1 == INFINITE
			/// </summary>
			public int P_IdleTimeoutMillis = -1;

			private IEnumerable<int> PreRecvSend()
			{
				if (this.SessionTimeoutTime != null && this.SessionTimeoutTime.Value < DateTime.Now)
				{
					throw new Exception("Session time-out");
				}
				if (this.ThreadTimeoutTime == null)
				{
					if (ThreadTimeoutMillis != -1)
						this.ThreadTimeoutTime = DateTime.Now + TimeSpan.FromMilliseconds((double)ThreadTimeoutMillis);
				}
				else if (this.ThreadTimeoutTime.Value < DateTime.Now)
				{
					WriteLog("Thread time-out");

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
						int recvSize = this.Handler.Receive(data, offset, size, SocketFlags.None);

						if (recvSize <= 0)
						{
							throw new Exception("Receive error (disconnect)");
						}
						if (10.0 <= (DateTime.Now - startedTime).TotalSeconds) // 長い無通信時間をモニタする。
						{
							WriteLog("IDLE-RECV " + (DateTime.Now - startedTime).TotalSeconds.ToString("F3"));
						}
						a_return(recvSize);
						break;
					}
					catch (SocketException e)
					{
						if (e.ErrorCode != 10035)
						{
							throw new Exception("Receive error", e);
						}
					}
					if (this.P_IdleTimeoutMillis != -1 && this.P_IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						throw new Exception("Receive error (idle time-out)");
					}
					this.ThreadTimeoutTime = null;
					yield return -1;
				}
				yield return 1;
			}

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
						int sentSize = this.Handler.Send(data, offset, size, SocketFlags.None);

						if (sentSize <= 0)
						{
							throw new Exception("Send error (disconnect)");
						}
						if (10.0 <= (DateTime.Now - startedTime).TotalSeconds) // 長い無通信時間をモニタする。
						{
							WriteLog("IDLE-SEND " + (DateTime.Now - startedTime).TotalSeconds.ToString("F3"));
						}
						a_return(sentSize);
						break;
					}
					catch (SocketException e)
					{
						if (e.ErrorCode != 10035)
						{
							throw new Exception("Send error", e);
						}
					}
					if (this.P_IdleTimeoutMillis != -1 && this.P_IdleTimeoutMillis < (DateTime.Now - startedTime).TotalMilliseconds)
					{
						throw new Exception("Send error (idle time-out)");
					}
					this.ThreadTimeoutTime = null;
					yield return -1;
				}
				yield return 1;
			}
		}

		private static UTF8Check P_UTF8Check = new UTF8Check();

		private class UTF8Check
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

		private static class IDIssuer
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

		private static class TimeWaitMonitor
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

			private static int ConnectedCount = 0;
			private static int[] TWCounters = new int[COUNTER_NUM]; // 直近数分間に発生した切断(TIME_WAIT)の回数
			private static int TWCounterIndex = 0;
			private static DateTime NextRotateTime = GetNextRotateTime();

			public static void Connected()
			{
				KickCounter(1, 0);

				if (COUNT_LIMIT < ConnectedCount + TWCounters.Sum()) // ? TIME_WAIT 多すぎ -> 時間当たりの接続数を制限する。-- TIME_WAIT を減らす。
				{
					WriteLog("PORT-EXHAUSTION");

					Thread.Sleep(50); // HACK: 送受信も止める。
				}
			}

			public static void Disconnect()
			{
				KickCounter(-1, 1);
			}

			private static void KickCounter(int connAdd, int twAdd)
			{
				ConnectedCount += connAdd;

				if (NextRotateTime < DateTime.Now)
				{
					TWCounterIndex++;
					TWCounterIndex %= TWCounters.Length;
					TWCounters[TWCounterIndex] = twAdd;
					NextRotateTime = GetNextRotateTime();
				}
				else
					TWCounters[TWCounterIndex] += twAdd;

				WriteLog($"TIME-WAIT-MONITOR: {twAdd}, {ConnectedCount} + {TWCounters.Sum()} = {ConnectedCount + TWCounters.Sum()}");
			}

			private static DateTime GetNextRotateTime()
			{
				return DateTime.Now + TimeSpan.FromSeconds((double)CTR_ROT_SEC);
			}
		}

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
		private static string ToFairLocalPath(string str, int dirSize)
		{
			const int MY_PATH_MAX = 250;
			const string NG_CHARS = "\"*/:<>?\\|";
			const string ALT_WORD = "_";

			int localPathSizeMax = Math.Max(0, MY_PATH_MAX - dirSize);

			if (localPathSizeMax < str.Length) // HACK: 元にしたコードではバイト列の長さで判定している。
				str = str.Substring(0, localPathSizeMax);

			//str = ToJString(str);

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

		private static void WriteLog(object message)
		{
			if (message is Exception)
				message = ((Exception)message).Message;

			Console.WriteLine("[" + DateTime.Now + "] " + message);
		}

		private static Dictionary<string, V> CreateDictionaryIgnoreCase<V>()
		{
			return new Dictionary<string, V>(new IECompStringIgnoreCase());
		}

		private static Func<T> Supplier<T>(IEnumerable<T> src)
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

		private class IECompStringIgnoreCase : IEqualityComparer<string>
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

		private static bool EqualsIgnoreCase(string a, string b)
		{
			return a.ToLower() == b.ToLower();
		}

		private static bool ContainsIgnoreCase(string str, string ptn)
		{
			return str.ToLower().Contains(ptn.ToLower());
		}

		private static char SJISHanKanaToUnicode(byte chr)
		{
			return (char)((int)chr + 65216);
		}

#if true
		private static IEnumerable<char> GetUnicodeListOfSJISMBC()
		{
			#region RANGES_RESOURCE

			string RANGES_RESOURCE = @"

00a700a800b000b100b400b400b600b600d700d700f700f7039103a103a303a903b103c103c303c9040104010410044f04510451201020102015201520182019
201c201d20202021202520262030203020322033203b203b210321032116211621212121212b212b21602169217021792190219321d221d221d421d422002200
2202220322072208220b220b22112211221a221a221d2220222522252227222c222e222e22342235223d223d225222522260226122662267226a226b22822283
2286228722a522a522bf22bf231223122460247325002503250c250c250f25102513251425172518251b251d252025202523252525282528252b252c252f2530
2533253425372538253b253c253f253f25422542254b254b25a025a125b225b325bc25bd25c625c725cb25cb25ce25cf25ef25ef260526062640264026422642
266a266a266d266d266f266f3000300330053015301d301d301f301f30413093309b309e30a130f630fb30fe323132323239323932a432a833033303330d330d
33143314331833183322332333263327332b332b33363336333b333b3349334a334d334d3351335133573357337b337e338e338f339c339e33a133a133c433c4
33cd33cd4e004e014e034e034e074e0b4e0d4e0e4e104e114e144e194e1e4e1e4e214e214e264e264e284e284e2a4e2a4e2d4e2d4e314e324e364e364e384e39
4e3b4e3c4e3f4e3f4e424e434e454e454e4b4e4b4e4d4e4f4e554e594e5d4e5f4e624e624e714e714e734e734e7e4e7e4e804e804e824e824e854e864e884e8c
4e8e4e8e4e914e924e944e954e984e994e9b4e9c4e9e4ea24ea44ea64ea84ea84eab4eae4eb04eb04eb34eb34eb64eb64eba4eba4ec04ec24ec44ec44ec64ec7
4eca4ecb4ecd4ecf4ed44ed94edd4edf4ee14ee14ee34ee54eed4eee4ef04ef04ef24ef24ef64ef74efb4efc4f004f014f034f034f094f0a4f0d4f114f1a4f1a
4f1c4f1d4f2f4f304f344f344f364f364f384f3a4f3c4f3d4f434f434f464f474f4d4f514f534f534f554f574f594f5e4f694f694f6f4f704f734f734f754f76
4f7b4f7c4f7f4f7f4f834f834f864f864f884f884f8a4f8b4f8d4f8d4f8f4f8f4f914f924f944f944f964f964f984f984f9a4f9b4f9d4f9d4fa04fa14fab4fab
4fad4faf4fb54fb64fbf4fbf4fc24fc44fc94fca4fcd4fce4fd04fd14fd44fd44fd74fd84fda4fdb4fdd4fdd4fdf4fdf4fe14fe14fe34fe54fee4fef4ff34ff3
4ff54ff64ff84ff84ffa4ffa4ffe4fff5005500650095009500b500b500d500d500f500f5011501250145014501650165019501a501e501f502150265028502d
5036503650395039504050405042504350465049504f505050555056505a505a505c505c50655065506c506c50705070507250725074507650785078507d507d
5080508050855085508d508d50915091509450945098509a50ac50ad50b250b550b750b750be50be50c250c250c550c550c950ca50cd50cd50cf50cf50d150d1
50d550d650d850d850da50da50de50de50e350e350e550e550e750e750ed50ee50f450f550f950f950fb50fb5100510251045104510951095112511251145116
51185118511a511a511f511f51215121512a512a5132513251375137513a513c513f51415143514e515051505152515251545154515a515a515c515c51625162
516451655168516e5171517151755178517c517c5180518051825182518551865189518a518c518d518f51935195519751995199519d519d51a051a051a251a2
51a451a651a851ac51b051b751bd51be51c451c651c951c951cb51cd51d651d651db51dd51e051e151e651e751e951ea51ec51ed51f051f151f551f651f851fa
51fd51fe520052005203520452065208520a520b520e520e521152115214521552175217521d521d52245225522752275229522a522e522e5230523052335233
5236523b5243524452475247524a524d524f524f5254525452565256525b525b525e525e526352655269526a526f5275527d527d527f527f5283528352875289
528d528d5291529252945294529b529c529f52a052a352a352a652a652a952ad52af52af52b152b152b452b552b952b952bc52bc52be52be52c052c152c352c3
52c552c552c752c752c952c952cd52cd52d252d252d552d552d752d952db52db52dd52e052e252e452e652e752f252f352f552f552f852fa52fe530253055308
530d530d530f5310531553175319531a531d531d5320532153235324532a532a532f532f53315331533353335338533b533f5341534353435345534a534d534d
5351535453575358535a535a535c535c535e535e536053605366536653695369536e537553775378537b537b537f537f53825382538453845393539353965396
53985398539a539a539f53a053a553a653a853a953ad53ae53b053b053b253b353b653b653bb53bb53c253c353c853ce53d453d453d653d753d953d953db53db
53dd53dd53df53df53e153e553e853f353f653f853fa53fa540154015403540454085411541b541b541d541d541f54205426542654295429542b542e54365436
54385439543b543e5440544054425442544654465448544a544e544e54515451545f545f54685468546a546a547054715473547354755477547b547d54805480
5484548454865486548a548c548e549054925492549c549c54a254a254a454a554a854a954ab54ac54af54af54b254b354b854b854bc54be54c054c254c454c4
54c754c954d854d854e154e254e554e654e854e954ed54ee54f254f254fa54fa54fd54fd54ff54ff5504550455065507550f55105514551455165516552e552f
553155315533553355385539553e553e5540554055445546554c554c554f554f5553555355565557555c555d55635563557b557c557e557e5580558055835584
558655875589558b5598559a559c559f55a755ac55ae55ae55b055b055b655b655c455c555c755c755d455d455da55da55dc55dc55df55df55e355e455f755f7
55f955f955fd55fe56065606560956095614561456165618561b561b56295629562f562f5631563256345634563656365638563856425642564c564c564e564e
56505650565b565b5664566456685668566a566c5674567456785678567a567a5680568056865687568a568a568f568f5694569456a056a056a256a256a556a5
56ae56ae56b456b456b656b656bc56bc56c056c356c856c856ce56ce56d156d156d356d356d756d856da56db56de56de56e056e056e356e356ee56ee56f056f0
56f256f356f956fa56fd56fd56ff57005703570457085709570b570b570d570d570f570f571257135716571657185718571c571c571f571f57265728572d572d
5730573057375738573b573b574057405742574257475747574a574a574e57515759575957615761576457665769576a577f577f5782578257885789578b578b
5793579357a057a057a257a457aa57aa57ac57ac57b057b057b357b357c057c057c357c357c657c857cb57cb57ce57ce57d257d457d657d657dc57dc57df57e0
57e357e357f457f457f757f757f957fa57fc57fc580058005802580258055806580a580b5815581558195819581d581d5821582158245824582a582a582f5831
58345835583a583a583d583d58405841584a584b58515852585458545857585a585e585e5862586258695869586b586b58705870587258725875587558795879
587e587e58835883588558855893589358975897589c589c589e589f58a858a858ab58ab58ae58ae58b258b358b858bb58be58be58c158c158c558c558c758c7
58ca58ca58cc58cc58d158d158d358d358d558d558d758d958dc58dc58de58df58e458e558eb58ec58ee58f258f758f758f958fd590259025909590b590f5910
591559165918591c5922592259255925592759275929592e5931593259375938593e593e5944594459475949594e59515953595559575958595a595b595d595d
5960596059625963596559655967596a596c596c596e596e5973597459785978597d597d59815984598a598a598d598d599359935996599659995999599b599b
599d599d59a359a559a859a859ac59ac59b259b259b959bb59be59be59c659c659c959c959cb59cb59d059d159d359d459d959da59dc59dc59e559e659e859e8
59ea59eb59f659f659fb59fb59ff59ff5a015a015a035a035a095a095a115a115a185a185a1a5a1a5a1c5a1c5a1f5a205a255a255a295a295a2f5a2f5a355a36
5a3c5a3c5a405a415a465a465a495a495a5a5a5a5a625a625a665a665a6a5a6a5a6c5a6c5a7f5a7f5a925a925a9a5a9b5abc5abe5ac15ac25ac95ac95acb5acc
5ad05ad05ad65ad75ae15ae15ae35ae35ae65ae65ae95ae95afa5afb5b095b095b0b5b0c5b165b165b225b225b2a5b2a5b2c5b2c5b305b305b325b325b365b36
5b3e5b3e5b405b405b435b435b455b455b505b515b545b585b5a5b5d5b5f5b5f5b635b665b695b695b6b5b6b5b705b715b735b735b755b755b785b785b7a5b7a
5b805b805b835b835b855b855b875b895b8b5b8d5b8f5b8f5b955b955b975b9d5b9f5b9f5ba25ba65bae5bae5bb05bb05bb35bb65bb85bb95bbf5bc05bc25bc7
5bc95bc95bcc5bcc5bd05bd05bd25bd45bd85bd85bdb5bdb5bdd5bdf5be15be25be45be95beb5bec5bee5bee5bf05bf05bf35bf35bf55bf65bf85bf85bfa5bfa
5bfe5bff5c015c025c045c0b5c0d5c0f5c115c115c135c135c165c165c1a5c1a5c1e5c1e5c205c205c225c225c245c245c285c285c2d5c2d5c315c315c385c41
5c455c465c485c485c4a5c4b5c4d5c515c535c535c555c555c5e5c5e5c605c615c645c655c6c5c6c5c6e5c6f5c715c715c765c765c795c795c8c5c8c5c905c91
5c945c945ca15ca15ca65ca65ca85ca95cab5cac5cb15cb15cb35cb35cb65cb85cba5cbc5cbe5cbe5cc55cc55cc75cc75cd95cd95ce05ce15ce85cea5ced5ced
5cef5cf05cf55cf65cfa5cfb5cfd5cfd5d075d075d0b5d0b5d0e5d0e5d115d115d145d1b5d1f5d1f5d225d225d275d275d295d295d425d425d4b5d4c5d4e5d4e
5d505d505d525d535d5c5d5c5d695d695d6c5d6d5d6f5d6f5d735d735d765d765d825d825d845d845d875d875d8b5d8c5d905d905d9d5d9d5da25da25dac5dac
5dae5dae5db75dba5dbc5dbd5dc95dc95dcc5dcd5dd05dd05dd25dd35dd65dd65ddb5ddb5ddd5dde5de15de15de35de35de55de85deb5deb5dee5dee5df15df5
5df75df75dfb5dfb5dfd5dfe5e025e035e065e065e0b5e0c5e115e115e165e165e195e1b5e1d5e1d5e255e255e2b5e2b5e2d5e2d5e2f5e305e335e335e365e38
5e3d5e3d5e405e405e435e455e475e475e4c5e4c5e4e5e4e5e545e555e575e575e5f5e5f5e615e645e725e765e785e7f5e815e815e835e845e875e875e8a5e8a
5e8f5e8f5e955e975e9a5e9a5e9c5e9c5ea05ea05ea65ea75eab5eab5ead5ead5eb55eb85ec15ec35ec85eca5ecf5ed05ed35ed35ed65ed65eda5edb5edd5edd
5edf5ee35ee85ee95eec5eec5ef05ef15ef35ef45ef65ef85efa5efc5efe5eff5f015f015f035f045f095f0d5f0f5f115f135f185f1b5f1b5f1f5f1f5f215f21
5f255f275f295f295f2d5f2d5f2f5f2f5f315f315f345f355f375f385f3c5f3c5f3e5f3e5f415f415f455f455f485f485f4a5f4a5f4c5f4c5f4e5f4e5f515f51
5f535f535f565f575f595f595f5c5f5d5f615f625f665f675f695f6d5f705f715f735f735f775f775f795f795f7c5f7c5f7f5f855f875f885f8a5f8c5f905f93
5f975f995f9e5f9e5fa05fa15fa85faa5fad5fae5fb35fb45fb75fb75fb95fb95fbc5fbd5fc35fc35fc55fc55fcc5fcd5fd65fd95fdc5fde5fe05fe05fe45fe4
5feb5feb5ff05ff15ff55ff55ff85ff85ffb5ffb5ffd5ffd5fff5fff600e6010601260126015601660196019601b601d602060216025602b602f602f60316031
603a603a6041604360466046604a604b604d604d6050605060526052605560556059605a605d605d605f6060606260656068606d606f60706075607560776077
60816081608360856089608d609260926094609460966097609a609b609f60a060a360a360a660a760a960aa60b260b660b860b860bc60bd60c560c760d160d1
60d360d360d560d560d860d860da60da60dc60dc60de60e160e360e360e760e860f060f460f660f760f960fb61006101610361036106610661086109610d610f
6111611161156115611a611b611f612161276128612c612c613061306134613461376137613c613f614261426144614461476148614a614e6153615361556155
6158615a615d615d615f615f616261636165616561676168616b616b616e617161736177617e617e6182618261876187618a618a618e618e6190619161946194
619661966198619a61a461a461a761a761a961a961ab61ac61ae61ae61b261b261b661b661ba61ba61be61be61c361c361c661cd61d061d061e361e361e661e6
61f261f261f461f461f661f861fa61fa61fc62006208620a620c620e6210621462166216621a621b621d621f6221622162266226622a622a622e623062326234
62386238623b623b623f624162476249624b624b624d624e625362536255625562586258625b625b625e625e626062606263626362686268626e626e62716271
6276627662796279627c627c627e6280628262846289628a62916298629b629c629e629e62a662a662ab62ac62b162b162b562b562b962b962bb62bd62c262c2
62c562ca62cc62cd62cf62d462d762d962db62dd62e062e162ec62ef62f162f162f362f362f562f762fe62ff6301630263076309630c630c6311631163196319
631f631f63276328632b632b632f632f633a633a633d633f63496349634c634d634f63506355635563576357635c635c63676369636b636b636e636e63726372
63766377637a637b638063806383638363886389638c638c638e638f639263926396639663986398639b639b639f63a363a563a563a763ac63b263b263b463b5
63bb63bb63be63be63c063c063c363c463c663c663c963c963cf63d063d263d263d663d663da63db63e163e163e363e363e963e963ee63ee63f463f663fa63fa
64066406640d640d640f640f6413641364166417641c641c6426642664286428642c642d6434643464366436643a643a643e643e64426442644e644e64586458
646064606467646764696469646f646f6476647664786478647a647a64836483648864886492649364956495649a649a649d649e64a464a564a964a964ab64ab
64ad64ae64b064b064b264b264b964b964bb64bc64c164c264c564c564c764c764cd64ce64d264d264d464d464d864d864da64da64e064e364e664e764ec64ec
64ef64ef64f164f264f464f464f664f664fa64fa64fd64fe650065006505650565186518651c651d65236524652a652c652f652f65346539653b653b653e653f
6545654565486548654d654f6551655165556559655d655e6562656365666566656c656c657065706572657265746575657765786582658365876589658c658c
658e658e659065916597659765996599659b659c659f659f65a165a165a465a565a765a765ab65ad65af65b065b765b765b965b965bc65bd65c165c165c365c6
65cb65cc65cf65cf65d265d265d765d765d965d965db65db65e065e265e565e965ec65ed65f165f165fa65fb6600660066026603660666076609660a660c660c
660e660f66136615661c661c661e66206624662566276628662d662f6631663166346636663b663c663f663f6641664466496649664b664b664f664f66526652
6657665766596659665d665f6662666266646669666e66706673667466766676667a667a668166816683668466876689668e668e6691669166966699669d669d
66a066a066a266a266a666a666ab66ab66ae66ae66b266b266b466b466b866b966bc66bc66be66bf66c166c166c466c466c766c766c966c966d666d666d966da
66dc66dd66e066e066e666e666e966e966f066f066f266f566f767006703670367086709670b670b670d670f67146717671b671b671d671f67266728672a672e
673167316734673467366738673a673a673d673d673f673f674167416746674667496749674e6751675367536756675667596759675c675c675e6766676a676a
676d676d676f67736775677567776777677c677c677e677f678567856787678767896789678b678c679067906795679567976797679a679a679c679d67a067a2
67a667a667a967a967af67af67b367b467b667b967bb67bb67c067c167c467c467c667c667ca67ca67ce67d167d367d467d867d867da67da67dd67de67e267e2
67e467e467e767e767e967e967ec67ec67ee67ef67f167f167f367f567fb67fb67fe67ff680168046813681368166817681e681e682168226829682b68326832
6834683468386839683c683d684068446846684668486848684d684e6850685468596859685c685d685f685f68636863686768676874687468766877687e687f
688168816883688368856885688d688d688f688f6893689468976897689b689b689d689d689f68a068a268a268a668a868ad68ad68af68b168b368b368b568b6
68b968ba68bc68bc68c468c468c668c668c868cb68cd68cd68cf68cf68d268d268d468d568d768d868da68da68df68e168e368e368e768e768ee68ef68f268f2
68f968fa690069016904690569086908690b690f691269126919691c692169236925692669286928692a692a69306930693469346936693669396939693d693d
693f693f694a694a695369556959695a695c695e6960696269686968696a696b696d696f6973697569776979697c697e69816982698a698a698e698e69916991
6994699569986998699b699c69a069a069a769a769ae69ae69b169b269b469b469bb69bb69be69bf69c169c169c369c369c769c769ca69ce69d069d069d369d3
69d869d969dd69de69e269e269e769e869eb69eb69ed69ed69f269f269f969f969fb69fb69fd69fd69ff69ff6a026a026a056a056a0a6a0c6a126a146a176a17
6a196a196a1b6a1b6a1e6a1f6a216a236a296a2b6a2e6a2e6a306a306a356a366a386a3a6a3d6a3d6a446a446a466a486a4b6a4b6a586a596a5f6a5f6a616a62
6a666a666a6b6a6b6a726a736a786a786a7e6a806a846a846a8d6a8e6a906a906a976a976a9c6a9c6aa06aa06aa26aa36aaa6aaa6aac6aac6aae6aae6ab36ab3
6ab86ab86abb6abb6ac16ac36ad16ad16ad36ad36ada6adb6ade6adf6ae26ae26ae46ae46ae86ae86aea6aea6afa6afb6b046b056b0a6b0a6b126b126b166b16
6b1d6b1d6b1f6b216b236b236b276b276b326b326b376b3a6b3d6b3e6b436b436b476b476b496b496b4c6b4c6b4e6b4e6b506b506b536b546b596b596b5b6b5b
6b5f6b5f6b616b646b666b666b696b6a6b6f6b6f6b736b746b786b796b7b6b7b6b7f6b806b836b846b866b866b896b8b6b8d6b8d6b956b966b986b986b9e6b9e
6ba46ba46baa6bab6baf6baf6bb16bb56bb76bb76bba6bbc6bbf6bc06bc56bc66bcb6bcb6bcd6bce6bd26bd46bd66bd66bd86bd86bdb6bdb6bdf6bdf6beb6bec
6bef6bef6bf36bf36c086c086c0f6c0f6c116c116c136c146c176c176c1b6c1b6c236c246c346c346c376c386c3e6c426c4e6c4e6c506c506c556c556c576c57
6c5a6c5a6c5c6c606c626c626c686c686c6a6c6a6c6f6c706c726c736c7a6c7a6c7d6c7e6c816c836c866c866c886c886c8c6c8d6c906c906c926c936c966c96
6c996c9b6ca16ca26cab6cab6cae6cae6cb16cb16cb36cb36cb86cbf6cc16cc16cc46cc56cc96cca6ccc6ccc6cd36cd36cd56cd56cd76cd76cd96cdb6cdd6cdd
6ce16ce36ce56ce56ce86ce86cea6cea6cef6cf16cf36cf36d046d046d0b6d0c6d126d126d176d176d196d196d1b6d1b6d1e6d1f6d256d256d296d2b6d326d33
6d356d366d386d386d3b6d3b6d3d6d3e6d416d416d446d456d596d5a6d5c6d5c6d636d646d666d666d696d6a6d6c6d6c6d6e6d6f6d746d746d776d796d856d85
6d876d886d8c6d8c6d8e6d8e6d936d936d956d966d996d996d9b6d9c6dac6dac6daf6daf6db26db26db56db56db86db86dbc6dbc6dc06dc06dc56dc76dcb6dcc
6dcf6dcf6dd16dd26dd56dd56dd86dd96dde6dde6de16de16de46de46de66de66de86de86dea6dec6dee6dee6df16df36df56df56df76dfc6e056e056e076e0b
6e136e136e156e156e196e1b6e1d6e1d6e1f6e216e236e276e296e296e2b6e2f6e386e3a6e3c6e3c6e3e6e3e6e436e436e4a6e4a6e4d6e4e6e566e566e586e58
6e5b6e5c6e5f6e5f6e676e676e6b6e6b6e6e6e6f6e726e726e766e766e7e6e806e826e826e8c6e8c6e8f6e906e966e966e986e986e9c6e9d6e9f6e9f6ea26ea2
6ea56ea56eaa6eaa6eaf6eaf6eb26eb26eb66eb76eba6eba6ebd6ebd6ebf6ebf6ec26ec26ec46ec56ec96ec96ecb6ecc6ed16ed16ed36ed56edd6ede6eec6eec
6eef6eef6ef26ef26ef46ef46ef76ef86efe6eff6f016f026f066f066f096f096f0f6f0f6f116f116f136f156f206f206f226f236f2b6f2c6f316f326f386f38
6f3e6f3f6f416f416f456f456f546f546f586f586f5b6f5c6f5f6f5f6f646f646f666f666f6d6f706f746f746f786f786f7a6f7a6f7c6f7c6f806f826f846f84
6f866f866f886f886f8e6f8e6f916f916f976f976fa16fa16fa36fa46faa6faa6fb16fb16fb36fb36fb56fb56fb96fb96fc06fc36fc66fc66fd46fd56fd86fd8
6fdb6fdb6fdf6fe16fe46fe46feb6fec6fee6fef6ff16ff16ff36ff36ff56ff66ffa6ffa6ffe6ffe70017001700570057007700770097009700b700b700f700f
701170117015701570187018701a701b701d701f70267028702c702c7030703070327032703e703e704c704c705170517058705870637063706b706b706f7070
70787078707c707d708570857089708a708e708e709270927099709970ab70af70b370b370b870bb70c870c870cb70cb70cf70cf70d970d970dd70dd70df70df
70f170f170f970f970fd70fd7104710471097109710f710f711471147119711a711c711c712171217126712671367136713c713c7146714771497149714c714c
714e714e7155715671597159715c715c716271627164716771697169716c716c716e716e717d717d7184718471887188718a718a718f718f7194719571997199
719f719f71a871a871ac71ac71b171b171b971b971be71be71c171c171c371c371c871c971ce71ce71d071d071d271d271d471d571d771d771df71e071e571e7
71ec71ee71f571f571f971f971fb71fc71fe71ff72067206720d720d72107210721b721b72287228722a722a722c722d723072307232723272357236723a7240
72467248724b724c7252725272587259725b725b725d725d725f725f726172627267726772697269727272727274727472797279727d727e7280728272877287
729272927296729672a072a072a272a272a772a772ac72ac72af72af72b172b272b672b672b972b972be72be72c272c472c672c672ce72ce72d072d072d272d2
72d772d772d972d972db72db72e072e272e972e972ec72ed72f772f972fc72fd730a730a73167317731b731d731f731f732473257329732b732e732f73347334
73367337733e733f73447345734e734f735773577363736373687368736a736a73707370737273727375737573777378737a737b738473847387738773897389
738b738b7396739673a973a973b273b373bb73bb73bd73bd73c073c073c273c273c873ca73cd73ce73d273d273d673d673de73de73e073e073e373e373e573e5
73ea73ea73ed73ee73f173f173f573f573f873f873fe73fe74037403740574077409740974227422742574267429742a742e742e74327436743a743a743f743f
74417441745574557459745c745e7460746274647469746a746f74707473747374767476747e747e7483748374897489748b748b749e749f74a274a274a774a7
74b074b074bd74bd74ca74ca74cf74cf74d474d474dc74dc74e074e074e274e374e674e774e974e974ee74ee74f074f274f674f87501750175037505750c750e
75117511751375137515751575187518751a751a751c751c751e751f752375237525752675287528752b752c752f753375377538753a753c7544754475467546
7549754d754f754f75517551755475547559755d7560756075627562756475677569756b756d756d756f75707573757475767578757f757f7582758275867587
7589758b758e758f7591759175947594759a759a759d759d75a375a375a575a575ab75ab75b175b375b575b575b875b975bc75be75c275c375c575c575c775c7
75ca75ca75cd75cd75d275d275d475d575d875d975db75db75de75de75e275e375e975e975f075f075f275f475fa75fa75fc75fc75fe75ff7601760176097609
760b760b760d760d761f762276247624762776277630763076347634763b763b7642764276467648764c764c765276527656765676587658765c765c76617662
7667766a766c766c76707670767276727676767676787678767a767e768076807682768476867688768b768b768e768e7690769076937693769676967699769c
769e769e76a676a676ae76ae76b076b076b476b476b776ba76bf76bf76c276c376c676c676c876c876ca76ca76cd76cd76d276d276d676d776db76dc76de76df
76e176e176e376e576e776e776ea76ea76ee76ee76f276f276f476f476f876f876fb76fb76fe76fe770177017704770477077709770b770c771b771b771e7720
772477267729772977377738773a773a773c773c7740774077467747775a775b77617761776377637765776677687768776b776b77797779777e777f778b778b
778e778e77917791779e779e77a077a077a577a577ac77ad77b077b077b377b377b677b677b977b977bb77bd77bf77bf77c777c777cd77cd77d777d777da77dc
77e277e377e577e577e777e777e977e977ed77ef77f377f377fc77fc78027802780c780c781278127814781578207821782578277832783278347834783a783a
783f783f78457845784e784e785d785d78647864786b786c786f786f7872787278747874787a787a787c787c7881788178867887788c788e7891789178937893
7895789578977897789a789a78a378a378a778a778a978aa78af78af78b578b578ba78ba78bc78bc78be78be78c178c178c578c678ca78cb78d078d178d478d4
78da78da78e778e878ec78ec78ef78ef78f478f478fd78fd7901790179077907790e790e791179127919791979267926792a792c79307930793a793a793c793c
793e793e7940794179477949795079507953795379557957795a795a795d7960796279627965796579687968796d796d79777977797a797a797f798179847985
798a798a798d798f79947994799b799b799d799d79a679a779aa79aa79ae79ae79b079b079b379b379b979ba79bd79c179c979c979cb79cb79d179d279d579d5
79d879d879df79df79e179e179e379e479e679e779e979e979ec79ec79f079f079fb79fb7a007a007a087a087a0b7a0b7a0d7a0e7a147a147a177a1a7a1c7a1c
7a1f7a207a2e7a2e7a317a327a377a377a3b7a407a427a437a467a467a497a497a4d7a507a577a577a617a637a697a697a6b7a6b7a707a707a747a747a767a76
7a797a7a7a7d7a7d7a7f7a7f7a817a817a837a847a887a887a927a937a957a987a9f7a9f7aa97aaa7aae7ab07ab67ab67aba7aba7abf7abf7ac37ac57ac77ac8
7aca7acb7acd7acd7acf7acf7ad17ad37ad57ad57ad97ada7adc7add7adf7ae37ae57ae77aea7aeb7aed7aed7aef7af07af67af67af87afa7aff7aff7b027b02
7b047b047b067b067b087b087b0a7b0b7b0f7b0f7b117b117b187b197b1b7b1b7b1e7b1e7b207b207b257b267b287b287b2c7b2c7b337b337b357b367b397b39
7b457b467b487b497b4b7b4d7b4f7b527b547b547b567b567b5d7b5d7b657b657b677b677b6c7b6c7b6e7b6e7b707b717b747b757b7a7b7a7b867b877b8b7b8b
7b8d7b8d7b8f7b8f7b927b927b947b957b977b9a7b9c7b9f7ba17ba17baa7baa7bad7bad7bb17bb17bb47bb47bb87bb87bc07bc17bc47bc47bc67bc77bc97bc9
7bcb7bcc7bcf7bcf7bdd7bdd7be07be07be47be67be97be97bed7bed7bf37bf37bf67bf77c007c007c077c077c0d7c0d7c117c147c177c177c1f7c1f7c217c21
7c237c237c277c277c2a7c2b7c377c387c3d7c407c437c437c4c7c4d7c4f7c507c547c547c567c567c587c587c5f7c607c647c657c6c7c6c7c737c737c757c75
7c7e7c7e7c817c837c897c897c8b7c8b7c8d7c8d7c907c907c927c927c957c957c977c987c9b7c9b7c9f7c9f7ca17ca27ca47ca57ca77ca87cab7cab7cad7cae
7cb17cb37cb97cb97cbd7cbe7cc07cc07cc27cc27cc57cc57cca7cca7cce7cce7cd27cd27cd67cd67cd87cd87cdc7cdc7cde7ce07ce27ce27ce77ce77cef7cef
7cf27cf27cf47cf47cf67cf67cf87cf87cfa7cfb7cfe7cfe7d007d007d027d027d047d067d0a7d0b7d0d7d0d7d107d107d147d157d177d1c7d207d227d2b7d2c
7d2e7d307d327d337d357d357d397d3a7d3f7d3f7d427d467d487d487d4b7d4c7d4e7d507d567d567d5b7d5c7d5e7d5e7d617d637d667d667d687d687d6e7d6e
7d717d737d757d767d797d797d7d7d7d7d897d897d8f7d8f7d937d937d997d9c7d9f7da07da27da37dab7db27db47db57db77db87dba7dbb7dbd7dbf7dc77dc7
7dca7dcb7dcf7dcf7dd17dd27dd57dd67dd87dd87dda7dda7ddc7dde7de07de17de47de47de87de97dec7dec7def7def7df27df27df47df47dfb7dfb7e017e01
7e047e057e097e0b7e127e127e1b7e1b7e1e7e1f7e217e237e267e267e2b7e2b7e2e7e2e7e317e327e357e357e377e377e397e3b7e3d7e3e7e417e417e437e43
7e467e467e4a7e4b7e4d7e4d7e527e527e547e567e597e5a7e5d7e5e7e667e677e697e6a7e6d7e6d7e707e707e797e797e7b7e7d7e7f7e7f7e827e837e887e8a
7e8c7e8c7e8e7e907e927e947e967e967e9b7e9c7f367f367f387f387f3a7f3a7f457f457f477f477f4c7f4e7f507f517f547f557f587f587f5f7f607f677f6b
7f6e7f6e7f707f707f727f727f757f757f777f797f827f837f857f887f8a7f8a7f8c7f8c7f8e7f8e7f947f947f9a7f9a7f9d7f9e7fa17fa17fa37fa47fa87fa9
7fae7faf7fb27fb27fb67fb67fb87fb97fbd7fbd7fc17fc17fc57fc67fca7fca7fcc7fcc7fd27fd27fd47fd57fe07fe17fe67fe67fe97fe97feb7feb7ff07ff0
7ff37ff37ff97ff97ffb7ffc8000800180038006800b800c80108010801280128015801580178019801c801c80218021802880288033803380368036803b803b
803d803d803f803f80468046804a804a805280528056805680588058805a805a805e805f8061806280688068806f8070807280748076807780798079807d807f
8084808780898089808b808c809380938096809680988098809a809b809d809d80a180a280a580a580a980aa80ac80ad80af80af80b180b280b480b480ba80ba
80c380c480c680c680cc80cc80ce80ce80d680d680d980db80dd80de80e180e180e480e580ef80ef80f180f180f480f480f880f880fc80fd810281028105810a
811a811b8123812381298129812f812f813181318133813381398139813e813e81468146814b814b814e814e8150815181538155815f815f81658166816b816b
816e816e81708171817481748178817a817f81808182818381888188818a818a818f818f8193819381958195819a819a819c819d81a081a081a381a481a881a9
81b081b081b381b381b581b581b881b881ba81ba81bd81c081c281c281c681c681c881c981cd81cd81d181d181d381d381d881da81df81e081e381e381e581e5
81e781e881ea81ea81ed81ed81f381f481fa81fc81fe81fe82018202820582058207820a820c820e821082108212821282168218821b821c821e821f8229822c
822e822e823382338235823982408240824782478258825a825d825d825f825f82628262826482648266826682688268826a826b826e826f8271827282768278
827e827e828b828b828d828d8292829282998299829d829d829f829f82a582a682ab82ad82af82af82b182b182b382b382b882b982bb82bb82bd82bd82c582c5
82d182d482d782d782d982d982db82dc82de82df82e182e182e382e382e582e782eb82eb82f182f182f382f482f982fb8301830683098309830e830e83168318
831c831c8323832383288328832b832b832f832f83318332833483368338833983408340834583458349834a834f835083528352835883588362836283738373
8375837583778377837b837c837f837f83858385838783878389838a838e838e8393839383968396839a839a839e83a083a283a283a883a883aa83ab83b183b1
83b583b583bd83bd83c183c183c583c583c783c783ca83ca83cc83cc83ce83ce83d383d383d683d683d883d883dc83dc83df83e083e983e983eb83eb83ef83f2
83f483f483f683f783fb83fb83fd83fd8403840484078407840b840e8413841384208420842284228429842a842c842c843184318435843584388438843c843d
8446844684488449844e844e84578457845b845b846184638466846684698469846b846f8471847184758475847784778479847a8482848284848484848b848b
849084908494849484998499849c849c849f849f84a184a184ad84ad84b284b284b484b484b884b984bb84bc84bf84bf84c184c184c484c484c684c684c984cb
84cd84cd84d084d184d684d684d984da84dc84dc84ec84ec84ee84ee84f484f484fc84fc84ff850085068506851185118513851585178518851a851a851f851f
8521852185268526852c852d85358535853d853d85408541854385438548854b854e854e85538553855585558557855a856385638568856b856d856d85778577
857e857e858085808584858485878588858a858a85908591859485948597859785998599859b859c85a485a485a685a685a885ac85ae85b085b985ba85c185c1
85c985c985cd85cd85cf85d085d585d585dc85dd85e485e585e985ea85f785f785f985fb85fe85fe8602860286068607860a860b8613861386168617861a861a
86228622862d862d862f8630863f863f864d864e8650865086548655865a865a865c865c865e865f86678667866b866b8671867186798679867b867b868a868c
869386938695869586a386a486a986ab86af86b086b686b686c486c486c686c786c986c986cb86cb86cd86ce86d486d486d986d986db86db86de86df86e486e4
86e986e986ec86ef86f886f986fb86fb86fe86fe8700870087028703870687068708870a870d870d8711871287188718871a871a871c871c8725872587298729
8734873487378737873b873b873f873f87498749874b874c874e874e87538753875587558757875787598759875f8760876387638766876687688768876a876a
876e876e877487748776877687788778877f877f87828782878d878d879f879f87a287a287ab87ab87af87af87b387b387ba87bb87bd87bd87c087c087c487c4
87c687c787cb87cb87d087d087d287d287e087e087ef87ef87f287f287f687f787f987f987fb87fb87fe87fe8805880588078807880d880f8811881188158816
8821882388278827883188318836883688398839883b883b88408840884288428844884488468846884c884d885288538857885788598859885b885b885d885e
8861886388688868886b886b88708870887288728875887588778877887d887f8881888288888888888b888b888d888d889288928896889788998899889e889e
88a288a288a488a488ab88ab88ae88ae88b088b188b488b588b788b788bf88bf88c188c588cf88cf88d488d588d888d988dc88dd88df88df88e188e188e888e8
88f288f588f888f988fc88fe890289028904890489078907890a890a890c890c8910891089128913891c891e89258925892a892b8936893689388938893b893b
8941894189438944894c894d89568956895e89608964896489668966896a896a896d896d896f896f897289728974897489778977897e897f8981898189838983
89868988898a898b898f898f8993899389968998899a899a89a189a189a689a789a989aa89ac89ac89af89af89b289b389ba89ba89bd89bd89bf89c089d289d2
89da89da89dc89dd89e389e389e689e789f489f489f889f88a008a008a028a038a088a088a0a8a0a8a0c8a0c8a0e8a0e8a108a108a128a138a168a188a1b8a1b
8a1d8a1d8a1f8a1f8a238a238a258a258a2a8a2a8a2d8a2d8a318a318a338a348a368a378a3a8a3c8a418a418a468a468a488a488a508a528a548a558a5b8a5b
8a5e8a5e8a608a608a628a638a668a668a698a698a6b8a6e8a708a738a798a798a7c8a7c8a828a828a848a858a878a878a898a898a8c8a8d8a918a918a938a93
8a958a958a988a988a9a8a9a8a9e8a9e8aa08aa18aa38aa88aac8aad8ab08ab08ab28ab28ab98ab98abc8abc8abe8abf8ac28ac28ac48ac48ac78ac78acb8acd
8acf8acf8ad28ad28ad68ad68ada8adc8ade8ae28ae48ae48ae68ae78aeb8aeb8aed8aee8af18af18af38af38af68af88afa8afa8afe8afe8b008b028b048b04
8b078b078b0c8b0c8b0e8b0e8b108b108b148b148b168b178b198b1b8b1d8b1d8b208b218b268b268b288b288b2b8b2c8b338b338b398b398b3e8b3e8b418b41
8b498b498b4c8b4c8b4e8b4f8b538b538b568b568b588b588b5a8b5c8b5f8b5f8b668b668b6b8b6c8b6f8b728b748b748b778b778b7d8b7d8b7f8b808b838b83
8b8a8b8a8b8c8b8c8b8e8b8e8b908b908b928b938b968b968b998b9a8c378c378c3a8c3a8c3f8c3f8c418c418c468c468c488c488c4a8c4a8c4c8c4c8c4e8c4e
8c508c508c558c558c5a8c5a8c618c628c6a8c6c8c788c7a8c7c8c7c8c828c828c858c858c898c8a8c8c8c8e8c948c948c988c988c9d8c9e8ca08ca28ca78cb0
8cb28cb48cb68cb88cbb8cbd8cbf8cc48cc78cc88cca8cca8ccd8cce8cd18cd18cd38cd38cda8cdc8cde8cde8ce08ce08ce28ce48ce68ce68cea8cea8ced8ced
8cf08cf08cf48cf48cfa8cfd8d048d058d078d088d0a8d0b8d0d8d0d8d0f8d108d128d148d168d168d648d648d668d678d6b8d6b8d6d8d6d8d708d718d738d74
8d768d778d818d818d858d858d8a8d8a8d998d998da38da38da88da88db38db38dba8dba8dbe8dbe8dc28dc28dcb8dcc8dcf8dcf8dd68dd68dda8ddb8ddd8ddd
8ddf8ddf8de18de18de38de38de88de88dea8deb8def8def8df38df38df58df58dfc8dfc8dff8dff8e088e0a8e0f8e108e1d8e1f8e2a8e2a8e308e308e348e35
8e428e428e448e448e478e4a8e4c8e4c8e508e508e558e558e598e598e5f8e608e638e648e728e728e748e748e768e768e7c8e7c8e818e818e848e858e878e87
8e8a8e8b8e8d8e8d8e918e918e938e948e998e998ea18ea18eaa8eac8eaf8eb18ebe8ebe8ec58ec68ec88ec88eca8ecd8ecf8ecf8ed28ed28edb8edb8edf8edf
8ee28ee38eeb8eeb8ef88ef88efb8efe8f038f038f058f058f098f0a8f0c8f0c8f128f158f198f198f1b8f1d8f1f8f1f8f268f268f298f2a8f2f8f2f8f338f33
8f388f398f3b8f3b8f3e8f3f8f428f428f448f468f498f498f4c8f4e8f578f578f5c8f5c8f5f8f5f8f618f648f9b8f9c8f9e8f9f8fa38fa38fa78fa88fad8fb2
8fb78fb78fba8fbc8fbf8fbf8fc28fc28fc48fc58fce8fce8fd18fd18fd48fd48fda8fda8fe28fe28fe58fe68fe98feb8fed8fed8fef8ff08ff48ff48ff78ffa
8ffd8ffd900090019003900390059006900b900b900d9011901390179019901a901d902390279027902e902e903190329035903690389039903c903c903e903e
9041904290459045904790479049904b904d905690589059905c905c905e905e90609061906390639065906590679069906d906f9072907290759078907a907a
907c907d907f9084908790879089908a908f908f9091909190a390a390a690a690a890a890aa90aa90af90af90b190b190b590b590b890b890c190c190ca90ca
90ce90ce90db90db90de90de90e190e290e490e490e890e890ed90ed90f590f590f790f790fd90fd9102910291129112911591159119911991279127912d912d
91309130913291329149914e9152915291549154915691569158915891629163916591659169916a916c916c9172917391759175917791789182918291879187
91899189918b918b918d918d919091909192919291979197919c919c91a291a291a491a491aa91ab91af91af91b491b591b891b891ba91ba91c091c191c691c9
91cb91d191d691d891da91df91e191e191e391e791ed91ee91f591f691fc91fc91ff91ff92069206920a920a920d920e9210921192149215921e921e92299229
922c922c92349234923792379239923a923c923c923f92409244924592489249924b924b924e924e92509251925792579259925b925e925e9262926292649264
926692679271927192779278927e927e9280928092839283928592859288928892919291929392939295929692989298929a929c92a792a792ad92ad92b792b7
92b992b992cf92d092d292d392d592d592d792d792d992d992e092e092e492e492e792e792e992ea92ed92ed92f292f392f892fc92ff92ff9302930293069306
930f93109318931a931d931e932093239325932693289328932b932c932e932f9332933293359335933a933b9344934493489348934b934b934d934d93549354
93569357935b935c93609360936c936c936e936e9370937093759375937c937c937e937e938c938c9394939493969397939a939a93a493a493a793a793ac93ae
93b093b093b993b993c393c393c693c693c893c893d093d193d693d893dd93de93e193e193e493e593e893e893f893f894039403940794079410941094139414
9418941a94219421942b942b943194319435943694389438943a943a94419441944494459448944894519453945a945b945e945e9460946094629462946a946a
947094709475947594779477947c947f94819481957795779580958095829583958795879589958b958f958f95919594959695969598959995a095a095a295a5
95a795a895ad95ad95b295b295b995b995bb95bc95be95be95c395c395c795c795ca95ca95cc95cd95d495d695d895d895dc95dc95e195e295e595e5961c961c
9621962196289628962a962a962e962f96329632963b963b963f96409642964296449644964b964d964f9650965b965f96629666966a966a966c966c96709670
9672967396759678967a967a967d967d9685968696889688968a968b968d968f9694969596979699969b969d96a096a096a396a396a796a896aa96aa96af96b2
96b496b496b696b996bb96bc96c096c196c496c796c996c996cb96ce96d196d196d596d696d996d996db96dc96e296e396e896e896ea96eb96f096f096f296f2
96f696f796f996f996fb96fb970097009704970497069708970a970a970d970f97119711971397139716971697199719971c971c971e971e9724972497279727
972a972a973097309732973397389739973b973b973d973e974297449746974697489749974d974d974f974f975197529755975697599759975c975c975e975e
97609762976497649766976697689769976b976b976d976d97719771977497749779977a977c977c9781978197849786978b978b978d978d978f979097989798
979c979c97a097a097a397a397a697a697a897a897ab97ab97ad97ad97b397b497c397c397c697c697c897c897cb97cb97d397d397dc97dc97ed97ee97f297f3
97f597f697fb97fb97ff97ff980198039805980698089808980c980c980f981398179818981a981a9821982198249824982c982d9834983498379838983b983d
98469846984b984f9854985598579858985b985b985e985e9865986598679867986b986b986f98719873987498a898a898aa98aa98af98af98b198b198b698b6
98c398c498c698c698db98dc98df98df98e298e298e998e998eb98eb98ed98ef98f298f298f498f498fc98fe99039903990599059909990a990c990c99109910
9912991499189918991d991e992099219924992499279928992c992c992e992e993d993e994299429945994599499949994b994c995099529955995599579957
99969999999e999e99a599a599a899a899ac99ae99b399b499bc99bc99c199c199c499c699c899c899d099d299d599d599d899d899db99db99dd99dd99df99df
99e299e299ed99ee99f199f299f899f899fb99fb99ff99ff9a019a019a059a059a0e9a0f9a129a139a199a199a289a289a2b9a2b9a309a309a379a379a3e9a3e
9a409a409a429a439a459a459a4d9a4e9a559a559a579a579a5a9a5b9a5f9a5f9a629a629a649a659a699a6b9aa89aa89aad9aad9ab09ab09ab89ab89abc9abc
9ac09ac09ac49ac49acf9acf9ad19ad19ad39ad49ad89ad99adc9adc9ade9adf9ae29ae39ae69ae69aea9aeb9aed9aef9af19af19af49af49af79af79afb9afb
9b069b069b189b189b1a9b1a9b1f9b1f9b229b239b259b259b279b2a9b2e9b2f9b319b329b3b9b3c9b419b459b4d9b4f9b519b519b549b549b589b589b5a9b5a
9b6f9b6f9b729b729b749b759b839b839b8e9b8f9b919b939b969b979b9f9ba09ba89ba89baa9bab9bad9bae9bb19bb19bb49bb49bb99bb99bbb9bbb9bc09bc0
9bc69bc69bc99bca9bcf9bcf9bd19bd29bd49bd49bd69bd69bdb9bdb9be19be49be89be89bf09bf29bf59bf59c009c009c049c049c069c069c089c0a9c0c9c0d
9c109c109c129c159c1b9c1b9c219c219c249c259c2d9c309c329c329c399c3b9c3e9c3e9c469c489c529c529c579c579c5a9c5a9c609c609c679c679c769c76
9c789c789ce59ce59ce79ce79ce99ce99ceb9cec9cf09cf09cf39cf49cf69cf69d039d039d069d099d0e9d0e9d129d129d159d159d1b9d1b9d1f9d1f9d239d23
9d269d269d289d289d2a9d2c9d3b9d3b9d3e9d3f9d419d419d449d449d469d469d489d489d509d519d599d599d5c9d5e9d609d619d649d649d6b9d6c9d6f9d70
9d729d729d7a9d7a9d879d879d899d899d8f9d8f9d9a9d9a9da49da49da99da99dab9dab9daf9daf9db29db29db49db49db89db89dba9dbb9dc19dc29dc49dc4
9dc69dc69dcf9dcf9dd39dd39dd99dd99de69de69ded9ded9def9def9df29df29df89dfa9dfd9dfd9e199e1b9e1e9e1e9e759e759e789e799e7d9e7d9e7f9e7f
9e819e819e889e889e8b9e8c9e919e939e959e959e979e979e9d9e9d9e9f9e9f9ea59ea69ea99eaa9ead9ead9eb89ebc9ebe9ebf9ec49ec49ecc9ed29ed49ed4
9ed89ed99edb9ede9ee09ee09ee59ee59ee89ee89eef9eef9ef49ef49ef69ef79ef99ef99efb9efd9f079f089f0e9f0e9f139f139f159f159f209f219f2c9f2c
9f3b9f3b9f3e9f3e9f4a9f4b9f4e9f4f9f529f529f549f549f5f9f639f669f679f6a9f6a9f6c9f6c9f729f729f769f779f8d9f8d9f959f959f9c9f9d9fa09fa0
f929f929f9dcf9dcfa0efa2dff01ff5effe0ffe5

";

			#endregion

			RANGES_RESOURCE = new string(RANGES_RESOURCE.Where(chr => ' ' <= chr).ToArray());

			for (int index = 0; index < RANGES_RESOURCE.Length; index += 8)
			{
				char chrMin = (char)Convert.ToInt32(RANGES_RESOURCE.Substring(index + 0, 4), 16);
				char chrMax = (char)Convert.ToInt32(RANGES_RESOURCE.Substring(index + 4, 4), 16);

				for (char chr = chrMin; chr <= chrMax; chr++)
				{
					yield return chr;
				}
			}
		}
#else
		private static IEnumerable<char> GetUnicodeListOfSJISMBC()
		{
			return P_GetUnicodeListOfSJISMBC().OrderBy(v => v).Distinct();
		}

		private static IEnumerable<char> P_GetUnicodeListOfSJISMBC()
		{
		#region RANGES_RESOURCE

			string RANGES_RESOURCE = @"

8140817e818081ac81b881bf81c881ce81da81e881f081f781fc81fc824f8258
826082798281829a829f82f18340837e83808396839f83b683bf83d684408460
8470847e84808491849f84be8740875d875f8775877e877e8780879c889f88fc
8940897e898089fc8a408a7e8a808afc8b408b7e8b808bfc8c408c7e8c808cfc
8d408d7e8d808dfc8e408e7e8e808efc8f408f7e8f808ffc9040907e908090fc
9140917e918091fc9240927e928092fc9340937e938093fc9440947e948094fc
9540957e958095fc9640967e968096fc9740977e978097fc98409872989f98fc
9940997e998099fc9a409a7e9a809afc9b409b7e9b809bfc9c409c7e9c809cfc
9d409d7e9d809dfc9e409e7e9e809efc9f409f7e9f809ffce040e07ee080e0fc
e140e17ee180e1fce240e27ee280e2fce340e37ee380e3fce440e47ee480e4fc
e540e57ee580e5fce640e67ee680e6fce740e77ee780e7fce840e87ee880e8fc
e940e97ee980e9fcea40ea7eea80eaa4ed40ed7eed80edfcee40ee7eee80eeec
eeefeefcfa40fa7efa80fafcfb40fb7efb80fbfcfc40fc4b

";

		#endregion

			RANGES_RESOURCE = new string(RANGES_RESOURCE.Where(chr => ' ' <= chr).ToArray());

			//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // for .NET Core
			Encoding SJIS = Encoding.GetEncoding(932);

			for (int index = 0; index < RANGES_RESOURCE.Length; index += 8)
			{
				int chrMin = Convert.ToInt32(RANGES_RESOURCE.Substring(index + 0, 4), 16);
				int chrMax = Convert.ToInt32(RANGES_RESOURCE.Substring(index + 4, 4), 16);

				for (int chr = chrMin; chr <= chrMax; chr++)
				{
					yield return SJIS.GetString(new byte[] { (byte)(chr >> 8), (byte)(chr & 0xff) })[0];
				}
			}
		}
#endif
	}
}
