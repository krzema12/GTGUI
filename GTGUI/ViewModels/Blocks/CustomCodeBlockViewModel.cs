using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("General")]
	public class CustomCodeBlockViewModel : BlockViewModel
	{
		public string Code { get; set; }

		public override void Execute(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ScriptingEnvironment.Execute(Code);
		}
	}
}
