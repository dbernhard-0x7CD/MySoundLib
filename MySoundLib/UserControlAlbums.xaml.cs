using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlAlbums.xaml
	/// </summary>
	public partial class UserControlAlbums
	{
		private readonly ServerConnectionManager _serverConnectionManager;

		public UserControlAlbums(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
            _serverConnectionManager = connectionManager;

            var albums = _serverConnectionManager.GetDataTable(CommandFactory.GetAlbums());

			DataGridAlbums.ItemsSource = albums.DefaultView;
		}

		private void ButtonAddNewAlbum_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonRenameAlbum_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonDeleteAlbum_Click(object sender, RoutedEventArgs e)
		{

		}

		private void DataGridAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
