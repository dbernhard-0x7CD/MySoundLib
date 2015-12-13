using MySoundLib.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlUploadAlbum.xaml
    /// </summary>
    public partial class UserControlUploadAlbum : UserControl
    {
        private MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }


        public UserControlUploadAlbum(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 1;
            _mainWindow.GridContent.Children.Add(new UserControlAlbums(_mainWindow));
        }

        private void ButtonAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Missing data");
                return;
            }

            var result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewAlbum(TextBoxName.Text));

            if (result != 1)
            {
                Debug.WriteLine("Unable to create genre");
                return;
            }
            int albumId;
            if (!int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString(), out albumId))
            {
                Debug.WriteLine("unable to get id");
            }

            if (result == 1)
            {
                _mainWindow.GridContent.Children.Clear();
                _mainWindow.ListBoxCategory.SelectedIndex = 1;
                _mainWindow.GridContent.Children.Add(new UserControlAlbums(_mainWindow));
            }
            else
            {
                Debug.WriteLine("unable to insert");
            }
        }
    }
}
