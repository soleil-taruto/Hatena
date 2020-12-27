using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000032 RID: 50
	public class Shot_MedicineBomb : Shot
	{
		// Token: 0x06000098 RID: 152 RVA: 0x00007BC2 File Offset: 0x00005DC2
		public Shot_MedicineBomb(double x, double y) : base(x, y, Shot.Kind_e.BOMB, 10000)
		{
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00007C0F File Offset: 0x00005E0F
		protected override IEnumerable<bool> E_Draw()
		{
			RippleEffect.Add_波紋(this.X, this.Y, 60);
			RippleEffect.Add_波紋(this.X, this.Y, 120);
			RippleEffect.Add_波紋(this.X, this.Y, 180);
			RippleEffect.Add_波紋(this.X, this.Y, 360);
			int num;
			for (int frame = 0; frame < 240; frame = num + 1)
			{
				base.Crash = DDCrashUtils.Rect(D4Rect.LTRB(0.0, 0.0, 512.0, 512.0));
				yield return true;
				num = frame;
			}
			yield break;
		}
	}
}
