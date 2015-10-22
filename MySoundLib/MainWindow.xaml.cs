using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using MySoundLib.Configuration;
using MySoundLib.Windows;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private ServerConnectionManager _connectionManager;

		public MainWindow()
		{
			InitializeComponent();
			ShowLoginWindow(true);

			var response = _connectionManager.ExecuteScalar("show databases like 'my_sound_lib'");
			if (response == null)
			{
				var r = _connectionManager.ExecuteCommand(Properties.Resources.create_my_sound_lib);
				Debug.WriteLine("Created database and tables. Result: " + r);
			}
		}

		void ShowLoginWindow(bool tryAutoConnect)
		{
			var loginWindow = new LoginWindow(tryAutoConnect);

			loginWindow.ShowDialog();

			if (loginWindow.ResultConnectionManager != null)
			{
				_connectionManager = loginWindow.ResultConnectionManager;
			}
			else
			{
				Debug.WriteLine("MainWindow: Unable to connect to database");
			}
		}

		private void ButtonTest_Click(object sender, RoutedEventArgs e)
		{
			var dT = _connectionManager.GetDataTable("show databases");

			foreach (DataRow dR in dT.Rows)
			{
				Debug.WriteLine(dR[0]);
			}
		}

		private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemTestingWindow_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemDisconnect_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_connectionManager.Disconnect();
			ShowLoginWindow(false);
			Show();
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
		{
		}
	}
}
