using System;
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
			ShowLoginWindow(true);

			HideCurrentSong();
		}

		bool ShowLoginWindow(bool tryAutoConnect)
		{
			var loginWindow = new LoginWindow(tryAutoConnect);

			loginWindow.ShowDialog();

			if (loginWindow.ResultConnectionManager != null)
			{
				_connectionManager = loginWindow.ResultConnectionManager;
				return true;
			}
			Close();
			return false;
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
			Grid.SetRowSpan(GridContent, 2);
		}

		public void ShowCurrentSong()
		{
			StackPanelCurrentSongInformation.Visibility = Visibility.Visible;
			Grid.SetRowSpan(GridContent, 1);
		}

		private void ListBoxItemSongs_OnSelected(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ListBoxItemAlbums_OnSelected(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ListBoxItemArtists_OnSelected(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ListBoxItemGenres_OnSelected(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
