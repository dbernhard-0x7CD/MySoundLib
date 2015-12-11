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
        private readonly ServerConnectionManager _serverConnectionManager;

        public UserControlAlbums(ServerConnectionManager connectionManager)
        {
            InitializeComponent();
            _serverConnectionManager = connectionManager;

            var albums = _serverConnectionManager.GetDataTable(CommandFactory.GetAlbums());

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
                        var rowsAffected = _serverConnectionManager.ExecuteCommand(CommandFactory.DeleteAlbum(id));
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
