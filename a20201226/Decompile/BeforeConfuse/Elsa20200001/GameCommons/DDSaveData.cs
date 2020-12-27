using System;
using System.Collections.Generic;
using System.IO;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000087 RID: 135
	public static class DDSaveData
	{
		// Token: 0x06000217 RID: 535 RVA: 0x0000DB10 File Offset: 0x0000BD10
		public static void Save()
		{
			File.WriteAllBytes("SaveData.dat", DDJammer.Encode(SCommon.SplittableJoin(new List<byte[]>
			{
				DDUtils.SplitableJoin(new List<string>
				{
					"{6519e425-fd6e-4762-a840-7391b5dd8632}",
					"Elsa20200001",
					DDGround.RealScreen_W.ToString() ?? "",
					DDGround.RealScreen_H.ToString() ?? "",
					DDGround.RealScreenDraw_L.ToString() ?? "",
					DDGround.RealScreenDraw_T.ToString() ?? "",
					DDGround.RealScreenDraw_W.ToString() ?? "",
					DDGround.RealScreenDraw_H.ToString() ?? "",
					SCommon.ToLong(DDGround.MusicVolume * 1000000000.0).ToString() ?? "",
					SCommon.ToLong(DDGround.SEVolume * 1000000000.0).ToString() ?? "",
					DDInput.DIR_2.BtnId.ToString() ?? "",
					DDInput.DIR_4.BtnId.ToString() ?? "",
					DDInput.DIR_6.BtnId.ToString() ?? "",
					DDInput.DIR_8.BtnId.ToString() ?? "",
					DDInput.A.BtnId.ToString() ?? "",
					DDInput.B.BtnId.ToString() ?? "",
					DDInput.C.BtnId.ToString() ?? "",
					DDInput.D.BtnId.ToString() ?? "",
					DDInput.E.BtnId.ToString() ?? "",
					DDInput.F.BtnId.ToString() ?? "",
					DDInput.L.BtnId.ToString() ?? "",
					DDInput.R.BtnId.ToString() ?? "",
					DDInput.PAUSE.BtnId.ToString() ?? "",
					DDInput.START.BtnId.ToString() ?? "",
					DDInput.DIR_2.KeyId.ToString() ?? "",
					DDInput.DIR_4.KeyId.ToString() ?? "",
					DDInput.DIR_6.KeyId.ToString() ?? "",
					DDInput.DIR_8.KeyId.ToString() ?? "",
					DDInput.A.KeyId.ToString() ?? "",
					DDInput.B.KeyId.ToString() ?? "",
					DDInput.C.KeyId.ToString() ?? "",
					DDInput.D.KeyId.ToString() ?? "",
					DDInput.E.KeyId.ToString() ?? "",
					DDInput.F.KeyId.ToString() ?? "",
					DDInput.L.KeyId.ToString() ?? "",
					DDInput.R.KeyId.ToString() ?? "",
					DDInput.PAUSE.KeyId.ToString() ?? "",
					DDInput.START.KeyId.ToString() ?? "",
					(DDGround.RO_MouseDispMode ? 1 : 0).ToString() ?? ""
				}.ToArray()),
				DDUtils.SplitableJoin(new List<string>
				{
					Ground.I.HiScore.ToString() ?? ""
				}.ToArray())
			}.ToArray())));
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000DFF8 File Offset: 0x0000C1F8
		public static void Load()
		{
			DDSaveData.<>c__DisplayClass1_0 CS$<>8__locals1 = new DDSaveData.<>c__DisplayClass1_0();
			if (!File.Exists("SaveData.dat"))
			{
				return;
			}
			CS$<>8__locals1.blocks = SCommon.Split(DDJammer.Decode(File.ReadAllBytes("SaveData.dat")));
			CS$<>8__locals1.bc = 0;
			DDSaveData.<>c__DisplayClass1_0 CS$<>8__locals2 = CS$<>8__locals1;
			byte[][] blocks = CS$<>8__locals1.blocks;
			int num = CS$<>8__locals1.bc;
			CS$<>8__locals1.bc = num + 1;
			CS$<>8__locals2.lines = DDUtils.Split(blocks[num]);
			CS$<>8__locals1.c = 0;
			string[] lines = CS$<>8__locals1.lines;
			num = CS$<>8__locals1.c;
			CS$<>8__locals1.c = num + 1;
			if (lines[num] != "{6519e425-fd6e-4762-a840-7391b5dd8632}")
			{
				throw new DDError();
			}
			string[] lines2 = CS$<>8__locals1.lines;
			num = CS$<>8__locals1.c;
			CS$<>8__locals1.c = num + 1;
			if (lines2[num] != "Elsa20200001")
			{
				throw new DDError();
			}
			try
			{
				string[] lines3 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreen_W = int.Parse(lines3[num]);
				string[] lines4 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreen_H = int.Parse(lines4[num]);
				string[] lines5 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreenDraw_L = int.Parse(lines5[num]);
				string[] lines6 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreenDraw_T = int.Parse(lines6[num]);
				string[] lines7 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreenDraw_W = int.Parse(lines7[num]);
				string[] lines8 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RealScreenDraw_H = int.Parse(lines8[num]);
				string[] lines9 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.MusicVolume = (double)long.Parse(lines9[num]) / 1000000000.0;
				string[] lines10 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.SEVolume = (double)long.Parse(lines10[num]) / 1000000000.0;
				DDInput.Button dir_ = DDInput.DIR_2;
				string[] lines11 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_.BtnId = int.Parse(lines11[num]);
				DDInput.Button dir_2 = DDInput.DIR_4;
				string[] lines12 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_2.BtnId = int.Parse(lines12[num]);
				DDInput.Button dir_3 = DDInput.DIR_6;
				string[] lines13 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_3.BtnId = int.Parse(lines13[num]);
				DDInput.Button dir_4 = DDInput.DIR_8;
				string[] lines14 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_4.BtnId = int.Parse(lines14[num]);
				DDInput.Button a = DDInput.A;
				string[] lines15 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				a.BtnId = int.Parse(lines15[num]);
				DDInput.Button b = DDInput.B;
				string[] lines16 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				b.BtnId = int.Parse(lines16[num]);
				DDInput.Button c = DDInput.C;
				string[] lines17 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				c.BtnId = int.Parse(lines17[num]);
				DDInput.Button d = DDInput.D;
				string[] lines18 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				d.BtnId = int.Parse(lines18[num]);
				DDInput.Button e2 = DDInput.E;
				string[] lines19 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				e2.BtnId = int.Parse(lines19[num]);
				DDInput.Button f = DDInput.F;
				string[] lines20 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				f.BtnId = int.Parse(lines20[num]);
				DDInput.Button l = DDInput.L;
				string[] lines21 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				l.BtnId = int.Parse(lines21[num]);
				DDInput.Button r = DDInput.R;
				string[] lines22 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				r.BtnId = int.Parse(lines22[num]);
				DDInput.Button pause = DDInput.PAUSE;
				string[] lines23 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				pause.BtnId = int.Parse(lines23[num]);
				DDInput.Button start = DDInput.START;
				string[] lines24 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				start.BtnId = int.Parse(lines24[num]);
				DDInput.Button dir_5 = DDInput.DIR_2;
				string[] lines25 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_5.KeyId = int.Parse(lines25[num]);
				DDInput.Button dir_6 = DDInput.DIR_4;
				string[] lines26 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_6.KeyId = int.Parse(lines26[num]);
				DDInput.Button dir_7 = DDInput.DIR_6;
				string[] lines27 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_7.KeyId = int.Parse(lines27[num]);
				DDInput.Button dir_8 = DDInput.DIR_8;
				string[] lines28 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				dir_8.KeyId = int.Parse(lines28[num]);
				DDInput.Button a2 = DDInput.A;
				string[] lines29 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				a2.KeyId = int.Parse(lines29[num]);
				DDInput.Button b2 = DDInput.B;
				string[] lines30 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				b2.KeyId = int.Parse(lines30[num]);
				DDInput.Button c2 = DDInput.C;
				string[] lines31 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				c2.KeyId = int.Parse(lines31[num]);
				DDInput.Button d2 = DDInput.D;
				string[] lines32 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				d2.KeyId = int.Parse(lines32[num]);
				DDInput.Button e3 = DDInput.E;
				string[] lines33 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				e3.KeyId = int.Parse(lines33[num]);
				DDInput.Button f2 = DDInput.F;
				string[] lines34 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				f2.KeyId = int.Parse(lines34[num]);
				DDInput.Button l2 = DDInput.L;
				string[] lines35 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				l2.KeyId = int.Parse(lines35[num]);
				DDInput.Button r2 = DDInput.R;
				string[] lines36 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				r2.KeyId = int.Parse(lines36[num]);
				DDInput.Button pause2 = DDInput.PAUSE;
				string[] lines37 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				pause2.KeyId = int.Parse(lines37[num]);
				DDInput.Button start2 = DDInput.START;
				string[] lines38 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				start2.KeyId = int.Parse(lines38[num]);
				string[] lines39 = CS$<>8__locals1.lines;
				num = CS$<>8__locals1.c;
				CS$<>8__locals1.c = num + 1;
				DDGround.RO_MouseDispMode = (int.Parse(lines39[num]) != 0);
			}
			catch (Exception e)
			{
				ProcMain.WriteLog(e);
			}
			DDSaveData.Load_Delay = delegate()
			{
				byte[][] blocks2 = CS$<>8__locals1.blocks;
				int num2 = CS$<>8__locals1.bc;
				CS$<>8__locals1.bc = num2 + 1;
				CS$<>8__locals1.lines = DDUtils.Split(blocks2[num2]);
				CS$<>8__locals1.c = 0;
				try
				{
					Ground i = Ground.I;
					string[] lines40 = CS$<>8__locals1.lines;
					num2 = CS$<>8__locals1.c;
					CS$<>8__locals1.c = num2 + 1;
					i.HiScore = long.Parse(lines40[num2]);
				}
				catch (Exception e4)
				{
					ProcMain.WriteLog(e4);
				}
				DDSaveData.Load_Delay = delegate()
				{
				};
			};
		}

		// Token: 0x040001E5 RID: 485
		public static Action Load_Delay = delegate()
		{
		};
	}
}
