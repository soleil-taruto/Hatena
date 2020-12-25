using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Commons;

namespace Charlotte.GameCommons
{
	/// <summary>
	/// タスクを単体のメソッドで実装したい場合は IEnumerable(bool) を返すメソッドを定義して SCommon.Supplier で Func(bool) 化する。
	/// タスクをクラスにしたい場合は、本クラスを継承して実装する。本クラスは単純なので継承するまでも無いかもしれない。
	/// タスクの考え方に慣れるまで分かりやすさのためにクラス化しておく。
	/// 使用例：
	/// -- インスタンス task を生成して、毎フレーム task.Execute(); を実行する。
	/// -- インスタンス task を生成 f = task.Task; を実行して、毎フレーム f(); を実行する。
	/// -- タスクリストに追加する場合は DDGround.EL.Add(インスタンス.Task); のようにする。
	/// </summary>
	public abstract class DDTask
	{
		public bool Execute()
		{
			return this.Task();
		}

		private Func<bool> _task = null;

		public Func<bool> Task
		{
			get
			{
				if (_task == null)
					_task = SCommon.Supplier(this.E_Task());

				return _task;
			}
		}

		public abstract IEnumerable<bool> E_Task();
	}
}
