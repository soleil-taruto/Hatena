using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// <para>ゲームを終了する。</para>
	/// <para>その場でゲームを終了したい場合にこれを投げること。</para>
	/// <para>なので、これを DDMain2 以外で catch してはならない。</para>
	/// <para>右上の[X]ボタン、エスケープ押下時も DDEngine からこれを投げる。</para>
	/// </summary>
	public class DDCoffeeBreak : Exception
	{ }
}
