using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Enemies;
using Charlotte.Games.Scripts;
using Charlotte.Games.Shots;
using Charlotte.Games.Surfaces;
using Charlotte.Games.Walls;

namespace Charlotte.Games
{
	public class Game : IDisposable
	{
		public int Zanki = GameConsts.DEFAULT_ZANKI;
		public int ZanBomb = GameConsts.DEFAULT_ZAN_BOMB;
		public long Score = 0;
		public Script Script = new Script_Dummy0001(); // 軽量なダミー初期オブジェクト
		public Player Player = new Player();
		public GameStatus Status = new GameStatus(); // 軽量なダミー初期オブジェクト

		// <---- prm

		public static Game I;

		public DDSubScreen Field;
		public DDSubScreen Field_Last;

		public Game()
		{
			I = this;

			this.Field = new DDSubScreen(GameConsts.FIELD_W, GameConsts.FIELD_H);
			this.Field_Last = new DDSubScreen(GameConsts.FIELD_W, GameConsts.FIELD_H);
		}

		public void Dispose()
		{
			this.Field.Dispose();
			this.Field = null;
			this.Field_Last.Dispose();
			this.Field_Last = null;

			I = null;
		}

		public struct InputInfo
		{
			public bool Dir2;
			public bool Dir4;
			public bool Dir6;
			public bool Dir8;
			public bool Slow;
			public bool Shot;
			public bool Bomb;
		}

		public InputInfo Input;
		public InputInfo LastInput;

		public int Frame;

		public SurfaceManager SurfaceManager = new SurfaceManager();
		public bool 掛け合い中 = false;
		public bool BossBattleStarted = false; // 開戦時のボス Enemy によってセットされる。
		public bool BossKilled = false; // ボス Enemy の Kill() によってセットされる。
		public double BackgroundSlideRate = 0.5;
		//public bool BombUsed = false; // ユーザー系ボムの Shot を使用すると true にセットされる。
		public bool PlayerWasDead = false; // 死亡すると true にセットされる。

		public void Perform()
		{
			DDUtils.Random = new DDRandom(1u, 1u, 1u, 1u); // 電源パターン確保のため

			DDCurtain.SetCurtain();
			DDEngine.FreezeInput();

			Func<bool> f_ゴミ回収 = SCommon.Supplier(this.E_ゴミ回収());

			// reset
			{
				RippleEffect.Clear();
				FieldDivider.Enabled = false;
			}

			this.Player.Reset(false);

			// ★★★★★ ステータス反映
			{
				this.Score = this.Status.Score;
				this.Player.Power = this.Status.PlayerPower;
				this.Zanki = this.Status.PlayerZanki;
				this.ZanBomb = this.Status.PlayerZanBomb;
			}

			for (this.Frame = 0; ; this.Frame++)
			{
				if (!this.Script.EachFrame()) // シナリオ進行
					break;

				// ********
				// **** チート ****
				// ********
				{
					if (DDKey.IsPound(DX.KEY_INPUT_PGUP))
					{
						this.Player.Power += GameConsts.PLAYER_POWER_PER_LEVEL;
					}
					if (DDKey.IsPound(DX.KEY_INPUT_PGDN))
					{
						this.Player.Power -= GameConsts.PLAYER_POWER_PER_LEVEL;
					}
					DDUtils.ToRange(ref this.Player.Power, 0, GameConsts.PLAYER_POWER_MAX);
				}

				if (DDInput.PAUSE.GetInput() == 1 && 10 < this.Frame) // ポーズ
				{
					this.Pause();

					if (this.Pause_RestartGame)
					{
						GameMaster.RestartFlag = true;
						break;
					}
					if (this.Pause_ReturnToTitleMenu)
					{
						GameMaster.ReturnToTitleMenu = true;
						break;
					}
				}

				this.LastInput = this.Input; // 前回のプレイヤー入力を退避

				// プレイヤー入力
				{
					this.Input.Dir2 = 1 <= DDInput.DIR_2.GetInput();
					this.Input.Dir4 = 1 <= DDInput.DIR_4.GetInput();
					this.Input.Dir6 = 1 <= DDInput.DIR_6.GetInput();
					this.Input.Dir8 = 1 <= DDInput.DIR_8.GetInput();
					this.Input.Slow = 1 <= DDInput.A.GetInput();
					this.Input.Shot = 1 <= DDInput.B.GetInput();
					this.Input.Bomb = 1 <= DDInput.C.GetInput();
				}

				this.Player.LastX = this.Player.X;
				this.Player.LastY = this.Player.Y;

			startBornPlayer:
				if (1 <= this.Player.BornFrame) // プレイヤー登場中の処理
				{
					int frm = this.Player.BornFrame - 1;

					if (GameConsts.PLAYER_BORN_FRAME_MAX < frm)
					{
						this.Player.BornFrame = 0;
						goto endBornPlayer;
					}
					this.Player.BornFrame++;
					double rate = (double)frm / GameConsts.PLAYER_BORN_FRAME_MAX;

					if (frm == 0) // init
					{
						this.Player.BornFollowX = GameConsts.FIELD_W * 0.5;
						this.Player.BornFollowY = GameConsts.FIELD_H * 1.2;
					}

					double approachingRate = 0.99 - 0.01 * frm;
					DDUtils.ToRange(ref approachingRate, 0.0, 1.0);

					DDUtils.Approach(ref this.Player.BornFollowX, this.Player.X, approachingRate);
					DDUtils.Approach(ref this.Player.BornFollowY, this.Player.Y, approachingRate);
				}
			endBornPlayer:

				//startDeadPlayer:
				if (1 <= this.Player.DeadFrame) // プレイヤー死亡中の処理
				{
					int frm = this.Player.DeadFrame - 1;

					if (GameConsts.PLAYER_DEAD_FRAME_MAX < frm)
					{
						if (this.Zanki <= 0) // 残機不足のため終了
							break;

						this.Zanki--;
						this.Player.Reset(true);
						goto startBornPlayer;
					}
					this.Player.DeadFrame++;

					if (frm == 0) // init
					{
						this.DeadPlayerMoment();
						this.PlayerEffects.Add(SCommon.Supplier(Effects.PlayerDead(this.Player.X, this.Player.Y)));
					}
					goto endPlayerMove;
				}
				//endDeadPlayer:

				//startBombPlayer:
				if (1 <= this.Player.BombFrame) // ボム使用中の処理
				{
					int frm = this.Player.BombFrame - 1;

					if (GameConsts.PLAYER_BOMB_FRAME_MAX < frm)
					{
						this.Player.BombFrame = 0;
						goto endBombPlayer;
					}
					this.Player.BombFrame++;

					if (frm == 0) // init
					{
						this.Player.Bomb();
					}
				}
			endBombPlayer:

				//startPlayerMove:
				// プレイヤー移動
				{
					bool d2 = this.Input.Dir2;
					bool d4 = this.Input.Dir4;
					bool d6 = this.Input.Dir6;
					bool d8 = this.Input.Dir8;

					double speed;

					if (this.Input.Slow)
						speed = 2.5;
					else
						speed = 5.0;

					double nanameSpeed = speed / Math.Sqrt(2.0);

					if (d2 && d4) // 左下
					{
						this.Player.X -= nanameSpeed;
						this.Player.Y += nanameSpeed;
					}
					else if (d2 && d6) // 右下
					{
						this.Player.X += nanameSpeed;
						this.Player.Y += nanameSpeed;
					}
					else if (d4 && d8) // 左上
					{
						this.Player.X -= nanameSpeed;
						this.Player.Y -= nanameSpeed;
					}
					else if (d6 && d8) // 右上
					{
						this.Player.X += nanameSpeed;
						this.Player.Y -= nanameSpeed;
					}
					else if (d2) // 下
					{
						this.Player.Y += speed;
					}
					else if (d4) // 左
					{
						this.Player.X -= speed;
					}
					else if (d6) // 右
					{
						this.Player.X += speed;
					}
					else if (d8) // 上
					{
						this.Player.Y -= speed;
					}

					DDUtils.ToRange(ref this.Player.X, 0.0, (double)GameConsts.FIELD_W);
					DDUtils.ToRange(ref this.Player.Y, 0.0, (double)GameConsts.FIELD_H);

					if (d4) // 左
					{
						DDUtils.Minim(ref this.Player.XMoveFrame, 0);
						this.Player.XMoveFrame--;
					}
					else if (d6) // 右
					{
						DDUtils.Maxim(ref this.Player.XMoveFrame, 0);
						this.Player.XMoveFrame++;
					}
					else
					{
						this.Player.XMoveFrame = 0;
					}

					if (d8) // 上
					{
						DDUtils.Minim(ref this.Player.YMoveFrame, 0);
						this.Player.YMoveFrame--;
					}
					else if (d2) // 下
					{
						DDUtils.Maxim(ref this.Player.YMoveFrame, 0);
						this.Player.YMoveFrame++;
					}
					else
					{
						this.Player.YMoveFrame = 0;
					}

					DDUtils.Approach(ref this.Player.XMoveRate, DDUtils.Sign(this.Player.XMoveFrame), 0.95);
					DDUtils.Approach(ref this.Player.YMoveRate, DDUtils.Sign(this.Player.YMoveFrame), 0.95);
				}
			endPlayerMove:

				if (this.Input.Slow)
				{
					DDUtils.Maxim(ref this.Player.SlowFrame, 0);
					this.Player.SlowFrame++;
				}
				else
				{
					DDUtils.Minim(ref this.Player.SlowFrame, 0);
					this.Player.SlowFrame--;
				}

				if (this.Input.Shot)
				{
					DDUtils.Maxim(ref this.Player.ShotFrame, 0);
					this.Player.ShotFrame++;
				}
				else
				{
					DDUtils.Minim(ref this.Player.ShotFrame, 0);
					this.Player.ShotFrame--;
				}

				DDUtils.Approach(ref this.Player.SlowRate, this.Player.SlowFrame < 0 ? 0.0 : 1.0, 0.85);
				DDUtils.Approach(ref this.Player.ShotRate, this.Player.ShotFrame < 0 ? 0.0 : 1.0, 0.85);

				if (this.LastInput.Shot && this.Input.Shot && SCommon.IsRange(this.Player.SlowFrame, -1, 1)) // ? ショット中に低速・高速を切り替えた。
				{
					this.Player.ShotRate = 0.0;
				}

				// ----

				if (this.Input.Shot && this.Player.DeadFrame == 0) // プレイヤーショット
				{
					this.Player.Shot();
				}
				if (this.Input.Bomb && this.Player.DeadFrame == 0 && this.Player.BombFrame == 0 && 1 <= this.ZanBomb) // ボム使用
				{
					this.ZanBomb--;
					this.Player.BombFrame = 1;
				}

				// 当たり判定 reset
				{
					this.PlayerCrashes.Clear();
					this.GrazeCrashes.Clear();
					this.EnemyCrashes.Clear();
					this.ShotCrashes.Clear();
				}

				this.Player.Put当たり判定();

				// ====
				// 描画ここから
				// ====

				// Swap
				{
					DDSubScreen tmp = this.Field;
					this.Field = this.Field_Last;
					this.Field_Last = tmp;
				}

				using (this.Field.Section()) // フィールド描画
				{
					// Walls
					{
						int filledIndex = -1;

						for (int index = 0; index < this.Walls.Count; index++)
						{
							Wall wall = this.Walls[index];

							if (!wall.Draw())
								this.Walls[index] = null;
							else if (wall.Filled)
								filledIndex = index;
						}
						for (int index = 0; index < filledIndex; index++)
							this.Walls[index] = null;

						this.Walls.RemoveAll(v => v == null);
					}

					RippleEffect.EachFrame(this.Field);

					this.EL_AfterDrawWalls.ExecuteAllTask();

					// Shots
					{
						for (int index = 0; index < this.Shots.Count; index++)
						{
							Shot shot = this.Shots[index];

							if (!shot.Draw())
								this.Shots[index] = null;
						}
						this.Shots.RemoveAll(v => v == null);
					}

					this.EL_AfterDrawShots.ExecuteAllTask();

					if (this.Player.DeadFrame == 0)
						this.Player.Draw();

					this.EL_AfterDrawPlayer.ExecuteAllTask();

					// Enemies
					{
						foreach (Enemy.Kind_e kind in new Enemy.Kind_e[]
						{
							Enemy.Kind_e.ENEMY,
							Enemy.Kind_e.TAMA,
							Enemy.Kind_e.ITEM,
						})
						{
							for (int index = 0; index < this.Enemies.Count; index++)
							{
								Enemy enemy = this.Enemies[index];

								if (enemy != null && enemy.Kind == kind)
									if (!enemy.Draw())
										this.Enemies[index] = null;
							}
						}
						this.Enemies.RemoveAll(v => v == null);
					}

					this.EnemyEffects.ExecuteAllTask();
					this.PlayerEffects.ExecuteAllTask();

					if (1 <= DDInput.R.GetInput()) // 当たり判定表示(チート)
					{
						this.Draw当たり判定();
					}

					FieldDivider.EachFrame(this.Field);
				}

				{
					DDUtils.Approach(ref this.BackgroundSlideRate, this.Player.Y * 1.0 / GameConsts.FIELD_H, 0.99); // HACK

					D4Rect rect = DDUtils.AdjustRectExterior(
						new D2Size(GameConsts.FIELD_W, GameConsts.FIELD_H),
						new D4Rect(0, 0, DDConsts.Screen_W, DDConsts.Screen_H),
						this.BackgroundSlideRate
						//this.Player.Y * 1.0 / GameConsts.FIELD_H
						);

					DDDraw.DrawRect(this.Field.ToPicture(), rect);
				}

				{
					const int MARGIN = 5;

					DDDraw.SetBright(0, 0, 0);
					DDDraw.DrawRect(
						DDGround.GeneralResource.WhiteBox,
						GameConsts.FIELD_L - MARGIN,
						GameConsts.FIELD_T - MARGIN,
						GameConsts.FIELD_W + MARGIN * 2,
						GameConsts.FIELD_H + MARGIN * 2
						);
					DDDraw.Reset();
				}

				DX.GraphFilter(
					DDGround.MainScreen.GetHandle(),
					DX.DX_GRAPH_FILTER_GAUSS,
					16,
					SCommon.ToInt(500.0)
					);

				DDCurtain.DrawCurtain(-0.2);

				DDDraw.DrawSimple(this.Field.ToPicture(), GameConsts.FIELD_L, GameConsts.FIELD_T);
				this.DrawStatus();
				this.SurfaceManager.Draw();

				// ====
				// 描画ここまで
				// ====

				foreach (Enemy enemy in this.Enemies.Iterate())
				{
					if (!DDUtils.IsOut(new D2Point(enemy.X, enemy.Y), new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H))) // ? フィールド内
					{
						DDUtils.CountDown(ref enemy.TransFrame);
						enemy.OnFieldFrame++;
					}
				}

				this.当たり判定();

				if (this.AH_Grazed)
				{
					if (
						1 <= this.Player.BornFrame ||
						1 <= this.Player.DeadFrame
						)
					{
						// 登場中・死亡中は何もしない。
					}
					else
					{
						Ground.I.SE.SE_KASURI.Play();
						this.Score++;
					}
				}
				if (this.AH_PlayerCrashedFlag)
				{
					if (
						1 <= this.Player.BornFrame ||
						1 <= this.Player.DeadFrame ||
						1 <= this.Player.BombFrame
						)
					{
						// 登場中・死亡中・ボム使用中は何もしない。
					}
					else
					{
						this.Player.DeadFrame = 1; // プレイヤー死亡
						this.PlayerWasDead = true;
					}
				}

				if (Ground.I.HiScore < this.Score)
				{
					if (Ground.I.HiScore + 10L < this.Score)
					{
						Ground.I.HiScore -= this.Score;
						Ground.I.HiScore /= 2L;
						Ground.I.HiScore += this.Score;
					}
					else
						Ground.I.HiScore++;
				}

				f_ゴミ回収();

				this.Enemies.RemoveAll(v => v.HP == -1);
				this.Shots.RemoveAll(v => v.Vanished);

				DDEngine.EachFrame();

				// ★★★ ゲームループの終わり ★★★
			}

			DDUtils.Maxim(ref Ground.I.HiScore, this.Score); // 確実な同期

			// ★★★★★ ステータス反映
			{
				this.Status.Score = this.Score;
				this.Status.PlayerPower = this.Player.Power;
				this.Status.PlayerZanki = this.Zanki;
				this.Status.PlayerZanBomb = this.ZanBomb;
			}

			DDMain.KeepMainScreen();

			DDMusicUtils.Fade();
			DDCurtain.SetCurtain(60, -1.0);

			foreach (DDScene scene in DDSceneUtils.Create(120))
			{
				DDDraw.DrawRect(DDGround.KeptMainScreen.ToPicture(), 0, 0, DDConsts.Screen_W, DDConsts.Screen_H);
				DDEngine.EachFrame();
			}

			// ★★★ end of Perform() ★★★
		}

		/// <summary>
		/// あまりにもフィールドから離れすぎている敵・自弾を除去する。
		/// </summary>
		/// <returns></returns>
		private IEnumerable<bool> E_ゴミ回収()
		{
			for (; ; )
			{
				foreach (Enemy enemy in this.Enemies.Iterate())
				{
					if (this.IsProbablyEvacuated(enemy.X, enemy.Y))
						enemy.HP = -1;

					yield return true;
				}
				foreach (Shot shot in this.Shots.Iterate())
				{
					if (this.IsProbablyEvacuated(shot.X, shot.Y))
						shot.Vanished = true;

					yield return true;
				}
				yield return true; // ループ内で1度も実行されない場合を想定
			}
		}

		private bool Pause_RestartGame = false;
		private bool Pause_ReturnToTitleMenu = false;

		private void Pause()
		{
			DDMain.KeepMainScreen();

			DDSimpleMenu simpleMenu = new DDSimpleMenu()
			{
				Color = new I3Color(255, 255, 255),
				BorderColor = new I3Color(0, 64, 128),
				WallPicture = DDGround.KeptMainScreen.ToPicture(),
				WallCurtain = -0.5,
			};

			int selectIndex = 0;

			for (; ; )
			{
				selectIndex = simpleMenu.Perform(
					"ポーズメニュー(仮)",
					new string[]
					{
						"最初からやり直す",
						"タイトルに戻る",
						"ゲームに戻る",
					},
					selectIndex,
					true,
					true
					);

				switch (selectIndex)
				{
					case 0:
						this.Pause_RestartGame = true;
						goto endLoop;

					case 1:
						this.Pause_ReturnToTitleMenu = true;
						goto endLoop;

					case 2:
						goto endLoop;

					default:
						throw null; // never
				}
				//DDEngine.EachFrame(); // 不要
			}
		endLoop:
			DDEngine.FreezeInput();
		}

		private void DeadPlayerMoment()
		{
			DDMain.KeepMainScreen();

			// memo: 喰らいボムはここでやれば良いんじゃないだろうか。

			foreach (DDScene scene in DDSceneUtils.Create(30))
			{
				DDDraw.DrawRect(DDGround.KeptMainScreen.ToPicture(), 0, 0, DDConsts.Screen_W, DDConsts.Screen_H);

				DDDraw.SetAlpha(0.2 + 0.2 * scene.Rate);
				DDDraw.SetBright(1.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0, 0, DDConsts.Screen_W, DDConsts.Screen_H));
				DDDraw.Reset();

				DDEngine.EachFrame();
			}
			DDEngine.FreezeInput();
		}

		// ---- 敵などのフィールド上のオブジェクト ----

		// Walls:
		// 壁紙リスト
		// 壁紙の重ね合わせ, Filled == true によって、それより下の(古い)壁紙が除去される方式

		public DDList<Enemy> Enemies = new DDList<Enemy>();
		public DDList<Shot> Shots = new DDList<Shot>();
		public DDList<Wall> Walls = new DDList<Wall>();
		public DDTaskList EnemyEffects = new DDTaskList();
		public DDTaskList PlayerEffects = new DDTaskList();
		public DDTaskList EL_AfterDrawWalls = new DDTaskList();
		public DDTaskList EL_AfterDrawShots = new DDTaskList();
		public DDTaskList EL_AfterDrawPlayer = new DDTaskList();

		// ---- 敵などのフィールド上のオブジェクトの当たり判定 ----

		public DDList<DDCrash> PlayerCrashes = new DDList<DDCrash>();
		public DDList<DDCrash> GrazeCrashes = new DDList<DDCrash>();
		public DDList<DDCrash> EnemyCrashes = new DDList<DDCrash>();
		public DDList<DDCrash> ShotCrashes = new DDList<DDCrash>();

		// ---- 敵などのフィールド上のオブジェクト系 (ここまで) ----

		#region 当たり判定

		/// <summary>
		/// プレイヤーかすり判定
		/// </summary>
		private bool AH_Grazed;

		/// <summary>
		/// プレイヤー被弾判定
		/// </summary>
		private bool AH_PlayerCrashedFlag;

		/// <summary>
		/// 敵 被弾
		/// </summary>
		/// <param name="enemy">被弾した敵</param>
		/// <param name="shot">被弾させた自弾</param>
		private void AH_EnemyCrashed(Enemy enemy, Shot shot, DDCrash shotCrash)
		{
			switch (shot.Kind)
			{
				case Shot.Kind_e.NORMAL:
					{
						if (!shot.Vanished) // ? まだ消滅していない。
						{
							this.PlayerEffects.Add(SCommon.Supplier(Effects.ShotDead(shot.X, shot.Y, shotCrash.R))); // 自弾消滅エフェクト
							shot.Vanished = true; // 敵に当たったら「通常弾」は消滅する。
						}
					}
					break;

				case Shot.Kind_e.BOMB:
					{
						// 敵に当たっても「ボム」は消滅しない。

						if (enemy.IsBoss()) // ボスの場合は回復！
						{
							DDGround.EL.Add(SCommon.Supplier(Effects.ボス回復(GameConsts.FIELD_L + enemy.X, GameConsts.FIELD_T + enemy.Y)));

#if true
							enemy.HP -= enemy.InitialHP;
							enemy.HP = (int)(enemy.HP * 0.99);
							enemy.HP += enemy.InitialHP;
#else // old
							enemy.HP++;
							enemy.HP = Math.Min(enemy.HP, enemy.InitialHP);
#endif

							return;
						}
					}
					break;

				default:
					throw null; // never
			}

			enemy.HP -= shot.AttackPoint;

			if (1 <= enemy.HP) // ? まだ生存している。
			{
				enemy.Damaged();
			}
			else // ? 死亡した。
			{
				enemy.HP = -1; // 死亡をセットする。
				enemy.Killed();
			}
		}

		/// <summary>
		/// プレイヤー被弾
		/// </summary>
		/// <param name="enemy">被弾させた敵</param>
		private void AH_PlayerCrashed(Enemy enemy)
		{
			switch (enemy.Kind)
			{
				case Enemy.Kind_e.ENEMY:
				case Enemy.Kind_e.TAMA:
					{
						this.AH_PlayerCrashedFlag = true;
					}
					break;

				case Enemy.Kind_e.ITEM:
					{
						// アイテム回収
						{
							enemy.HP = -1;
							enemy.Killed();
						}
					}
					break;

				default:
					throw null; // never
			}
		}

		private void 当たり判定()
		{
			// reset
			{
				this.AH_Grazed = false;
				this.AH_PlayerCrashedFlag = false;
			}

			if (this.掛け合い中) // 掛け合い中は何にも当たらない。
				return;

			for (int ecIndex = 0; ecIndex < this.EnemyCrashes.Count; ecIndex++) // 敵 x 自弾(通常弾)
			{
				DDCrash ec = this.EnemyCrashes[ecIndex];

				if (ec.OwnerEnemy.HP <= 0) // ? 無敵 | アイテム | 死亡
					continue;

				if (1 <= ec.OwnerEnemy.TransFrame) // ? 無敵時間
					continue;

				for (int scIndex = 0; scIndex < this.ShotCrashes.Count; scIndex++)
				{
					DDCrash sc = this.ShotCrashes[scIndex];

					if (sc.OwnerShot.Kind != Shot.Kind_e.NORMAL) // ? 通常弾ではない。
						continue;

					if (sc.OwnerShot.Vanished) // ? 既に消滅した。
						continue;

					if (DDCrashUtils.IsCrashed(ec, sc))
					{
						this.AH_EnemyCrashed(ec.OwnerEnemy, sc.OwnerShot, sc);

						if (ec.OwnerEnemy.HP == -1) // ? 死亡
						{
							break; // この敵は死亡したので、このクラッシュについて以降の当たり判定は不要
						}
					}
				}
			}
			for (int scIndex = 0; scIndex < this.ShotCrashes.Count; scIndex++) // 自弾(ボム) x 敵
			{
				DDCrash sc = this.ShotCrashes[scIndex];

				if (sc.OwnerShot.Kind != Shot.Kind_e.BOMB) // ? ボムではない。
					continue;

				// ボムは消滅しない。
				//if (sc.OwnerShot.Vanished) // ? 既に消滅した。
				//continue;

				for (int ecIndex = 0; ecIndex < this.EnemyCrashes.Count; ecIndex++)
				{
					DDCrash ec = this.EnemyCrashes[ecIndex];

					if (ec.OwnerEnemy.HP == -1) // ? 死亡
						continue;

					// 無敵・無敵時間であっても「ボム」には当たる。

					if (ec.OwnerEnemy.Kind == Enemy.Kind_e.ITEM) // アイテムには当たらない。
						continue;

					// 画面外の敵には当たらない。
					if (DDUtils.IsOut(
						new D2Point(ec.OwnerEnemy.X, ec.OwnerEnemy.Y),
						new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H),
						20.0 // マージンはこれ良いか？
						))
						continue;

					if (DDCrashUtils.IsCrashed(sc, ec))
					{
						this.AH_EnemyCrashed(ec.OwnerEnemy, sc.OwnerShot, sc);
					}
				}
			}
			for (int ecIndex = 0; ecIndex < this.EnemyCrashes.Count; ecIndex++) // 敵 x 自機
			{
				DDCrash ec = this.EnemyCrashes[ecIndex];

				if (ec.OwnerEnemy.HP == -1) // ? 死亡
					continue;

				if (!this.AH_Grazed) // かすり判定
				{
					if (ec.OwnerEnemy.Kind == Enemy.Kind_e.ITEM) // アイテムとはグレイズしない。
						goto endGrazeHantei;

					for (int gcIndex = 0; gcIndex < this.GrazeCrashes.Count; gcIndex++)
					{
						DDCrash gc = this.GrazeCrashes[gcIndex];

						if (DDCrashUtils.IsCrashed(ec, gc))
						{
							this.AH_Grazed = true;
							break;
						}
					}
				}
			endGrazeHantei:

				// 被弾判定
				{
					for (int pcIndex = 0; pcIndex < this.PlayerCrashes.Count; pcIndex++)
					{
						DDCrash pc = this.PlayerCrashes[pcIndex];

						if (DDCrashUtils.IsCrashed(ec, pc))
						{
							this.AH_PlayerCrashed(ec.OwnerEnemy);
							break;
						}
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// フィールドに全ての当たり判定を描画する。
		/// チート機能
		/// 描画先はフィールドのサブスクリーンであること。
		/// </summary>
		private void Draw当たり判定()
		{
			DDDraw.SetAlpha(0.8);
			DDDraw.SetBright(0, 0, 0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0, 0, GameConsts.FIELD_W, GameConsts.FIELD_H));
			DDDraw.Reset();

			const double A = 0.2;

			DDCrashView.Draw(this.GrazeCrashes.Iterate(), new I3Color(0, 128, 0), A);
			DDCrashView.Draw(this.PlayerCrashes.Iterate(), new I3Color(255, 0, 0), A);
			DDCrashView.Draw(this.EnemyCrashes.Iterate(), new I3Color(255, 255, 255), A);
			DDCrashView.Draw(this.ShotCrashes.Iterate(), new I3Color(0, 255, 255), A);
		}

		private void DrawStatus()
		{
			DDPrint.SetPrint(760, 30, 20);

			DDPrint.SetBorder(new I3Color(100, 0, 150));
			DDPrint.PrintLine("HiSCORE");
			DDPrint.PrintLine(string.Format("{0,20}", Ground.I.HiScore));

			DDPrint.SetBorder(new I3Color(110, 0, 140));
			DDPrint.PrintLine("  SCORE");
			DDPrint.PrintLine(string.Format("{0,20}", this.Score));
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");

			DDPrint.SetBorder(new I3Color(120, 0, 130));
			DDPrint.PrintLine(string.Format(" PLAYER  {0}", string.Join("", Enumerable.Repeat("★", this.Zanki))));
			DDPrint.PrintLine("");

			DDPrint.SetBorder(new I3Color(130, 0, 120));
			DDPrint.PrintLine(string.Format("   BOMB  {0}", string.Join("", Enumerable.Repeat("＠", this.ZanBomb))));
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");

			DDPrint.SetBorder(new I3Color(150, 150, 100));
			DDPrint.PrintLine("  POWER");
			DDPrint.Reset();

			DrawStatus_Power();
		}

		private void DrawStatus_Power()
		{
			int power = this.Player.Power;
			string sPower = string.Format("{0}.{1:D2}", power / 100, power % 100);
			int digColor;

			switch (this.Player.Level)
			{
				case 0:
					digColor = 0;
					break;

				case 1:
				case 2:
					digColor = 1;
					break;

				case 3:
				case 4:
					digColor = 2;
					break;

				case 5:
					digColor = 3;
					break;

				default:
					throw null; // never
			}

			const int DIG_W = 16;
			const int DIG_H = 32;
			const int DIG_SPAN = 2;
			int x = 850;
			int y = 260;

			DrawDigit(x, y, DIG_W * 2, DIG_H * 2, digColor, this.Player.Level);
			x += DIG_W * 2 + DIG_SPAN;
			y += DIG_H;

			DrawDigit(x, y, DIG_W, DIG_H, digColor, 12);
			x += DIG_W + DIG_SPAN;

			DrawDigit(x, y, DIG_W, DIG_H, digColor, this.Player.Power / 10 % 10);
			x += DIG_W + DIG_SPAN;

			DrawDigit(x, y, DIG_W, DIG_H, digColor, this.Player.Power % 10);
		}

		private void DrawDigit(int l, int t, int w, int h, int digColor, int digIndex)
		{
			DDPicture[][] pictureTable = new DDPicture[][]
			{
				Ground.I.Picture2.D_DIGITS_W_00,
				Ground.I.Picture2.D_DIGITS_DDY_00,
				Ground.I.Picture2.D_DIGITS_DY_00,
				Ground.I.Picture2.D_DIGITS_Y_00,
			};

			DDDraw.SetMosaic();
			DDDraw.DrawRect(pictureTable[digColor][digIndex], new D4Rect(l, t, w, h));
			DDDraw.Reset();
		}

		/// <summary>
		/// フィールドから離れすぎているか
		/// 退場と見なして良いか
		/// </summary>
		/// <param name="x">X-座標</param>
		/// <param name="y">Y-座標</param>
		/// <returns></returns>
		private bool IsProbablyEvacuated(double x, double y)
		{
			const int MGN_FIELD_NUM = 3;

			return
				x < -DDConsts.Screen_W * MGN_FIELD_NUM || DDConsts.Screen_W * (1 + MGN_FIELD_NUM) < x ||
				y < -DDConsts.Screen_H * MGN_FIELD_NUM || DDConsts.Screen_H * (1 + MGN_FIELD_NUM) < y;
		}
	}
}
