using System.Data;
using System.Windows;
using MySoundLib.Configuration;

namespace MySoundLib.Windows
{
	/// <summary>
	/// Interaction logic for TestWindow.xaml
	/// </summary>
	public partial class TestWindow
	{
		private readonly ServerConnectionManager _connectionManager;

		public TestWindow(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_connectionManager = connectionManager;
			LabelAmountBytesForLocalSongs.Content = Settings.GetSizeOfLocalSongs();
        }

		private MainWindow MainWindow => Owner as MainWindow;

		private void ButtonExecuteCommand_OnClick(object sender, RoutedEventArgs e)
		{
			var result = _connectionManager.ExecuteCommand(TextBoxCommand.Text);

			TextBoxResult.Text = result.ToString();
		}

		private void ButtonGetDataTable_OnClick(object sender, RoutedEventArgs e)
		{
			var result = _connectionManager.GetDataTable(TextBoxCommand.Text);
			TextBoxResult.Clear();

			foreach (var x in result.Columns)
			{
				TextBoxResult.Text += x + "\t\t";
			}

			TextBoxResult.Text += "\n";

			foreach (DataRow x in result.Rows)
			{
				foreach (var cell in x.ItemArray)
				{
					TextBoxResult.Text += cell + "\t";
				}
				TextBoxResult.Text += "\n";
			}
		}

		private void ButonExecuteScalar_OnClick(object sender, RoutedEventArgs e)
		{
			var result = _connectionManager.ExecuteScalar(TextBoxCommand.Text);

			TextBoxResult.Text = result.ToString();
		}

		private void ButtonResetDatabase_OnClick(object sender, RoutedEventArgs e)
		{
			var rDrop = _connectionManager.ExecuteCommand("drop database if exists my_sound_lib");
			var rCreate = _connectionManager.ExecuteCommand(Properties.Resources.create_my_sound_lib);

			if (rDrop == -1)
			{
				MessageBox.Show("Failed to drop database");
			}
			if (rCreate == -1)
			{
				MessageBox.Show("Failed to create database");
			}
		}

		private void ButtonStopAutoConnect_OnClick(object sender, RoutedEventArgs e)
		{
			Settings.RemoveProperty(Property.AutoConnect);
			Settings.SaveConfig();
		}

		private void ButtonClearSongs_Click(object sender, RoutedEventArgs e)
		{
			Settings.DeleteLocalSongs();
			LabelAmountBytesForLocalSongs.Content = Settings.GetSizeOfLocalSongs();
		}
	}
}
