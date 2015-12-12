using System.ComponentModel;
using System.Windows;
using MySoundLib.Windows;
using System.Data;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlArtists.xaml
    /// </summary>
    public partial class UserControlArtists
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
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

            var songs = _connectionManager.GetDataTable("select artist_id, artist_name, count(s.song_id) as song_count from artists a left join songs s on (a.artist_id = s.artist) group by a.artist_id");

            DataGridArtists.ItemsSource = songs.DefaultView;
            DataGridArtists.Items.SortDescriptions.Add(new SortDescription("artist_name", ListSortDirection.Ascending));
        }

        private void ButtonAddArtist_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ListBoxCategory.UnselectAll();

            _mainWindow.GridContent.Children.Clear();
            _mainWindow.GridContent.Children.Add(new UserControlUploadArtist(_mainWindow));
        }

        private void DataGridArtists_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataGridArtists.SelectedIndex == -1)
            {
                ButtonDeleteArtist.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonDeleteArtist.Visibility = Visibility.Visible;
            }
        }

        private void ButtonRenameArtist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteArtist_Click(object sender, RoutedEventArgs e)
        {
            while (DataGridArtists.SelectedItems.Count != 0)
            {
                var dataRowView = DataGridArtists.SelectedItems[0] as DataRowView;

                if (dataRowView != null)
                {
                    int id;
                    if (int.TryParse(dataRowView.Row["artist_id"].ToString(), out id))
                    {
                        var rowsAffected = _connectionManager.ExecuteCommand(CommandFactory.DeleteArtist(id));
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
    }
}
