using System;
using System.Collections.Generic;
using System.Linq;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte.Games.Surfaces
{
	// Token: 0x0200002E RID: 46
	public class Surface_Chara : Surface
	{
		// Token: 0x0600008E RID: 142 RVA: 0x000077E1 File Offset: 0x000059E1
		public override IEnumerable<bool> E_Draw()
		{
			for (;;)
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
			yield break;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000077F4 File Offset: 0x000059F4
		public override void Invoke_02(string command, string[] arguments)
		{
			int c = 0;
			if (command == "Y")
			{
				double ya = double.Parse(arguments[0]);
				this.ActivePos.Y = this.ActivePos.Y + ya;
				this.UnactivePos.Y = this.UnactivePos.Y + ya;
				return;
			}
			if (command == "位置")
			{
				string position = arguments[c++];
				if (position == "左")
				{
					this.ActivePos = new D2Point(220.0, 330.0);
					this.UnactivePos = new D2Point(200.0, 350.0);
				}
				else
				{
					if (!(position == "右"))
					{
						throw new DDError();
					}
					this.ActivePos = new D2Point(740.0, 330.0);
					this.UnactivePos = new D2Point(760.0, 350.0);
				}
				this.X = this.UnactivePos.X;
				this.Y = this.UnactivePos.Y;
				return;
			}
			if (command == "画像_左右反転")
			{
				this.PictureXReversed = true;
				return;
			}
			if (command == "画像")
			{
				string name = arguments[c++];
				this.Picture = this.PictureList.First((Surface_Chara.PictureInfo v) => v.Name == name).Picture;
				return;
			}
			if (command == "アクティブ")
			{
				foreach (Surface_Chara surface_Chara in Game.I.SurfaceManager.GetAllキャラクタ())
				{
					surface_Chara.Active = false;
				}
				this.Active = true;
				return;
			}
			if (command == "終了")
			{
				this.Ended = true;
				return;
			}
			base.Invoke_02(command, arguments);
		}

		// Token: 0x040000E9 RID: 233
		private double A;

		// Token: 0x040000EA RID: 234
		private double Bright = 0.5;

		// Token: 0x040000EB RID: 235
		private D2Point ActivePos = new D2Point(480.0, 270.0);

		// Token: 0x040000EC RID: 236
		private D2Point UnactivePos = new D2Point(480.0, 270.0);

		// Token: 0x040000ED RID: 237
		private bool PictureXReversed;

		// Token: 0x040000EE RID: 238
		private DDPicture Picture = DDGround.GeneralResource.Dummy;

		// Token: 0x040000EF RID: 239
		private bool Active;

		// Token: 0x040000F0 RID: 240
		private bool Ended;

		// Token: 0x040000F1 RID: 241
		private Surface_Chara.PictureInfo[] PictureList = new Surface_Chara.PictureInfo[]
		{
			new Surface_Chara.PictureInfo("小悪魔_普通", Ground.I.Picture.立ち絵_小悪魔_01),
			new Surface_Chara.PictureInfo("小悪魔_ジト目", Ground.I.Picture.立ち絵_小悪魔_02),
			new Surface_Chara.PictureInfo("鍵山雛_普通", Ground.I.Picture.立ち絵_鍵山雛_01),
			new Surface_Chara.PictureInfo("鍵山雛_ジト目", Ground.I.Picture.立ち絵_鍵山雛_02),
			new Surface_Chara.PictureInfo("メディスン_普通", Ground.I.Picture.立ち絵_メディスン_01),
			new Surface_Chara.PictureInfo("メディスン_ジト目", Ground.I.Picture.立ち絵_メディスン_02),
			new Surface_Chara.PictureInfo("ルーミア_普通", Ground.I.Picture.立ち絵_ル\u30FCミア_01),
			new Surface_Chara.PictureInfo("ルーミア_ジト目", Ground.I.Picture.立ち絵_ル\u30FCミア_02)
		};

		// Token: 0x020000CC RID: 204
		private class PictureInfo
		{
			// Token: 0x0600041B RID: 1051 RVA: 0x00015854 File Offset: 0x00013A54
			public PictureInfo(string name, DDPicture picture)
			{
				this.Name = name;
				this.Picture = picture;
			}

			// Token: 0x04000313 RID: 787
			public string Name;

			// Token: 0x04000314 RID: 788
			public DDPicture Picture;
		}
	}
}
