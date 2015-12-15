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
	public class PostTrackableLogViewModel : BlockViewModel
	{
		public string TrackableExpression { get; set; }
		public string ClientVariableName { get; set; }
		public string Comment { get; set; }
		public DateTime Date { get; set; }

		private TrackableLogType _selectedLogType;
		public TrackableLogType SelectedLogType
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

		private BindableCollection<TrackableLogType> _logTypes;
		[XmlIgnore]
		public BindableCollection<TrackableLogType> LogTypes
		{
			get { return _logTypes; }
			set
			{
				_logTypes = value;
				NotifyOfPropertyChange(() => LogTypes);
			}
		}

		public PostTrackableLogViewModel()
		{
			Date = DateTime.Today;
			ClientVariableName = "Client";
			SelectedLogType = TrackableLogType.Discovered;
			Comment = "";

			LogTypes = new BindableCollection<TrackableLogType>(Enum.GetValues(typeof(TrackableLogType)).Cast<TrackableLogType>());
			SelectedLogType = TrackableLogType.Discovered;
		}

		public override void Execute(CancellationToken cancellationToken)
		{
			try
			{
				ScriptingEnvironment.Execute(string.Format(@"{0}.PostTrackableLog<Trackable>({1}, {2}, {3}, ""{4}"");",
					ClientVariableName, TrackableExpression, "TrackableLogType." + SelectedLogType, @"DateTime.Parse(""" + Date.ToString() + @""")", Comment));
			}
			catch (TrackableNotFoundException e)
			{
				GTConsole.WriteLine(e.Message);
			}
		}
	}
}
