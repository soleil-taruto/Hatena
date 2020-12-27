using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000033 RID: 51
	public class Shot_BossBomb : Shot
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00007C1F File Offset: 0x00005E1F
		public Shot_BossBomb() : base(0.0, 0.0, Shot.Kind_e.BOMB, 1000000000)
		{
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00007C3F File Offset: 0x00005E3F
		protected override IEnumerable<bool> E_Draw()
		{
			foreach (DDScene scene in DDSceneUtils.Create(60))
			{
				base.Crash = DDCrashUtils.Rect(D4Rect.LTRB(0.0, DDUtils.AToBRate(482.0, -30.0, scene.Rate), 512.0, 512.0));
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}
	}
}
