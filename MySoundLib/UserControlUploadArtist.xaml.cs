using MySoundLib.Windows;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlUploadArtist.xaml
	/// </summary>
	public partial class UserControlUploadArtist : UserControl
	{
		private readonly MainWindow _mainWindow;
		ServerConnectionManager _connectionManager {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }

		public UserControlUploadArtist(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
			TextBoxArtistName.Focus();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			ShowArtists();
		}

		private void ButtonAddArtist_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TextBoxArtistName.Text)) {
				MessageBox.Show("Please insert name");
				return;
			}
			int result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewArtist(TextBoxArtistName.Text));

			if (result != 1) {
				Debug.WriteLine("Unable to insert artist: " + TextBoxArtistName.Text);
			}
			if (result == 1) {
				ShowArtists();
			}
		}

		private void ShowArtists() {
			_mainWindow.GridContent.Children.Clear();
			_mainWindow.ListBoxCategory.SelectedIndex = 2;
			_mainWindow.GridContent.Children.Add(new UserControlArtists(_mainWindow));
		}
	}
}
