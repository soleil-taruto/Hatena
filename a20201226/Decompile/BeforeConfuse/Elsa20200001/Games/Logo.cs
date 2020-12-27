using System;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	// Token: 0x02000017 RID: 23
	public class Logo : IDisposable
	{
		// Token: 0x0600003D RID: 61 RVA: 0x00004D44 File Offset: 0x00002F44
		public Logo()
		{
			Logo.I = this;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004D52 File Offset: 0x00002F52
		public void Dispose()
		{
			Logo.I = null;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004D5C File Offset: 0x00002F5C
		public void Perform()
		{
			foreach (DDScene ddscene in DDSceneUtils.Create(30))
			{
				DDCurtain.DrawCurtain(-1.0);
				DDEngine.EachFrame();
			}
			double z = 0.3;
			double z2 = 2.0;
			double z3 = 3.7;
			foreach (DDScene scene in DDSceneUtils.Create(60))
			{
				DDCurtain.DrawCurtain(-1.0);
				DDDraw.SetAlpha(scene.Rate);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(z);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.7);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(0.8 + 0.5 * scene.Rate);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(z2);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.3);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(z3);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDUtils.Approach(ref z, 1.0, 0.9);
				DDUtils.Approach(ref z2, 1.0, 0.98);
				DDUtils.Approach(ref z3, 1.0, 0.95);
				DDEngine.EachFrame();
			}
			foreach (DDScene ddscene2 in DDSceneUtils.Create(90))
			{
				DDCurtain.DrawCurtain(-1.0);
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDEngine.EachFrame();
			}
			foreach (DDScene scene2 in DDSceneUtils.Create(60))
			{
				DDCurtain.DrawCurtain(-1.0);
				DDDraw.SetAlpha((1.0 - scene2.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(1.0 - 0.3 * scene2.Rate);
				DDDraw.DrawRotate(scene2.Rate * -0.1);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene2.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, 480.0, 270.0);
				DDDraw.DrawZoom(1.0 + 0.8 * scene2.Rate);
				DDDraw.DrawRotate(scene2.Rate * 0.1);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene2.Rate) * 0.3);
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, 480.0 + scene2.Rate * 100.0, 270.0);
				DDDraw.Reset();
				DDDraw.SetAlpha((1.0 - scene2.Rate) * 0.3);
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, 480.0, 270.0 + scene2.Rate * 50.0);
				DDDraw.Reset();
				DDEngine.EachFrame();
			}
		}

		// Token: 0x040000AB RID: 171
		public static Logo I;
	}
}
