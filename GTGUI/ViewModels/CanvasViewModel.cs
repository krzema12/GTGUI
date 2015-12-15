using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using GTGUI.ViewModels.Blocks;
using GTGUI.Views;
using GTGUI.Views.Blocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace GTGUI.ViewModels
{
	[Export(typeof(CanvasViewModel))]
	public class CanvasViewModel : PropertyChangedBase, IDropTarget, IDragSource
	{
		private BlockViewModel _startBlock;
		private BlockViewModel _currentBlock;
		private Task _currentBlockTask;
		private CancellationTokenSource _currentBlockCancellationToken;
		private string _currentConnectionName;
		private bool _isDraggingExistingBlock;

		private BlockViewModel _currentConnectionStartBlock;
		private Edge _newEdge;
		private FrameworkElement _canvas;

		private Point _startMovingPos;

		private ObservableCollection<BlockViewModel> _listOfBlocks = new ObservableCollection<BlockViewModel>();
		public ObservableCollection<BlockViewModel> ListOfBlocks
		{
			get { return _listOfBlocks; }
			set
			{
				_listOfBlocks = value;
				NotifyOfPropertyChange(() => ListOfBlocks);
			}
		}

		private ObservableCollection<Edge> _connections = new ObservableCollection<Edge>();
		public ObservableCollection<Edge> Connections
		{
			get { return _connections; }
			set
			{
				_connections = value;
				NotifyOfPropertyChange(() => Connections);
			}
		}

		public Stack<BlockViewModel> ExecutionStack { get; set; }
		public bool IsExecuting { get { return _currentBlock != null; } }

		private Dictionary<Tuple<BlockViewModel, string, BlockViewModel>, Edge> _blocksToConnections
			= new Dictionary<Tuple<BlockViewModel,string,BlockViewModel>,Edge>();

		// Used only while moving interconnected things around.
		private List<Edge> _edgesMovingStarts = new List<Edge>();
		private List<Edge> _edgesMovingEnds = new List<Edge>();


		public CanvasViewModel()
		{
			BlockViewModel.Canvas = this;
			ExecutionStack = new Stack<BlockViewModel>();
			ExecutionStack.Push(null);
		}

		public void SetAsStart(BlockViewModel block)
		{
			foreach (var existingBlock in ListOfBlocks)
			{
				existingBlock.IsStartBlock = false;
			}

			_startBlock = block;
			block.IsStartBlock = true;
		}

		public void Delete(BlockViewModel blockViewModel)
		{
			var wasStartingBlock = blockViewModel.IsStartBlock;

			// Remove references to this block from a block that points it as a next one.
			List<Tuple<BlockViewModel, String>> referencesToDelete = new List<Tuple<BlockViewModel, string>>();

			foreach (var block in ListOfBlocks)
			{
				foreach (var connection in block.Connections)
				{
					if (connection.Value == blockViewModel)
					{
						referencesToDelete.Add(new Tuple<BlockViewModel, string>(block, connection.Key));
					}
				}
			}

			foreach (var connectionToDelete in referencesToDelete)
			{
				connectionToDelete.Item1.Connections.Remove(connectionToDelete.Item2);
			}

			// Remove edges (visual connections).
			List<Edge> edgesToDelete = new List<Edge>();

			foreach (var connection in _blocksToConnections)
			{
				if (connection.Key.Item1 == blockViewModel || connection.Key.Item3 == blockViewModel)
				{
					edgesToDelete.Add(connection.Value);
				}
			}

			foreach (var edgeToDelete in edgesToDelete)
			{
				Connections.Remove(edgeToDelete);
			}

			ListOfBlocks.Remove(blockViewModel);
		}

		private Task RunCurrentBlock(CancellationTokenSource cancellationToken)
		{
			return Task.Factory.StartNew(() =>
			{
				_currentBlock.State = ExecutionState.Current;

				try
				{
					_currentBlock.Execute(cancellationToken.Token);
					_currentBlock.State = ExecutionState.Executed;
				}
				catch (Exception e)
				{
					_currentBlock.State = ExecutionState.Error;
					_currentBlock = null;
					GTConsole.WriteLine(e.Message);
				}
			});
		}

		public void StartExecuting()
		{
			_currentBlock = _startBlock;
			SetAllBlockStatesAsNotExecuted();
			ScriptingEnvironment.Reset();
			Execute();
		}

		private void SetAllBlockStatesAsNotExecuted()
		{
			_currentBlock.Reset();
		}

		public void StopExecuting()
		{
			if (_currentBlockCancellationToken != null)
			{
				_currentBlockCancellationToken.Cancel();
			}
		}

		private void Execute()
		{
			if (_currentBlock == null)
			{
				return;
			}

			_currentBlockCancellationToken = new CancellationTokenSource();
			_currentBlockTask = RunCurrentBlock(_currentBlockCancellationToken);
			_currentBlockTask.ContinueWith(task =>
			{
				SetNextBlockToRun();
				Execute();
			}, _currentBlockCancellationToken.Token);
		}

		private void SetNextBlockToRun()
		{
			if (_currentBlock != null)
			{
				_currentBlock = _currentBlock.GetNextBlock() ?? ExecutionStack.Peek();
			}
		}

		public void Drop(IDropInfo dropInfo)
		{
			var droppedBlock = (BlockViewModel)dropInfo.Data;
			BlockViewModel newBlock = droppedBlock;

			if (_isDraggingExistingBlock == false)
			{
				newBlock = (BlockViewModel)Activator.CreateInstance(droppedBlock.GetType(), null);
			}

			newBlock.Left = (int)(dropInfo.DropPosition.X - dropInfo.DragInfo.PositionInDraggedItem.X);
			newBlock.Top = (int)(dropInfo.DropPosition.Y - dropInfo.DragInfo.PositionInDraggedItem.Y);

			if (_isDraggingExistingBlock == false)
			{
				AddNewBlock(newBlock);
			}

			_isDraggingExistingBlock = false;
			DragDrop.DefaultDragHandler.Dropped(dropInfo);
		}

		private void AddNewBlock(BlockViewModel newBlock)
		{
			ListOfBlocks.Add(newBlock);
			//BlockViewModel.Canvas = this;

			if (ListOfBlocks.Count == 1)
			{
				SetAsStart(newBlock);
			}
		}

		public bool CanStartDrag(IDragInfo dragInfo)
		{
			return true;
		}

		public void DragCancelled()
		{
			_isDraggingExistingBlock = false;
		}

		public void StartDrag(IDragInfo dragInfo)
		{
			_isDraggingExistingBlock = true;
			DragDrop.DefaultDragHandler.StartDrag(dragInfo);
			var block = (BlockViewModel)dragInfo.Data;

			_startMovingPos = dragInfo.DragStartPosition;

			_edgesMovingStarts.Clear();

			foreach (var edge in _blocksToConnections)
			{
				if (edge.Key.Item1 == block)
				{
					_edgesMovingStarts.Add(edge.Value);
				}
			}

			_edgesMovingEnds.Clear();

			foreach (var edge in _blocksToConnections)
			{
				if (edge.Key.Item3 == block)
				{
					_edgesMovingEnds.Add(edge.Value);
				}
			}
		}

		public void DragOver(IDropInfo dropInfo)
		{
			dropInfo.Effects = DragDropEffects.Move;

			if (_isDraggingExistingBlock)
			{
				var droppedBlock = (BlockViewModel)dropInfo.Data;
				droppedBlock.Left = (int)(dropInfo.DropPosition.X - dropInfo.DragInfo.PositionInDraggedItem.X);
				droppedBlock.Top = (int)(dropInfo.DropPosition.Y - dropInfo.DragInfo.PositionInDraggedItem.Y);

				var delta = dropInfo.DropPosition - _startMovingPos;
				_startMovingPos = dropInfo.DropPosition;

				foreach (var edge in _edgesMovingStarts)
				{
					edge.Source += delta;
				}

				foreach (var edge in _edgesMovingEnds)
				{
					edge.Destination += delta;
				}
			}
		}

		public void Dropped(IDropInfo dropInfo)
		{

		}

		public void MouseMove(MouseEventArgs args)
		{
			if (_newEdge == null)
			{
				return;
			}

			var position = args.GetPosition((UIElement)args.Source);

			if (position == null)
			{
				return;
			}

			_newEdge.Destination = position;
		}


		private Point GetAnchorCenterPosition(FrameworkElement anchor)
		{
			var position = anchor.TranslatePoint(new Point(0, 0), _canvas);
			position.X += anchor.Width / 2;
			position.Y += anchor.Height / 2;
			return position;
		}

		public void StartTerminalDragFromBlock(BlockViewModel blockViewModel, FrameworkElement anchor, FrameworkElement parent)
		{
			_canvas = parent ?? _canvas;

			var position = GetAnchorCenterPosition(anchor);

			_currentConnectionStartBlock = blockViewModel;
			_currentConnectionName = anchor.Name;
			_newEdge = new Edge { Source = position, Destination = position };
			Connections.Add(_newEdge);
		}

		public void FinishTerminalDragFromBlock(BlockViewModel blockViewModel, FrameworkElement anchor)
		{
			if (blockViewModel == _currentConnectionStartBlock)
			{
				Connections.Remove(_newEdge);
			}
			else if (_currentConnectionName != null)
			{
				_blocksToConnections.Add(new Tuple<BlockViewModel, string, BlockViewModel>(
					_currentConnectionStartBlock, _currentConnectionName, blockViewModel), _newEdge);

				_currentConnectionStartBlock.Connections[_currentConnectionName] = blockViewModel;

				var position = GetAnchorCenterPosition(anchor);
				_newEdge.Destination = position;
			}

			_newEdge = null;
			_currentConnectionName = null;
		}

		public void Serialize(string path)
		{
			var blocksAllTypes = ReflectiveEnumerator.GetEnumerableOfType<BlockViewModel>();
			var types = new List<BlockViewModel>(blocksAllTypes).ConvertAll(b => b.GetType()).ToArray();

			List<BlockViewModel> parentBlocks = FindParentBlocks();

			GeocachingToolboxWorkspace workspace = new GeocachingToolboxWorkspace();
			workspace.Blocks = new List<BlockViewModel>();

			foreach (var block in parentBlocks)
			{
				workspace.Blocks.Add(block);
			}

			// TODO: use DataContractSerializer or anything else that serializes a given object only once.
			// Currently, if any block is referenced from more than one block, it's serialized multiple
			// times and during deserialization, it's created multiple times. 
			XmlSerializer x = new XmlSerializer(typeof(GeocachingToolboxWorkspace), types);
			using (TextWriter writer = new StreamWriter(path))
			{
				x.Serialize(writer, workspace);
			}
		}

		public List<BlockViewModel> FindParentBlocks()
		{
			// Temporary, messy solution: we must find "parent" block, i.e. those that don't have any previous ones
			// Effectively, we're looking for all connected components of the graph.
			Dictionary<BlockViewModel, bool> hasPreviousBlock = new Dictionary<BlockViewModel, bool>();

			foreach (var block in ListOfBlocks)
			{
				hasPreviousBlock[block] = false;
			}

			foreach (var block in ListOfBlocks)
			{
				foreach (var next in block.Connections)
				{
					hasPreviousBlock[next.Value] = true;
				}
			}

			List<BlockViewModel> parentBlocks = new List<BlockViewModel>();

			foreach (var block in hasPreviousBlock)
			{
				if (block.Value == false)
				{
					parentBlocks.Add(block.Key);
				}
			}

			return parentBlocks;
		}

		public void Deserialize(string path)
		{
			var blocksAllTypes = ReflectiveEnumerator.GetEnumerableOfType<BlockViewModel>();
			var types = new List<BlockViewModel>(blocksAllTypes).ConvertAll(b => b.GetType()).ToArray();

			StreamReader reader = new StreamReader(path);

			XmlSerializer x = new XmlSerializer(typeof(GeocachingToolboxWorkspace), types);
			var workspace = (GeocachingToolboxWorkspace)x.Deserialize(reader);
			reader.Close();

			ListOfBlocks.Clear();
			Connections.Clear();
			_blocksToConnections.Clear();

			foreach (var block in workspace.Blocks)
			{
				AddRecursively(block);
			}
		}

		private void AddRecursively(BlockViewModel block)
		{
			ListOfBlocks.Add(block);

			if (block.IsStartBlock)
			{
				_startBlock = block;
			}

			foreach (var b in block.Connections)
			{
				var startPos = BlockConnectionsHelper.GetPositionOfTerminal(block.GetType(), b.Key);
				var endPos = BlockConnectionsHelper.GetPositionOfTerminal(b.Value.GetType(), "Previous");

				startPos.Offset(block.Left, block.Top);
				endPos.Offset(b.Value.Left, b.Value.Top);

				var newEdge = new Edge()
				{
					Source = startPos,
					Destination = endPos
				};

				Connections.Add(newEdge);
				_blocksToConnections.Add(new Tuple<BlockViewModel, string, BlockViewModel>(block, b.Key, b.Value), newEdge);
				AddRecursively(b.Value);
			}
		}

		public class GeocachingToolboxWorkspace
		{
			public List<BlockViewModel> Blocks { get; set; }
		}
	}
}
