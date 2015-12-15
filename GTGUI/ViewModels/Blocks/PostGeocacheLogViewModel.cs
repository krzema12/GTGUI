using Caliburn.Micro;
using GeocachingToolbox;
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
	public class PostGeocacheLogViewModel : BlockViewModel
	{
		public string GeocacheExpression { get; set; }
		public string ClientVariableName { get; set; }
		public string Comment { get; set; }
		public DateTime Date { get; set; }

		private GeocacheLogType _selectedLogType;
		public GeocacheLogType SelectedLogType
		{
			get
			{
				return _selectedLogType;
			}

			set
			{
				_selectedLogType = value;
				this.NotifyOfPropertyChange(() => SelectedLogType);
			}
		}

		private BindableCollection<GeocacheLogType> _logTypes;
		[XmlIgnore]
		public BindableCollection<GeocacheLogType> LogTypes
		{
			get { return _logTypes; }
			set
			{
				_logTypes = value;
				NotifyOfPropertyChange(() => LogTypes);
			}
		}

		public PostGeocacheLogViewModel()
		{
			Date = DateTime.Today;
			ClientVariableName = "Client";
			Comment = "";

			LogTypes = new BindableCollection<GeocacheLogType>(Enum.GetValues(typeof(GeocacheLogType)).Cast<GeocacheLogType>());
			SelectedLogType = GeocacheLogType.Found;
		}

		public override void Execute(CancellationToken cancellationToken)
		{
			ScriptingEnvironment.Execute(string.Format(@"{0}.PostGeocacheLog<Geocache>({1}, {2}, {3}, ""{4}"");",
				ClientVariableName, GeocacheExpression, "GeocacheLogType." + SelectedLogType, @"DateTime.Parse(""" + Date.ToString() + @""")", Comment));
		}
	}
}
