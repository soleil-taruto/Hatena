using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Enemies;
using Charlotte.Games.Scripts;
using Charlotte.Games.Shots;
using Charlotte.Games.Surfaces;
using Charlotte.Games.Walls;
using DxLibDLL;

namespace Charlotte.Games
{
	// Token: 0x02000014 RID: 20
	public class Game : IDisposable
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00003098 File Offset: 0x00001298
		public Game()
		{
			Game.I = this;
			this.Field = new DDSubScreen(512, 512, false);
			this.Field_Last = new DDSubScreen(512, 512, false);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000031AA File Offset: 0x000013AA
		public void Dispose()
		{
			this.Field.Dispose();
			this.Field = null;
			this.Field_Last.Dispose();
			this.Field_Last = null;
			Game.I = null;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000031D8 File Offset: 0x000013D8
		public void Perform()
		{
			DDUtils.Random = new DDRandom(1u, 1u, 1u, 1u);
			DDCurtain.SetCurtain(30, 0.0);
			DDEngine.FreezeInput(1);
			Func<bool> f_ゴミ回収 = SCommon.Supplier<bool>(this.E_ゴミ回収());
			RippleEffect.Clear();
			FieldDivider.Enabled = false;
			this.Player.Reset(false);
			this.Score = this.Status.Score;
			this.Player.Power = this.Status.PlayerPower;
			this.Zanki = this.Status.PlayerZanki;
			this.ZanBomb = this.Status.PlayerZanBomb;
			this.Frame = 0;
			while (this.Script.EachFrame())
			{
				if (DDKey.IsPound(201))
				{
					this.Player.Power += 100;
				}
				if (DDKey.IsPound(209))
				{
					this.Player.Power -= 100;
				}
				DDUtils.ToRange(ref this.Player.Power, 0, 500);
				if (DDInput.PAUSE.GetInput() == 1 && 10 < this.Frame)
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
				this.LastInput = this.Input;
				this.Input.Dir2 = (1 <= DDInput.DIR_2.GetInput());
				this.Input.Dir4 = (1 <= DDInput.DIR_4.GetInput());
				this.Input.Dir6 = (1 <= DDInput.DIR_6.GetInput());
				this.Input.Dir8 = (1 <= DDInput.DIR_8.GetInput());
				this.Input.Slow = (1 <= DDInput.A.GetInput());
				this.Input.Shot = (1 <= DDInput.B.GetInput());
				this.Input.Bomb = (1 <= DDInput.C.GetInput());
				this.Player.LastX = this.Player.X;
				this.Player.LastY = this.Player.Y;
				int frm2;
				for (;;)
				{
					if (1 <= this.Player.BornFrame)
					{
						int frm = this.Player.BornFrame - 1;
						if (300 < frm)
						{
							this.Player.BornFrame = 0;
						}
						else
						{
							this.Player.BornFrame++;
							double num = (double)frm / 300.0;
							if (frm == 0)
							{
								this.Player.BornFollowX = 256.0;
								this.Player.BornFollowY = 614.4;
							}
							double approachingRate = 0.99 - 0.01 * (double)frm;
							DDUtils.ToRange(ref approachingRate, 0.0, 1.0);
							DDUtils.Approach(ref this.Player.BornFollowX, this.Player.X, approachingRate);
							DDUtils.Approach(ref this.Player.BornFollowY, this.Player.Y, approachingRate);
						}
					}
					if (1 > this.Player.DeadFrame)
					{
						goto IL_3C3;
					}
					frm2 = this.Player.DeadFrame - 1;
					if (60 >= frm2)
					{
						goto IL_374;
					}
					if (this.Zanki <= 0)
					{
						goto IL_EF0;
					}
					this.Zanki--;
					this.Player.Reset(true);
				}
				IL_712:
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
				DDUtils.Approach(ref this.Player.SlowRate, (this.Player.SlowFrame < 0) ? 0.0 : 1.0, 0.85);
				DDUtils.Approach(ref this.Player.ShotRate, (this.Player.ShotFrame < 0) ? 0.0 : 1.0, 0.85);
				if (this.LastInput.Shot && this.Input.Shot && SCommon.IsRange(this.Player.SlowFrame, -1, 1))
				{
					this.Player.ShotRate = 0.0;
				}
				if (this.Input.Shot && this.Player.DeadFrame == 0)
				{
					this.Player.Shot();
				}
				if (this.Input.Bomb && this.Player.DeadFrame == 0 && this.Player.BombFrame == 0 && 1 <= this.ZanBomb)
				{
					this.ZanBomb--;
					this.Player.BombFrame = 1;
				}
				this.PlayerCrashes.Clear();
				this.GrazeCrashes.Clear();
				this.EnemyCrashes.Clear();
				this.ShotCrashes.Clear();
				this.Player.Put当たり判定();
				DDSubScreen tmp = this.Field;
				this.Field = this.Field_Last;
				this.Field_Last = tmp;
				using (this.Field.Section())
				{
					int filledIndex = -1;
					for (int index = 0; index < this.Walls.Count; index++)
					{
						Wall wall = this.Walls[index];
						if (!wall.Draw())
						{
							this.Walls[index] = null;
						}
						else if (wall.Filled)
						{
							filledIndex = index;
						}
					}
					for (int index2 = 0; index2 < filledIndex; index2++)
					{
						this.Walls[index2] = null;
					}
					this.Walls.RemoveAll((Wall v) => v == null);
					RippleEffect.EachFrame(this.Field);
					this.EL_AfterDrawWalls.ExecuteAllTask();
					for (int index3 = 0; index3 < this.Shots.Count; index3++)
					{
						if (!this.Shots[index3].Draw())
						{
							this.Shots[index3] = null;
						}
					}
					this.Shots.RemoveAll((Shot v) => v == null);
					this.EL_AfterDrawShots.ExecuteAllTask();
					if (this.Player.DeadFrame == 0)
					{
						this.Player.Draw();
					}
					this.EL_AfterDrawPlayer.ExecuteAllTask();
					foreach (Enemy.Kind_e kind in new Enemy.Kind_e[]
					{
						Enemy.Kind_e.ENEMY,
						Enemy.Kind_e.TAMA,
						Enemy.Kind_e.ITEM
					})
					{
						for (int index4 = 0; index4 < this.Enemies.Count; index4++)
						{
							Enemy enemy = this.Enemies[index4];
							if (enemy != null && enemy.Kind == kind && !enemy.Draw())
							{
								this.Enemies[index4] = null;
							}
						}
					}
					this.Enemies.RemoveAll((Enemy v) => v == null);
					this.EnemyEffects.ExecuteAllTask();
					this.PlayerEffects.ExecuteAllTask();
					if (1 <= DDInput.R.GetInput())
					{
						this.Draw当たり判定();
					}
					FieldDivider.EachFrame(this.Field);
				}
				DDUtils.Approach(ref this.BackgroundSlideRate, this.Player.Y * 1.0 / 512.0, 0.99);
				D4Rect rect = DDUtils.AdjustRectExterior(new D2Size(512.0, 512.0), new D4Rect(0.0, 0.0, 960.0, 540.0), this.BackgroundSlideRate);
				DDDraw.DrawRect(this.Field.ToPicture(), rect);
				DDDraw.SetBright(0.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 219.0, 9.0, 522.0, 522.0);
				DDDraw.Reset();
				DX.GraphFilter(DDGround.MainScreen.GetHandle(), 1, 16, SCommon.ToInt(500.0));
				DDCurtain.DrawCurtain(-0.2);
				DDDraw.DrawSimple(this.Field.ToPicture(), 224.0, 14.0);
				this.DrawStatus();
				this.SurfaceManager.Draw();
				foreach (Enemy enemy2 in this.Enemies.Iterate())
				{
					if (!DDUtils.IsOut(new D2Point(enemy2.X, enemy2.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
					{
						DDUtils.CountDown(ref enemy2.TransFrame);
						enemy2.OnFieldFrame++;
					}
				}
				this.当たり判定();
				if (this.AH_Grazed && 1 > this.Player.BornFrame && 1 > this.Player.DeadFrame)
				{
					Ground.I.SE.SE_KASURI.Play(true);
					this.Score += 1L;
				}
				if (this.AH_PlayerCrashedFlag && 1 > this.Player.BornFrame && 1 > this.Player.DeadFrame && 1 > this.Player.BombFrame)
				{
					this.Player.DeadFrame = 1;
					this.PlayerWasDead = true;
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
					{
						Ground.I.HiScore += 1L;
					}
				}
				f_ゴミ回収();
				this.Enemies.RemoveAll((Enemy v) => v.HP == -1);
				this.Shots.RemoveAll((Shot v) => v.Vanished);
				DDEngine.EachFrame();
				this.Frame++;
				continue;
				IL_374:
				this.Player.DeadFrame++;
				if (frm2 == 0)
				{
					this.DeadPlayerMoment();
					this.PlayerEffects.Add(SCommon.Supplier<bool>(Effects.PlayerDead(this.Player.X, this.Player.Y)));
					goto IL_712;
				}
				goto IL_712;
				IL_3C3:
				if (1 <= this.Player.BombFrame)
				{
					int frm3 = this.Player.BombFrame - 1;
					if (180 < frm3)
					{
						this.Player.BombFrame = 0;
					}
					else
					{
						this.Player.BombFrame++;
						if (frm3 == 0)
						{
							this.Player.Bomb();
						}
					}
				}
				bool d2 = this.Input.Dir2;
				bool d3 = this.Input.Dir4;
				bool d4 = this.Input.Dir6;
				bool d5 = this.Input.Dir8;
				double speed;
				if (this.Input.Slow)
				{
					speed = 2.5;
				}
				else
				{
					speed = 5.0;
				}
				double nanameSpeed = speed / Math.Sqrt(2.0);
				if (d2 && d3)
				{
					this.Player.X -= nanameSpeed;
					this.Player.Y += nanameSpeed;
				}
				else if (d2 && d4)
				{
					this.Player.X += nanameSpeed;
					this.Player.Y += nanameSpeed;
				}
				else if (d3 && d5)
				{
					this.Player.X -= nanameSpeed;
					this.Player.Y -= nanameSpeed;
				}
				else if (d4 && d5)
				{
					this.Player.X += nanameSpeed;
					this.Player.Y -= nanameSpeed;
				}
				else if (d2)
				{
					this.Player.Y += speed;
				}
				else if (d3)
				{
					this.Player.X -= speed;
				}
				else if (d4)
				{
					this.Player.X += speed;
				}
				else if (d5)
				{
					this.Player.Y -= speed;
				}
				DDUtils.ToRange(ref this.Player.X, 0.0, 512.0);
				DDUtils.ToRange(ref this.Player.Y, 0.0, 512.0);
				if (d3)
				{
					DDUtils.Minim(ref this.Player.XMoveFrame, 0);
					this.Player.XMoveFrame--;
				}
				else if (d4)
				{
					DDUtils.Maxim(ref this.Player.XMoveFrame, 0);
					this.Player.XMoveFrame++;
				}
				else
				{
					this.Player.XMoveFrame = 0;
				}
				if (d5)
				{
					DDUtils.Minim(ref this.Player.YMoveFrame, 0);
					this.Player.YMoveFrame--;
				}
				else if (d2)
				{
					DDUtils.Maxim(ref this.Player.YMoveFrame, 0);
					this.Player.YMoveFrame++;
				}
				else
				{
					this.Player.YMoveFrame = 0;
				}
				DDUtils.Approach(ref this.Player.XMoveRate, (double)DDUtils.Sign((double)this.Player.XMoveFrame), 0.95);
				DDUtils.Approach(ref this.Player.YMoveRate, (double)DDUtils.Sign((double)this.Player.YMoveFrame), 0.95);
				goto IL_712;
			}
			IL_EF0:
			DDUtils.Maxim(ref Ground.I.HiScore, this.Score);
			this.Status.Score = this.Score;
			this.Status.PlayerPower = this.Player.Power;
			this.Status.PlayerZanki = this.Zanki;
			this.Status.PlayerZanBomb = this.ZanBomb;
			DDMain.KeepMainScreen();
			DDMusicUtils.Fade(30, 0.0);
			DDCurtain.SetCurtain(60, -1.0);
			foreach (DDScene ddscene in DDSceneUtils.Create(120))
			{
				DDDraw.DrawRect(DDGround.KeptMainScreen.ToPicture(), 0.0, 0.0, 960.0, 540.0);
				DDEngine.EachFrame();
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000420C File Offset: 0x0000240C
		private IEnumerable<bool> E_ゴミ回収()
		{
			for (;;)
			{
				foreach (Enemy enemy in this.Enemies.Iterate())
				{
					if (this.IsProbablyEvacuated(enemy.X, enemy.Y))
					{
						enemy.HP = -1;
					}
					yield return true;
				}
				IEnumerator<Enemy> enumerator = null;
				foreach (Shot shot in this.Shots.Iterate())
				{
					if (this.IsProbablyEvacuated(shot.X, shot.Y))
					{
						shot.Vanished = true;
					}
					yield return true;
				}
				IEnumerator<Shot> enumerator2 = null;
				yield return true;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000421C File Offset: 0x0000241C
		private void Pause()
		{
			DDMain.KeepMainScreen();
			DDSimpleMenu simpleMenu = new DDSimpleMenu
			{
				Color = new I3Color?(new I3Color(255, 255, 255)),
				BorderColor = new I3Color?(new I3Color(0, 64, 128)),
				WallPicture = DDGround.KeptMainScreen.ToPicture(),
				WallCurtain = -0.5
			};
			int selectIndex = 0;
			switch (simpleMenu.Perform("ポーズメニュー(仮)", new string[]
			{
				"最初からやり直す",
				"タイトルに戻る",
				"ゲームに戻る"
			}, selectIndex, true, true))
			{
			case 0:
				this.Pause_RestartGame = true;
				break;
			case 1:
				this.Pause_ReturnToTitleMenu = true;
				break;
			case 2:
				break;
			default:
				throw null;
			}
			DDEngine.FreezeInput(1);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000042E8 File Offset: 0x000024E8
		private void DeadPlayerMoment()
		{
			DDMain.KeepMainScreen();
			foreach (DDScene scene in DDSceneUtils.Create(30))
			{
				DDDraw.DrawRect(DDGround.KeptMainScreen.ToPicture(), 0.0, 0.0, 960.0, 540.0);
				DDDraw.SetAlpha(0.2 + 0.2 * scene.Rate);
				DDDraw.SetBright(1.0, 0.0, 0.0);
				DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0.0, 0.0, 960.0, 540.0));
				DDDraw.Reset();
				DDEngine.EachFrame();
			}
			DDEngine.FreezeInput(1);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000043F8 File Offset: 0x000025F8
		private void AH_EnemyCrashed(Enemy enemy, Shot shot, DDCrash shotCrash)
		{
			Shot.Kind_e kind = shot.Kind;
			if (kind != Shot.Kind_e.NORMAL)
			{
				if (kind != Shot.Kind_e.BOMB)
				{
					throw null;
				}
				if (enemy.IsBoss())
				{
					DDGround.EL.Add(SCommon.Supplier<bool>(Effects.ボス回復(224.0 + enemy.X, 14.0 + enemy.Y)));
					enemy.HP -= enemy.InitialHP;
					enemy.HP = (int)((double)enemy.HP * 0.99);
					enemy.HP += enemy.InitialHP;
					return;
				}
			}
			else if (!shot.Vanished)
			{
				this.PlayerEffects.Add(SCommon.Supplier<bool>(Effects.ShotDead(shot.X, shot.Y, shotCrash.R)));
				shot.Vanished = true;
			}
			enemy.HP -= shot.AttackPoint;
			if (1 <= enemy.HP)
			{
				enemy.Damaged();
				return;
			}
			enemy.HP = -1;
			enemy.Killed();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004504 File Offset: 0x00002704
		private void AH_PlayerCrashed(Enemy enemy)
		{
			Enemy.Kind_e kind = enemy.Kind;
			if (kind - Enemy.Kind_e.ENEMY <= 1)
			{
				this.AH_PlayerCrashedFlag = true;
				return;
			}
			if (kind != Enemy.Kind_e.ITEM)
			{
				throw null;
			}
			enemy.HP = -1;
			enemy.Killed();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000453C File Offset: 0x0000273C
		private void 当たり判定()
		{
			this.AH_Grazed = false;
			this.AH_PlayerCrashedFlag = false;
			if (this.掛け合い中)
			{
				return;
			}
			for (int ecIndex = 0; ecIndex < this.EnemyCrashes.Count; ecIndex++)
			{
				DDCrash ec = this.EnemyCrashes[ecIndex];
				if (ec.OwnerEnemy.HP > 0 && 1 > ec.OwnerEnemy.TransFrame)
				{
					for (int scIndex = 0; scIndex < this.ShotCrashes.Count; scIndex++)
					{
						DDCrash sc = this.ShotCrashes[scIndex];
						if (sc.OwnerShot.Kind == Shot.Kind_e.NORMAL && !sc.OwnerShot.Vanished && DDCrashUtils.IsCrashed(ec, sc))
						{
							this.AH_EnemyCrashed(ec.OwnerEnemy, sc.OwnerShot, sc);
							if (ec.OwnerEnemy.HP == -1)
							{
								break;
							}
						}
					}
				}
			}
			for (int scIndex2 = 0; scIndex2 < this.ShotCrashes.Count; scIndex2++)
			{
				DDCrash sc2 = this.ShotCrashes[scIndex2];
				if (sc2.OwnerShot.Kind == Shot.Kind_e.BOMB)
				{
					for (int ecIndex2 = 0; ecIndex2 < this.EnemyCrashes.Count; ecIndex2++)
					{
						DDCrash ec2 = this.EnemyCrashes[ecIndex2];
						if (ec2.OwnerEnemy.HP != -1 && ec2.OwnerEnemy.Kind != Enemy.Kind_e.ITEM && !DDUtils.IsOut(new D2Point(ec2.OwnerEnemy.X, ec2.OwnerEnemy.Y), new D4Rect(0.0, 0.0, 512.0, 512.0), 20.0) && DDCrashUtils.IsCrashed(sc2, ec2))
						{
							this.AH_EnemyCrashed(ec2.OwnerEnemy, sc2.OwnerShot, sc2);
						}
					}
				}
			}
			for (int ecIndex3 = 0; ecIndex3 < this.EnemyCrashes.Count; ecIndex3++)
			{
				DDCrash ec3 = this.EnemyCrashes[ecIndex3];
				if (ec3.OwnerEnemy.HP != -1)
				{
					if (!this.AH_Grazed && ec3.OwnerEnemy.Kind != Enemy.Kind_e.ITEM)
					{
						for (int gcIndex = 0; gcIndex < this.GrazeCrashes.Count; gcIndex++)
						{
							DDCrash gc = this.GrazeCrashes[gcIndex];
							if (DDCrashUtils.IsCrashed(ec3, gc))
							{
								this.AH_Grazed = true;
								break;
							}
						}
					}
					for (int pcIndex = 0; pcIndex < this.PlayerCrashes.Count; pcIndex++)
					{
						DDCrash pc = this.PlayerCrashes[pcIndex];
						if (DDCrashUtils.IsCrashed(ec3, pc))
						{
							this.AH_PlayerCrashed(ec3.OwnerEnemy);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000047F0 File Offset: 0x000029F0
		private void Draw当たり判定()
		{
			DDDraw.SetAlpha(0.8);
			DDDraw.SetBright(0.0, 0.0, 0.0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, new D4Rect(0.0, 0.0, 512.0, 512.0));
			DDDraw.Reset();
			DDCrashView.Draw(this.GrazeCrashes.Iterate(), new I3Color(0, 128, 0), 0.2);
			DDCrashView.Draw(this.PlayerCrashes.Iterate(), new I3Color(255, 0, 0), 0.2);
			DDCrashView.Draw(this.EnemyCrashes.Iterate(), new I3Color(255, 255, 255), 0.2);
			DDCrashView.Draw(this.ShotCrashes.Iterate(), new I3Color(0, 255, 255), 0.2);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004908 File Offset: 0x00002B08
		private void DrawStatus()
		{
			DDPrint.SetPrint(760, 30, 20);
			DDPrint.SetBorder(new I3Color(100, 0, 150), 1);
			DDPrint.PrintLine("HiSCORE");
			DDPrint.PrintLine(string.Format("{0,20}", Ground.I.HiScore));
			DDPrint.SetBorder(new I3Color(110, 0, 140), 1);
			DDPrint.PrintLine("  SCORE");
			DDPrint.PrintLine(string.Format("{0,20}", this.Score));
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.SetBorder(new I3Color(120, 0, 130), 1);
			DDPrint.PrintLine(string.Format(" PLAYER  {0}", string.Join("", Enumerable.Repeat<string>("★", this.Zanki))));
			DDPrint.PrintLine("");
			DDPrint.SetBorder(new I3Color(130, 0, 120), 1);
			DDPrint.PrintLine(string.Format("   BOMB  {0}", string.Join("", Enumerable.Repeat<string>("＠", this.ZanBomb))));
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.PrintLine("");
			DDPrint.SetBorder(new I3Color(150, 150, 100), 1);
			DDPrint.PrintLine("  POWER");
			DDPrint.Reset();
			this.DrawStatus_Power();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004A80 File Offset: 0x00002C80
		private void DrawStatus_Power()
		{
			int power = this.Player.Power;
			string.Format("{0}.{1:D2}", power / 100, power % 100);
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
				throw null;
			}
			int x = 850;
			int y = 260;
			this.DrawDigit(x, y, 32, 64, digColor, this.Player.Level);
			x += 34;
			y += 32;
			this.DrawDigit(x, y, 16, 32, digColor, 12);
			x += 18;
			this.DrawDigit(x, y, 16, 32, digColor, this.Player.Power / 10 % 10);
			x += 18;
			this.DrawDigit(x, y, 16, 32, digColor, this.Player.Power % 10);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004B78 File Offset: 0x00002D78
		private void DrawDigit(int l, int t, int w, int h, int digColor, int digIndex)
		{
			DDPicture[][] array = new DDPicture[][]
			{
				Ground.I.Picture2.D_DIGITS_W_00,
				Ground.I.Picture2.D_DIGITS_DDY_00,
				Ground.I.Picture2.D_DIGITS_DY_00,
				Ground.I.Picture2.D_DIGITS_Y_00
			};
			DDDraw.SetMosaic();
			DDDraw.DrawRect(array[digColor][digIndex], new D4Rect((double)l, (double)t, (double)w, (double)h));
			DDDraw.Reset();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004BF6 File Offset: 0x00002DF6
		private bool IsProbablyEvacuated(double x, double y)
		{
			return x < -2880.0 || 3840.0 < x || y < -1620.0 || 2160.0 < y;
		}

		// Token: 0x04000082 RID: 130
		public int Zanki = 3;

		// Token: 0x04000083 RID: 131
		public int ZanBomb = 3;

		// Token: 0x04000084 RID: 132
		public long Score;

		// Token: 0x04000085 RID: 133
		public Script Script = new Script_Dummy0001();

		// Token: 0x04000086 RID: 134
		public Player Player = new Player();

		// Token: 0x04000087 RID: 135
		public GameStatus Status = new GameStatus();

		// Token: 0x04000088 RID: 136
		public static Game I;

		// Token: 0x04000089 RID: 137
		public DDSubScreen Field;

		// Token: 0x0400008A RID: 138
		public DDSubScreen Field_Last;

		// Token: 0x0400008B RID: 139
		public Game.InputInfo Input;

		// Token: 0x0400008C RID: 140
		public Game.InputInfo LastInput;

		// Token: 0x0400008D RID: 141
		public int Frame;

		// Token: 0x0400008E RID: 142
		public SurfaceManager SurfaceManager = new SurfaceManager();

		// Token: 0x0400008F RID: 143
		public bool 掛け合い中;

		// Token: 0x04000090 RID: 144
		public bool BossBattleStarted;

		// Token: 0x04000091 RID: 145
		public bool BossKilled;

		// Token: 0x04000092 RID: 146
		public double BackgroundSlideRate = 0.5;

		// Token: 0x04000093 RID: 147
		public bool PlayerWasDead;

		// Token: 0x04000094 RID: 148
		private bool Pause_RestartGame;

		// Token: 0x04000095 RID: 149
		private bool Pause_ReturnToTitleMenu;

		// Token: 0x04000096 RID: 150
		public DDList<Enemy> Enemies = new DDList<Enemy>();

		// Token: 0x04000097 RID: 151
		public DDList<Shot> Shots = new DDList<Shot>();

		// Token: 0x04000098 RID: 152
		public DDList<Wall> Walls = new DDList<Wall>();

		// Token: 0x04000099 RID: 153
		public DDTaskList EnemyEffects = new DDTaskList();

		// Token: 0x0400009A RID: 154
		public DDTaskList PlayerEffects = new DDTaskList();

		// Token: 0x0400009B RID: 155
		public DDTaskList EL_AfterDrawWalls = new DDTaskList();

		// Token: 0x0400009C RID: 156
		public DDTaskList EL_AfterDrawShots = new DDTaskList();

		// Token: 0x0400009D RID: 157
		public DDTaskList EL_AfterDrawPlayer = new DDTaskList();

		// Token: 0x0400009E RID: 158
		public DDList<DDCrash> PlayerCrashes = new DDList<DDCrash>();

		// Token: 0x0400009F RID: 159
		public DDList<DDCrash> GrazeCrashes = new DDList<DDCrash>();

		// Token: 0x040000A0 RID: 160
		public DDList<DDCrash> EnemyCrashes = new DDList<DDCrash>();

		// Token: 0x040000A1 RID: 161
		public DDList<DDCrash> ShotCrashes = new DDList<DDCrash>();

		// Token: 0x040000A2 RID: 162
		private bool AH_Grazed;

		// Token: 0x040000A3 RID: 163
		private bool AH_PlayerCrashedFlag;

		// Token: 0x020000BC RID: 188
		public struct InputInfo
		{
			// Token: 0x040002BA RID: 698
			public bool Dir2;

			// Token: 0x040002BB RID: 699
			public bool Dir4;

			// Token: 0x040002BC RID: 700
			public bool Dir6;

			// Token: 0x040002BD RID: 701
			public bool Dir8;

			// Token: 0x040002BE RID: 702
			public bool Slow;

			// Token: 0x040002BF RID: 703
			public bool Shot;

			// Token: 0x040002C0 RID: 704
			public bool Bomb;
		}
	}
}
