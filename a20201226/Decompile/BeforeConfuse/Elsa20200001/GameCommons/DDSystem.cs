using System;
using System.Runtime.InteropServices;

namespace Charlotte.GameCommons
{
	// Token: 0x02000091 RID: 145
	public static class DDSystem
	{
		// Token: 0x06000257 RID: 599 RVA: 0x0000FC00 File Offset: 0x0000DE00
		public static void PinOn<T>(T data, Action<IntPtr> routine)
		{
			GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
			try
			{
				routine(pinnedData.AddrOfPinnedObject());
			}
			finally
			{
				pinnedData.Free();
			}
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000FC44 File Offset: 0x0000DE44
		public static void Pin<T>(T data)
		{
			GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
			DDMain.Finalizers.Add(delegate
			{
				h.Free();
			});
		}
	}
}
