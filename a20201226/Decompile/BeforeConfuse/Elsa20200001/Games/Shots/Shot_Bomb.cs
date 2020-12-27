using System;
using System.Collections.Generic;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Shots
{
	// Token: 0x02000031 RID: 49
	public class Shot_Bomb : Shot
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00007BC2 File Offset: 0x00005DC2
		public Shot_Bomb(double x, double y) : base(x, y, Shot.Kind_e.BOMB, 10000)
		{
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007BD2 File Offset: 0x00005DD2
		protected override IEnumerable<bool> E_Draw()
		{
			double dc_r = (double)Ground.I.Picture2.D_DECOCIRCLE.Get_W() / 2.0;
			double dc_z = 200.0 / dc_r;
			Func<bool>[] circles = new Func<bool>[]
			{
				SCommon.Supplier<bool>(this.Circle(0.75, dc_z, 0.1, 0.05)),
				SCommon.Supplier<bool>(this.Circle(0.25, dc_z * 0.75, dc_z * 2.0, 0.01)),
				SCommon.Supplier<bool>(this.Circle(0.5, dc_z * 0.5, 0.1, -0.02))
			};
			int i;
			for (int frame = 0; frame < 240; frame = i + 1)
			{
				this.Y -= (double)frame / 60.0;
				Func<bool>[] array = circles;
				for (i = 0; i < array.Length; i++)
				{
					array[i]();
				}
				base.Crash = DDCrashUtils.Circle(new D2Point(this.X, this.Y), 200.0);
				yield return true;
				i = frame;
			}
			yield break;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00007BE2 File Offset: 0x00005DE2
		private IEnumerable<bool> Circle(double target_a, double target_z, double z, double r_add)
		{
			double a = 0.0;
			double r = 0.0;
			int frame = 0;
			for (;;)
			{
				if (frame == 180)
				{
					target_a = 0.0;
				}
				DDUtils.Approach(ref a, target_a, 0.9);
				DDUtils.Approach(ref z, target_z, 0.9);
				r += r_add;
				DDDraw.SetAlpha(a);
				DDDraw.DrawBegin(Ground.I.Picture2.D_DECOCIRCLE, this.X, this.Y);
				DDDraw.DrawZoom(z);
				DDDraw.DrawRotate(r);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				yield return true;
				int num = frame;
				frame = num + 1;
			}
			yield break;
		}

		// Token: 0x040000FB RID: 251
		private const double BOMB_R = 200.0;
	}
}
