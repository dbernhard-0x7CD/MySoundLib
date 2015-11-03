using System;
using System.Diagnostics;
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
			var dataTable =  _connectionManager.GetDataTable("SELECT create_time FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'my_sound_lib' group by table_schema");

			DateTime creationTime;

			if (DateTime.TryParse(dataTable.Rows[0][0].ToString(), out creationTime))
			{
				LabelDatabaseCreateTime.Content = creationTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}
			else
			{
				LabelDatabaseCreateTime.Content = "Nobody knows";
			}
		}
	}
}
