using MediaDevicesApp.View;

using MediaDevicesApp.ViewModel;

using System;
using System.Diagnostics;
using System.Security;
using System.Windows;

namespace MediaDevicesApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	[SecurityCritical]
	private void OnApplicationStartup(object sender, StartupEventArgs e)
	{
		AppDomain.CurrentDomain.UnhandledException += (s, a) =>
		{
			Exception ex = (Exception)a.ExceptionObject;
			Trace.TraceError(ex.ToString());
			_ = MessageBox.Show(ex.ToString(), "Unhandled Error !!!");
		};

		new MainView() { DataContext = new MainViewModel() }.Show();
	}
}
