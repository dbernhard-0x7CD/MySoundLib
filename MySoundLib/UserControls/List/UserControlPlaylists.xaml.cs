using MySoundLib.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
