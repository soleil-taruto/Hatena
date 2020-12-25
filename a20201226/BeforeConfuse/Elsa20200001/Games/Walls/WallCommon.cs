using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Walls
{
	public static class WallCommon
	{
		public static IEnumerable<bool> Standard(Wall wall, DDPicture picture, int xSpeed, int ySpeed, int xOrigin, int yOrigin, double a_add, double a_max, bool fillable, double brightLevel = 1.0)
		{
			double a = 0.0;

			for (; ; )
			{
				a += a_add;

				if (a_max < a)
				{
					a = a_max;

					if (fillable)
						wall.Filled = true;
				}

				xOrigin += xSpeed;
				yOrigin += ySpeed;

				int orig_x = xOrigin % picture.Get_W();
				int orig_y = yOrigin % picture.Get_H();

				if (0 < orig_x) orig_x -= picture.Get_W();
				if (0 < orig_y) orig_y -= picture.Get_H();

				DDDraw.SetAlpha(a);
				DDDraw.SetBright(brightLevel, brightLevel, brightLevel);

				for (int x = orig_x; x < GameConsts.FIELD_W; x += picture.Get_W())
				{
					for (int y = orig_y; y < GameConsts.FIELD_H; y += picture.Get_H())
					{
						DDDraw.DrawSimple(picture, x, y);
					}
				}
				DDDraw.Reset();

				//DDCurtain.DrawCurtain(darkLevel); // -> SetBright

				yield return true;
			}
		}
	}
}
