using System;
using System.Runtime.InteropServices;
using System.Text;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000096 RID: 150
	public static class DDWin32
	{
		// Token: 0x0600029D RID: 669
		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, out DDWin32.POINT lpPoint);

		// Token: 0x0600029E RID: 670
		[DllImport("user32.dll")]
		public static extern bool EnumWindows(DDWin32.EnumWindowsCallback callback, IntPtr lParam);

		// Token: 0x0600029F RID: 671
		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder buff, int buffLenMax);

		// Token: 0x060002A0 RID: 672 RVA: 0x00010784 File Offset: 0x0000E984
		public static string GetWindowTitleByHandle(IntPtr hWnd)
		{
			StringBuilder buff = new StringBuilder(1010);
			DDWin32.GetWindowText(hWnd, buff, 1000);
			return buff.ToString();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x000107AF File Offset: 0x0000E9AF
		public static void EnumWindowsHandleTitle(Func<IntPtr, string, bool> routine)
		{
			DDWin32.EnumWindows((IntPtr hWnd, IntPtr lParam) => routine(hWnd, DDWin32.GetWindowTitleByHandle(hWnd)), IntPtr.Zero);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x000107D4 File Offset: 0x0000E9D4
		public static IntPtr GetMainWindowHandle()
		{
			if (DDWin32.MainWindowHandle == null)
			{
				string markTitle = Guid.NewGuid().ToString("B");
				IntPtr handle = IntPtr.Zero;
				bool handleFound = false;
				DX.SetMainWindowText(markTitle);
				DDWin32.EnumWindowsHandleTitle(delegate(IntPtr hWnd, string title)
				{
					if (title == markTitle)
					{
						handle = hWnd;
						handleFound = true;
						return false;
					}
					return true;
				});
				if (!handleFound)
				{
					throw new DDError();
				}
				DDMain.SetMainWindowTitle();
				DDWin32.MainWindowHandle = new IntPtr?(handle);
			}
			return DDWin32.MainWindowHandle.Value;
		}

		// Token: 0x060002A3 RID: 675
		[DllImport("gdi32.dll")]
		public static extern int AddFontResourceEx(string file, uint fl, IntPtr res);

		// Token: 0x060002A4 RID: 676
		[DllImport("gdi32.dll")]
		public static extern int RemoveFontResourceEx(string file, uint fl, IntPtr res);

		// Token: 0x0400020C RID: 524
		private static IntPtr? MainWindowHandle;

		// Token: 0x0400020D RID: 525
		public const uint FR_PRIVATE = 16u;

		// Token: 0x0200014A RID: 330
		public struct POINT
		{
			// Token: 0x04000540 RID: 1344
			public int X;

			// Token: 0x04000541 RID: 1345
			public int Y;
		}

		// Token: 0x0200014B RID: 331
		// (Invoke) Token: 0x06000699 RID: 1689
		public delegate bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam);
	}
}
