using GeocachingToolbox.GeocachingCom;
using GTGUI.ViewModels;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTGUI
{
	public static class ScriptingEnvironment
	{
		public static VariablesViewModel Variables { get; set; }
		private static Session _session;
		private static DateTime _lastTimeUpdated = DateTime.Today.AddDays(-1); // Yesterday.

		static ScriptingEnvironment()
		{
			Reset();
		}

		public static void Reset()
		{
			var engine = new ScriptEngine();
			var gcToolboxAssembly = Assembly.GetAssembly(typeof(GCClient));
			engine.AddReference(gcToolboxAssembly);
			engine.AddReference("System.Core");

			_session = engine.CreateSession();
			_session.Execute("using GeocachingToolbox.GeocachingCom;");
			_session.Execute("using GeocachingToolbox.Opencaching;");
			_session.Execute("using GeocachingToolbox;");
			_session.Execute("using System;");
			_session.Execute("using System.Linq;");
			_session.Execute("using System.Reflection;");
			_session.Execute("using System.Collections.Generic;");

			if (Variables != null)
			{
				UpdateVariableListView();
			}
		}

		public static string EvaluateAsString(string code)
		{
			var evaluatedValue = _session.Execute(code);
			UpdateVariableListView();
			return evaluatedValue.ToString();
		}

		public static object Evaluate(string code)
		{
			var evaluatedValue = _session.Execute(code);
			UpdateVariableListView();
			return evaluatedValue;
		}

		public static void Execute(string code)
		{
			_session.Execute(code);
			UpdateVariableListView();
		}

		private static void UpdateVariableListView()
		{
			if ((DateTime.Now - _lastTimeUpdated).TotalSeconds < 1.0)
			{
				return;
			}

			// http://stackoverflow.com/questions/13056208/how-to-get-declared-variables-and-other-definitions
			var variables = _session.Execute<List<FieldInfo>>(@"
			var __GTPrivate_fields = from type in Assembly.GetExecutingAssembly().DefinedTypes.Reverse()
					from variable in type.GetFields(BindingFlags.Instance | BindingFlags.Public)
					select variable;
			var __GTPrivate_list = new List<FieldInfo>();
			foreach (var info in __GTPrivate_fields)
			{
				__GTPrivate_list.Add(info);
			}
			__GTPrivate_list");

			var notDisplayedVariablesRegex = new Regex("^(<.*>|__GTPrivate_.*)$");

			// To prevent updating until this update finishes.
			_lastTimeUpdated = DateTime.Now.AddDays(1.0);

			App.Current.Dispatcher.Invoke((Action)delegate
			{
				Variables.Clear();

				foreach (var v in variables)
				{
					if (notDisplayedVariablesRegex.IsMatch(v.Name))
					{
						continue;
					}

					var isNull = (bool)_session.Execute(v.Name + " == null");
					Variables.SetVariable(v.Name, isNull ? "[null]" : (string)_session.Execute(v.Name + ".ToString()"));
				}

				_lastTimeUpdated = DateTime.Now;
			});
		}
	}
}
