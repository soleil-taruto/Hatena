using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	public static class DDConsts
	{
		public const string ConfigFile = "Config.conf";
		public const string SaveDataFile = "SaveData.dat";
		public const string ResourceFile_01 = "Resource.dat";
		public const string ResourceFile_02 = "res.dat";
		public const string ResourceDir_DevEnv_01 = @"..\..\..\..\dat";
		public const string ResourceDir_DevEnv_02 = @"..\..\..\..\res";
		public const string ResourceDir_InternalRelease_01 = @".\Data\Elsa";
		public const string ResourceDir_InternalRelease_02 = @".\Data\res";
		public const string UserDatStringsFile = "Properties.dat";

		public const int Screen_W = 960;
		public const int Screen_H = 540;

		public const int Screen_W_Min = 100;
		public const int Screen_H_Min = 100;
		public const int Screen_W_Max = SCommon.IMAX;
		public const int Screen_H_Max = SCommon.IMAX;

		public const double DefaultVolume = 0.45;

		//public const int DEFAULT_DX_DRAWMODE = DX.DX_DRAWMODE_NEAREST;
		//public const int DEFAULT_DX_DRAWMODE = DX.DX_DRAWMODE_BILINEAR;
		public const int DEFAULT_DX_DRAWMODE = DX.DX_DRAWMODE_ANISOTROPIC;
	}
}
