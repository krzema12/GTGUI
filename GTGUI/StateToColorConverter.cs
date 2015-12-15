using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GTGUI
{
	public class StateToColorConverter : IValueConverter
	{
		private Dictionary<ExecutionState, Color> _colorMapping
			= new Dictionary<ExecutionState, Color>
			{
				{ ExecutionState.NotExecuted, Colors.Black },
				{ ExecutionState.Current, Colors.Orange },
				{ ExecutionState.Executed, Color.FromRgb(0, 192, 0) },
				{ ExecutionState.Error, Colors.Red }
			};

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var state = (ExecutionState)value;
			var brush = new SolidColorBrush(_colorMapping[state]);
			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
