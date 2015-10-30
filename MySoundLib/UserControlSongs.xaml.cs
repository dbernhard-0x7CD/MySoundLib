using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlSongs.xaml
	/// </summary>
	public partial class UserControlSongs
	{
		private readonly ServerConnectionManager _serverConnectionManager;

		public UserControlSongs(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_serverConnectionManager = connectionManager;

			var songs = _serverConnectionManager.GetDataTable("select song_id, song_title, artist_name, album_name, genre_name, length from songs s left join artists a on (a.artist_id = s.artist) left join genres g on (s.genre = g.genre_id) left join albums al on (al.album_id = s.album)");

			ListViewSongs.ItemsSource = songs.DefaultView;
		}

		private void ButtonAddNewSong_Click(object sender, RoutedEventArgs e)
		{
			var uiElementCollection = ((Grid) Parent).Children;

			uiElementCollection.Clear();
			uiElementCollection.Add(new UserControlUploadSong(_serverConnectionManager));
		}
	}
}
