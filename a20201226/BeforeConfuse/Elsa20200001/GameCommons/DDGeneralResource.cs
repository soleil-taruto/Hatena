using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// <para>全プロジェクトで共通のリソース</para>
	/// <para>当該リソースは e20200002_General\General フォルダに収録すること。</para>
	/// </summary>
	public class DDGeneralResource
	{
		// game_app.ico --> DDMain.GetAppIcon()

		public DDPicture Dummy = DDPictureLoaders.Standard(@"e20200002_General\General\Dummy.png");
		public DDPicture WhiteBox = DDPictureLoaders.Standard(@"e20200002_General\General\WhiteBox.png");
		public DDPicture WhiteCircle = DDPictureLoaders.Standard(@"e20200002_General\General\WhiteCircle.png");

		public DDMusic 無音 = new DDMusic(@"e20200002_General\General\muon.wav");

		// 新しいリソースをここへ追加...
	}
}
