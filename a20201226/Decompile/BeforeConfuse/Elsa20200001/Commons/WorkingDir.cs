using System;
using System.Diagnostics;
using System.IO;

namespace Charlotte.Commons
{
	// Token: 0x020000AE RID: 174
	public class WorkingDir : IDisposable
	{
		// Token: 0x0600036C RID: 876 RVA: 0x00013090 File Offset: 0x00011290
		public static WorkingDir.RootInfo CreateProcessRoot()
		{
			return new WorkingDir.RootInfo(Path.Combine(Environment.GetEnvironmentVariable("TMP"), "{6519e425-fd6e-4762-a840-7391b5dd8632}_" + Process.GetCurrentProcess().Id.ToString()));
		}

		// Token: 0x0600036D RID: 877 RVA: 0x000130D0 File Offset: 0x000112D0
		private string GetDir()
		{
			if (this.Dir == null)
			{
				if (WorkingDir.Root == null)
				{
					throw new Exception("Root is null");
				}
				string dir = WorkingDir.Root.GetDir();
				string str = "$";
				long ctorCounter = WorkingDir.CtorCounter;
				WorkingDir.CtorCounter = ctorCounter + 1L;
				long num = ctorCounter;
				this.Dir = Path.Combine(dir, str + num.ToString());
				SCommon.CreateDir(this.Dir);
			}
			return this.Dir;
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0001313D File Offset: 0x0001133D
		public string GetPath(string localName)
		{
			return Path.Combine(this.GetDir(), localName);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001314C File Offset: 0x0001134C
		public string MakePath()
		{
			string str = "$";
			long pathCounter = this.PathCounter;
			this.PathCounter = pathCounter + 1L;
			return this.GetPath(str + pathCounter.ToString());
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00013184 File Offset: 0x00011384
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

		// Token: 0x04000266 RID: 614
		public static WorkingDir.RootInfo Root;

		// Token: 0x04000267 RID: 615
		private static long CtorCounter;

		// Token: 0x04000268 RID: 616
		private string Dir;

		// Token: 0x04000269 RID: 617
		private long PathCounter;

		// Token: 0x0200015E RID: 350
		public class RootInfo
		{
			// Token: 0x060006D7 RID: 1751 RVA: 0x00023001 File Offset: 0x00021201
			public RootInfo(string dir)
			{
				this.Dir = dir;
			}

			// Token: 0x060006D8 RID: 1752 RVA: 0x00023010 File Offset: 0x00021210
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

			// Token: 0x060006D9 RID: 1753 RVA: 0x0002303D File Offset: 0x0002123D
			public void Delete()
			{
				if (this.Created)
				{
					SCommon.DeletePath(this.Dir);
					this.Created = false;
				}
			}

			// Token: 0x04000561 RID: 1377
			private string Dir;

			// Token: 0x04000562 RID: 1378
			private bool Created;
		}
	}
}
