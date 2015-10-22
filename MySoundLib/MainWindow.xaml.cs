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
			_connectionManager.test();
		}

		private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemTestingWindow_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemDisconnect_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
		}

		private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
		{
		}
	}
}
