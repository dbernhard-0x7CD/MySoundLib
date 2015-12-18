using MySoundLib.Windows;
using MySoundLib.UserControls.Create;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib.UserControls.List
{
    /// <summary>
    /// Interaction logic for UserControlAlbums.xaml
    /// </summary>
    public partial class UserControlAlbums
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }

        public UserControlAlbums(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var albums = _connectionManager.GetDataTable(CommandFactory.GetAlbums());

            DataGridAlbums.ItemsSource = albums.DefaultView;
        }

        private void ButtonAddNewAlbum_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();
            _mainWindow.GridContent.Children.Add(new UserControlUploadAlbum(_mainWindow));
        }

        private void ButtonRenameAlbum_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();

            var album = DataGridAlbums.SelectedItem as DataRowView;

            if (album != null)
            {
                _mainWindow.GridContent.Children.Add(new UserControlUploadAlbum(_mainWindow, (int)album["album_id"]));
            } else
            {
                MessageBox.Show("Unable to rename album. (Selection wrong)");
            }
        }

        private void ResetMainWindow()
        {
            _mainWindow.ListBoxCategory.UnselectAll();

            _mainWindow.GridContent.Children.Clear();
        }

        private void ButtonDeleteAlbum_Click(object sender, RoutedEventArgs e)
        {
            while (DataGridAlbums.SelectedItems.Count != 0)
            {
                var dataRowView = DataGridAlbums.SelectedItems[0] as DataRowView;

                if (dataRowView != null)
                {
                    int id;
                    if (int.TryParse(dataRowView.Row["album_id"].ToString(), out id))
                    {
                        var rowsAffected = _connectionManager.ExecuteCommand(CommandFactory.DeleteAlbum(id));
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

        private void DataGridAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridAlbums.SelectedIndex == -1)
            {
                ButtonDeleteAlbum.Visibility = Visibility.Collapsed;
                ButtonRenameAlbum.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonDeleteAlbum.Visibility = Visibility.Visible;
                ButtonRenameAlbum.Visibility = Visibility.Visible;
            }
        }
    }
}
