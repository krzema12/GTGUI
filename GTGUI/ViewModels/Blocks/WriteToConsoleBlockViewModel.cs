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
	public class WriteToConsoleBlockViewModel : BlockViewModel
	{
		public string Code { get; set; }

		public override void Execute(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			/*Progress = 0;

			while(Progress < 100)
			{
				cancellationToken.ThrowIfCancellationRequested();

				GTConsole.WriteLine("Progress: " + Progress + ", " + Text);
				Progress += 5;
				Thread.Sleep(300);
			}*/

			GTConsole.WriteLine(ScriptingEnvironment.EvaluateAsString(Code));

			//Progress = 100;
		}
	}
}
