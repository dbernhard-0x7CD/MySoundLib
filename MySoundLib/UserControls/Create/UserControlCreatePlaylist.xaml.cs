using MySoundLib.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace MySoundLib.UserControls.Create
{
    /// <summary>
    /// Interaction logic for UserControlCreatePlaylist.xaml
    /// </summary>
    public partial class UserControlCreatePlaylist : UserControl
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }

        public UserControlCreatePlaylist(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var songs = _connectionManager.GetDataTable(CommandFactory.GetSongs());

            DataGridSongs.ItemsSource = songs.DefaultView;

            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAddSong_Click(object sender, RoutedEventArgs e)
        {
            int[] songIds = new int[DataGridSongs.SelectedItems.Count];

            for (int i = 0; i < songIds.Length; i++)
            {
                songIds[i] = int.Parse((DataGridSongs.SelectedItems[i] as DataRowView)["song_id"].ToString());
            }

            var res = _connectionManager.ExecuteCommand(CommandFactory.InsertNewPlaylist(TextBoxName.Text, TextBoxDescription.Text, DateTime.Today));

            if (res != 1)
            {
                Debug.WriteLine("Error");
                return;
            }
            var id = _connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId());
            int playlistId = int.Parse(id.ToString());

            Debug.WriteLine("Playlist-ID: " + id);

            for (int i = 0; i < songIds.Length; i++)
            {
                var songId = int.Parse((DataGridSongs.SelectedItems[i] as DataRowView)["song_id"].ToString());
                _connectionManager.ExecuteCommand(CommandFactory.InsertNewSongToPlaylist(songId, playlistId));
            }

            _mainWindow.GridContent.Children.Clear();
        }

        private void DataGridRowPreviewMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var row = GetVisualParentByType((FrameworkElement)e.OriginalSource, typeof(DataGridRow)) as DataGridRow;
                if (row != null)
                {
                    row.IsSelected = !row.IsSelected;
                    e.Handled = true;
                }
            }
        }

        public static DependencyObject GetVisualParentByType(DependencyObject startObject, Type type)
        {
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }
    }
}
