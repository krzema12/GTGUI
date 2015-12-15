using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace GTGUI.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : Window
	{
		public MainView()
		{
			InitializeComponent();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.F8)
			{
				return;
			}

			string pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string fileName = "GTToolboxScreenshot.png";
			string fullPath = System.IO.Path.Combine(pathToDesktop, fileName);

			CreateBitmapFromVisual(this as Visual, fullPath, 600.0);

			Debug.WriteLine(string.Format("Screen shot saved to {0}", fullPath));
		}

		private static void CreateBitmapFromVisual(Visual target, string fileName, double dpi)
		{
			if (target == null || string.IsNullOrEmpty(fileName))
			{
				return;
			}

			int width = 0, height = 0;

			// Determine size (if the visual is a UIElement)
			UIElement uiElement = target as UIElement;

			if (uiElement != null)
			{
				width = (int)uiElement.RenderSize.Width;
				height = (int)uiElement.RenderSize.Height;
			}

			// Render
			var rtb = new RenderTargetBitmap((Int32)(width * dpi / 96.0),
				(Int32)(height * dpi / 96.0),
				dpi, dpi,
				PixelFormats.Pbgra32);

			rtb.Render(target);

			// Encode and save to PNG file
			var enc = new PngBitmapEncoder();
			enc.Frames.Add(BitmapFrame.Create(rtb));

			using (var stm = File.Create(fileName))
			{
				enc.Save(stm);
			}
		}
	}
}
