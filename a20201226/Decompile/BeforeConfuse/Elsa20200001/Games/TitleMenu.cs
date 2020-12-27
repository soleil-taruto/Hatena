using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games
{
	// Token: 0x0200001B RID: 27
	public class TitleMenu : IDisposable
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00006003 File Offset: 0x00004203
		public TitleMenu()
		{
			TitleMenu.I = this;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000601C File Offset: 0x0000421C
		public void Dispose()
		{
			TitleMenu.I = null;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00006024 File Offset: 0x00004224
		public void Perform()
		{
			DDCurtain.SetCurtain(30, 0.0);
			DDEngine.FreezeInput(1);
			Ground.I.Music.MUS_TITLE.Play(false, false, 1.0, 30);
			string[] items = new string[]
			{
				"ゲームスタート",
				"ステージ選択",
				"設定",
				"終了"
			};
			int selectIndex = 0;
			this.SimpleMenu = new DDSimpleMenu();
			this.SimpleMenu.BorderColor = new I3Color?(new I3Color(64, 0, 64));
			this.SimpleMenu.WallColor = new I3Color?(new I3Color(0, 70, 140));
			this.SimpleMenu.WallDrawer = new Action(this.DrawWall);
			for (;;)
			{
				selectIndex = this.SimpleMenu.Perform("東方っぽいシューティングの試作版", items, selectIndex, false, false);
				switch (selectIndex)
				{
				case 0:
					this.SelectChara(0);
					continue;
				case 1:
					this.SelectStage();
					continue;
				case 2:
					this.Setting();
					continue;
				case 3:
					goto IL_105;
				}
				break;
			}
			throw new DDError();
			IL_105:
			DDMusicUtils.Fade(30, 0.0);
			DDCurtain.SetCurtain(30, -1.0);
			foreach (DDScene ddscene in DDSceneUtils.Create(40))
			{
				this.SimpleMenu.DrawWall();
				DDEngine.EachFrame();
			}
			DDEngine.FreezeInput(1);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000061A8 File Offset: 0x000043A8
		private void DrawWall()
		{
			if (DDEngine.ProcFrame % 130 == 0)
			{
				this.DWTasks.Add(SCommon.Supplier<bool>(this.E_DWTask()));
			}
			this.DWTasks.ExecuteAllTask();
			DDDraw.SetAlpha(0.5);
			DDDraw.SetBright(0.0, 0.0, 0.0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 0.0, 0.0, 480.0, 540.0);
			DDDraw.Reset();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00006249 File Offset: 0x00004449
		private IEnumerable<bool> E_DWTask()
		{
			for (int index = 0; index <= 10; index++)
			{
				DDCCResource.GetPicture("e20200003_dat\\dairi\\67504816_p" + index.ToString() + ".png").GetHandle();
			}
			DDPicture picture = DDCCResource.GetPicture("e20200003_dat\\dairi\\67504816_p" + DDUtils.Random.GetRange(0, 10).ToString() + ".png");
			double x = 1260.0;
			double y = 340.0;
			for (;;)
			{
				x -= 3.0;
				if (x < -300.0)
				{
					break;
				}
				DDDraw.DrawCenter(picture, x, y);
				yield return true;
			}
			yield break;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00006254 File Offset: 0x00004454
		private void SelectStage()
		{
			int selectIndex = 0;
			for (;;)
			{
				selectIndex = this.SimpleMenu.Perform("ステージ選択", (from v in GameMaster.Stages
				select v.Name).Concat(new string[]
				{
					"戻る"
				}).ToArray<string>(), selectIndex, false, false);
				if (GameMaster.Stages.Length <= selectIndex)
				{
					break;
				}
				this.SelectChara(selectIndex);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000062CC File Offset: 0x000044CC
		private void SelectChara(int startStageIndex)
		{
			Action<Player.PlayerWho_e> a_gameStart = delegate(Player.PlayerWho_e plWho)
			{
				this.LeaveTitleMenu();
				GameMaster.Start(startStageIndex, plWho);
				this.ReturnTitleMenu();
			};
			switch (this.SimpleMenu.Perform("キャラ選択", new string[]
			{
				"小悪魔",
				"メディスン",
				"戻る"
			}, 0, false, false))
			{
			case 0:
				a_gameStart(Player.PlayerWho_e.小悪魔);
				return;
			case 1:
				a_gameStart(Player.PlayerWho_e.メディスン);
				return;
			case 2:
				return;
			default:
				throw null;
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00006350 File Offset: 0x00004550
		private void Setting()
		{
			DDCurtain.SetCurtain(30, 0.0);
			DDEngine.FreezeInput(1);
			string[] items = new string[]
			{
				"パッドのボタン設定",
				"ウィンドウサイズ変更",
				"ＢＧＭ音量",
				"ＳＥ音量",
				"戻る"
			};
			int selectIndex = 0;
			for (;;)
			{
				selectIndex = this.SimpleMenu.Perform("設定", items, selectIndex, false, false);
				switch (selectIndex)
				{
				case 0:
					this.SimpleMenu.PadConfig();
					continue;
				case 1:
					this.SimpleMenu.WindowSizeConfig();
					continue;
				case 2:
					this.SimpleMenu.VolumeConfig("ＢＧＭ音量", DDGround.MusicVolume, 0, 100, 1, 10, delegate(double volume)
					{
						DDGround.MusicVolume = volume;
						DDMusicUtils.UpdateVolume();
					}, delegate
					{
					});
					continue;
				case 3:
					this.SimpleMenu.VolumeConfig("ＳＥ音量", DDGround.SEVolume, 0, 100, 1, 10, delegate(double volume)
					{
						DDGround.SEVolume = volume;
						DDSEUtils.UpdateVolume();
					}, delegate
					{
						Ground.I.SE.SE_ITEMGOT.Play(true);
					});
					continue;
				case 4:
					goto IL_159;
				}
				break;
			}
			throw new DDError();
			IL_159:
			DDEngine.FreezeInput(1);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000064BC File Offset: 0x000046BC
		private void LeaveTitleMenu()
		{
			DDMusicUtils.Fade(30, 0.0);
			DDCurtain.SetCurtain(30, -1.0);
			foreach (DDScene ddscene in DDSceneUtils.Create(40))
			{
				this.SimpleMenu.DrawWall();
				DDEngine.EachFrame();
			}
			GC.Collect();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00006538 File Offset: 0x00004738
		private void ReturnTitleMenu()
		{
			Ground.I.Music.MUS_TITLE.Play(false, false, 1.0, 30);
			GC.Collect();
		}

		// Token: 0x040000C2 RID: 194
		public static TitleMenu I;

		// Token: 0x040000C3 RID: 195
		private DDSimpleMenu SimpleMenu;

		// Token: 0x040000C4 RID: 196
		private DDTaskList DWTasks = new DDTaskList();
	}
}
