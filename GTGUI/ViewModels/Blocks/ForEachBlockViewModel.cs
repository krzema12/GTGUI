using GTGUI.Views.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Flow")]
	public class ForEachBlockViewModel : BlockViewModel
	{
		public string CollectionName { get; set; }
		public string SingleItemName { get; set; }

		private int _allItemsCount;
		private int _currentElement = 0;

		private bool _finished = false;

		public override void Execute(CancellationToken cancellationToken)
		{
			if (_currentElement == 0)
			{
				Canvas.ExecutionStack.Push(this);
			}

			var code = CollectionName + ".Count()";
			_allItemsCount = (int)ScriptingEnvironment.Evaluate(code);

			if (_currentElement >= _allItemsCount)
			{
				_finished = true;
				Canvas.ExecutionStack.Pop();
				Progress = 100;
				return;
			}

			ResetForEachBranch();

			ScriptingEnvironment.Execute((_currentElement == 0 ? "var " : "") + SingleItemName + " = "
				+ CollectionName + ".ElementAt(" + _currentElement + ");");

			Progress = _allItemsCount == 1 ? 100 : _currentElement * 100 / (_allItemsCount - 1);
			_currentElement++;
		}

		public override void Reset()
		{
			_currentElement = 0;
			Progress = 0;
			_finished = false;

			ResetForEachBranch();
			base.Reset();
		}

		private void ResetForEachBranch()
		{
			if (Connections.ContainsKey("DoForEach"))
			{
				Connections["DoForEach"].Reset();
			}
		}

		public override BlockViewModel GetNextBlock()
		{
			return _finished ? (Connections.ContainsKey("Next") ? Connections["Next"] : null) : Connections["DoForEach"];
		}
	}
}
