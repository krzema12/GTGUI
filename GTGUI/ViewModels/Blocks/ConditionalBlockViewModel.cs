using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Flow")]
	public class ConditionalBlockViewModel : BlockViewModel
	{
		public string Condition { get; set; }
		private bool _conditionIsTrue;

		public override void Execute(CancellationToken cancellationToken)
		{
			_conditionIsTrue = (bool)ScriptingEnvironment.Evaluate(Condition);
			ResetIfTrueBranch();
		}

		public override BlockViewModel GetNextBlock()
		{
			return _conditionIsTrue ? Connections["IfTrue"] : Connections["Next"];
		}

		public override void Reset()
		{
			base.Reset();
			ResetIfTrueBranch();
		}

		private void ResetIfTrueBranch()
		{
			if (Connections.ContainsKey("IfTrue"))
			{
				Connections["IfTrue"].Reset();
			}
		}
	}
}
