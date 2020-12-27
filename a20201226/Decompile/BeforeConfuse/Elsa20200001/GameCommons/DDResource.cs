using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	// Token: 0x02000086 RID: 134
	public static class DDResource
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000D788 File Offset: 0x0000B988
		public static void INIT()
		{
			if (File.Exists("Resource.dat"))
			{
				DDResource.ReleaseMode = true;
			}
			else if (Directory.Exists(".\\Data\\Elsa"))
			{
				DDResource.ResourceDir_01 = ".\\Data\\Elsa";
				DDResource.ResourceDir_02 = ".\\Data\\res";
			}
			else
			{
				DDResource.ResourceDir_01 = "..\\..\\..\\..\\dat";
				DDResource.ResourceDir_02 = "..\\..\\..\\..\\res";
			}
			if (DDResource.ReleaseMode)
			{
				foreach (string resFile in new string[]
				{
					"Resource.dat",
					"res.dat"
				})
				{
					List<DDResource.ResInfo> resInfos = new List<DDResource.ResInfo>();
					using (FileStream reader = new FileStream(resFile, FileMode.Open, FileAccess.Read))
					{
						while (reader.Position < reader.Length)
						{
							int size = SCommon.ToInt(SCommon.Read(reader, 4), 0);
							if (size < 0)
							{
								throw new DDError();
							}
							resInfos.Add(new DDResource.ResInfo
							{
								ResFile = resFile,
								Offset = reader.Position,
								Size = size
							});
							reader.Seek((long)size, SeekOrigin.Current);
						}
					}
					string[] files = SCommon.TextToLines(SCommon.ENCODING_SJIS.GetString(DDResource.LoadFile(resInfos[0])));
					if (files.Length != resInfos.Count)
					{
						throw new DDError(files.Length.ToString() + ", " + resInfos.Count.ToString());
					}
					for (int index = 1; index < files.Length; index++)
					{
						string file = files[index];
						if (DDResource.File2ResInfo.ContainsKey(file))
						{
							throw new DDError(file);
						}
						DDResource.File2ResInfo.Add(file, resInfos[index]);
					}
				}
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000D940 File Offset: 0x0000BB40
		private static byte[] LoadFile(string resFile, long offset, int size)
		{
			byte[] result;
			using (FileStream reader = new FileStream(resFile, FileMode.Open, FileAccess.Read))
			{
				reader.Seek(offset, SeekOrigin.Begin);
				result = DDJammer.Decode(SCommon.Read(reader, size));
			}
			return result;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000D98C File Offset: 0x0000BB8C
		private static byte[] LoadFile(DDResource.ResInfo resInfo)
		{
			return DDResource.LoadFile(resInfo.ResFile, resInfo.Offset, resInfo.Size);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000D9A8 File Offset: 0x0000BBA8
		public static byte[] Load(string file)
		{
			if (DDResource.ReleaseMode)
			{
				return DDResource.LoadFile(DDResource.File2ResInfo[file]);
			}
			string datFile = Path.Combine(DDResource.ResourceDir_01, file);
			if (!File.Exists(datFile))
			{
				datFile = Path.Combine(DDResource.ResourceDir_02, file);
				if (!File.Exists(datFile))
				{
					throw new Exception(datFile);
				}
			}
			return File.ReadAllBytes(datFile);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000DA02 File Offset: 0x0000BC02
		public static void Save(string file, byte[] fileData)
		{
			if (DDResource.ReleaseMode)
			{
				throw new DDError();
			}
			File.WriteAllBytes(Path.Combine(DDResource.ResourceDir_02, file), fileData);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000DA24 File Offset: 0x0000BC24
		public static IEnumerable<string> GetFiles()
		{
			IEnumerable<string> files;
			if (DDResource.ReleaseMode)
			{
				files = DDResource.File2ResInfo.Keys;
			}
			else
			{
				IEnumerable<string>[] array = new IEnumerable<string>[2];
				array[0] = from file in Directory.GetFiles(DDResource.ResourceDir_01, "*", SearchOption.AllDirectories)
				select SCommon.ChangeRoot(file, DDResource.ResourceDir_01);
				array[1] = from file in Directory.GetFiles(DDResource.ResourceDir_02, "*", SearchOption.AllDirectories)
				select SCommon.ChangeRoot(file, DDResource.ResourceDir_02);
				files = SCommon.Concat<string>(array);
				files = from file in files
				where Path.GetFileName(file)[0] != '_'
				select file;
			}
			return SCommon.Sort<string>(files, new Comparison<string>(SCommon.CompIgnoreCase));
		}

		// Token: 0x040001E1 RID: 481
		private static bool ReleaseMode = false;

		// Token: 0x040001E2 RID: 482
		private static string ResourceDir_01;

		// Token: 0x040001E3 RID: 483
		private static string ResourceDir_02;

		// Token: 0x040001E4 RID: 484
		private static Dictionary<string, DDResource.ResInfo> File2ResInfo = SCommon.CreateDictionaryIgnoreCase<DDResource.ResInfo>();

		// Token: 0x02000138 RID: 312
		private class ResInfo
		{
			// Token: 0x04000511 RID: 1297
			public string ResFile;

			// Token: 0x04000512 RID: 1298
			public long Offset;

			// Token: 0x04000513 RID: 1299
			public int Size;
		}
	}
}
