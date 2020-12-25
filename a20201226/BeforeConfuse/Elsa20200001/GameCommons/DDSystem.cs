using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Charlotte.GameCommons
{
	public static class DDSystem
	{
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

		public static void Pin<T>(T data)
		{
			GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);

			DDMain.Finalizers.Add(() =>
			{
				h.Free();
			});
		}
	}
}
