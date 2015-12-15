using GTGUI.Views.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GTGUI
{
	public class BlockConnectionsHelper
	{
		private static Rect AnyNonzeroRect = new Rect(new Size(20.0, 20.0));
		private static Regex RemoveModelRegex = new Regex("^(.*)Model$");

		public static Point GetPositionOfTerminal(Type blockType, string terminalName)
		{
			var blockTypeName = blockType.Name;
			var matches = RemoveModelRegex.Match(blockTypeName);
			var blockViewTypName = matches.Groups[1].Value;

			var blockViewType = Type.GetType("GTGUI.Views.Blocks." + blockViewTypName);
			var parentBlock = (UIElement)Activator.CreateInstance(blockViewType);

			parentBlock.Arrange(AnyNonzeroRect);
			UIElement terminal = (UIElement)LogicalTreeHelper.FindLogicalNode(parentBlock, terminalName);
			Vector offset = VisualTreeHelper.GetOffset(terminal);

			return new Point(offset.X + terminal.RenderSize.Width/2, offset.Y + terminal.RenderSize.Height/2);
		}
	}
}
