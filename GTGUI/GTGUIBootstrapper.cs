using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Windows;
using GTGUI.ViewModels;
using System.Windows.Controls;

namespace GTGUI
{
	public class GTGUIBootstrapper : BootstrapperBase
	{
		private CompositionContainer _container;

		public GTGUIBootstrapper()
		{
			Initialize();
		}

		protected override void Configure()
		{
			_container = new CompositionContainer(
				new AggregateCatalog(
					AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
					)
				);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue(_container);

			_container.Compose(batch);

			ConventionManager.AddElementConvention<PasswordBox>(
				PasswordBoxHelper.BoundPasswordProperty, "Password", "PasswordChanged");
		}

		/// <summary>
		/// Get list of searchable assemblies
		/// </summary>
		/// <returns>List of searchable assemblies</returns>
		protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
		{
			return new[] { System.Reflection.Assembly.GetExecutingAssembly() };
		}

		/// <summary>
		/// Get instance of an object contained within IoC container.
		/// </summary>
		/// <param name="serviceType">Type pf service</param>
		/// <param name="key">Key</param>
		/// <returns>Object instance</returns>
		protected override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = _container.GetExportedValues<object>(contract);

			if (exports.Count() > 0)
			{
				return exports.First();
			}

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			DisplayRootViewFor<MainViewModel>();
		}
	}
}
