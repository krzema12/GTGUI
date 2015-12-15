using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GTGUI.ViewModels
{
	[Export(typeof(VariablesViewModel))]
	public class VariablesViewModel : PropertyChangedBase
	{
		public class VariableInfo
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		private ObservableCollection<VariableInfo> _variablesList = new ObservableCollection<VariableInfo>();
		public ObservableCollection<VariableInfo> VariablesList
		{
			get { return _variablesList; }
			set
			{
				_variablesList = value;
				NotifyOfPropertyChange(() => VariablesList);
			}
		}

		public ObservableDictionary<string, string> Variables = new ObservableDictionary<string, string>();

		public void Clear()
		{
			VariablesList.Clear();
		}

		public void SetVariable(string name, string value)
		{
			Variables[name] = value;
			NotifyOfPropertyChange(() => Variables);

			var list = new List<VariableInfo>();

			foreach (var variable in Variables)
			{
				list.Add(new VariableInfo { Name = variable.Key, Value = variable.Value });
			}

			VariablesList = new ObservableCollection<VariableInfo>(list);
		}
	}
}
