using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
			var uiElementCollection = ((Grid) Parent).Children;

			_mainWindow.ListBoxCategory.UnselectAll();

			uiElementCollection.Clear();
			uiElementCollection.Add(new UserControlUploadSong(_serverConnectionManager));
		}

		private void DataGridSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var dataRowView = DataGridSongs.SelectedItems[0] as DataRowView;
			if (dataRowView != null)
			{
				Debug.WriteLine(dataRowView[0]);
				_mainWindow.PlaySong(int.Parse(dataRowView[0].ToString()));
			}
		}
	}
}
