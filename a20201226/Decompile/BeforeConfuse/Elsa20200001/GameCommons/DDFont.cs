using System;
using DxLibDLL;

namespace Charlotte.GameCommons
{
	// Token: 0x02000070 RID: 112
	public class DDFont
	{
		// Token: 0x06000181 RID: 385 RVA: 0x0000B444 File Offset: 0x00009644
		public DDFont(string fontName, int fontSize, int fontThick = 6, bool antiAliasing = true, int edgeSize = 0, bool italicFlag = false)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				throw new DDError();
			}
			if (fontSize < 1 || 1000000000 < fontSize)
			{
				throw new DDError();
			}
			if (fontThick < 0 || 9 < fontThick)
			{
				throw new DDError();
			}
			if (edgeSize < 0 || 1000000000 < edgeSize)
			{
				throw new DDError();
			}
			this.FontName = fontName;
			this.FontSize = fontSize;
			this.FontThick = fontThick;
			this.AntiAliasing = antiAliasing;
			this.EdgeSize = edgeSize;
			this.ItalicFlag = italicFlag;
			DDFontUtils.Add(this);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000B4D4 File Offset: 0x000096D4
		public int GetHandle()
		{
			if (this.Handle == -1)
			{
				int fontType = 0;
				if (this.AntiAliasing)
				{
					fontType |= 34;
				}
				if (this.EdgeSize != 0)
				{
					fontType |= 34;
				}
				this.Handle = DX.CreateFontToHandle(this.FontName, this.FontSize, this.FontThick, fontType, -1, this.EdgeSize);
				if (this.Handle == -1)
				{
					throw new DDError();
				}
			}
			return this.Handle;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000B540 File Offset: 0x00009740
		public void Unload()
		{
			if (this.Handle != -1)
			{
				if (DX.DeleteFontToHandle(this.Handle) != 0)
				{
					throw new DDError();
				}
				this.Handle = -1;
			}
		}

		// Token: 0x04000188 RID: 392
		public string FontName;

		// Token: 0x04000189 RID: 393
		public int FontSize;

		// Token: 0x0400018A RID: 394
		public int FontThick;

		// Token: 0x0400018B RID: 395
		public bool AntiAliasing;

		// Token: 0x0400018C RID: 396
		public int EdgeSize;

		// Token: 0x0400018D RID: 397
		public bool ItalicFlag;

		// Token: 0x0400018E RID: 398
		private int Handle = -1;
	}
}
