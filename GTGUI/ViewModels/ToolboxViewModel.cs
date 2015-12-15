using Caliburn.Micro;
using GongSolutions.Wpf.DragDrop;
using GTGUI.ViewModels.Blocks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace GTGUI.ViewModels
{
	[Export(typeof(ToolboxViewModel))]
	public class ToolboxViewModel : PropertyChangedBase, IDragSource
	{
		//private ObservableCollection<BlockViewModel> _listOfBlocks = new ObservableCollection<BlockViewModel>();
		//public ObservableCollection<BlockViewModel> ListOfBlocks
		//{
		//	get { return _listOfBlocks; }
		//	set
		//	{
		//		_listOfBlocks = value;
		//		NotifyOfPropertyChange(() => ListOfBlocks);
		//	}
		//}

		private Dictionary<string, ObservableCollection<BlockViewModel>> _blocksDividedByGroup = new Dictionary<string, ObservableCollection<BlockViewModel>>();

		private ObservableCollection<BlockViewModel> _allBlocks = new ObservableCollection<BlockViewModel>();
		public ObservableCollection<BlockViewModel> AllBlocks
		{
			get { return _allBlocks; }
			set
			{
				_allBlocks = value;
				NotifyOfPropertyChange(() => AllBlocks);
			}
		}

		private ObservableCollection<BlockViewModel> _geocachingSpecificBlocks = new ObservableCollection<BlockViewModel>();
		public ObservableCollection<BlockViewModel> GeocachingSpecificBlocks
		{
			get { return _geocachingSpecificBlocks; }
			set
			{
				_geocachingSpecificBlocks = value;
				NotifyOfPropertyChange(() => GeocachingSpecificBlocks);
			}
		}

		private ObservableCollection<BlockViewModel> _flowBlocks = new ObservableCollection<BlockViewModel>();
		public ObservableCollection<BlockViewModel> FlowBlocks
		{
			get { return _flowBlocks; }
			set
			{
				_flowBlocks = value;
				NotifyOfPropertyChange(() => FlowBlocks);
			}
		}

		private ObservableCollection<BlockViewModel> _generalBlocks = new ObservableCollection<BlockViewModel>();
		public ObservableCollection<BlockViewModel> GeneralBlocks
		{
			get { return _generalBlocks; }
			set
			{
				_generalBlocks = value;
				NotifyOfPropertyChange(() => GeneralBlocks);
			}
		}

		public ToolboxViewModel()
		{
			var allAvailableBlocks = ReflectiveEnumerator.GetEnumerableOfType<BlockViewModel>();

			foreach (var block in allAvailableBlocks)
			{
				Attribute[] attributes = Attribute.GetCustomAttributes(block.GetType());
				var pos = Array.FindIndex(attributes, a => a.GetType() == typeof(BlockGroup));

				AddToGroup(((BlockGroup)attributes[pos]).GroupName, block);
				AddToGroup("All blocks", block);
			}

			// TODO: make it more generic, so that the groups are created dynamically
			AllBlocks = _blocksDividedByGroup["All blocks"];
			GeocachingSpecificBlocks = _blocksDividedByGroup["Geocaching-specific"];
			FlowBlocks = _blocksDividedByGroup["Flow"];
			GeneralBlocks = _blocksDividedByGroup["General"];
		}

		private void AddToGroup(string groupName, BlockViewModel block)
		{
			if (!_blocksDividedByGroup.ContainsKey(groupName))
			{
				_blocksDividedByGroup.Add(groupName, new ObservableCollection<BlockViewModel>());
			}

			_blocksDividedByGroup[groupName].Add(block);
		}

		public bool CanStartDrag(IDragInfo dragInfo)
		{
			return true;
		}

		public void StartDrag(IDragInfo dragInfo)
		{
			DragDrop.DefaultDragHandler.StartDrag(dragInfo);
		}

		public void DragCancelled()
		{

		}

		public void Dropped(IDropInfo dropInfo)
		{

		}
	}
}
