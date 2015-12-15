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
	public class TextToListViewModel : BlockViewModel
	{
		public string ItemsAsString { get; set; }
		public string CollectionName { get; set; }

		public override void Execute(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			ScriptingEnvironment.Execute("var itemsAsString = @\"" + ItemsAsString + "\";");
			ScriptingEnvironment.Execute("var items = itemsAsString.Split(new string[] { \"\\r\\n\", \"\\n\" }, StringSplitOptions.None);");
			ScriptingEnvironment.Execute("var " + CollectionName + " = items.AsEnumerable();");
		}
	}
}
