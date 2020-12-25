using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Charlotte.Commons
{
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
			// -- このため Root.Dir は以下が保証される。
			// ---- 空白を含まない。
			// ---- ENCODING_SJIS の文字列である。

			// 環境変数 TMP のフォルダの配下は定期的に削除される。-> プロセス終了時の削除漏れはケアしない。

			return new RootInfo(Path.Combine(Environment.GetEnvironmentVariable("TMP"), ProcMain.APP_IDENT + "_" + Process.GetCurrentProcess().Id));
		}

		private static long CtorCounter = 0L;

		private string Dir = null;

		/// <summary>
		/// 作業ディレクトリを返す。
		/// 作業ディレクトリは以下が保証される。
		/// -- 空白を含まない。
		/// -- ENCODING_SJIS の文字列である。
		/// </summary>
		/// <returns>作業ディレクトリ</returns>
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

		/// <summary>
		/// 作業ディレクトリ直下の「指定されたローカル名」のパス(作業パス)を返す。
		/// 作業パスの親ディレクトリは以下が保証される。
		/// -- 空白を含まない。
		/// -- ENCODING_SJIS の文字列である。
		/// </summary>
		/// <returns>作業パス</returns>
		public string GetPath(string localName)
		{
			return Path.Combine(this.GetDir(), localName);
		}

		private long PathCounter = 0L;

		/// <summary>
		/// 作業ディレクトリ直下の次のパス(作業パス)を返す。
		/// 作業パスは以下が保証される。
		/// -- 空白を含まない。
		/// -- ENCODING_SJIS の文字列である。
		/// </summary>
		/// <returns>作業パス</returns>
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
}
