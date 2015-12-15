using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTGUI
{
	[AttributeUsage(System.AttributeTargets.Class)]
	public class BlockGroup : Attribute
	{
		public string GroupName { get; set; }

		public BlockGroup(string groupName)
		{
			this.GroupName = groupName;
		}
	}
}
