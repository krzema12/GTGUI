using Caliburn.Micro;
using GTGUI.ViewModels.Blocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GTGUI.ViewModels.Blocks
{
	[BlockGroup("Geocaching-specific")]
	public class LogInToOpencachingBlockViewModel : BlockViewModel
	{
		public string TargetVariableName { get; set; }

		private BindableCollection<string> _services;
		[XmlIgnore]
		public BindableCollection<string> Services
		{
			get { return _services; }
			set
			{
				_services = value;
				NotifyOfPropertyChange(() => Services);
			}
		}

		private string _selectedService;
		public string SelectedService
		{
			get
			{
				return _selectedService;
			}

			set
			{
				_selectedService = value;
				this.NotifyOfPropertyChange(() => SelectedService);
			}
		}

		private string _currentUserName;
		public string CurrentUserName
		{
			get
			{
				return _currentUserName;
			}

			set
			{
				_currentUserName = value;
				this.NotifyOfPropertyChange(() => CurrentUserName);
			}
		}

		public LogInToOpencachingBlockViewModel()
		{
			Services = new BindableCollection<string>()
			{
				"opencaching.pl",
				"opencaching.de",
				"opencaching.us",
				"opencaching.nl",
				"opencaching.ro"
			};
			SelectedService = Services[0];
			TargetVariableName = "Client";
			CurrentUserName = "[none]";
		}

		public override void Execute(CancellationToken cancellationToken)
		{
			ScriptingEnvironment.Execute(string.Format(@"var {0} = new OCClient(""{1}"");", TargetVariableName, SelectedService));
			ScriptingEnvironment.Execute(string.Format(@"{0}.Connect();", TargetVariableName));
			CurrentUserName = ScriptingEnvironment.EvaluateAsString(string.Format(@"{0}.User.Name", TargetVariableName));
		}
	}
}
