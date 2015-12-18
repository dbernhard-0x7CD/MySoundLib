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
        private bool IsEditMode = false;
        private int _albumId;

        public UserControlUploadAlbum(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            TextBoxName.Focus();
        }

        public UserControlUploadAlbum(MainWindow mainWindow, int albumId) : this (mainWindow)
        {
            _albumId = albumId;

            LabelHeaderTitle.Content = "Edit album";
            ButtonAddAlbum.Content = "Save";
            IsEditMode = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxName.Focus();

            if (IsEditMode)
            {
                var albumInformation = _connectionManager.GetDataTable(CommandFactory.GetAlbumInformation(_albumId)).Rows[0];

                TextBoxName.Text = albumInformation["album_name"].ToString();
                TextBoxName.Select(TextBoxName.Text.Length, 0);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            SelectAlbumsMainWindow();
            _mainWindow.GridContent.Children.Add(new UserControlAlbums(_mainWindow));
        }

        private void ButtonAddAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MessageBox.Show("Missing data");
                return;
            }

            int result;

            if (IsEditMode)
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.UpdateAlbum(_albumId, TextBoxName.Text));
            } else
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewAlbum(TextBoxName.Text));
            }

            if (result != 1)
            {
                Debug.WriteLine("Unable to create album");
                return;
            }

            if (result == 1)
            {
                SelectAlbumsMainWindow();
                _mainWindow.GridContent.Children.Add(new UserControlAlbums(_mainWindow));
            }
            else
            {
                Debug.WriteLine("unable to insert");
            }
        }

        private void SelectAlbumsMainWindow()
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 1;
        }
    }
}
