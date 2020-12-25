using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Commons;
using Charlotte.Games;

namespace Charlotte.GameCommons
{
	public static class DDSaveData
	{
		public static void Save()
		{
			List<byte[]> blocks = new List<byte[]>();

			// Donut3 用のセーブデータ
			{
				List<string> lines = new List<string>();

				lines.Add(ProcMain.APP_IDENT);
				lines.Add(ProcMain.APP_TITLE);

				lines.Add("" + DDGround.RealScreen_W);
				lines.Add("" + DDGround.RealScreen_H);

				lines.Add("" + DDGround.RealScreenDraw_L);
				lines.Add("" + DDGround.RealScreenDraw_T);
				lines.Add("" + DDGround.RealScreenDraw_W);
				lines.Add("" + DDGround.RealScreenDraw_H);

				lines.Add("" + SCommon.ToLong(DDGround.MusicVolume * SCommon.IMAX));
				lines.Add("" + SCommon.ToLong(DDGround.SEVolume * SCommon.IMAX));

				lines.Add("" + DDInput.DIR_2.BtnId);
				lines.Add("" + DDInput.DIR_4.BtnId);
				lines.Add("" + DDInput.DIR_6.BtnId);
				lines.Add("" + DDInput.DIR_8.BtnId);
				lines.Add("" + DDInput.A.BtnId);
				lines.Add("" + DDInput.B.BtnId);
				lines.Add("" + DDInput.C.BtnId);
				lines.Add("" + DDInput.D.BtnId);
				lines.Add("" + DDInput.E.BtnId);
				lines.Add("" + DDInput.F.BtnId);
				lines.Add("" + DDInput.L.BtnId);
				lines.Add("" + DDInput.R.BtnId);
				lines.Add("" + DDInput.PAUSE.BtnId);
				lines.Add("" + DDInput.START.BtnId);

				lines.Add("" + DDInput.DIR_2.KeyId);
				lines.Add("" + DDInput.DIR_4.KeyId);
				lines.Add("" + DDInput.DIR_6.KeyId);
				lines.Add("" + DDInput.DIR_8.KeyId);
				lines.Add("" + DDInput.A.KeyId);
				lines.Add("" + DDInput.B.KeyId);
				lines.Add("" + DDInput.C.KeyId);
				lines.Add("" + DDInput.D.KeyId);
				lines.Add("" + DDInput.E.KeyId);
				lines.Add("" + DDInput.F.KeyId);
				lines.Add("" + DDInput.L.KeyId);
				lines.Add("" + DDInput.R.KeyId);
				lines.Add("" + DDInput.PAUSE.KeyId);
				lines.Add("" + DDInput.START.KeyId);

				lines.Add("" + (DDGround.RO_MouseDispMode ? 1 : 0));

				// 新しい項目をここへ追加...

				blocks.Add(DDUtils.SplitableJoin(lines.ToArray()));
			}

			// アプリ固有のセーブデータ
			{
				List<string> lines = new List<string>();

				//lines.Add("Donut3-SaveData"); // Dummy
				//lines.Add("Donut3-SaveData"); // Dummy
				//lines.Add("Donut3-SaveData"); // Dummy

				lines.Add("" + Ground.I.HiScore);

				// 新しい項目をここへ追加...

				blocks.Add(DDUtils.SplitableJoin(lines.ToArray()));
			}

			File.WriteAllBytes(DDConsts.SaveDataFile, DDJammer.Encode(SCommon.SplittableJoin(blocks.ToArray())));
		}

		public static void Load()
		{
			if (!File.Exists(DDConsts.SaveDataFile))
				return;

			byte[][] blocks = SCommon.Split(DDJammer.Decode(File.ReadAllBytes(DDConsts.SaveDataFile)));
			int bc = 0;

			string[] lines = DDUtils.Split(blocks[bc++]);
			int c = 0;

			if (lines[c++] != ProcMain.APP_IDENT)
				throw new DDError();

			if (lines[c++] != ProcMain.APP_TITLE)
				throw new DDError();

			// アプリのアップデートによって項目の更新・増減があっても処理を続行するように try ～ catch しておく。

			try // Donut3 のセーブデータ
			{
				// TODO int.Parse -> IntTools.ToInt

				DDGround.RealScreen_W = int.Parse(lines[c++]);
				DDGround.RealScreen_H = int.Parse(lines[c++]);

				DDGround.RealScreenDraw_L = int.Parse(lines[c++]);
				DDGround.RealScreenDraw_T = int.Parse(lines[c++]);
				DDGround.RealScreenDraw_W = int.Parse(lines[c++]);
				DDGround.RealScreenDraw_H = int.Parse(lines[c++]);

				DDGround.MusicVolume = long.Parse(lines[c++]) / (double)SCommon.IMAX;
				DDGround.SEVolume = long.Parse(lines[c++]) / (double)SCommon.IMAX;

				DDInput.DIR_2.BtnId = int.Parse(lines[c++]);
				DDInput.DIR_4.BtnId = int.Parse(lines[c++]);
				DDInput.DIR_6.BtnId = int.Parse(lines[c++]);
				DDInput.DIR_8.BtnId = int.Parse(lines[c++]);
				DDInput.A.BtnId = int.Parse(lines[c++]);
				DDInput.B.BtnId = int.Parse(lines[c++]);
				DDInput.C.BtnId = int.Parse(lines[c++]);
				DDInput.D.BtnId = int.Parse(lines[c++]);
				DDInput.E.BtnId = int.Parse(lines[c++]);
				DDInput.F.BtnId = int.Parse(lines[c++]);
				DDInput.L.BtnId = int.Parse(lines[c++]);
				DDInput.R.BtnId = int.Parse(lines[c++]);
				DDInput.PAUSE.BtnId = int.Parse(lines[c++]);
				DDInput.START.BtnId = int.Parse(lines[c++]);

				DDInput.DIR_2.KeyId = int.Parse(lines[c++]);
				DDInput.DIR_4.KeyId = int.Parse(lines[c++]);
				DDInput.DIR_6.KeyId = int.Parse(lines[c++]);
				DDInput.DIR_8.KeyId = int.Parse(lines[c++]);
				DDInput.A.KeyId = int.Parse(lines[c++]);
				DDInput.B.KeyId = int.Parse(lines[c++]);
				DDInput.C.KeyId = int.Parse(lines[c++]);
				DDInput.D.KeyId = int.Parse(lines[c++]);
				DDInput.E.KeyId = int.Parse(lines[c++]);
				DDInput.F.KeyId = int.Parse(lines[c++]);
				DDInput.L.KeyId = int.Parse(lines[c++]);
				DDInput.R.KeyId = int.Parse(lines[c++]);
				DDInput.PAUSE.KeyId = int.Parse(lines[c++]);
				DDInput.START.KeyId = int.Parse(lines[c++]);

				DDGround.RO_MouseDispMode = int.Parse(lines[c++]) != 0;

				// 新しい項目をここへ追加...
			}
			catch (Exception e)
			{
				ProcMain.WriteLog(e);
			}

			Load_Delay = () =>
			{
				lines = DDUtils.Split(blocks[bc++]);
				c = 0;

				try // アプリ固有のセーブデータ
				{
					//DDUtils.Noop(lines[c++]); // Dummy
					//DDUtils.Noop(lines[c++]); // Dummy
					//DDUtils.Noop(lines[c++]); // Dummy

					Ground.I.HiScore = long.Parse(lines[c++]);

					// 新しい項目をここへ追加...
				}
				catch (Exception e)
				{
					ProcMain.WriteLog(e);
				}

				Load_Delay = () => { }; // reset
			};
		}

		public static Action Load_Delay = () => { };
	}
}
