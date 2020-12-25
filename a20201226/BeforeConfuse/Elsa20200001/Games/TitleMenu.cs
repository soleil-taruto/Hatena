using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.Games.Scripts;
using Charlotte.GameCommons.Options;

namespace Charlotte.Games
{
	public class TitleMenu : IDisposable
	{
		public static TitleMenu I;

		public TitleMenu()
		{
			I = this;
		}

		public void Dispose()
		{
			I = null;
		}

		private DDSimpleMenu SimpleMenu;

		public void Perform()
		{
			DDCurtain.SetCurtain();
			DDEngine.FreezeInput();

			Ground.I.Music.MUS_TITLE.Play();

			string[] items = new string[]
			{
				"ゲームスタート",
				"ステージ選択",
				"設定",
				"終了",
			};

			int selectIndex = 0;

			this.SimpleMenu = new DDSimpleMenu();

			this.SimpleMenu.BorderColor = new I3Color(64, 0, 64);
			this.SimpleMenu.WallColor = new I3Color(0, 70, 140);
			//this.SimpleMenu.WallPicture = Ground.I.Picture.P_TITLE_WALL;
			//this.SimpleMenu.WallCurtain = -0.8;
			this.SimpleMenu.WallDrawer = this.DrawWall;

			for (; ; )
			{
				selectIndex = this.SimpleMenu.Perform("東方っぽいシューティングの試作版", items, selectIndex);

				switch (selectIndex)
				{
					case 0:
						this.SelectChara(0);
						break;

					case 1:
						this.SelectStage();
						break;

					case 2:
						this.Setting();
						break;

					case 3:
						goto endMenu;

					default:
						throw new DDError();
				}
			}
		endMenu:
			DDMusicUtils.Fade();
			DDCurtain.SetCurtain(30, -1.0);

			foreach (DDScene scene in DDSceneUtils.Create(40))
			{
				this.SimpleMenu.DrawWall();
				DDEngine.EachFrame();
			}

			DDEngine.FreezeInput();
		}

		private DDTaskList DWTasks = new DDTaskList();

		private void DrawWall()
		{
			if (DDEngine.ProcFrame % 130 == 0)
				this.DWTasks.Add(SCommon.Supplier(this.E_DWTask()));

			this.DWTasks.ExecuteAllTask();

			DDDraw.SetAlpha(0.5);
			DDDraw.SetBright(0, 0, 0);
			DDDraw.DrawRect(DDGround.GeneralResource.WhiteBox, 0, 0, DDConsts.Screen_W / 2, DDConsts.Screen_H);
			DDDraw.Reset();
		}

		private IEnumerable<bool> E_DWTask()
		{
			const string PIC_PREFIX = @"e20200003_dat\dairi\67504816_p";
			const string PIC_SUFFIX = ".png";
			const int PIC_INDEX_MIN = 0;
			const int PIC_INDEX_MAX = 10;

			// Touch -- ロードする度にガクガクするので、最初のタイミングで全部触っておく
			{
				for (int index = PIC_INDEX_MIN; index <= PIC_INDEX_MAX; index++)
					DDCCResource.GetPicture(PIC_PREFIX + index + PIC_SUFFIX).GetHandle();
			}

			DDPicture picture = DDCCResource.GetPicture(PIC_PREFIX + DDUtils.Random.GetRange(PIC_INDEX_MIN, PIC_INDEX_MAX) + PIC_SUFFIX);
			double x = DDConsts.Screen_W + 300.0;
			double y = DDConsts.Screen_H - 200.0;

			for (; ; )
			{
				x -= 3.0;

				if (x < -300.0)
					break;

				DDDraw.DrawCenter(picture, x, y);

				yield return true;
			}
		}

		private void SelectStage()
		{
			int selectIndex = 0;

			for (; ; )
			{
				selectIndex = this.SimpleMenu.Perform("ステージ選択", GameMaster.Stages.Select(v => v.Name).Concat(new string[] { "戻る" }).ToArray(), selectIndex);

				if (GameMaster.Stages.Length <= selectIndex)
					break;

				this.SelectChara(selectIndex);
			}
		}

		private void SelectChara(int startStageIndex)
		{
			Action<Player.PlayerWho_e> a_gameStart = plWho =>
			{
				this.LeaveTitleMenu();
				GameMaster.Start(startStageIndex, plWho);
				this.ReturnTitleMenu();
			};

			switch (this.SimpleMenu.Perform(
				"キャラ選択",
				new string[]
				{
					"小悪魔",
					"メディスン",
					"戻る",
				},
				0
				))
			{
				case 0:
					a_gameStart(Player.PlayerWho_e.小悪魔);
					break;

				case 1:
					a_gameStart(Player.PlayerWho_e.メディスン);
					break;

				case 2:
					break;

				default:
					throw null; // never
			}
		}

		private void Setting()
		{
			DDCurtain.SetCurtain();
			DDEngine.FreezeInput();

			string[] items = new string[]
			{
				"パッドのボタン設定",
				"ウィンドウサイズ変更",
				"ＢＧＭ音量",
				"ＳＥ音量",
				"戻る",
			};

			int selectIndex = 0;

			for (; ; )
			{
				selectIndex = this.SimpleMenu.Perform("設定", items, selectIndex);

				switch (selectIndex)
				{
					case 0:
						this.SimpleMenu.PadConfig();
						break;

					case 1:
						this.SimpleMenu.WindowSizeConfig();
						break;

					case 2:
						this.SimpleMenu.VolumeConfig("ＢＧＭ音量", DDGround.MusicVolume, 0, 100, 1, 10, volume =>
						{
							DDGround.MusicVolume = volume;
							DDMusicUtils.UpdateVolume();
						},
						() => { }
						);
						break;

					case 3:
						this.SimpleMenu.VolumeConfig("ＳＥ音量", DDGround.SEVolume, 0, 100, 1, 10, volume =>
						{
							DDGround.SEVolume = volume;
							DDSEUtils.UpdateVolume();
						},
						() =>
						{
							Ground.I.SE.SE_ITEMGOT.Play();
						}
						);
						break;

					case 4:
						goto endMenu;

					default:
						throw new DDError();
				}
			}
		endMenu:
			DDEngine.FreezeInput();
		}

		private void LeaveTitleMenu()
		{
			DDMusicUtils.Fade();
			DDCurtain.SetCurtain(30, -1.0);

			foreach (DDScene scene in DDSceneUtils.Create(40))
			{
				this.SimpleMenu.DrawWall();
				DDEngine.EachFrame();
			}

			GC.Collect();
		}

		private void ReturnTitleMenu()
		{
			Ground.I.Music.MUS_TITLE.Play();

			GC.Collect();
		}
	}
}
