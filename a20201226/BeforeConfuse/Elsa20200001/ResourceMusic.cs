using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using Charlotte.GameCommons;

namespace Charlotte
{
	public class ResourceMusic
	{
		//public DDMusic Dummy = new DDMusic("Dummy.mp3");

		//public DDMusic MUS_TITLE = new DDMusic(@"e20200003_dat\hmix\c22.mp3");
		public DDMusic MUS_TITLE = new DDMusic(@"e20200003_dat\とぼそ\nc161701.mp3");
		public DDMusic MUS_GAMEOVER = new DDMusic(@"e20200003_dat\hmix\c26.mp3");
		public DDMusic MUS_STAGE_01 = new DDMusic(@"e20200003_dat\hmix\v8.mp3");
		public DDMusic MUS_BOSS_01 = new DDMusic(@"e20200003_dat\Mirror of ES\nc213704.mp3").SetLoop(30000, 5240000);
		public DDMusic MUS_STAGE_02 = new DDMusic(@"e20200003_dat\hmix\n62.mp3");
		public DDMusic MUS_BOSS_02 = new DDMusic(@"e20200003_dat\Reda\nc136551.mp3").SetLoop(625000, 7365000);

		public ResourceMusic()
		{
			//this.Dummy.Volume = 0.1;

			this.MUS_BOSS_01.Volume = 1.0;
		}
	}
}
