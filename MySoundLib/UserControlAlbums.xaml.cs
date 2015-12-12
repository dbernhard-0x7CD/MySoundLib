using MySoundLib.Windows;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
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

        }

        private void ButtonRenameAlbum_Click(object sender, RoutedEventArgs e)
        {

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
            }
            else
            {
                ButtonDeleteAlbum.Visibility = Visibility.Visible;
            }
        }
    }
}
