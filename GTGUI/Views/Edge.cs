using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace GTGUI.Views
{
	public sealed class Edge : UserControl
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Point), typeof(Edge), new FrameworkPropertyMetadata(default(Point)));
		public Point Source { get { return (Point)this.GetValue(SourceProperty); } set { this.SetValue(SourceProperty, value); } }

		public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register("Destination", typeof(Point), typeof(Edge), new FrameworkPropertyMetadata(default(Point)));
		public Point Destination { get { return (Point)this.GetValue(DestinationProperty); } set { this.SetValue(DestinationProperty, value); } }

		private Path _glowPath;
		private Path _mainPath;

		public Edge()
		{
			LineSegment segment = new LineSegment(default(Point), true);
			PathFigure figure = new PathFigure(default(Point), new[] { segment }, false);
			PathGeometry geometry = new PathGeometry(new[] { figure });
			BindingBase sourceBinding = new Binding { Source = this, Path = new PropertyPath(SourceProperty) };
			BindingBase destinationBinding = new Binding { Source = this, Path = new PropertyPath(DestinationProperty) };
			BindingOperations.SetBinding(figure, PathFigure.StartPointProperty, sourceBinding);
			BindingOperations.SetBinding(segment, LineSegment.PointProperty, destinationBinding);


			var canvas = new Canvas();

			_glowPath = new Path
			{
				Data = geometry,
				StrokeThickness = 5,
				Stroke = new SolidColorBrush(Color.FromRgb(65, 125, 130)),
				MinWidth = 1,
				MinHeight = 1,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round,
				Effect = new BlurEffect() { Radius = 7 }
			};

			_mainPath = new Path
			{
				Data = geometry,
				StrokeThickness = 4,
				Stroke = new SolidColorBrush(Color.FromRgb(0, 114, 188)),
				MinWidth = 1,
				MinHeight = 1,
				StrokeStartLineCap = PenLineCap.Round,
				StrokeEndLineCap = PenLineCap.Round
			};

			canvas.Children.Add(_glowPath);
			canvas.Children.Add(_mainPath);

			Content = canvas;
		}

		private bool _selected = false;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (value != _selected)
				{
					_selected = value;

					if (_selected)
					{
						_mainPath.Stroke = new SolidColorBrush(Color.FromRgb(80, 255, 39));
						_glowPath.Stroke = new SolidColorBrush(Color.FromRgb(172, 255, 153));
					}
					else
					{
						_mainPath.Stroke = new SolidColorBrush(Color.FromRgb(0, 114, 188));
						_glowPath.Stroke = new SolidColorBrush(Color.FromRgb(65, 125, 130));
					}
				}
			}
		}
	}
}
