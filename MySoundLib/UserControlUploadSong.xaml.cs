using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for UserControlUploadSong.xaml
	/// </summary>
	public partial class UserControlUploadSong : UserControl
	{
		private readonly ServerConnectionManager _connectionManager;
		private string _filePath;
		
		public UserControlUploadSong(ServerConnectionManager connectionManager)
		{
			InitializeComponent();
			_connectionManager = connectionManager;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var artists = _connectionManager.GetDataTable("select artist_name from artists");

			foreach (DataRow row in artists.Rows)
			{
				ComboBoxArtist.Items.Add(row[0]);
			}

			var genres = _connectionManager.GetDataTable("select genre_name from genres");

			foreach (DataRow row in genres.Rows)
			{
				ComboBoxGenre.Items.Add(row[0]);
			}
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

				ButtonSelectFile.Content = System.IO.Path.GetFileName(_filePath);
			}
			else
			{
				MessageBox.Show("No path choosen");
			}
		}

		private void ButtonAddSong_Click(object sender, RoutedEventArgs e)
		{
			var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
			var br = new BinaryReader(fs);
			var imageData = br.ReadBytes((int)fs.Length);
			br.Close();
			fs.Close();

			var command = new MySqlCommand("INSERT INTO Songs(song_title, track) VALUES(@title, @track)");

			command.Parameters.AddWithValue("@title", TextBoxSongTitle.Text);
			command.Parameters.AddWithValue("@track", imageData);

			_connectionManager.ExecuteCommand(command);
		}
	}
}
