using GTGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTGUI
{
	public static class GTConsole
	{
		public static ConsoleViewModel Console { get; set; }

		public static void WriteLine(string text)
		{
			Console.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + text);
		}
	}
}
