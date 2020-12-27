using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies.Hinas
{
	// Token: 0x0200005D RID: 93
	public class Enemy_Hina_Tama_03 : Enemy
	{
		// Token: 0x06000118 RID: 280 RVA: 0x000098E5 File Offset: 0x00007AE5
		public Enemy_Hina_Tama_03(double x, double y, double rad, EnemyCommon.TAMA_COLOR_e color) : base(x, y, Enemy.Kind_e.TAMA, 0, 0, -1)
		{
			this.Speed = DDUtils.AngleToPoint(rad, 1.0);
			this.Color = color;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00009910 File Offset: 0x00007B10
		protected override IEnumerable<bool> E_Draw()
		{
			int frame = 0;
			while (!DDUtils.IsOut(new D2Point(this.X, this.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
			{
				this.X += this.Speed.X;
				this.Y += this.Speed.Y;
				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);
				DDDraw.DrawEnd();
				Game.I.EL_AfterDrawWalls.Add(SCommon.Supplier<bool>(this.E_魔法陣_1(this.X, this.Y)));
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			Game.I.EL_AfterDrawWalls.Add(SCommon.Supplier<bool>(this.E_魔法陣_V(this.X, this.Y)));
			if (this.X < 0.0)
			{
				this.Speed = new D2Point(4.0, 0.0);
				this.Color = EnemyCommon.TAMA_COLOR_e.YELLOW;
			}
			else if (512.0 < this.X)
			{
				this.Speed = new D2Point(-4.0, 0.0);
				this.Color = EnemyCommon.TAMA_COLOR_e.YELLOW;
			}
			else if (this.Y < 0.0)
			{
				this.Speed = new D2Point(0.0, 4.0);
				this.Color = EnemyCommon.TAMA_COLOR_e.WHITE;
			}
			else
			{
				this.Speed = new D2Point(0.0, -4.0);
				this.Color = EnemyCommon.TAMA_COLOR_e.WHITE;
			}
			frame = 0;
			for (;;)
			{
				this.X += this.Speed.X;
				this.Y += this.Speed.Y;
				DDDraw.DrawBegin(EnemyCommon.GetTamaPicture(EnemyCommon.TAMA_KIND_e.DOUBLE, this.Color), this.X, this.Y);
				DDDraw.DrawEnd();
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 6.0);
				yield return !EnemyCommon.IsEvacuated(this);
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00009920 File Offset: 0x00007B20
		private IEnumerable<bool> E_魔法陣_1(double x, double y)
		{
			DDDraw.SetAlpha(0.2);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
			DDDraw.DrawRotate((double)DDEngine.ProcFrame / 30.0);
			DDDraw.DrawZoom(0.5);
			DDDraw.DrawEnd();
			DDDraw.Reset();
			yield return false;
			yield break;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00009937 File Offset: 0x00007B37
		private IEnumerable<bool> E_魔法陣_V(double x, double y)
		{
			foreach (DDScene scene in DDSceneUtils.Create(30))
			{
				DDDraw.SetAlpha(0.2 - scene.Rate * 0.2);
				DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], x, y);
				DDDraw.DrawRotate((double)DDEngine.ProcFrame / 30.0);
				DDDraw.DrawZoom(0.5 + scene.Rate * 1.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
			}
			IEnumerator<DDScene> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000093B9 File Offset: 0x000075B9
		public override void Killed()
		{
			EnemyCommon.Killed(this, 0);
		}

		// Token: 0x04000152 RID: 338
		private D2Point Speed;

		// Token: 0x04000153 RID: 339
		private EnemyCommon.TAMA_COLOR_e Color;
	}
}
