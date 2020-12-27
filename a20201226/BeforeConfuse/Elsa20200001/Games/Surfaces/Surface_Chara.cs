using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.GameCommons;
using Charlotte.Commons;

namespace Charlotte.Games.Surfaces
{
	// サーフェス - キャラクタ(登場人物)

	public class Surface_Chara : Surface
	{
		private double A = 0.0;
		private double Bright = 0.5;
		private D2Point ActivePos = new D2Point(DDConsts.Screen_W / 2.0, DDConsts.Screen_H / 2.0);
		private D2Point UnactivePos = new D2Point(DDConsts.Screen_W / 2.0, DDConsts.Screen_H / 2.0);
		private bool PictureXReversed = false;
		private DDPicture Picture = DDGround.GeneralResource.Dummy;
		private bool Active = false;
		private bool Ended = false;

		private class PictureInfo
		{
			public string Name;
			public DDPicture Picture;

			public PictureInfo(string name, DDPicture picture)
			{
				this.Name = name;
				this.Picture = picture;
			}
		}

		#region PictureList

		private PictureInfo[] PictureList = new PictureInfo[]
		{
			new PictureInfo("小悪魔_普通", Ground.I.Picture.立ち絵_小悪魔_01),
			new PictureInfo("小悪魔_ジト目", Ground.I.Picture.立ち絵_小悪魔_02),
			new PictureInfo("鍵山雛_普通", Ground.I.Picture.立ち絵_鍵山雛_01),
			new PictureInfo("鍵山雛_ジト目", Ground.I.Picture.立ち絵_鍵山雛_02),
			new PictureInfo("メディスン_普通", Ground.I.Picture.立ち絵_メディスン_01),
			new PictureInfo("メディスン_ジト目", Ground.I.Picture.立ち絵_メディスン_02),
			new PictureInfo("ルーミア_普通", Ground.I.Picture.立ち絵_ルーミア_01),
			new PictureInfo("ルーミア_ジト目", Ground.I.Picture.立ち絵_ルーミア_02),
		};

		#endregion

		public override IEnumerable<bool> E_Draw()
		{
			for (; ; )
			{
				DDUtils.Approach(ref this.A, this.Ended ? 0.0 : 1.0, 0.91);
				DDUtils.Approach(ref this.Bright, this.Active ? 1.0 : 0.5, 0.8);
				DDUtils.Approach(ref this.X, this.Active ? this.ActivePos.X : this.UnactivePos.X, 0.85);
				DDUtils.Approach(ref this.Y, this.Active ? this.ActivePos.Y : this.UnactivePos.Y, 0.85);

				DDDraw.SetAlpha(this.A);
				DDDraw.SetBright(this.Bright, this.Bright, this.Bright);
				DDDraw.DrawBegin(this.Picture, this.X, this.Y);
				DDDraw.DrawZoom_X(this.PictureXReversed ? -1.0 : 1.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();

				yield return !this.Ended || 0.003 < this.A;
			}
		}

		public override void Invoke_02(string command, string[] arguments)
		{
			int c = 0;

			if (command == "Y") // Y-位置_調整
			{
				double ya = double.Parse(arguments[0]);

				this.ActivePos.Y += ya;
				this.UnactivePos.Y += ya;
			}
			else if (command == "位置")
			{
				string position = arguments[c++];

				if (position == "左")
				{
					this.ActivePos = new D2Point(220, 330);
					this.UnactivePos = new D2Point(200, 350);
				}
				else if (position == "右")
				{
					this.ActivePos = new D2Point(DDConsts.Screen_W - 220, 330);
					this.UnactivePos = new D2Point(DDConsts.Screen_W - 200, 350);
				}
				else
				{
					throw new DDError();
				}

				this.X = this.UnactivePos.X;
				this.Y = this.UnactivePos.Y;
			}
			else if (command == "画像_左右反転")
			{
				this.PictureXReversed = true;
			}
			else if (command == "画像")
			{
				string name = arguments[c++];

				this.Picture = this.PictureList.First(v => v.Name == name).Picture;
			}
			else if (command == "アクティブ")
			{
				foreach (Surface_Chara surface in Game.I.SurfaceManager.GetAllキャラクタ())
					surface.Active = false;

				this.Active = true;
			}
			else if (command == "終了")
			{
				this.Ended = true;
			}
			else
			{
				base.Invoke_02(command, arguments);
			}
		}
	}
}
