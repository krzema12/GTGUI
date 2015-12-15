using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTGUI
{
	/// <summary>
	/// Interaction logic for PanelHeader.xaml
	/// </summary>
	public partial class PanelHeader : UserControl
	{
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(PanelHeader), new PropertyMetadata(default(string)));
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		static PanelHeader()
		{

		}

		public PanelHeader()
		{
			InitializeComponent();
			DataContext = this;
		}
	}
}
