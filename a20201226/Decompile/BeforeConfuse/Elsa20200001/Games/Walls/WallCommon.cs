using System;
using System.Collections.Generic;
using Charlotte.GameCommons;

namespace Charlotte.Games.Walls
{
	// Token: 0x02000020 RID: 32
	public static class WallCommon
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00007190 File Offset: 0x00005390
		public static IEnumerable<bool> Standard(Wall wall, DDPicture picture, int xSpeed, int ySpeed, int xOrigin, int yOrigin, double a_add, double a_max, bool fillable, double brightLevel = 1.0)
		{
			double a = 0.0;
			for (;;)
			{
				a += a_add;
				if (a_max < a)
				{
					a = a_max;
					if (fillable)
					{
						wall.Filled = true;
					}
				}
				xOrigin += xSpeed;
				yOrigin += ySpeed;
				int orig_x = xOrigin % picture.Get_W();
				int orig_y = yOrigin % picture.Get_H();
				if (0 < orig_x)
				{
					orig_x -= picture.Get_W();
				}
				if (0 < orig_y)
				{
					orig_y -= picture.Get_H();
				}
				DDDraw.SetAlpha(a);
				DDDraw.SetBright(brightLevel, brightLevel, brightLevel);
				for (int x = orig_x; x < 512; x += picture.Get_W())
				{
					for (int y = orig_y; y < 512; y += picture.Get_H())
					{
						DDDraw.DrawSimple(picture, (double)x, (double)y);
					}
				}
				DDDraw.Reset();
				yield return true;
			}
			yield break;
		}
	}
}
