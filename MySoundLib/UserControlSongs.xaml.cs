using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlSongs.xaml
	/// </summary>
	public partial class UserControlSongs : UserControl
	{
		private readonly ServerConnectionManager _serverConnectionManager;

		public UserControlSongs(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_serverConnectionManager = connectionManager;

			var songs = _serverConnectionManager.GetDataTable("select * from songs s left join artists a on (a.artist_id = s.artist)");

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
