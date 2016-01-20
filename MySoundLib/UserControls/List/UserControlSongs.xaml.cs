using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MySoundLib.Windows;
using MySoundLib.UserControls.Create;
using System.Diagnostics;

namespace MySoundLib.UserControls.List
{
    /// <summary>
    /// Interaction logic for UserControlSongs.xaml
    /// </summary>
    public partial class UserControlSongs
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }
        private static DataGridRow _currentlyPlayingDataGridRow;
        private Brush _lastSongBackground;
        /// <summary>
        /// Id from the song
        /// </summary>
        private int _recentlyAddedSong;

        public UserControlSongs(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var songs = _connectionManager.GetDataTable(CommandFactory.GetSongs());

            DataGridSongs.ItemsSource = songs.DefaultView;
            DataGridSongs.SelectedIndex = -1;

            MarkCurrentSong();
        }

        public UserControlSongs(MainWindow mainWindow, int song_id) : this(mainWindow)
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
                ButtonPlaySong.Visibility = Visibility.Collapsed;
                ButtonEditSong.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonDeleteSong.Visibility = Visibility.Visible;
                ButtonPlaySong.Visibility = Visibility.Visible;
                ButtonEditSong.Visibility = Visibility.Visible;
            }
        }

        private void DataGridRow_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataGridRow = e.Source as DataGridRow;
            var dataRowView = dataGridRow?.DataContext as DataRowView;
            StartPlayingSong(dataRowView);
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
            _mainWindow.ListBoxCategory.UnselectAll();

            _mainWindow.GridContent.Children.Clear();

            var song = DataGridSongs.SelectedItem as DataRowView;

            if (song != null)
            {
                _mainWindow.GridContent.Children.Add(new UserControlUploadSong(_mainWindow, (int)song["song_id"]));
            }
            else
            {
                MessageBox.Show("Unable to edit song. (Selection wrong)");
            }
        }

        private void ButtonDeleteSong_Click(object sender, RoutedEventArgs e)
        {
            while (DataGridSongs.SelectedItems.Count != 0)
            {
                var dataRowView = DataGridSongs.SelectedItems[0] as DataRowView;

                if (dataRowView != null)
                {
                    int id;
                    if (int.TryParse(dataRowView.Row["song_id"].ToString(), out id))
                    {
                        var rowsAffected = _connectionManager.ExecuteCommand(CommandFactory.DeleteSong(id));
                        if (rowsAffected == 1)
                        {
                            dataRowView.Delete();
                        }
                        else
                        {
                            MessageBox.Show("Unable to delete row");
                        }
                    }
                }
            }
        }

        private void ButtonPlaySong_Click(object sender, RoutedEventArgs e)
        {
            StartPlayingSong(DataGridSongs.SelectedItem as DataRowView);
        }

        void StartPlayingSong(DataRowView dataRowView)
        {
            if (dataRowView == null)
            {
                Debug.WriteLine("DataRowView is null");
                return;
            }
            DataGridRow dataGridRow = (DataGridRow)DataGridSongs.ItemContainerGenerator.ContainerFromItem(dataRowView);

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

                _mainWindow.PlaySong(int.Parse(dataRowView["song_id"].ToString()));
            }
            ButtonPlaySong.Visibility = Visibility.Collapsed;
        }

        private void DataGridCell_MouseEnter(object sender, MouseEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.Column.SortMemberPath == "song_title")
            {

            }
            else if (cell.Column.SortMemberPath == "artist_name" && !string.IsNullOrEmpty(cell.ToString()))
            {
                cell.Cursor = Cursors.Hand;
            }
            else
            {
                cell.Cursor = Cursors.Arrow;
            }
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = TextBoxSearch.Text.ToLower();

            var dataView = DataGridSongs.ItemsSource as DataView;

            if (dataView != null)
            {
                dataView.RowFilter = $"song_title like '%{searchTerm}%' OR artist_name like '%{searchTerm}%' OR genre_name like '%{searchTerm}%' OR album_name like '%{searchTerm}%'";
            }
        }
    }
}
