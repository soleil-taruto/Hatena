using System;
using Charlotte.Commons;
using Charlotte.Games.Enemies;
using Charlotte.Games.Shots;

namespace Charlotte.GameCommons.Options
{
	// Token: 0x02000098 RID: 152
	public struct DDCrash
	{
		// Token: 0x060002A9 RID: 681 RVA: 0x00010908 File Offset: 0x0000EB08
		public bool IsCrashed(DDCrash other)
		{
			return DDCrashUtils.IsCrashed(this, other);
		}

		// Token: 0x04000211 RID: 529
		public Enemy OwnerEnemy;

		// Token: 0x04000212 RID: 530
		public Shot OwnerShot;

		// Token: 0x04000213 RID: 531
		public DDCrashUtils.Kind_e Kind;

		// Token: 0x04000214 RID: 532
		public D2Point Pt;

		// Token: 0x04000215 RID: 533
		public double R;

		// Token: 0x04000216 RID: 534
		public D4Rect Rect;

		// Token: 0x04000217 RID: 535
		public DDCrash[] Crashes;
	}
}
