using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GTGUI.ViewModels.Blocks
{
	public abstract class BlockViewModel : PropertyChangedBase, IComparable<BlockViewModel>
	{
		#region Properties

		[XmlIgnore]
		public static CanvasViewModel Canvas { get; set; }

		private ExecutionState _state;
		[XmlIgnore]
		public ExecutionState State
		{
			get { return _state; }
			set
			{
				_state = value;
				NotifyOfPropertyChange(() => State);
			}
		}

		private int _progress;
		[XmlIgnore]
		public int Progress
		{
			get { return _progress; }
			set
			{
				_progress = value;
				NotifyOfPropertyChange(() => Progress);
			}
		}

		
		private int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				NotifyOfPropertyChange(() => Left);
			}
		}

		private int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				NotifyOfPropertyChange(() => Top);
			}
		}

		private bool _isStartBlock;
		public bool IsStartBlock
		{
			get { return _isStartBlock; }
			set
			{
				_isStartBlock = value;
				NotifyOfPropertyChange(() => IsStartBlock);
			}
		}

		public SerializableDictionary<string, BlockViewModel> Connections { get; set; }

		#endregion

		public BlockViewModel()
		{
			Progress = -1;
			Connections = new SerializableDictionary<string, BlockViewModel>();
		}

		abstract public void Execute(CancellationToken cancellationToken);

		public virtual void Reset()
		{
			State = ExecutionState.NotExecuted;
			if (Connections.ContainsKey("Next"))
			{
				Connections["Next"].Reset();
			}
		}

		public void SetAsStart()
		{
			if (Canvas != null)
			{
				Canvas.SetAsStart(this);
			}
		}

		public void Delete()
		{
			if (Canvas != null)
			{
				Canvas.Delete(this);
			}
		}

		public int CompareTo(BlockViewModel other)
		{
			return this.GetType().Name.CompareTo(other.GetType().Name);
		}

		public void BeginTerminal(MouseButtonEventArgs args)
		{
			var sourceTerminal = (FrameworkElement)args.Source;
			var canvas = GetCanvas((DependencyObject)args.Source);

			if (canvas == null)
			{
				return;
			}

			Canvas.FinishTerminalDragFromBlock(this, sourceTerminal);
		}

		public virtual void EndTerminal(MouseButtonEventArgs args)
		{
			if (args == null)
			{
				return;
			}

			var sourceTerminal = (FrameworkElement)args.Source;
			var canvas = GetCanvas((DependencyObject)args.Source);

			if (canvas == null)
			{
				return;
			}

			Canvas.StartTerminalDragFromBlock(this, sourceTerminal, (FrameworkElement)canvas);
		}

		private static DependencyObject GetCanvas(DependencyObject source)
		{
			while (source != null)
			{
				source = VisualTreeHelper.GetParent(source);
				var uiElement = (FrameworkElement)source;

				if (uiElement is Canvas && uiElement.Name == "BlocksCanvas")
				{
					break;
				}
			}

			return source;
		}

		public virtual BlockViewModel GetNextBlock()
		{
			return Connections.ContainsKey("Next") ? Connections["Next"] : null;
		}
	}
}
