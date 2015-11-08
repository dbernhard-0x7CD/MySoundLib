using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySoundLib.Windows;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlSongs.xaml
	/// </summary>
	public partial class UserControlSongs
	{
		private readonly ServerConnectionManager _serverConnectionManager;
		private readonly MainWindow _mainWindow;
		private DataGridRow _currentlyPlayingDataGridRow;
		private Brush _lastSongBackground; 

		public UserControlSongs(ServerConnectionManager connectionManager, MainWindow mainWindow)
		{
			InitializeComponent();
			_serverConnectionManager = connectionManager;
			_mainWindow = mainWindow;

			var songs = _serverConnectionManager.GetDataTable("select song_id, song_title, artist_name, album_name, genre_name, length from songs s left join artists a on (a.artist_id = s.artist) left join genres g on (s.genre = g.genre_id) left join albums al on (al.album_id = s.album)");

			DataGridSongs.ItemsSource = songs.DefaultView;
		}

		private void ButtonAddNewSong_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.ListBoxCategory.UnselectAll();

			_mainWindow.GridContent.Children.Clear();
			_mainWindow.GridContent.Children.Add(new UserControlUploadSong(_serverConnectionManager, _mainWindow));
		}

		private void DataGridSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			
		}

		private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			var dataGridRow = e.Source as DataGridRow;
			var dataRowView = dataGridRow?.DataContext as DataRowView;

			if (dataRowView != null)
			{
				if (_currentlyPlayingDataGridRow != null) // there was a song playing before this one
				{
					ResetBackgroundFromRecentSong();
				}
				DataGridSongs.SelectedValue = dataGridRow;
				_lastSongBackground = dataGridRow.Background;
				dataGridRow.Background = new SolidColorBrush(Colors.BurlyWood);
				_currentlyPlayingDataGridRow = dataGridRow;

				_mainWindow.PlaySong(int.Parse(dataRowView[0].ToString()));
			}
		}

		public void ResetBackgroundFromRecentSong()
		{
			_currentlyPlayingDataGridRow.Background = _lastSongBackground;
		}
	}
}
