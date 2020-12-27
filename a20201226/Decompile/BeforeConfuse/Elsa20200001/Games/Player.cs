using System;
using Charlotte.Commons;
using Charlotte.GameCommons;
using Charlotte.GameCommons.Options;
using Charlotte.Games.Shots;

namespace Charlotte.Games
{
	// Token: 0x02000018 RID: 24
	public class Player
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00005284 File Offset: 0x00003484
		public int Level
		{
			get
			{
				return this.Power / 100;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00005290 File Offset: 0x00003490
		public void Reset(bool 死亡した)
		{
			this.X = 256.0;
			this.Y = 409.6;
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
				this.Power -= 100;
				this.Power = Math.Max(this.Power, 0);
			}
			else
			{
				this.Power = 0;
			}
			this.BornFrame = 1;
			this.DeadFrame = 0;
			this.BombFrame = 0;
			this.BornFollowX = 256.0;
			this.BornFollowY = 614.4;
			this.AbsorbedWeapon = -1;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00005394 File Offset: 0x00003594
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
			Player.PlayerWho_e playerWho = this.PlayerWho;
			if (playerWho == Player.PlayerWho_e.メディスン)
			{
				int pic_y = Game.I.Frame / 7 % 3;
				int pic_x;
				if (this.XMoveFrame < 0)
				{
					pic_x = 2;
				}
				else if (0 < this.XMoveFrame)
				{
					pic_x = 1;
				}
				else
				{
					pic_x = 0;
				}
				DDDraw.SetAlpha(dr_a);
				DDDraw.DrawCenter(Ground.I.Picture2.メディスン[pic_x, pic_y], (double)dr_x, (double)dr_y);
				DDDraw.Reset();
				return;
			}
			if (playerWho != Player.PlayerWho_e.小悪魔)
			{
				throw null;
			}
			if (-20 < this.SlowFrame)
			{
				double r = this.SlowRate;
				DDDraw.SetBlendAdd(dr_a * r * 0.7);
				DDDraw.DrawBegin(Ground.I.Picture2.D_SLOWBACK, (double)dr_x, (double)dr_y);
				DDDraw.DrawRotate((double)Game.I.Frame * 0.01);
				DDDraw.DrawZoom(1.0 + (1.0 - r) * 4.0);
				DDDraw.DrawEnd();
				DDDraw.Reset();
			}
			if (-20 < this.ShotFrame)
			{
				double r2 = this.ShotRate;
				if (0 < this.SlowFrame)
				{
					int bookKoma = Game.I.Frame / 3;
					bookKoma %= 3;
					DDDraw.SetAlpha(dr_a * r2 * 0.3);
					DDDraw.DrawBegin(Ground.I.Picture2.D_BOOKBACK, (double)dr_x, (double)dr_y - 24.0 * r2);
					DDDraw.DrawZoom(1.5 + 1.0 * r2);
					DDDraw.DrawRotate((double)Game.I.Frame * 0.02);
					DDDraw.DrawZoom_Y(0.5);
					DDDraw.DrawEnd();
					DDDraw.SetAlpha(dr_a * r2 * 1.0);
					for (int c = -1; c <= 1; c += 2)
					{
						DDDraw.DrawCenter(Ground.I.Picture2.D_BOOK_00[bookKoma], (double)dr_x + 18.0 * (double)c * r2, (double)dr_y - 14.0 * r2);
					}
					DDDraw.Reset();
				}
				else if (1 <= this.Level)
				{
					int bookKoma2 = Game.I.Frame / 5;
					bookKoma2 %= 3;
					DDDraw.SetAlpha(dr_a * r2 * 0.3);
					for (int c2 = -1; c2 <= 1; c2 += 2)
					{
						DDDraw.DrawBegin(Ground.I.Picture2.D_BOOKBACK, (double)dr_x + 25.0 * (double)c2 * r2, (double)dr_y - 12.0 * r2);
						DDDraw.DrawZoom(0.5 + 1.0 * r2);
						DDDraw.DrawRotate((double)Game.I.Frame * 0.02);
						DDDraw.DrawZoom_Y(0.5);
						DDDraw.DrawEnd();
					}
					DDDraw.SetAlpha(dr_a * r2 * 1.0);
					for (int c3 = -1; c3 <= 1; c3 += 2)
					{
						DDDraw.DrawCenter(Ground.I.Picture2.D_BOOK_00[bookKoma2], (double)dr_x + 25.0 * (double)c3 * r2, (double)dr_y);
					}
					DDDraw.Reset();
				}
			}
			int koma = Game.I.Frame / 7;
			if (this.XMoveFrame != 0)
			{
				koma %= 3;
				koma += ((this.XMoveFrame < 0) ? 4 : 8);
			}
			else
			{
				koma %= 4;
			}
			DDDraw.SetAlpha(dr_a);
			DDDraw.DrawCenter(Ground.I.Picture2.D_KOAKUMA_00[koma], (double)dr_x, (double)dr_y);
			DDDraw.Reset();
			if (-20 < this.SlowFrame)
			{
				DDDraw.SetAlpha(dr_a * this.SlowRate * 2.0);
				DDDraw.DrawBegin(Ground.I.Picture2.D_ATARIPOINT, (double)dr_x, (double)dr_y);
				DDDraw.DrawRotate((double)Game.I.Frame * 0.02);
				DDDraw.DrawEnd();
				DDDraw.Reset();
				return;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00005804 File Offset: 0x00003A04
		public void Shot()
		{
			double pl_x;
			double pl_y;
			if (1 <= this.BornFrame)
			{
				pl_x = this.BornFollowX;
				pl_y = this.BornFollowY;
				if (DDUtils.IsOut(new D2Point(pl_x, pl_y), new D4Rect(0.0, 0.0, 512.0, 512.0), 0.0))
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
				goto IL_21B;
			case 1:
				if (Game.I.Frame % 4 == 0)
				{
					double ZURE = 20.0;
					Game.I.Shots.Add(new Shot_Strong(pl_x + ZURE, pl_y - ZURE, 0.78539816339744828));
					Game.I.Shots.Add(new Shot_Strong(pl_x + ZURE, pl_y, 1.5707963267948966));
					Game.I.Shots.Add(new Shot_Strong(pl_x - ZURE, pl_y, 4.71238898038469));
					Game.I.Shots.Add(new Shot_Strong(pl_x - ZURE, pl_y - ZURE, 5.497787143782138));
					goto IL_21B;
				}
				goto IL_21B;
			case 2:
				if (Game.I.Frame % 2 == 0)
				{
					for (int c = -1; c <= 1; c += 2)
					{
						Game.I.Shots.Add(new Shot_WaveBehind(pl_x + 20.0 * (double)c, pl_y - 5.0, 0.3 * (double)c, 0.3 * (double)c, 0.95 + SCommon.ToRange((double)this.SlowFrame * 0.0005, -0.02, 0.02)));
					}
					goto IL_21B;
				}
				goto IL_21B;
			case 3:
				if (Game.I.Frame % 4 == 0)
				{
					for (int c2 = -1; c2 <= 1; c2 += 2)
					{
						Game.I.Shots.Add(new Shot_Homing(pl_x, pl_y - 10.0));
					}
					goto IL_21B;
				}
				goto IL_21B;
			}
			throw null;
			IL_21B:
			Player.PlayerWho_e playerWho = this.PlayerWho;
			if (playerWho != Player.PlayerWho_e.メディスン)
			{
				if (playerWho != Player.PlayerWho_e.小悪魔)
				{
					throw null;
				}
				if (1 <= this.SlowFrame)
				{
					if (Game.I.Frame % 4 == 0)
					{
						Ground.I.SE.SE_PLAYERSHOT.Play(true);
						Game.I.Shots.Add(new Shot_Laser(pl_x, pl_y - 10.0, this.Level));
						return;
					}
				}
				else
				{
					if (Game.I.Frame % 3 == 0)
					{
						Ground.I.SE.SE_PLAYERSHOT.Play(true);
					}
					switch (this.Level)
					{
					case 0:
					case 1:
						if (Game.I.Frame % 3 == 0)
						{
							Game.I.Shots.Add(new Shot_Normal(pl_x, pl_y - 5.0, 0.0));
						}
						break;
					case 2:
						if (Game.I.Frame % 3 == 0)
						{
							for (int c3 = -1; c3 <= 1; c3 += 2)
							{
								Game.I.Shots.Add(new Shot_Normal(pl_x + 8.0 * (double)c3, pl_y - 5.0, 0.0));
							}
						}
						break;
					case 3:
						if (Game.I.Frame % 3 == 0)
						{
							Game.I.Shots.Add(new Shot_Strong(pl_x, pl_y - 5.0, 0.0));
							for (int c4 = -1; c4 <= 1; c4 += 2)
							{
								Game.I.Shots.Add(new Shot_Normal(pl_x + 16.0 * (double)c4, pl_y - 5.0, 0.1 * (double)c4));
							}
						}
						break;
					case 4:
						if (Game.I.Frame % 3 == 0)
						{
							for (int c5 = -1; c5 <= 1; c5 += 2)
							{
								Game.I.Shots.Add(new Shot_Normal(pl_x + 8.0 * (double)c5, pl_y - 5.0, 0.0));
							}
							for (int c6 = -1; c6 <= 1; c6 += 2)
							{
								Game.I.Shots.Add(new Shot_Normal(pl_x + 16.0 * (double)c6, pl_y - 5.0, 0.1 * (double)c6));
							}
						}
						break;
					case 5:
						if (Game.I.Frame % 3 == 0)
						{
							Game.I.Shots.Add(new Shot_Strong(pl_x, pl_y - 5.0, 0.0));
							for (int c7 = -1; c7 <= 1; c7 += 2)
							{
								Game.I.Shots.Add(new Shot_Strong(pl_x + 16.0 * (double)c7, pl_y - 5.0, 0.05 * (double)c7));
								Game.I.Shots.Add(new Shot_Normal(pl_x + 24.0 * (double)c7, pl_y - 5.0, 0.1 * (double)c7));
							}
						}
						break;
					default:
						throw null;
					}
					int level = this.Level;
					if (level != 0)
					{
						if (level - 1 > 4)
						{
							throw null;
						}
						if (Game.I.Frame % 4 == 0)
						{
							for (int c8 = -1; c8 <= 1; c8 += 2)
							{
								Game.I.Shots.Add(new Shot_Wave(pl_x + 20.0 * (double)c8, pl_y - 5.0, 0.3 * (double)c8, (-0.021 - 0.003 * this.YMoveRate) * (double)c8));
							}
							return;
						}
					}
				}
			}
			else if (Game.I.Frame % 4 == 0)
			{
				Ground.I.SE.SE_PLAYERSHOT.Play(true);
				Game.I.Shots.Add(new Shot_MedicineLaser(pl_x, pl_y - 10.0, this.Level));
				return;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005E58 File Offset: 0x00004058
		public void Bomb()
		{
			Player.PlayerWho_e playerWho = this.PlayerWho;
			if (playerWho == Player.PlayerWho_e.メディスン)
			{
				Game.I.Shots.Add(new Shot_MedicineBomb(this.X, this.Y));
				return;
			}
			if (playerWho != Player.PlayerWho_e.小悪魔)
			{
				throw null;
			}
			Game.I.Shots.Add(new Shot_Bomb(this.X, this.Y));
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005EB8 File Offset: 0x000040B8
		public void Put当たり判定()
		{
			foreach (DDScene scene in DDSceneUtils.Create(5))
			{
				D2Point pt = DDUtils.AToBRate(new D2Point(this.X, this.Y), new D2Point(this.LastX, this.LastY), scene.Rate);
				Game.I.PlayerCrashes.Add(DDCrashUtils.Point(pt));
				Game.I.GrazeCrashes.Add(DDCrashUtils.Circle(pt, 10.0));
			}
		}

		// Token: 0x040000AC RID: 172
		public Player.PlayerWho_e PlayerWho;

		// Token: 0x040000AD RID: 173
		public double X;

		// Token: 0x040000AE RID: 174
		public double Y;

		// Token: 0x040000AF RID: 175
		public double LastX;

		// Token: 0x040000B0 RID: 176
		public double LastY;

		// Token: 0x040000B1 RID: 177
		public int XMoveFrame;

		// Token: 0x040000B2 RID: 178
		public int YMoveFrame;

		// Token: 0x040000B3 RID: 179
		public double XMoveRate;

		// Token: 0x040000B4 RID: 180
		public double YMoveRate;

		// Token: 0x040000B5 RID: 181
		public int SlowFrame;

		// Token: 0x040000B6 RID: 182
		public int ShotFrame;

		// Token: 0x040000B7 RID: 183
		public double SlowRate;

		// Token: 0x040000B8 RID: 184
		public double ShotRate;

		// Token: 0x040000B9 RID: 185
		public int Power;

		// Token: 0x040000BA RID: 186
		public int BornFrame;

		// Token: 0x040000BB RID: 187
		public int DeadFrame;

		// Token: 0x040000BC RID: 188
		public int BombFrame;

		// Token: 0x040000BD RID: 189
		public double BornFollowX;

		// Token: 0x040000BE RID: 190
		public double BornFollowY;

		// Token: 0x040000BF RID: 191
		public int AbsorbedWeapon = -1;

		// Token: 0x020000C1 RID: 193
		public enum PlayerWho_e
		{
			// Token: 0x040002D1 RID: 721
			メディスン,
			// Token: 0x040002D2 RID: 722
			小悪魔
		}
	}
}
