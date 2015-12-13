using MySoundLib.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlUploadGenre.xaml
    /// </summary>
    public partial class UserControlUploadGenre : UserControl
    {
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }
        private MainWindow _mainWindow;

        public UserControlUploadGenre(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void ButtonAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Missing data");
                return;
            }

            var result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewGenre(TextBoxName.Text));

            if (result != 1)
            {
                Debug.WriteLine("Unable to create genre");
                return;
            }
            int genreId;
            if (!int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString(), out genreId))
            {
                Debug.WriteLine("unable to get id");
            }

            if (result == 1)
            {
                _mainWindow.GridContent.Children.Clear();
                _mainWindow.ListBoxCategory.SelectedIndex = 3;
                _mainWindow.GridContent.Children.Add(new UserControlGenres(_mainWindow));
            }
            else
            {
                Debug.WriteLine("unable to insert");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 3;
            _mainWindow.GridContent.Children.Add(new UserControlGenres(_mainWindow));
        }
    }
}
