using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Enemies.Rumias
{
	// Token: 0x02000060 RID: 96
	public class Enemy_Rumia : Enemy
	{
		// Token: 0x06000122 RID: 290 RVA: 0x00009737 File Offset: 0x00007937
		public Enemy_Rumia() : base(-20.0, -20.0, Enemy.Kind_e.ENEMY, 0, 0, -1)
		{
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00009B84 File Offset: 0x00007D84
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			while (!this.NextFlag)
			{
				DDUtils.Approach(ref this.X, 256.0 + Math.Sin((double)DDEngine.ProcFrame / 57.0) * 3.0, 0.97);
				DDUtils.Approach(ref this.Y, 73.0 + Math.Sin((double)DDEngine.ProcFrame / 53.0) * 5.0, 0.91);
				EnemyCommon_Rumia.Draw(this.X, this.Y);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			Game.I.BossBattleStarted = true;
			Game.I.Enemies.Add(new Enemy_Rumia_01(this.X, this.Y));
			yield break;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00009765 File Offset: 0x00007965
		public override void Killed()
		{
			throw null;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00009768 File Offset: 0x00007968
		public override bool IsBoss()
		{
			return true;
		}

		// Token: 0x04000157 RID: 343
		public bool NextFlag;
	}
}
