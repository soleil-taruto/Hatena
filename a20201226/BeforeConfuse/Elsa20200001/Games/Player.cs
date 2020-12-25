using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games
{
	/// <summary>
	/// プレイヤーに関する情報と機能
	/// 唯一のインスタンスを Game.I.Player に保持する。
	/// </summary>
	public class Player
	{
		public enum PlayerWho_e
		{
			メディスン,
			小悪魔,
		}

		public PlayerWho_e PlayerWho = PlayerWho_e.メディスン;

		// <---- prm

		public double X; // 0.0 ～ FIELD_W
		public double Y; // 0.0 ～ FIELD_H
		public double LastX; // 前フレームの X
		public double LastY; // 前フレームの Y
		public int XMoveFrame; // ～(-1) == 左移動中, 0 == 左右移動ナシ, 1～ == 右移動中
		public int YMoveFrame; // ～(-1) == 上移動中, 0 == 上下移動ナシ, 1～ == 下移動中
		public double XMoveRate; // 左移動中 <-- 0.0 ～ 1.0 --> 右移動中
		public double YMoveRate; // 上移動中 <-- 0.0 ～ 1.0 --> 下移動中
		public int SlowFrame; // ～(-1) == 高速移動中, 0 == 初期値, 1～ == 低速移動中
		public int ShotFrame; // ～(-1) == 無ショット, 0 == 初期値, 1～ == ショット中
		public double SlowRate; // 高速移動中 <-- 0.0 ～ 1.0 --> 低速移動中
		public double ShotRate; // 無ショット <-- 0.0 ～ 1.0 --> ショット中, パワーアップ時 0.0 にリセットする。
		public int Power; // 0 ～ PLAYERPOWER_MAX
		public int BornFrame; // 0 == 無効, 1～ == 登場中
		public int DeadFrame; // 0 == 無効, 1～ == 死亡中
		public int BombFrame; // 0 == 無効, 1～ == ボム使用中
		public double BornFollowX; // 登場中に X に追従
		public double BornFollowY; // 登場中に Y に追従
		public int AbsorbedWeapon = -1; // -1 == 無効, 0～ == 吸収した武器

		public int Level
		{
			get
			{
				return this.Power / GameConsts.PLAYER_POWER_PER_LEVEL;
			}
		}

		/// <summary>
		/// プレイヤーの状態を初期化する。
		/// 以下の場合
		/// -- ステージ開始時
		/// -- 死亡からの残機消費してリスポーンした時
		/// </summary>
		/// <param name="死亡した">死亡したため呼び出されたか</param>
		public void Reset(bool 死亡した)
		{
			this.X = GameConsts.FIELD_W * 0.5;
			this.Y = GameConsts.FIELD_H * 0.8;
			this.LastX = this.X;
			this.LastY = this.Y;
			this.XMoveFrame = 0;
			this.YMoveFrame = 0;
			this.XMoveRate = 0.0;
			this.YMoveRate = 0.0;
			this.SlowFrame = 0;
			this.ShotFrame = 0;
			this.SlowRate = 0.0;
			this.ShotRate = 0.0;

			if (死亡した)
			{
				this.Power -= GameConsts.PLAYER_POWER_PER_LEVEL;
				this.Power = Math.Max(this.Power, 0);
			}
			else
				this.Power = 0;

			this.BornFrame = 1; // 登場するために 1 を設定する。
			this.DeadFrame = 0;
			this.BombFrame = 0;
			this.BornFollowX = GameConsts.FIELD_W * 0.5;
			this.BornFollowY = GameConsts.FIELD_H * 1.2;

			this.AbsorbedWeapon = -1; // 吸収した武器を削除するようにしてみた。
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw()
		{
			int dr_x;
			int dr_y;
			double dr_a;

			if (1 <= this.BornFrame)
			{
				dr_x = SCommon.ToInt(this.BornFollowX);
				dr_y = SCommon.ToInt(this.BornFollowY);
				dr_a = 0.5;
			}
			else if (1 <= this.BombFrame)
			{
				dr_x = SCommon.ToInt(this.X);
				dr_y = SCommon.ToInt(this.Y);
				dr_a = 0.5;
			}
			else
			{
				dr_x = SCommon.ToInt(this.X);
				dr_y = SCommon.ToInt(this.Y);
				dr_a = 1.0;
			}

			switch (this.PlayerWho)
			{
				case PlayerWho_e.メディスン:
					{
						int pic_x;
						int pic_y = Game.I.Frame / 7 % 3;

						if (this.XMoveFrame < 0)
							pic_x = 2;
						else if (0 < this.XMoveFrame)
							pic_x = 1;
						else
							pic_x = 0;

						DDDraw.SetAlpha(dr_a);
						DDDraw.DrawCenter(Ground.I.Picture2.メディスン[pic_x, pic_y], dr_x, dr_y);
						DDDraw.Reset();
					}
					break;

				case PlayerWho_e.小悪魔:
					{
						if (-20 < this.SlowFrame) // ? 低速移動中(余韻アリ)
						{
							double r = this.SlowRate;
							double a = dr_a * r * 0.7;

							DDDraw.SetBlendAdd(a);
							DDDraw.DrawBegin(Ground.I.Picture2.D_SLOWBACK, dr_x, dr_y);
							DDDraw.DrawRotate(Game.I.Frame * 0.01);
							DDDraw.DrawZoom(1.0 + (1.0 - r) * 4.0);
							DDDraw.DrawEnd();
							DDDraw.Reset();
						}
						if (-20 < this.ShotFrame) // ? ショット中(余韻アリ)
						{
							double r = this.ShotRate;

							if (0 < this.SlowFrame) // 低速
							{
								int bookKoma = Game.I.Frame / 3;
								bookKoma %= 3;

								DDDraw.SetAlpha(dr_a * r * 0.3);

								DDDraw.DrawBegin(
									Ground.I.Picture2.D_BOOKBACK,
									dr_x,
									dr_y - 24.0 * r
									);
								DDDraw.DrawZoom(1.5 + 1.0 * r);
								DDDraw.DrawRotate(Game.I.Frame * 0.02);
								DDDraw.DrawZoom_Y(0.5);
								DDDraw.DrawEnd();

								DDDraw.SetAlpha(dr_a * r * 1.0);

								for (int c = -1; c <= 1; c += 2)
								{
									DDDraw.DrawCenter(
										Ground.I.Picture2.D_BOOK_00[bookKoma],
										dr_x + 18.0 * c * r,
										dr_y - 14.0 * r
										);
								}
								DDDraw.Reset();
							}
							else if (1 <= this.Level) // 高速 && ウェーブショット有り
							{
								int bookKoma = Game.I.Frame / 5;
								bookKoma %= 3;

								DDDraw.SetAlpha(dr_a * r * 0.3);

								for (int c = -1; c <= 1; c += 2)
								{
									DDDraw.DrawBegin(
										Ground.I.Picture2.D_BOOKBACK,
										dr_x + 25.0 * c * r,
										dr_y - 12.0 * r
										);
									DDDraw.DrawZoom(0.5 + 1.0 * r);
									DDDraw.DrawRotate(Game.I.Frame * 0.02);
									DDDraw.DrawZoom_Y(0.5);
									DDDraw.DrawEnd();
								}
								DDDraw.SetAlpha(dr_a * r * 1.0);

								for (int c = -1; c <= 1; c += 2)
								{
									DDDraw.DrawCenter(
										Ground.I.Picture2.D_BOOK_00[bookKoma],
										dr_x + 25.0 * c * r,
										dr_y
										);
								}
								DDDraw.Reset();
							}
						}

						int koma = Game.I.Frame / 7;

						if (this.XMoveFrame != 0)
						{
							koma %= 3;
							koma += this.XMoveFrame < 0 ? 4 : 8;
						}
						else
							koma %= 4;

						DDDraw.SetAlpha(dr_a);
						DDDraw.DrawCenter(Ground.I.Picture2.D_KOAKUMA_00[koma], dr_x, dr_y);
						DDDraw.Reset();

						if (-20 < this.SlowFrame) // ? 低速移動中(余韻アリ)
						{
							double a = dr_a * this.SlowRate;
							a *= 2.0;

							DDDraw.SetAlpha(a);
							DDDraw.DrawBegin(Ground.I.Picture2.D_ATARIPOINT, dr_x, dr_y);
							DDDraw.DrawRotate(Game.I.Frame * 0.02);
							DDDraw.DrawEnd();
							DDDraw.Reset();
						}
					}
					break;

				default:
					throw null; // never
			}
		}

		/// <summary>
		/// ショット実行
		/// </summary>
		public void Shot()
		{
			double pl_x;
			double pl_y;

			if (1 <= this.BornFrame)
			{
				pl_x = this.BornFollowX;
				pl_y = this.BornFollowY;

				if (DDUtils.IsOut(new D2Point(pl_x, pl_y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H))) // 画面外からは撃てない。
				{
					return;
				}
			}
			else
			{
				pl_x = this.X;
				pl_y = this.Y;
			}

			switch (this.AbsorbedWeapon)
			{
				case -1:
					break;

				case 1:
					if (Game.I.Frame % 4 == 0)
					{
						double ZURE = 20;

						Game.I.Shots.Add(new Shot_Strong(
							pl_x + ZURE,
							pl_y - ZURE,
							(Math.PI / 4.0) * 1
							));
#if true // 背後へは撃たない
						Game.I.Shots.Add(new Shot_Strong(
							pl_x + ZURE,
							pl_y,
							(Math.PI / 4.0) * 2
							));
						Game.I.Shots.Add(new Shot_Strong(
							pl_x - ZURE,
							pl_y,
							(Math.PI / 4.0) * 6
							));
#else // 全部斜め
						Game.I.Shots.Add(new Shot_Strong(
							pl_x + ZURE,
							pl_y + ZURE,
							(Math.PI / 4.0) * 3
							));
						Game.I.Shots.Add(new Shot_Strong(
							pl_x - ZURE,
							pl_y + ZURE,
							(Math.PI / 4.0) * 5
							));
#endif
						Game.I.Shots.Add(new Shot_Strong(
							pl_x - ZURE,
							pl_y - ZURE,
							(Math.PI / 4.0) * 7
							));
					}
					break;

				case 2:
					if (Game.I.Frame % 2 == 0)
					{
						for (int c = -1; c <= 1; c += 2)
						{
							Game.I.Shots.Add(new Shot_WaveBehind(
								pl_x + 20.0 * c,
								pl_y - 5.0,
								0.3 * c,
								0.3 * c,
								0.95 + SCommon.ToRange(this.SlowFrame * 0.0005, -0.02, 0.02)
								));
						}
					}
					break;

				case 3:
					if (Game.I.Frame % 4 == 0)
					{
						for (int c = -1; c <= 1; c += 2)
						{
							Game.I.Shots.Add(new Shot_Homing(
								pl_x,
								pl_y - 10.0
								));
						}
					}
					break;

				default:
					throw null; // never
			}

			switch (this.PlayerWho)
			{
				case PlayerWho_e.メディスン:
					{
						if (Game.I.Frame % 4 == 0)
						{
							Ground.I.SE.SE_PLAYERSHOT.Play();
							Game.I.Shots.Add(new Shot_Laserメディスン(pl_x, pl_y - 10.0, this.Level));
						}
					}
					break;

				case PlayerWho_e.小悪魔:
					{
						if (1 <= this.SlowFrame) // 低速移動
						{
							if (Game.I.Frame % 4 == 0)
							{
								Ground.I.SE.SE_PLAYERSHOT.Play();
								Game.I.Shots.Add(new Shot_Laser(pl_x, pl_y - 10.0, this.Level));
							}
						}
						else // 高速移動
						{
							if (Game.I.Frame % 3 == 0)
							{
								Ground.I.SE.SE_PLAYERSHOT.Play();
							}

							switch (this.Level) // 前方ショット
							{
								case 0:
								case 1:
									{
										if (Game.I.Frame % 3 == 0)
										{
											Game.I.Shots.Add(new Shot_Normal(
												pl_x,
												pl_y - 5.0,
												0.0
												));
										}
									}
									break;

								case 2:
									{
										if (Game.I.Frame % 3 == 0)
										{
											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Normal(
													pl_x + 8.0 * c,
													pl_y - 5.0,
													0.0
													));
											}
										}
									}
									break;

								case 3:
									{
										if (Game.I.Frame % 3 == 0)
										{
											Game.I.Shots.Add(new Shot_Strong(
												pl_x,
												pl_y - 5.0,
												0.0
												));

											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Normal(
													pl_x + 16.0 * c,
													pl_y - 5.0,
													0.1 * c
													));
											}
										}
									}
									break;

								case 4:
									{
										if (Game.I.Frame % 3 == 0)
										{
											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Normal(
													pl_x + 8.0 * c,
													pl_y - 5.0,
													0.0
													));
											}
											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Normal(
													pl_x + 16.0 * c,
													pl_y - 5.0,
													0.1 * c
													));
											}
										}
									}
									break;

								case 5:
									{
										if (Game.I.Frame % 3 == 0)
										{
											Game.I.Shots.Add(new Shot_Strong(
												pl_x,
												pl_y - 5.0,
												0.0
												));

											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Strong(
													pl_x + 16.0 * c,
													pl_y - 5.0,
													0.05 * c
													));

												Game.I.Shots.Add(new Shot_Normal(
													pl_x + 24.0 * c,
													pl_y - 5.0,
													0.1 * c
													));
											}
										}
									}
									break;

								default:
									throw null; // never
							}
							switch (this.Level) // サイドショット
							{
								case 0:
									break;

								case 1:
								case 2:
								case 3:
								case 4:
								case 5:
									{
										if (Game.I.Frame % 4 == 0)
										{
											for (int c = -1; c <= 1; c += 2)
											{
												Game.I.Shots.Add(new Shot_Wave(
													pl_x + 20.0 * c,
													pl_y - 5.0,
													0.3 * c,
													(-0.021 - 0.003 * this.YMoveRate) * c
													));
											}
										}
									}
									break;

								default:
									throw null; // never
							}
						}
					}
					break;

				default:
					throw null; // never
			}
		}

		/// <summary>
		/// ボム使用
		/// </summary>
		public void Bomb()
		{
			switch (this.PlayerWho)
			{
				case PlayerWho_e.メディスン:
					{
						Game.I.Shots.Add(new Shot_Bombメディスン(this.X, this.Y));
					}
					break;

				case PlayerWho_e.小悪魔:
					{
						Game.I.Shots.Add(new Shot_Bomb(this.X, this.Y));
					}
					break;

				default:
					throw null; // never
			}

			//Game.I.BombUsed = true; // ボム使用しました！フラグを立てる。
		}

		/// <summary>
		/// 当たり判定を配置する。
		/// 処理すべきこと：
		/// Game.I.PlayerCrashes への追加
		/// Game.I.GrazeCrashes への追加
		/// </summary>
		public void Put当たり判定()
		{
			const double GRAZE_R = 10.0;

			foreach (DDScene scene in DDSceneUtils.Create(5))
			{
				D2Point pt = DDUtils.AToBRate(
					new D2Point(this.X, this.Y),
					new D2Point(this.LastX, this.LastY),
					scene.Rate
					);

				Game.I.PlayerCrashes.Add(DDCrashUtils.Point(pt));
				Game.I.GrazeCrashes.Add(DDCrashUtils.Circle(pt, GRAZE_R));
			}
		}
	}
}
