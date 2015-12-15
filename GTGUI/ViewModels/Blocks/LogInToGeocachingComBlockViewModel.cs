using GeocachingToolbox.GeocachingCom;
using GTGUI.ViewModels.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Geocaching-specific")]
	public class LogInToGeocachingComBlockViewModel : BlockViewModel
	{
		public string Login { get; set; }
		public string Password { get; set; }
		public string TargetVariableName { get; set; }

		public LogInToGeocachingComBlockViewModel()
		{
			TargetVariableName = "Client";
		}

		public override void Execute(CancellationToken cancellationToken)
		{
			ScriptingEnvironment.Execute(string.Format(
				@"var {0} = new GCClient();
				{0}.Login(""{1}"", ""{2}"");", TargetVariableName, Login, Password));
		}
	}
}
