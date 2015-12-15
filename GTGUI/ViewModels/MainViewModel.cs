using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTGUI.ViewModels
{
	[Export(typeof(MainViewModel))]
	public class MainViewModel : Screen, IGuardClose, IHaveDisplayName
	{
		public ToolboxViewModel Toolbox { get; set; }
		public CanvasViewModel Canvas { get; set; }
		public ConsoleViewModel Console { get; set; }
		public VariablesViewModel Variables { get; set; }

		public MainViewModel() { }

		[Import]
		IWindowManager WindowManager { get; set; }

		[ImportingConstructor]
        public MainViewModel(ToolboxViewModel toolbox, CanvasViewModel canvas,
			ConsoleViewModel console, VariablesViewModel variables)
		{
			DisplayName = "GeocachingToolbox GUI";

			Toolbox = toolbox;
			Canvas = canvas;
			Console = console;
			Variables = variables;

			ScriptingEnvironment.Variables = Variables;
		}

		public void Execute()
		{
			Canvas.StartExecuting();
		}

		public void Stop()
		{
			Canvas.StopExecuting();
		}

		public void Open()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Geocaching Toolbox GUI workspace (*.gtw)|*.gtw";

			if (dialog.ShowDialog() == true)
			{
				Canvas.Deserialize(dialog.FileName);
			}
		}

		public void Save()
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "Geocaching Toolbox GUI workspace (*.gtw)|*.gtw";

			if (dialog.ShowDialog() == true)
			{
				Canvas.Serialize(dialog.FileName);
			}
		}
	}
}
