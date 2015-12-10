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
        private static DataGridRow _currentlyPlayingDataGridRow;
        private Brush _lastSongBackground;
        /// <summary>
        /// Id from the song
        /// </summary>
        private int _recentlyAddedSong;

        public UserControlSongs(ServerConnectionManager connectionManager, MainWindow mainWindow)
        {
            InitializeComponent();
            _serverConnectionManager = connectionManager;
            _mainWindow = mainWindow;

            var songs = _serverConnectionManager.GetDataTable("select song_id, song_title, artist_name, album_name, genre_name, length from songs s left join artists a on (a.artist_id = s.artist) left join genres g on (s.genre = g.genre_id) left join albums al on (al.album_id = s.album) order by song_title");

            DataGridSongs.ItemsSource = songs.DefaultView;
            DataGridSongs.SelectedIndex = -1;

            MarkCurrentSong();
        }

        public UserControlSongs(ServerConnectionManager connectionManager, MainWindow mainWindow, int song_id) : this(connectionManager, mainWindow)
        {
            _recentlyAddedSong = song_id;
        }

        private void ButtonAddNewSong_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ListBoxCategory.UnselectAll();

            _mainWindow.GridContent.Children.Clear();
            _mainWindow.GridContent.Children.Add(new UserControlUploadSong(_mainWindow));
        }

        private void DataGridSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridSongs.SelectedIndex == -1)
            {
                ButtonDeleteSong.Visibility = Visibility.Collapsed;
            } else
            {
                ButtonDeleteSong.Visibility = Visibility.Visible;
            }
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

                _currentlyPlayingDataGridRow = dataGridRow;
                MarkCurrentSong();

                _mainWindow.PlaySong(int.Parse(dataRowView[0].ToString()));
            }
        }

        private void MarkCurrentSong()
        {
            if (_currentlyPlayingDataGridRow != null)
                _currentlyPlayingDataGridRow.Background = (Brush)FindResource("CurrentlyPlaying");
        }

        public void ResetBackgroundFromRecentSong()
        {
            _currentlyPlayingDataGridRow.Background = _lastSongBackground;
        }

        private void DataGridSongs_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dataRowView = e.Row.DataContext as DataRowView;

            if (dataRowView != null)
            {
                if (dataRowView.Row["song_id"].Equals(_recentlyAddedSong))
                {
                    var clr = (Brush)FindResource("RecentlyAddedItem");
                    e.Row.Background = clr;
                    e.Row.BorderBrush = clr;
                }
            }
        }

        private void ButtonEditSong_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteSong_Click(object sender, RoutedEventArgs e)
        {
            var drv = DataGridSongs.SelectedItem as DataRowView;

            if (drv != null)
            {
                int id;
                if (int.TryParse(drv.Row["song_id"].ToString(), out id))
                {
                    var rowsAffected = _serverConnectionManager.ExecuteCommand(CommandFactory.DeleteSong(id));
                    if (rowsAffected == 1)
                    {
                        drv.Delete();
                    } else
                    {
                        MessageBox.Show("Unable to delete row");
                    }
                }
            }
        }
    }
}
