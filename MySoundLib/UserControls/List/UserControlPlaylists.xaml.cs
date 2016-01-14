using MySoundLib.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MySoundLib.UserControls.List
{
    /// <summary>
    /// Interaction logic for UserControlPlaylists.xaml
    /// </summary>
    public partial class UserControlPlaylists : UserControl
    {
        private MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }
        private int _playlistId;

        public UserControlPlaylists(MainWindow mainWindow, int playlistId)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _playlistId = playlistId;

            var information = _connectionManager.GetDataTable(CommandFactory.GetPlaylistInformation(playlistId)).Rows[0];

            LabelHeaderTitle.Content = "Playlist " + information["name"];
            LabelPlaylistDescription.Content = "Description: " + information["description"];

            var songs = _connectionManager.GetDataTable(CommandFactory.GetSongsForPlaylist(playlistId));

            DataGridSongs.ItemsSource = songs.DefaultView;
        }

        private void DataGridRowPreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var row = Create.UserControlCreatePlaylist.GetVisualParentByType((FrameworkElement)e.OriginalSource, typeof(DataGridRow)) as DataGridRow;
                if (row != null)
                {
                    row.IsSelected = !row.IsSelected;
                    e.Handled = true;
                }
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = _connectionManager.ExecuteCommand(CommandFactory.DeletePlaylist(_playlistId));

            if (result != 1)
            {
                Debug.WriteLine("Did not delete playlist");
            }

            _mainWindow.GridContent.Children.Clear();
            _mainWindow.UpdatePlaylists();
        }
    }
}
