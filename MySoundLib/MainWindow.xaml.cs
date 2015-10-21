using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySoundLib.Windows;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
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
	}
}
