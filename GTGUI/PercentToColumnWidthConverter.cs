using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GTGUI
{
	class PercentToColumnWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			GridLength length;
			int percent = Math.Max(Math.Min((int) value, 100), 0);

			if ((string)parameter == "EmptySpace")
				length = new GridLength(100.0 - percent, GridUnitType.Star);
			else
				length = new GridLength(percent, GridUnitType.Star);

			return length;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
