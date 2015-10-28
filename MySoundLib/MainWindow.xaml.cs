using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
			if (!ShowLoginWindow(true))
			{
				Close();
				return;
			}

			HideCurrentSong();
		}

		bool ShowLoginWindow(bool tryAutoConnect)
		{
			var loginWindow = new LoginWindow(tryAutoConnect);

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
			if (ShowLoginWindow(false))
				Show();
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
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
	}
}
