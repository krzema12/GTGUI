using System.Windows.Controls;

namespace GTGUI.Views
{
	/// <summary>
	/// Interaction logic for ConsoleView.xaml
	/// </summary>
	public partial class ConsoleView : UserControl
	{
		public ConsoleView()
		{
			InitializeComponent();
		}

		private void Contents_TextChanged(object sender, TextChangedEventArgs e)
		{
			Contents.ScrollToEnd();
		}
	}
}
