using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTGUI.ViewModels
{
	[Export(typeof(ConsoleViewModel))]
	public class ConsoleViewModel : PropertyChangedBase
	{
		private string _contents;
		public string Contents
		{
			get { return _contents; }
			set
			{
				_contents = value;
				NotifyOfPropertyChange(() => Contents);
			}
		}

		public ConsoleViewModel()
		{
			GTConsole.Console = this;
		}

		public void AppendLine(string text)
		{
			Contents += text + "\n";
		}

		public void Clear()
		{
			Contents = "";
		}
	}
}
