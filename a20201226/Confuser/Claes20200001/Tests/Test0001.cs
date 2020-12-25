using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Charlotte.Tests
{
	public class Test0001
	{
		public void Test01()
		{
			// memo: CSSolution.Confuse() はソリューション配下のソースファイルを書き換えるので、安全のため ElsaConfuser.Perform() 経由でテストすること。

			// -- choose one --

			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20200928_NovelAdv\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201003_NovelAdv\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201006_NovelAdv\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201007_NovelAdv\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201008_NovelAdv_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201009_NovelAdv_Base_old\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201010_TateShoot\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201014_TateShoot\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201017_NovelAdv_Base\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201018_TateShoot_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201020_YokoShoot\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201022_YokoShoot_Demo_old\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201023_YokoActTM\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201025_YokoActTM\Elsa20200001\Elsa20200001.sln", @"C:\temp"); // 未対応 @ 2020.11.16
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201027_YokoActTM_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201028_YokoShoot_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201029_TopViewAct\Elsa20200001\Elsa20200001.sln", @"C:\temp"); // 未対応 @ 2020.11.16
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201103_TopViewAct_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201104_YokoActTK\Elsa20200001\Elsa20200001.sln", @"C:\temp"); // 未対応 @ 2020.11.16
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201108_YokoActTK\Elsa20200001\Elsa20200001.sln", @"C:\temp"); // 未対応 @ 2020.11.16
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201109_YokoActTK_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201115_Dungeon\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			//ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201209_YokoActTK_Demo\Elsa20200001\Elsa20200001.sln", @"C:\temp");
			ElsaConfuser.Perform(@"C:\Dev\Elsa\e20201212_DoremyRockman_old\Elsa20200001\Elsa20200001.sln", @"C:\temp");

			// --
		}
	}
}
