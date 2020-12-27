using System;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games.Enemies
{
	// Token: 0x0200004F RID: 79
	public static class EnemyCommon
	{
		// Token: 0x060000DC RID: 220 RVA: 0x00008068 File Offset: 0x00006268
		public static bool FairyDraw(EnemyCommon.FairyInfo fairy)
		{
			if (fairy.Enemy == null)
			{
				throw new DDError();
			}
			if (fairy.Kind < 0 || 9 <= fairy.Kind)
			{
				throw new DDError();
			}
			EnemyCommon.FairyDraw_Main(fairy, fairy.Kind);
			fairy.Frame++;
			return !EnemyCommon.IsEvacuated(fairy.Enemy);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000080C4 File Offset: 0x000062C4
		private static void FairyDraw_Main(EnemyCommon.FairyInfo fairy, int kind)
		{
			if (kind < 4)
			{
				EnemyCommon.LittleFairyDraw(fairy, kind);
				return;
			}
			kind -= 4;
			if (kind < 2)
			{
				EnemyCommon.BigFairyDraw(fairy, kind);
				return;
			}
			kind -= 2;
			EnemyCommon.OnibiDraw(fairy, kind);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000080F0 File Offset: 0x000062F0
		private static void LittleFairyDraw(EnemyCommon.FairyInfo fairy, int color)
		{
			if (fairy.Frame == 0)
			{
				fairy.LastX = fairy.Enemy.X;
			}
			double xDiff = fairy.Enemy.X - fairy.LastX;
			fairy.LastX = fairy.Enemy.X;
			if (fairy.Frame % 7 == 0)
			{
				if (Math.Abs(xDiff) < 1E-09)
				{
					DDUtils.CountDown(ref fairy.XMoveCount);
					DDUtils.ToRange(ref fairy.XMoveCount, -2, 2);
				}
				else if (xDiff < 0.0)
				{
					fairy.XMoveCount--;
					DDUtils.Minim(ref fairy.XMoveCount, 2);
				}
				else
				{
					fairy.XMoveCount++;
					DDUtils.Maxim(ref fairy.XMoveCount, -2);
				}
			}
			int mode = DDUtils.Sign((double)fairy.XMoveCount);
			int koma;
			if (mode != 0)
			{
				koma = Math.Abs(fairy.XMoveCount) - 1;
				if (4 <= koma)
				{
					koma = 2 + koma % 2;
				}
			}
			else
			{
				koma = fairy.Frame / 7 % 3;
			}
			if (1 <= fairy.Enemy.HP && fairy.Enemy.TransFrame == 0)
			{
				DDUtils.Approach(ref fairy.UntransRate, 0.5, 0.91);
			}
			DDDraw.SetAlpha(fairy.UntransRate);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawRotate((double)fairy.Frame / 20.0);
			DDDraw.DrawZoom(0.4);
			DDDraw.DrawEnd();
			DDDraw.Reset();
			DDDraw.DrawCenter(EnemyCommon.GetLittleFairyPicture(color, mode, koma), fairy.Enemy.X, fairy.Enemy.Y);
			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 13.0);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000082E0 File Offset: 0x000064E0
		private static void BigFairyDraw(EnemyCommon.FairyInfo fairy, int color)
		{
			if (fairy.Frame == 0)
			{
				fairy.LastX = fairy.Enemy.X;
			}
			double xDiff = fairy.Enemy.X - fairy.LastX;
			fairy.LastX = fairy.Enemy.X;
			if (fairy.Frame % 7 == 0)
			{
				if (Math.Abs(xDiff) < 1E-09)
				{
					DDUtils.CountDown(ref fairy.XMoveCount);
				}
				else if (xDiff < 0.0)
				{
					fairy.XMoveCount--;
				}
				else
				{
					fairy.XMoveCount++;
				}
				DDUtils.ToRange(ref fairy.XMoveCount, -2, 2);
			}
			int mode = DDUtils.Sign((double)fairy.XMoveCount);
			int koma;
			if (mode != 0)
			{
				koma = Math.Abs(fairy.XMoveCount) - 1;
			}
			else
			{
				koma = fairy.Frame / 7 % 3;
			}
			if (1 <= fairy.Enemy.HP && fairy.Enemy.TransFrame == 0)
			{
				DDUtils.Approach(ref fairy.UntransRate, 0.5, 0.92);
			}
			DDDraw.SetAlpha(fairy.UntransRate);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawRotate((double)fairy.Frame / 20.0);
			DDDraw.DrawZoom(0.4);
			DDDraw.DrawEnd();
			DDDraw.Reset();
			DDDraw.DrawCenter(EnemyCommon.GetBigFairyPicture(color, mode, koma), fairy.Enemy.X, fairy.Enemy.Y);
			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 30.0);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000084AC File Offset: 0x000066AC
		private static void OnibiDraw(EnemyCommon.FairyInfo fairy, int color)
		{
			int koma = fairy.Frame / 4 % 4;
			DDDraw.DrawBegin(EnemyCommon.GetOnibiPicture(color, koma), fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawZoom(2.0);
			DDDraw.DrawEnd();
			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 22.0);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00008530 File Offset: 0x00006730
		private static DDPicture GetLittleFairyPicture(int color, int mode, int koma)
		{
			if (color < 0 || 4 <= color)
			{
				throw new DDError();
			}
			if (mode < -1 || 1 < mode)
			{
				throw new DDError();
			}
			if (koma < 0 || 3 < koma)
			{
				throw new DDError();
			}
			if (mode == 0 && koma == 3)
			{
				throw new DDError();
			}
			int komaTop;
			if (mode == -1)
			{
				komaTop = 8;
			}
			else if (mode == 1)
			{
				komaTop = 12;
			}
			else
			{
				komaTop = 0;
			}
			return Ground.I.Picture2.D_FAIRY_00[color * 16 + komaTop + koma];
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000085A0 File Offset: 0x000067A0
		private static DDPicture GetBigFairyPicture(int color, int mode, int koma)
		{
			if (color < 0 || 2 <= color)
			{
				throw new DDError();
			}
			if (mode < -1 || 1 < mode)
			{
				throw new DDError();
			}
			if (koma < 0 || 2 < koma)
			{
				throw new DDError();
			}
			if (mode != 0 && koma == 2)
			{
				throw new DDError();
			}
			int relKoma;
			if (mode == -1)
			{
				relKoma = 7 - koma;
			}
			else if (mode == 1)
			{
				relKoma = 4 + koma;
			}
			else
			{
				relKoma = 0;
			}
			return Ground.I.Picture2.D_BIGFAIRY_00[color * 8 + relKoma + koma];
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00008611 File Offset: 0x00006811
		private static DDPicture GetOnibiPicture(int color, int koma)
		{
			if (color < 0 || 3 <= color)
			{
				throw new DDError();
			}
			if (koma < 0 || 3 < koma)
			{
				throw new DDError();
			}
			return Ground.I.Picture2.D_ONIBI_00[color * 4 + koma];
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00008644 File Offset: 0x00006844
		public static DDPicture GetTamaPicture(EnemyCommon.TAMA_KIND_e kind, EnemyCommon.TAMA_COLOR_e color)
		{
			return Ground.I.Picture2.D_TAMA_00[(int)kind][(int)color];
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000865C File Offset: 0x0000685C
		public static void Shot(Enemy enemy, int shotType)
		{
			if (DDUtils.IsOut(new D2Point(enemy.X, enemy.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
			{
				return;
			}
			if (shotType <= 101)
			{
				if (shotType == 0)
				{
					return;
				}
				if (shotType - 100 <= 1)
				{
					int onFieldFrame = enemy.OnFieldFrame;
					int cyc = 10;
					int div = onFieldFrame / cyc;
					if (onFieldFrame % cyc == 0 && SCommon.IsRange(div, 3, 12))
					{
						EnemyCommon.TAMA_COLOR_e color;
						if (shotType != 100)
						{
							if (shotType != 101)
							{
								throw null;
							}
							color = EnemyCommon.TAMA_COLOR_e.RED;
						}
						else
						{
							color = EnemyCommon.TAMA_COLOR_e.WHITE;
						}
						Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, EnemyCommon.TAMA_KIND_e.NORMAL, color, 4.0, 0.0, -1));
						return;
					}
					return;
				}
			}
			else if (shotType - 110 > 1)
			{
				if (shotType != 200)
				{
					if (shotType == 210)
					{
						int onFieldFrame2 = enemy.OnFieldFrame;
						int cyc2 = 5;
						int div2 = onFieldFrame2 / cyc2;
						bool flag = onFieldFrame2 % cyc2 != 0;
						div2 -= 10;
						div2 %= 15;
						if (!flag && SCommon.IsRange(div2, 0, 5))
						{
							for (int c = 0; c <= div2; c++)
							{
								double angleStep = 0.1;
								double angle = (double)c * angleStep - (double)div2 * angleStep / 2.0;
								EnemyCommon.TAMA_COLOR_e color2 = EnemyCommon.TAMA_COLOR_e.YELLOW;
								int absorbableWeapon = -1;
								if (div2 == 5 && (c == 0 || c == 5))
								{
									color2 = EnemyCommon.TAMA_COLOR_e.BLUE;
									absorbableWeapon = 3;
								}
								Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, EnemyCommon.TAMA_KIND_e.NORMAL, color2, 5.0, angle, absorbableWeapon));
							}
							return;
						}
						return;
					}
				}
				else
				{
					int onFieldFrame3 = enemy.OnFieldFrame;
					int cyc3 = 5;
					int div3 = onFieldFrame3 / cyc3;
					bool flag2 = onFieldFrame3 % cyc3 != 0;
					div3 -= 10;
					div3 %= 15;
					if (flag2 || !SCommon.IsRange(div3, 0, 5))
					{
						return;
					}
					if (shotType == 200)
					{
						EnemyCommon.TAMA_COLOR_e color3 = EnemyCommon.TAMA_COLOR_e.WHITE;
						for (int c2 = 0; c2 <= div3; c2++)
						{
							double angleStep2 = 0.1;
							double angle2 = (double)c2 * angleStep2 - (double)div3 * angleStep2 / 2.0;
							Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, EnemyCommon.TAMA_KIND_e.NORMAL, color3, 5.0, angle2, -1));
						}
						return;
					}
					throw null;
				}
			}
			else
			{
				int onFieldFrame4 = enemy.OnFieldFrame;
				int cyc4 = 10;
				int div4 = onFieldFrame4 / cyc4;
				if (onFieldFrame4 % cyc4 != 0 || !SCommon.IsRange(div4, 3, 12))
				{
					return;
				}
				if (div4 == 3)
				{
					int absorbableWeapon2;
					if (shotType != 110)
					{
						if (shotType != 111)
						{
							throw null;
						}
						absorbableWeapon2 = 2;
					}
					else
					{
						absorbableWeapon2 = 1;
					}
					Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, EnemyCommon.TAMA_KIND_e.NORMAL, EnemyCommon.TAMA_COLOR_e.BLUE, 4.0, 0.0, absorbableWeapon2));
					return;
				}
				EnemyCommon.TAMA_COLOR_e color4;
				if (shotType != 110)
				{
					if (shotType != 111)
					{
						throw null;
					}
					color4 = EnemyCommon.TAMA_COLOR_e.RED;
				}
				else
				{
					color4 = EnemyCommon.TAMA_COLOR_e.WHITE;
				}
				Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, EnemyCommon.TAMA_KIND_e.NORMAL, color4, 4.0, 0.0, -1));
				return;
			}
			throw null;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00008984 File Offset: 0x00006B84
		public static bool IsEvacuated(Enemy enemy)
		{
			return DDUtils.IsOut(new D2Point(enemy.X, enemy.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), (enemy.Kind == Enemy.Kind_e.TAMA) ? 100.0 : ((1 <= enemy.OnFieldFrame) ? 100.0 : 10000.0));
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00008A01 File Offset: 0x00006C01
		public static void Killed(Enemy enemy, int dropItemMode)
		{
			EnemyCommon.PutDeadEffect(enemy);
			EnemyCommon.DropItem(enemy, dropItemMode);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00008A10 File Offset: 0x00006C10
		private static void PutDeadEffect(Enemy enemy)
		{
			switch (enemy.Kind)
			{
			case Enemy.Kind_e.ENEMY:
				Ground.I.SE.SE_ENEMYKILLED.Play(true);
				Game.I.EnemyEffects.Add(SCommon.Supplier<bool>(Effects.EnemyDead(enemy.X, enemy.Y)));
				return;
			case Enemy.Kind_e.TAMA:
				Game.I.EnemyEffects.Add(SCommon.Supplier<bool>(Effects.TamaDead(enemy.X, enemy.Y)));
				return;
			case Enemy.Kind_e.ITEM:
				throw null;
			default:
				throw null;
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00008AA0 File Offset: 0x00006CA0
		private static void DropItem(Enemy enemy, int dropItemMode)
		{
			switch (dropItemMode)
			{
			case 0:
				return;
			case 1:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.STAR, 1, 0.0, 0.0);
				return;
			case 2:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.CANDY, 1, 0.0, 0.0);
				return;
			case 3:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.HEART, 1, 0.0, 0.0);
				return;
			case 4:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.BOMB, 1, 0.0, 0.0);
				return;
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
				break;
			case 11:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.STAR, 10, 20.0, 80.0);
				return;
			case 12:
				EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.CANDY, 10, 20.0, 80.0);
				return;
			default:
				if (dropItemMode == 21)
				{
					EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.STAR, 20, 20.0, 110.0);
					return;
				}
				if (dropItemMode == 22)
				{
					EnemyCommon.DropItem(enemy, EnemyCommon.DROP_ITEM_TYPE_e.CANDY, 20, 20.0, 110.0);
					return;
				}
				break;
			}
			throw null;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00008BDC File Offset: 0x00006DDC
		private static void DropItem(Enemy enemy, EnemyCommon.DROP_ITEM_TYPE_e dropItemType, int num = 1, double rMin = 0.0, double rBnd = 0.0)
		{
			if (num == 1)
			{
				Game.I.Enemies.Add(new Enemy_Item(enemy.X, enemy.Y, dropItemType, 0, -1));
				return;
			}
			Enemy[] addingItems = new Enemy[num];
			for (int index = 0; index < num; index++)
			{
				double rot = DDUtils.Random.Real2() * 3.1415926535897931 * 2.0;
				double r = rMin + DDUtils.Random.Real2() * rBnd;
				addingItems[index] = new Enemy_Item(enemy.X + Math.Cos(rot) * r, enemy.Y + Math.Sin(rot) * r, dropItemType, 0, -1);
			}
			for (int c = 0; c < 3; c++)
			{
				for (int r2 = 1; r2 < num; r2++)
				{
					Enemy ri = addingItems[r2];
					for (int i = 0; i < r2; i++)
					{
						Enemy li = addingItems[i];
						if (DDUtils.GetDistance(li.X - ri.X, li.Y - ri.Y) < 10.0)
						{
							double rax;
							double ray;
							DDUtils.MakeXYSpeed(li.X, li.Y, ri.X, ri.Y, 5.0, out rax, out ray);
							li.X -= rax;
							li.Y -= ray;
							ri.X += rax;
							ri.Y += ray;
						}
					}
				}
			}
			foreach (Enemy addingItem in addingItems)
			{
				Game.I.Enemies.Add(addingItem);
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00008D98 File Offset: 0x00006F98
		public static void Damaged(Enemy enemy)
		{
			switch (enemy.Kind)
			{
			case Enemy.Kind_e.ENEMY:
				if (enemy.HP == 0)
				{
					throw null;
				}
				Ground.I.SE.SE_ENEMYDAMAGED.Play(true);
				return;
			case Enemy.Kind_e.TAMA:
				throw null;
			case Enemy.Kind_e.ITEM:
				throw null;
			default:
				throw null;
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008DE8 File Offset: 0x00006FE8
		public static void DrawBossPosition(double x)
		{
			DDGround.EL.Add(delegate
			{
				DDPrint.SetPrint(224 + (int)x - 24, 524, 16);
				DDPrint.SetBorder(new I3Color(255, 0, 0), 1);
				DDPrint.Print("<BOSS>");
				DDPrint.Reset();
				return false;
			});
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00008E18 File Offset: 0x00007018
		public static void Drawノ\u30FCミス()
		{
			if (Game.I.PlayerWasDead)
			{
				return;
			}
			int endFrame = DDEngine.ProcFrame + 120;
			DDGround.EL.Add(delegate
			{
				DDPrint.SetPrint(840, 510, 16);
				DDPrint.SetBorder(new I3Color(0, 0, 255), 1);
				DDPrint.Print("ノーミス撃破");
				DDPrint.Reset();
				return DDEngine.ProcFrame < endFrame;
			});
		}

		// Token: 0x04000118 RID: 280
		public const int LITTLE_FAIRY_COLOR_NUM = 4;

		// Token: 0x04000119 RID: 281
		public const int BIG_FAIRY_COLOR_NUM = 2;

		// Token: 0x0400011A RID: 282
		public const int ONIBI_COLOR_NUM = 3;

		// Token: 0x0400011B RID: 283
		public const int FAIRY_KIND_NUM = 9;

		// Token: 0x020000F1 RID: 241
		public enum TAMA_KIND_e
		{
			// Token: 0x040003BE RID: 958
			SMALL,
			// Token: 0x040003BF RID: 959
			NORMAL,
			// Token: 0x040003C0 RID: 960
			DOUBLE,
			// Token: 0x040003C1 RID: 961
			BIG,
			// Token: 0x040003C2 RID: 962
			LARGE,
			// Token: 0x040003C3 RID: 963
			KNIFE,
			// Token: 0x040003C4 RID: 964
			ECLIPSE,
			// Token: 0x040003C5 RID: 965
			ECLIPSE_DOUBLE
		}

		// Token: 0x020000F2 RID: 242
		public enum TAMA_COLOR_e
		{
			// Token: 0x040003C7 RID: 967
			RED,
			// Token: 0x040003C8 RID: 968
			ORANGE,
			// Token: 0x040003C9 RID: 969
			YELLOW,
			// Token: 0x040003CA RID: 970
			GREEN,
			// Token: 0x040003CB RID: 971
			CYAN,
			// Token: 0x040003CC RID: 972
			INDIGO,
			// Token: 0x040003CD RID: 973
			BLUE,
			// Token: 0x040003CE RID: 974
			PURPLE,
			// Token: 0x040003CF RID: 975
			PINK,
			// Token: 0x040003D0 RID: 976
			WHITE
		}

		// Token: 0x020000F3 RID: 243
		public enum DROP_ITEM_TYPE_e
		{
			// Token: 0x040003D2 RID: 978
			STAR,
			// Token: 0x040003D3 RID: 979
			CANDY,
			// Token: 0x040003D4 RID: 980
			HEART,
			// Token: 0x040003D5 RID: 981
			BOMB,
			// Token: 0x040003D6 RID: 982
			ABSORBABLE_SHOT
		}

		// Token: 0x020000F4 RID: 244
		public class FairyInfo
		{
			// Token: 0x040003D7 RID: 983
			public Enemy Enemy;

			// Token: 0x040003D8 RID: 984
			public int Kind;

			// Token: 0x040003D9 RID: 985
			public int Frame;

			// Token: 0x040003DA RID: 986
			public double LastX;

			// Token: 0x040003DB RID: 987
			public int XMoveCount;

			// Token: 0x040003DC RID: 988
			public double UntransRate;
		}
	}
}
