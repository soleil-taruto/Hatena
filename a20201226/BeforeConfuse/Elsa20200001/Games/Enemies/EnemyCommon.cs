using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games.Enemies
{
	public static class EnemyCommon
	{
		/// <summary>
		/// 敵弾の種類(形状)
		/// </summary>
		public enum TAMA_KIND_e
		{
			SMALL,
			NORMAL,
			DOUBLE,
			BIG,
			LARGE,
			KNIFE,
			ECLIPSE,
			ECLIPSE_DOUBLE,
		}

		/// <summary>
		/// 敵弾の色
		/// </summary>
		public enum TAMA_COLOR_e
		{
			RED,
			ORANGE,
			YELLOW,
			GREEN,
			CYAN,
			INDIGO,
			BLUE,
			PURPLE,
			PINK,
			WHITE,
		}

		/// <summary>
		/// ドロップアイテムの種類
		/// </summary>
		public enum DROP_ITEM_TYPE_e
		{
			STAR,
			CANDY,
			HEART,
			BOMB,
			ABSORBABLE_SHOT,
		}

		public const int LITTLE_FAIRY_COLOR_NUM = 4;
		public const int BIG_FAIRY_COLOR_NUM = 2;
		public const int ONIBI_COLOR_NUM = 3;

		public const int FAIRY_KIND_NUM = LITTLE_FAIRY_COLOR_NUM + BIG_FAIRY_COLOR_NUM + ONIBI_COLOR_NUM;

		public class FairyInfo
		{
			public Enemy Enemy;
			public int Kind; // 0 ～ (FAIRY_KIND_NUM - 1)

			// <---- prm

			public int Frame = 0;
			public double LastX;
			public int XMoveCount = 0; // <0 == 左向く, 0 == 正面向く, 0< == 右向く
			public double UntransRate = 0.0;
		}

		/// <summary>
		/// 以下の敵を描画する。
		/// -- 小フェアリー
		/// -- 大フェアリー
		/// -- 鬼火
		/// </summary>
		/// <param name="fairy">ステータス</param>
		/// <returns>Enemy.E_Draw の戻り値</returns>
		public static bool FairyDraw(FairyInfo fairy)
		{
			if (fairy.Enemy == null) throw new DDError();
			if (fairy.Kind < 0 || FAIRY_KIND_NUM <= fairy.Kind) throw new DDError();

			FairyDraw_Main(fairy, fairy.Kind);

			fairy.Frame++;

			return !IsEvacuated(fairy.Enemy);
		}

		#region FairyDraw's subroutines

		private static void FairyDraw_Main(FairyInfo fairy, int kind)
		{
			if (kind < LITTLE_FAIRY_COLOR_NUM)
			{
				LittleFairyDraw(fairy, kind);
				return;
			}
			kind -= LITTLE_FAIRY_COLOR_NUM;

			if (kind < BIG_FAIRY_COLOR_NUM)
			{
				BigFairyDraw(fairy, kind);
				return;
			}
			kind -= BIG_FAIRY_COLOR_NUM;
			OnibiDraw(fairy, kind);
		}

		private static void LittleFairyDraw(FairyInfo fairy, int color)
		{
			if (fairy.Frame == 0) // init
			{
				fairy.LastX = fairy.Enemy.X;
			}
			double xDiff = fairy.Enemy.X - fairy.LastX;
			fairy.LastX = fairy.Enemy.X;

			if (fairy.Frame % 7 == 0)
			{
				if (Math.Abs(xDiff) < SCommon.MICRO) // ? 左右に動いてない。
				{
					DDUtils.CountDown(ref fairy.XMoveCount);
					DDUtils.ToRange(ref fairy.XMoveCount, -2, 2);
				}
				else if (xDiff < 0.0) // ? 左に動いてる。
				{
					fairy.XMoveCount--;
					DDUtils.Minim(ref fairy.XMoveCount, 2);
				}
				else // ? 右に動いてる。
				{
					fairy.XMoveCount++;
					DDUtils.Maxim(ref fairy.XMoveCount, -2);
				}
			}
			int mode = DDUtils.Sign(fairy.XMoveCount);
			int koma;

			if (mode != 0)
			{
				koma = Math.Abs(fairy.XMoveCount) - 1;

				if (4 <= koma)
					koma = 2 + koma % 2;
			}
			else
				koma = (fairy.Frame / 7) % 3;

			if (1 <= fairy.Enemy.HP && fairy.Enemy.TransFrame == 0) // ? 無敵ではない。
				DDUtils.Approach(ref fairy.UntransRate, 0.5, 0.91);

			DDDraw.SetAlpha(fairy.UntransRate);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawRotate(fairy.Frame / 20.0);
			DDDraw.DrawZoom(0.4);
			DDDraw.DrawEnd();
			DDDraw.Reset();

			DDDraw.DrawCenter(GetLittleFairyPicture(color, mode, koma), fairy.Enemy.X, fairy.Enemy.Y);

			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 13.0);
		}

		private static void BigFairyDraw(FairyInfo fairy, int color)
		{
			if (fairy.Frame == 0) // init
			{
				fairy.LastX = fairy.Enemy.X;
			}
			double xDiff = fairy.Enemy.X - fairy.LastX;
			fairy.LastX = fairy.Enemy.X;

			if (fairy.Frame % 7 == 0)
			{
				if (Math.Abs(xDiff) < SCommon.MICRO) // ? 左右に動いてない。
				{
					DDUtils.CountDown(ref fairy.XMoveCount);
				}
				else if (xDiff < 0.0) // ? 左に動いてる。
				{
					fairy.XMoveCount--;
				}
				else // ? 右に動いてる。
				{
					fairy.XMoveCount++;
				}
				DDUtils.ToRange(ref fairy.XMoveCount, -2, 2);
			}
			int mode = DDUtils.Sign(fairy.XMoveCount);
			int koma;

			if (mode != 0)
				koma = Math.Abs(fairy.XMoveCount) - 1;
			else
				koma = (fairy.Frame / 7) % 3;

			if (1 <= fairy.Enemy.HP && fairy.Enemy.TransFrame == 0) // ? 無敵ではない。
				DDUtils.Approach(ref fairy.UntransRate, 0.5, 0.92);

			DDDraw.SetAlpha(fairy.UntransRate);
			DDDraw.DrawBegin(Ground.I.Picture2.D_MAHOJIN_HAJIKE_00[5], fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawRotate(fairy.Frame / 20.0);
			DDDraw.DrawZoom(0.4);
			DDDraw.DrawEnd();
			DDDraw.Reset();

			DDDraw.DrawCenter(GetBigFairyPicture(color, mode, koma), fairy.Enemy.X, fairy.Enemy.Y);

			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 30.0);
		}

		private static void OnibiDraw(FairyInfo fairy, int color)
		{
			int koma = (fairy.Frame / 4) % 4;

			DDDraw.DrawBegin(GetOnibiPicture(color, koma), fairy.Enemy.X, fairy.Enemy.Y);
			DDDraw.DrawZoom(2.0);
			DDDraw.DrawEnd();

			fairy.Enemy.Crash = DDCrashUtils.Circle(new D2Point(fairy.Enemy.X, fairy.Enemy.Y), 22.0);
		}

		private static DDPicture GetLittleFairyPicture(int color, int mode, int koma)
		{
			if (color < 0 || LITTLE_FAIRY_COLOR_NUM <= color) throw new DDError();
			if (mode < -1 || 1 < mode) throw new DDError();
			if (koma < 0 || 3 < koma) throw new DDError();
			if (mode == 0 && koma == 3) throw new DDError(); // mode == 0 のとき koma == 3 は無い

			int komaTop;

			if (mode == -1)
				komaTop = 8;
			else if (mode == 1)
				komaTop = 12;
			else
				komaTop = 0;

			return Ground.I.Picture2.D_FAIRY_00[color * 16 + komaTop + koma];
		}

		private static DDPicture GetBigFairyPicture(int color, int mode, int koma)
		{
			if (color < 0 || BIG_FAIRY_COLOR_NUM <= color) throw new DDError();
			if (mode < -1 || 1 < mode) throw new DDError();
			if (koma < 0 || 2 < koma) throw new DDError();
			if (mode != 0 && koma == 2) throw new DDError(); // mode != 0 のとき koma == 2 は無い

			int relKoma;

			if (mode == -1)
				relKoma = 7 - koma;
			else if (mode == 1)
				relKoma = 4 + koma;
			else
				relKoma = 0;

			return Ground.I.Picture2.D_BIGFAIRY_00[color * 8 + relKoma + koma];
		}

		private static DDPicture GetOnibiPicture(int color, int koma)
		{
			if (color < 0 || ONIBI_COLOR_NUM <= color) throw new DDError();
			if (koma < 0 || 3 < koma) throw new DDError();

			return Ground.I.Picture2.D_ONIBI_00[color * 4 + koma];
		}

		#endregion

		public static DDPicture GetTamaPicture(EnemyCommon.TAMA_KIND_e kind, EnemyCommon.TAMA_COLOR_e color)
		{
			return Ground.I.Picture2.D_TAMA_00[(int)kind][(int)color];
		}

		public static void Shot(Enemy enemy, int shotType)
		{
			if (DDUtils.IsOut(new D2Point(enemy.X, enemy.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H))) // ? フィールド外
				return;

			switch (shotType)
			{
				case 0: // ショット無し
					break;

				case 100: // 自機狙い_Normal_W
				case 101: // 自機狙い_Normal_R
					{
						int frm = enemy.OnFieldFrame;
						int cyc = 10;
						int div = frm / cyc;
						int mod = frm % cyc;

						if (mod == 0 && SCommon.IsRange(div, 3, 12))
						{
							TAMA_COLOR_e color;

							switch (shotType)
							{
								case 100: color = TAMA_COLOR_e.WHITE; break;
								case 101: color = TAMA_COLOR_e.RED; break;

								default:
									throw null; // never
							}
							Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, TAMA_KIND_e.NORMAL, color, 4.0, 0.0));
						}
					}
					break;

				case 110: // 自機狙い_Normal_W + 吸収
				case 111: // 自機狙い_Normal_R + 吸収
					{
						int frm = enemy.OnFieldFrame;
						int cyc = 10;
						int div = frm / cyc;
						int mod = frm % cyc;

						if (mod == 0 && SCommon.IsRange(div, 3, 12))
						{
							if (div == 3)
							{
								int absorbableWeapon;

								switch (shotType)
								{
									case 110: absorbableWeapon = 1; break;
									case 111: absorbableWeapon = 2; break;

									default:
										throw null; // never
								}
								Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, TAMA_KIND_e.NORMAL, TAMA_COLOR_e.BLUE, 4.0, 0.0, absorbableWeapon));
							}
							else
							{
								TAMA_COLOR_e color;

								switch (shotType)
								{
									case 110: color = TAMA_COLOR_e.WHITE; break;
									case 111: color = TAMA_COLOR_e.RED; break;

									default:
										throw null; // never
								}
								Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, TAMA_KIND_e.NORMAL, color, 4.0, 0.0));
							}
						}
					}
					break;

				case 200: // 三角形_Normal_W
					{
						int frm = enemy.OnFieldFrame;
						int cyc = 5;
						int div = frm / cyc;
						int mod = frm % cyc;

						div -= 10;
						div %= 15;

						if (mod == 0 && SCommon.IsRange(div, 0, 5))
						{
							TAMA_COLOR_e color;

							switch (shotType)
							{
								case 200: color = TAMA_COLOR_e.WHITE; break;

								default:
									throw null; // never
							}

							for (int c = 0; c <= div; c++)
							{
								double angleStep = 0.1;
								double angle = c * angleStep - div * angleStep / 2.0;

								Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, TAMA_KIND_e.NORMAL, color, 5.0, angle));
							}
						}
					}
					break;

				case 210: // 三角形_Normal_W + 吸収
					{
						int frm = enemy.OnFieldFrame;
						int cyc = 5;
						int div = frm / cyc;
						int mod = frm % cyc;

						div -= 10;
						div %= 15;

						if (mod == 0 && SCommon.IsRange(div, 0, 5))
						{
							for (int c = 0; c <= div; c++)
							{
								double angleStep = 0.1;
								double angle = c * angleStep - div * angleStep / 2.0;

								TAMA_COLOR_e color = TAMA_COLOR_e.YELLOW;
								int absorbableWeapon = -1;

								if (div == 5 && (c == 0 || c == 5))
								{
									color = TAMA_COLOR_e.BLUE;
									absorbableWeapon = 3;
								}
								Game.I.Enemies.Add(new Enemy_Tama_01(enemy.X, enemy.Y, TAMA_KIND_e.NORMAL, color, 5.0, angle, absorbableWeapon));
							}
						}
					}
					break;

				default:
					throw null; // never
			}
		}

		/// <summary>
		/// この敵を退場させるべきか判定する。
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <returns>この敵を退場させるべきか</returns>
		public static bool IsEvacuated(Enemy enemy)
		{
			// memo: Game.Perform にゴミ回収を設置した。
			// -- Game.IsProbablyEvacuated()

			return DDUtils.IsOut(
				new D2Point(enemy.X, enemy.Y),
				new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H),
				enemy.Kind == Enemy.Kind_e.TAMA ?
				100.0 : // 画面外の敵弾はすぐに消えてもらう。
				1 <= enemy.OnFieldFrame ? 100.0 : 10000.0 // 画面に登場する前ならそう簡単に退場させない。
				);
		}

		/// <summary>
		/// 敵 撃破 共通
		/// </summary>
		/// <param name="enemy">敵</param>
		/// <param name="dropItemMode">ドロップアイテム・モード</param>
		public static void Killed(Enemy enemy, int dropItemMode)
		{
			PutDeadEffect(enemy);
			DropItem(enemy, dropItemMode);
		}

		private static void PutDeadEffect(Enemy enemy)
		{
			switch (enemy.Kind)
			{
				case Enemy.Kind_e.ENEMY:
					Ground.I.SE.SE_ENEMYKILLED.Play();
					Game.I.EnemyEffects.Add(SCommon.Supplier(Effects.EnemyDead(enemy.X, enemy.Y)));
					break;

				case Enemy.Kind_e.TAMA:
					Game.I.EnemyEffects.Add(SCommon.Supplier(Effects.TamaDead(enemy.X, enemy.Y)));
					break;

				case Enemy.Kind_e.ITEM:
					throw null; // never // アイテムは独自の死亡イベントを行う。

				default:
					throw null; // never
			}
		}

		private static void DropItem(Enemy enemy, int dropItemMode)
		{
			switch (dropItemMode)
			{
				case 0:
					break;

				case 1:
					DropItem(enemy, DROP_ITEM_TYPE_e.STAR);
					break;

				case 2:
					DropItem(enemy, DROP_ITEM_TYPE_e.CANDY);
					break;

				case 3:
					DropItem(enemy, DROP_ITEM_TYPE_e.HEART);
					break;

				case 4:
					DropItem(enemy, DROP_ITEM_TYPE_e.BOMB);
					break;

				case 11:
					DropItem(enemy, DROP_ITEM_TYPE_e.STAR, 10, 20.0, 80.0);
					break;

				case 12:
					DropItem(enemy, DROP_ITEM_TYPE_e.CANDY, 10, 20.0, 80.0);
					break;

				case 21:
					DropItem(enemy, DROP_ITEM_TYPE_e.STAR, 20, 20.0, 110.0);
					break;

				case 22:
					DropItem(enemy, DROP_ITEM_TYPE_e.CANDY, 20, 20.0, 110.0);
					break;

				default:
					throw null; // never
			}
		}

		private static void DropItem(Enemy enemy, DROP_ITEM_TYPE_e dropItemType, int num = 1, double rMin = 0.0, double rBnd = 0.0)
		{
			if (num == 1)
			{
				Game.I.Enemies.Add(new Enemy_Item(enemy.X, enemy.Y, dropItemType));
			}
			else
			{
				Enemy[] addingItems = new Enemy[num];

				for (int index = 0; index < num; index++)
				{
					double rot = DDUtils.Random.Real2() * Math.PI * 2.0;
					double r = rMin + DDUtils.Random.Real2() * rBnd;

					addingItems[index] = new Enemy_Item(
						enemy.X + Math.Cos(rot) * r,
						enemy.Y + Math.Sin(rot) * r,
						dropItemType
						);
				}

				// 重なっているアイテムを離す。
				{
					for (int c = 0; c < 3; c++)
					{
						for (int r = 1; r < num; r++)
						{
							Enemy ri = addingItems[r];

							for (int l = 0; l < r; l++)
							{
								Enemy li = addingItems[l];

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
				}

				foreach (Enemy addingItem in addingItems)
					Game.I.Enemies.Add(addingItem);
			}
		}

		public static void Damaged(Enemy enemy)
		{
			switch (enemy.Kind)
			{
				case Enemy.Kind_e.ENEMY:
					if (enemy.HP == 0) // ? 無敵
						throw null; // never

					Ground.I.SE.SE_ENEMYDAMAGED.Play();
					break;

				case Enemy.Kind_e.TAMA:
					throw null; // never

				case Enemy.Kind_e.ITEM:
					throw null; // never

				default:
					throw null; // never
			}
		}

		public static void DrawBossPosition(double x)
		{
			DDGround.EL.Add(() =>
			{
				DDPrint.SetPrint(GameConsts.FIELD_L + (int)x - 8 * 3, DDConsts.Screen_H - 16);
				DDPrint.SetBorder(new I3Color(255, 0, 0));
				DDPrint.Print("<BOSS>");
				DDPrint.Reset();

				return false;
			});
		}

		public static void Drawノーミス()
		{
			if (Game.I.PlayerWasDead)
				return;

			const int DISPLAY_FRAME = 120;
			int endFrame = DDEngine.ProcFrame + DISPLAY_FRAME;

			DDGround.EL.Add(() =>
			{
				DDPrint.SetPrint(DDConsts.Screen_W - 120, DDConsts.Screen_H - 30);
				DDPrint.SetBorder(new I3Color(0, 0, 255));
				DDPrint.Print("ノーミス撃破");
				DDPrint.Reset();

				return DDEngine.ProcFrame < endFrame;
			});
		}
	}
}
