using MySoundLib.Windows;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlUploadSong.xaml
	/// </summary>
	public partial class UserControlUploadSong
	{
		private readonly ServerConnectionManager _connectionManager;
		private readonly MainWindow _mainWindow;
		private string _filePath;
		private DataTable _dataTableArtists;
		private DataTable _dataTableAlbums;
		private DataTable _dataTableGenres;

		public UserControlUploadSong(ServerConnectionManager connectionManager, MainWindow mainWindow)
		{
			InitializeComponent();
			_connectionManager = connectionManager;
			_mainWindow = mainWindow;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_dataTableArtists = _connectionManager.GetDataTable(CommandFactory.GetArtists());

			ComboBoxArtist.ItemsSource = _dataTableArtists.DefaultView;

			_dataTableGenres = _connectionManager.GetDataTable(CommandFactory.GetGenres());

			ComboBoxGenre.ItemsSource = _dataTableGenres.DefaultView;

			_dataTableAlbums = _connectionManager.GetDataTable(CommandFactory.GetAlbums());

			ComboBoxAlbum.ItemsSource = _dataTableAlbums.DefaultView;
		}

		private void ButtonSelectFile_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				FilterIndex = 3,
				Filter = "MP3 Files (*.mp3)|*.mp3|MPEG 4 Audio (*.m4a)|*.m4a|Audio Files|*.mp3;*.m4a"
			};

			var result = dlg.ShowDialog();

			if (result == true)
			{
				_filePath = dlg.FileName;

				ButtonSelectFile.Content = Path.GetFileName(_filePath);
				TextBoxSongTitle.Text = Path.GetFileNameWithoutExtension(_filePath);
			}
		}

		private void ButtonAddSong_Click(object sender, RoutedEventArgs e)
		{
			if (TextBoxSongTitle.Text == "" || _filePath == "")
			{
				MessageBox.Show("Missing data");
				return;
			}

			var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
			var br = new BinaryReader(fs);
			var data = br.ReadBytes((int)fs.Length);
			br.Close();
			fs.Close();

			int? artistId = null;
			int? albumId = null;
			int? genreId = null;

			// ensure artist exist
			if (ComboBoxArtist.SelectedValue == null)
			{
				if (ComboBoxArtist.Text != "")
				{
					var result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewArtist(ComboBoxArtist.Text));

					if (result != 1)
					{
						Debug.WriteLine("Unable to create artist");
						return;
					}
					artistId = ToNullableInt32(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString());
				}
			}
			else
			{
				artistId = (int)ComboBoxArtist.SelectedValue;
			}

			// ensure album exists
			if (ComboBoxAlbum.SelectedValue == null)
			{
				if (ComboBoxAlbum.Text != "")
				{
					var result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewAlbum(ComboBoxAlbum.Text));

					if (result != 1)
					{
						Debug.WriteLine("Unable to create album");
						return;
					}
					albumId = ToNullableInt32(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString());
				}
			}
			else
			{
				albumId = (int)ComboBoxAlbum.SelectedValue;
			}

			// ensure genre exists
			if (ComboBoxGenre.SelectedValue == null)
			{
				if (ComboBoxGenre.Text != "")
				{
					var result = _connectionManager.ExecuteCommand(CommandFactory.InsertNewGenre(ComboBoxGenre.Text));

					if (result != 1)
					{
						Debug.WriteLine("Unable to create genre");
						return;
					}
					genreId = ToNullableInt32(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString());
				}
			}
			else
			{
				genreId = (int)ComboBoxGenre.SelectedValue;
			}

			var affectedLines = _connectionManager.ExecuteCommand(CommandFactory.InsertNewSong(TextBoxSongTitle.Text, data, artistId, albumId, genreId, null));
			if (affectedLines > 1)
			{
				Debug.WriteLine("More lines affected");
			}
			if (affectedLines >= 1)
			{
				int song_id;
				if(int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString(), out song_id)) {
					Debug.WriteLine("Successfully added song: " + song_id);
					_mainWindow.GridContent.Children.Clear();
					_mainWindow.GridContent.Children.Add(new UserControlSongs(_connectionManager, _mainWindow, song_id));
				}
			}
		}

		private void ButtonAddArtist_OnClick(object sender, RoutedEventArgs e)
		{
			EnableEditing(ComboBoxArtist);
		}

		private void ButtonAddGenre_OnClick(object sender, RoutedEventArgs e)
		{
			EnableEditing(ComboBoxGenre);
		}

		private void ButtonAddAlbum_OnClick(object sender, RoutedEventArgs e)
		{
			EnableEditing(ComboBoxAlbum);
		}

		private static void EnableEditing(ComboBox comboBox)
		{
			comboBox.IsEditable = true;
			comboBox.IsReadOnly = false;
			comboBox.Text = "";
		}

		public static int? ToNullableInt32(string s)
		{
			int i;
			if (int.TryParse(s, out i)) return i;
			return null;
		}
	}
}
