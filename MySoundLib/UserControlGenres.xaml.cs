using System.Windows.Controls;
using System.Windows;
using System.Data;
using MySoundLib.Windows;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlGenres.xaml
    /// </summary>
    public partial class UserControlGenres : UserControl
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }

        public UserControlGenres(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var genres = _connectionManager.GetDataTable(CommandFactory.GetGenres());

            DataGridGenres.ItemsSource = genres.DefaultView;
        }

        private void DataGridSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridGenres.SelectedIndex == -1)
            {
                ButtonDeleteGenre.Visibility = Visibility.Collapsed;
                ButtonRenameGenre.Visibility = Visibility.Collapsed;
            }
            else
            {
                ButtonDeleteGenre.Visibility = Visibility.Visible;
                ButtonRenameGenre.Visibility = Visibility.Visible;
            }
        }

        private void ButtonAddGenre_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();
            _mainWindow.GridContent.Children.Add(new UserControlUploadGenre(_mainWindow));
        }

        private void ButtonRenameGenre_Click(object sender, RoutedEventArgs e)
        {
            ResetMainWindow();

            var genre = DataGridGenres.SelectedItem as DataRowView;

            if (genre != null)
            {
                _mainWindow.GridContent.Children.Add(new UserControlUploadGenre(_mainWindow, (int)genre["genre_id"]));
            } else
            {
                MessageBox.Show("Unable to rename genre. (Selection wrong");
            }
        }

        private void ResetMainWindow()
        {
            _mainWindow.ListBoxCategory.UnselectAll();
            _mainWindow.GridContent.Children.Clear();
        }

        private void ButtonDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            while (DataGridGenres.SelectedItems.Count != 0)
            {
                var dataRowView = DataGridGenres.SelectedItems[0] as DataRowView;

                if (dataRowView != null)
                {
                    int id;
                    if (int.TryParse(dataRowView.Row["genre_id"].ToString(), out id))
                    {
                        var rowsAffected = _connectionManager.ExecuteCommand(CommandFactory.DeleteGenre(id));
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
