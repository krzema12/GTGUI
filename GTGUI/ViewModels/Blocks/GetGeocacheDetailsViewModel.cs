using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Geocaching-specific")]
	public class GetGeocacheDetailsViewModel : BlockViewModel
	{
		public string ClientName { get; set; }
		public string VariableName { get; set; }

		public override void Execute(System.Threading.CancellationToken cancellationToken)
		{
			ScriptingEnvironment.Execute(string.Format(
				@"({0}).GetGeocacheDetails({1});", ClientName, VariableName));
		}
	}
}
