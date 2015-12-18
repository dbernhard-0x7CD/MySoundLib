using MySoundLib.Windows;
using MySoundLib.UserControls.List;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib.UserControls.Create
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
        public bool IsEditMode = false;
        private int _genreId;

        public UserControlUploadGenre(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        public UserControlUploadGenre(MainWindow mainWindow, int genreId) : this (mainWindow)
        {
            _genreId = genreId;

            LabelHeaderTitle.Content = "Edit genre";
            ButtonAddGenre.Content = "Save";
            IsEditMode = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxName.Focus();

            if (IsEditMode)
            {
                var genreInformation = _connectionManager.GetDataTable(CommandFactory.GetGenreInformation(_genreId)).Rows[0];

                TextBoxName.Text = genreInformation["genre_name"].ToString();
                TextBoxName.Select(TextBoxName.Text.Length,0);
            }
        }

        private void ButtonAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Missing data");
                return;
            }

            var existsGenre = _connectionManager.ExecuteScalar(CommandFactory.ExistGenre(TextBoxName.Text));

            if (existsGenre != null)
            {
                MessageBox.Show("Genre already exists. Change genres of songs individually");
                return;
            }

            int result;

            if (IsEditMode)
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.UpdateGenre(_genreId, TextBoxName.Text));
            } else
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewGenre(TextBoxName.Text));
            }

            if (result != 1)
            {
                Debug.WriteLine("Unable to create or update genre");
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
