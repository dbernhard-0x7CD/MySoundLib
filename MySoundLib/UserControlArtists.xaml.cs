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

            var songs = _connectionManager.GetDataTable(CommandFactory.GetArtists());

            DataGridArtists.ItemsSource = songs.DefaultView;
        }

        private void ButtonAddArtist_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();
            _mainWindow.GridContent.Children.Add(new UserControlUploadArtist(_mainWindow));
        }

        private void DataGridArtists_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DataGridArtists.SelectedIndex == -1)
            {
                ButtonDeleteArtist.Visibility = Visibility.Collapsed;
                ButtonRenameArtist.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonDeleteArtist.Visibility = Visibility.Visible;
                ButtonRenameArtist.Visibility = Visibility.Visible;
            }
        }

        private void ResetMainWindow()
        {
            _mainWindow.ListBoxCategory.UnselectAll();

            _mainWindow.GridContent.Children.Clear();
        }

        private void ButtonRenameArtist_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();

            var artist = DataGridArtists.SelectedItem as DataRowView;

            if (artist != null)
            {
                _mainWindow.GridContent.Children.Add(new UserControlUploadArtist(_mainWindow, (int)artist["artist_id"]));
            } else
            {
                MessageBox.Show("Unable to rename artist. (Selection wrong)");
            }
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
