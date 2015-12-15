using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Geocaching-specific")]
	public class GetNearbyGeocachesBlockViewModel : BlockViewModel
	{
		public string LocationVariableName { get; set; }
		public string ClientVariableName { get; set; }
		public string TargetVariableName { get; set; }

		public override void Execute(CancellationToken cancellationToken)
		{
			ScriptingEnvironment.Execute(string.Format(
				@"var {0} = {1}.GetNearestGeocaches<Geocache>({2});",
				TargetVariableName, ClientVariableName, LocationVariableName));
		}
	}
}
