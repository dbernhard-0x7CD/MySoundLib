using System;
using System.Globalization;
using System.Windows;

namespace MySoundLib.Windows
{
	/// <summary>
	/// Interaction logic for AboutWindow.xaml
	/// </summary>
	public partial class AboutWindow
	{
		private readonly ServerConnectionManager _connectionManager;
		
		public AboutWindow(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_connectionManager = connectionManager;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var creationTime = (DateTime)_connectionManager.ExecuteScalar("SELECT create_time FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'my_sound_lib'");

			LabelDatabaseCreateTime.Content = creationTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}
	}
}
