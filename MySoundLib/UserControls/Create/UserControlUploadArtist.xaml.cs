using MySoundLib.Windows;
using MySoundLib.UserControls.List;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib.UserControls.Create
{
    /// <summary>
    /// Interaction logic for UserControlUploadArtist.xaml
    /// </summary>
    public partial class UserControlUploadArtist : UserControl
    {
        private readonly MainWindow _mainWindow;
        ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }
        private bool IsEditMode = false;
        private int _artistId;

        public UserControlUploadArtist(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            TextBoxArtistName.Focus();
        }

        public UserControlUploadArtist(MainWindow mainWindow, int artistId) : this (mainWindow)
        {
            _artistId = artistId;

            LabelHeaderTitle.Content = "Edit artist";
            ButtonAddArtist.Content = "Save";
            IsEditMode = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxArtistName.Focus();

            if (IsEditMode)
            {
                var artistInformation = _connectionManager.GetDataTable(CommandFactory.GetArtistInformation(_artistId)).Rows[0];

                TextBoxArtistName.Text = artistInformation["artist_name"].ToString();
                TextBoxArtistName.Select(TextBoxArtistName.Text.Length, 0);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowArtists();
        }

        private void ButtonAddArtist_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxArtistName.Text))
            {
                MessageBox.Show("Please insert name");
                return;
            }

            var existsArtist = _connectionManager.ExecuteScalar(CommandFactory.ExistsArtist(TextBoxArtistName.Text));

            if (existsArtist != null)
            {
                MessageBox.Show("Artist already exists. Change artists of songs individually");
                return;
            }

            int result;

            if (IsEditMode)
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.UpdateArtist(_artistId, TextBoxArtistName.Text));
            } else
            {
                result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewArtist(TextBoxArtistName.Text));
            }

            if (result != 1)
            {
                Debug.WriteLine("Unable to insert or update artist: " + TextBoxArtistName.Text);
            }
            if (result == 1)
            {
                ShowArtists();
            }
        }

        private void ShowArtists()
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 2;
            _mainWindow.GridContent.Children.Add(new UserControlArtists(_mainWindow));
        }
    }
}
