using System.ComponentModel;
using MySoundLib.Windows;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlArtists.xaml
	/// </summary>
	public partial class UserControlArtists
	{
		private readonly ServerConnectionManager _serverConnectionManager;
		private readonly MainWindow _mainWindow;

		public UserControlArtists(ServerConnectionManager connectionManager, MainWindow mainWindow)
		{
			InitializeComponent();
			_serverConnectionManager = connectionManager;
			_mainWindow = mainWindow;

			var songs = _serverConnectionManager.GetDataTable("select artist_id, artist_name, count(s.song_id) as song_count from artists a left join songs s on (a.artist_id = s.artist) group by a.artist_id");

			DataGridArtists.ItemsSource = songs.DefaultView;
			DataGridArtists.Items.SortDescriptions.Add(new SortDescription("artist_name", ListSortDirection.Ascending));
		}
	}
}
