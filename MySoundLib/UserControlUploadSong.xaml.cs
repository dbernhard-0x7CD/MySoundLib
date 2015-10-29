using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlUploadSong.xaml
	/// </summary>
	public partial class UserControlUploadSong
	{
		private readonly ServerConnectionManager _connectionManager;
		private string _filePath;
		private DataTable _dataTableArtists;
		private DataTable _dataTableGenres;
		
		public UserControlUploadSong(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_connectionManager = connectionManager;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_dataTableArtists = _connectionManager.GetDataTable("select * from artists");

			ComboBoxArtist.ItemsSource = _dataTableArtists.DefaultView;

			_dataTableGenres = _connectionManager.GetDataTable("select * from genres");

			ComboBoxGenre.ItemsSource = _dataTableGenres.DefaultView;
		}

		private void ButtonSelectFile_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				DefaultExt = ".mp3",
				Filter = "MP3 Files (*.mp3)|*.mp3|MPEG 4 Audio (*.m4a)|*.m4a"
			};

			var result = dlg.ShowDialog();

			if (result == true)
			{
				_filePath = dlg.FileName;

				ButtonSelectFile.Content = Path.GetFileName(_filePath);
			}
			else
			{
				MessageBox.Show("No path choosen");
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
			var imageData = br.ReadBytes((int)fs.Length);
			br.Close();
			fs.Close();

			var command = new MySqlCommand("INSERT INTO Songs(song_title, track, artist) VALUES(@title, @track, @artist)");

			command.Parameters.AddWithValue("@title", TextBoxSongTitle.Text);
			command.Parameters.AddWithValue("@track", imageData);

			if (ComboBoxArtist.SelectedValue == null)
			{
				Debug.WriteLine("not implemented to add an artist on the fly");
				command.Parameters.AddWithValue("@artist", null);
			}
			else
			{
				command.Parameters.AddWithValue("@artist", ComboBoxArtist.SelectedValue);
			}

			_connectionManager.ExecuteCommand(command);
		}

		private void ButtonAddArtist_OnClick(object sender, RoutedEventArgs e)
		{
			ComboBoxArtist.IsEditable = true;
			ComboBoxArtist.IsReadOnly = false;
			ComboBoxArtist.Text = "";
		}

		private void ButtonAddGenre_OnClick(object sender, RoutedEventArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}
