using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x02000058 RID: 88
	public class Enemy_Hina : Enemy
	{
		// Token: 0x06000106 RID: 262 RVA: 0x00009737 File Offset: 0x00007937
		public Enemy_Hina() : base(-20.0, -20.0, Enemy.Kind_e.ENEMY, 0, 0, -1)
		{
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00009755 File Offset: 0x00007955
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			while (!this.NextFlag)
			{
				DDUtils.Approach(ref this.X, 256.0 + Math.Sin((double)DDEngine.ProcFrame / 57.0) * 3.0, 0.97);
				DDUtils.Approach(ref this.Y, 73.0 + Math.Sin((double)DDEngine.ProcFrame / 53.0) * 5.0, 0.91);
				EnemyCommon_Hina.Draw(this.X, this.Y, frame < 60, 0.0);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			Game.I.Enemies.Add(new Enemy_Hina_01(this.X, this.Y));
			yield break;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00009765 File Offset: 0x00007965
		public override void Killed()
		{
			throw null;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}

		// Token: 0x0400014C RID: 332
		public bool NextFlag;
	}
}
