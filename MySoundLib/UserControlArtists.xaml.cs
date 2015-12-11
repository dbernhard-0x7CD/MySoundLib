using System.ComponentModel;
using MySoundLib.Windows;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlArtists.xaml
    /// </summary>
    public partial class UserControlArtists
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _serverConnectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }

        public UserControlArtists(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var songs = _serverConnectionManager.GetDataTable("select artist_id, artist_name, count(s.song_id) as song_count from artists a left join songs s on (a.artist_id = s.artist) group by a.artist_id");

            DataGridArtists.ItemsSource = songs.DefaultView;
            DataGridArtists.Items.SortDescriptions.Add(new SortDescription("artist_name", ListSortDirection.Ascending));
        }

        private void ButtonAddArtist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.GridContent.Children.Add(new UserControlUploadArtist(_mainWindow));
        }

        private void DataGridArtists_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ButtonRenameArtist_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ButtonDeleteArtist_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
