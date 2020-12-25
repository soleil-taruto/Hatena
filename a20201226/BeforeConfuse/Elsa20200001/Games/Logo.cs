using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;

namespace Charlotte.Games
{
	public class Logo : IDisposable
	{
		// <---- prm

		public static Logo I;

		public Logo()
		{
			I = this;
		}

		public void Dispose()
		{
			I = null;
		}

		public void Perform()
		{
			foreach (DDScene scene in DDSceneUtils.Create(30))
			{
				DDCurtain.DrawCurtain();
				DDEngine.EachFrame();
			}

			double z1 = 0.3;
			double z2 = 2.0;
			double z3 = 3.7;

			foreach (DDScene scene in DDSceneUtils.Create(60))
			{
				DDCurtain.DrawCurtain();

				DDDraw.SetAlpha(scene.Rate);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(z1);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.7);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(0.8 + 0.5 * scene.Rate);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(z2);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.3);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(z3);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDUtils.Approach(ref z1, 1.0, 0.9);
				DDUtils.Approach(ref z2, 1.0, 0.98);
				DDUtils.Approach(ref z3, 1.0, 0.95);

				DDEngine.EachFrame();
			}
			foreach (DDScene scene in DDSceneUtils.Create(90))
			{
				DDCurtain.DrawCurtain();
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDEngine.EachFrame();
			}
			foreach (DDScene scene in DDSceneUtils.Create(60))
			{
				DDCurtain.DrawCurtain();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(1.0 - 0.3 * scene.Rate);
				DDDraw.DrawRotate(scene.Rate * -0.1);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.5);
				DDDraw.DrawBegin(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2);
				DDDraw.DrawZoom(1.0 + 0.8 * scene.Rate);
				DDDraw.DrawRotate(scene.Rate * 0.1);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.3);
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2 + scene.Rate * 100.0, DDConsts.Screen_H / 2);
				DDDraw.Reset();

				DDDraw.SetAlpha((1.0 - scene.Rate) * 0.3);
				DDDraw.DrawCenter(Ground.I.Picture.Copyright, DDConsts.Screen_W / 2, DDConsts.Screen_H / 2 + scene.Rate * 50.0);
				DDDraw.Reset();

				DDEngine.EachFrame();
			}
		}
	}
}
