using MySoundLib.Windows;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlAlbums.xaml
	/// </summary>
	public partial class UserControlAlbums
	{
		private readonly ServerConnectionManager _serverConnectionManager;
		private readonly MainWindow _mainWindow;

		public UserControlAlbums(ServerConnectionManager connectionManager, MainWindow mainWindow)
		{
			InitializeComponent();
			_serverConnectionManager = connectionManager;
			_mainWindow = mainWindow;

			var songs = _serverConnectionManager.GetDataTable(CommandFactory.GetAlbums());

			DataGridAlbums.ItemsSource = songs.DefaultView;
		}
	}
}
