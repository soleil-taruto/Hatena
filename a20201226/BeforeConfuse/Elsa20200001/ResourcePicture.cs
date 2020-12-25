using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;
using Charlotte.GameCommons;

namespace Charlotte
{
	public class ResourcePicture
	{
		//public DDPicture Dummy = DDPictureLoaders.Standard("Dummy.png");

		public DDPicture Copyright = DDPictureLoaders.Standard(@"e20200003_dat\Logo\Copyright.png");

		//public DDPicture Udonge = DDPictureLoaders.Standard(@"e20200003_dat\dairi\67252239_p0.png");

		public DDPicture 立ち絵_小悪魔_01 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\66051062_p0.png");
		public DDPicture 立ち絵_小悪魔_02 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\66051062_p2.png");
		public DDPicture 立ち絵_鍵山雛_01 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\67049351_p0.png");
		public DDPicture 立ち絵_鍵山雛_02 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\67049351_p3.png");
		public DDPicture 立ち絵_メディスン_01 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\66537404_p0.png");
		public DDPicture 立ち絵_メディスン_02 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\66537404_p3.png");
		public DDPicture 立ち絵_ルーミア_01 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\65997323_p0.png");
		public DDPicture 立ち絵_ルーミア_02 = DDPictureLoaders.Standard(@"e20200003_dat\dairi\65997323_p2.png");

		public DDPicture MessageWindow = DDPictureLoaders.Standard(@"e20200003_dat\フキダシデザイン\e0165_1.png");

		public DDPicture メディスン = DDPictureLoaders.Standard(@"e20200003_dat\メディスン\medicine.png");
		public DDPicture DotOther = DDPictureLoaders.RGBToTrans(@"e20200003_dat\Shoot_old_Resource\th_fuuchi-sozai\img\dot-Other.png", new I3Color(0, 0, 0));

		public DDPicture P_KOAKUMA_P1 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p1.png");
		public DDPicture P_KOAKUMA_P3 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p3.png");
		public DDPicture P_KOAKUMA_TACHIE_00 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p8.png");
		public DDPicture P_KOAKUMA_TACHIE_01 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p9.png");
		public DDPicture P_KOAKUMA_TACHIE_02 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p10.png");
		public DDPicture P_KOAKUMA_TACHIE_03 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p11.png");
		public DDPicture P_KOAKUMA_TACHIE_04 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p12.png");
		public DDPicture P_KOAKUMA_TACHIE_05 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p13.png");
		public DDPicture P_KOAKUMA_TACHIE_06 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p14.png");
		public DDPicture P_KOAKUMA_TACHIE_07 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p15.png");
		public DDPicture P_KOAKUMA_TACHIE_08 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p16.png");
		public DDPicture P_KOAKUMA_WALL_TEXT = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p17.png");
		public DDPicture P_KOAKUMA_WALL = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\jiki-koakuma-sozai\19740345_big_p18.png");
		public DDPicture P_ENEMYDIE = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\bhmqm120\explosionA-color_10F_xp.png");
		public DDPicture P_ENEMYDIE_ABGR = DDPictureLoaders.SelectARGB(@"e20200003_dat\Shoot_old_Resource\bhmqm120\explosionA-color_10F_xp.png", "ABGR");
		public DDPicture P_ENEMYSHOTDIE = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\sentou-effect-anime9\320x240\pipo-btleffect072.png");
		public DDPicture P_PLAYERDIE = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\sentou-effect-anime4\640x480\pipo-btleffect049.png");
		public DDPicture P_PUMPKIN = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\DVDM\Enemy11.png");
		public DDPicture P_PUMPKIN_AGRB = DDPictureLoaders.SelectARGB(@"e20200003_dat\Shoot_old_Resource\DVDM\Enemy11.png", "AGRB");
		public DDPicture P_TAMA = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\kuushot\kuushot.png");
		public DDPicture P_TAMA_B = DDPictureLoaders.RGBToTrans(@"e20200003_dat\Shoot_old_Resource\kuushot\kuushot.png", new I3Color(0, 0, 0));
		public DDPicture P_BLUETILE_01 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\989-tileable-midnight-blue-grunge-patterns\tileable_midnight_blue_grunge_pattern_1.jpg");
		public DDPicture P_BLUETILE_02 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\989-tileable-midnight-blue-grunge-patterns\tileable_midnight_blue_grunge_pattern_2.jpg");
		public DDPicture P_BLUETILE_02_REDUCT4 = DDPictureLoaders.Reduct(@"e20200003_dat\Shoot_old_Resource\speckboy\989-tileable-midnight-blue-grunge-patterns\tileable_midnight_blue_grunge_pattern_2.jpg", 4);
		public DDPicture P_BLUETILE_03 = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\989-tileable-midnight-blue-grunge-patterns\tileable_midnight_blue_grunge_pattern_3.jpg");
		public DDPicture P_BW_ARMY = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\pattern8-pattern-2\pattern8-pattern-camo-1a.png");
		public DDPicture P_BW_NAVY = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\pattern8-pattern-2\pattern8-pattern-camo-1b.png");
		public DDPicture P_BW_PUMPKIN = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\speckboy\pattern8-pattern-2\pattern8-pattern-55b.png");
		public DDPicture P_DIGITS_W = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\suuji-font\suuji16x32_06.png");
		public DDPicture P_DIGITS_DDY = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\suuji-font\suuji16x32_01.png");
		public DDPicture P_DIGITS_DY = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\suuji-font\suuji16x32_02.png");
		public DDPicture P_DIGITS_Y = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\suuji-font\suuji16x32_03.png");
		public DDPicture P_WALL = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\run\22350006_big_p26.jpg");
		//public DDPicture P_FAIRYETC = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\th_fuuchi-sozai\img\dot-kaze-ti.png"); // del
		public DDPicture P_FAIRYETC = DDPictureLoaders.RGBToTrans(@"e20200003_dat\Shoot_old_Resource\th_fuuchi-sozai\img\dot-kaze-ti.png", new I3Color(0, 0, 0));
		public DDPicture P_MAHOJIN_HAJIKE = DDPictureLoaders.Standard(@"e20200003_dat\ぴぽや倉庫\sentou-effect-anime1\640x480\pipo-btleffect007.png");
		public DDPicture P_TITLE_WALL = DDPictureLoaders.Standard(@"e20200003_dat\Shoot_old_Resource\run\22350006_big_p20.jpg");
		public DDPicture P_FUJINBOSS = DDPictureLoaders.RGBToTrans(@"e20200003_dat\Shoot_old_Resource\th_fuuchi-sozai\img\dot-kaze-ti2.png", new I3Color(0, 0, 0));
	}
}
