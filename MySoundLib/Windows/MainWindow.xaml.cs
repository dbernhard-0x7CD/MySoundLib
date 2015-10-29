using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySoundLib.Configuration;

namespace MySoundLib.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private ServerConnectionManager _connectionManager;

		public MainWindow()
		{
			Settings.LoadSettings();
			if (Settings.Contains(Property.AutoConnect) && Settings.Contains(Property.LastServer) &&
			    Settings.Contains(Property.LastUser))
			{
				_connectionManager = new ServerConnectionManager();
				_connectionManager.Connect(Settings.GetValue(Property.LastServer), Settings.GetValue(Property.LastUser),
					LoginWindow.GetDecryptedPassword(), "my_sound_lib");
			}
			else
			{
				if (!ShowLoginWindow())
				{
					Close();
					return;
				}
			}

			InitializeComponent();
			
			var songs = _connectionManager.GetDataTable("select * from songs");

			if (songs.Rows.Count == 0)
			{
				GridContent.Children.Add(new UserControlUploadSong(_connectionManager));
			}
			else
			{
				GridContent.Children.Add(new UserControlSongs(_connectionManager));
			}

			HideCurrentSong();
		}

		bool ShowLoginWindow()
		{
			var loginWindow = new LoginWindow();
			loginWindow.ShowDialog();

			if (loginWindow.ResultConnectionManager == null) return false;
			_connectionManager = loginWindow.ResultConnectionManager;
			return true;
		}

		private void MenuItemTestingWindow_OnClick(object sender, RoutedEventArgs e)
		{
			var testWindow = new TestWindow(_connectionManager) {Owner =  this};

			testWindow.Show();
		}

		private void MenuItemDisconnect_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_connectionManager.Disconnect();
			if (ShowLoginWindow())
				Show();
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(_connectionManager) {Owner = this};

			aboutWindow.ShowDialog();
		}

		public void HideCurrentSong()
		{
			StackPanelCurrentSongInformation.Visibility = Visibility.Collapsed;
			GridSplitterHor.Visibility = Visibility.Collapsed;
			Grid.SetRowSpan(GridContent, 3);
		}

		public void ShowCurrentSong()
		{
			StackPanelCurrentSongInformation.Visibility = Visibility.Visible;
			GridSplitterHor.Visibility = Visibility.Visible;
			Grid.SetRowSpan(GridContent, 1);
		}

		private void ListBoxItemSongs_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			GridContent.Children.Add(new UserControlSongs(_connectionManager));
		}

		private void ListBoxItemAlbums_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
		}

		private void ListBoxItemArtists_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			var userControlArtists = new UserControlArtists();
			GridContent.Children.Add(userControlArtists);

			var artists = _connectionManager.GetDataTable("select artist_name from artists");

			foreach (DataRow row in artists.Rows)
			{
				userControlArtists.ListBoxArtists.Items.Add(row[0]);
			}
		}

		private void ListBoxItemGenres_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			var userControlGenres = new UserControlGenres();
			GridContent.Children.Add(userControlGenres);

			var genres = _connectionManager.GetDataTable("select genre_name from genres");

			foreach (DataRow row in genres.Rows)
			{
				userControlGenres.ListBoxGenres.Items.Add(row[0]);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_connectionManager.Disconnect();
		}
	}
}
