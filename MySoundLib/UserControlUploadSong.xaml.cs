using MySoundLib.Windows;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TagLib.Mpeg;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for UserControlUploadSong.xaml
    /// </summary>
    public partial class UserControlUploadSong
    {
        private readonly MainWindow _mainWindow;
        private ServerConnectionManager _connectionManager
        {
            get
            {
                return _mainWindow.ConnectionManager;
            }
        }
        private string _filePath;
        private DataTable _dataTableArtists;
        private DataTable _dataTableAlbums;
        private DataTable _dataTableGenres;

        public UserControlUploadSong(MainWindow mainWindow)
        {
            InitializeComponent();
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
                try // load data from tags
                {
                    var file = new AudioFile(_filePath);

                    // Song title
                    TextBoxSongTitle.Text = file.Tag.Title;

                    // Song artist
                    if (string.IsNullOrEmpty(file.Tag.FirstPerformer))
                    {
                        ComboBoxArtist.SelectedIndex = -1;
                    }
                    else
                    {
                        int index = GetArtistIndex(file.Tag.FirstPerformer);
                        if (index == -1)
                        {
                            EnableEditing(ComboBoxArtist);
                            ComboBoxArtist.Text = file.Tag.FirstPerformer;
                        }
                        else
                        {
                            ComboBoxArtist.SelectedIndex = index;
                        }
                    }

                    // Song album
                    if (string.IsNullOrEmpty(file.Tag.Album))
                    {
                        ComboBoxAlbum.SelectedIndex = -1;
                    }
                    else
                    {
                        int index = GetAlbumIndex(file.Tag.Album);
                        if (index == -1)
                        {
                            EnableEditing(ComboBoxAlbum);
                            ComboBoxAlbum.Text = file.Tag.Album;
                        }
                        else
                        {
                            ComboBoxAlbum.SelectedIndex = index;
                        }
                    }

                    // Song genre
                    if (string.IsNullOrEmpty(file.Tag.Genres[0]))
                    {
                        ComboBoxGenre.SelectedIndex = -1;
                    }
                    else
                    {
                        int index = GetGenreIndex(file.Tag.Genres[0]);
                        if (index == -1)
                        {
                            EnableEditing(ComboBoxGenre);
                            ComboBoxGenre.Text = file.Tag.Genres[0];
                        }
                        else
                        {
                            ComboBoxGenre.SelectedIndex = index;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to read tags from file. " + ex.Message);
                }
            }
        }

        private int GetArtistIndex(string artistName)
        {
            foreach (var x in ComboBoxArtist.Items)
            {
                var row = x as DataRowView;
                if (row.Row["artist_name"].ToString() == artistName)
                {
                    return ComboBoxArtist.Items.IndexOf(x);
                }
            }
            return -1;
        }

        private int GetAlbumIndex(string albumName)
        {
            foreach (var x in ComboBoxAlbum.Items)
            {
                var row = x as DataRowView;
                if (row.Row["album_name"].ToString() == albumName)
                {
                    return ComboBoxAlbum.Items.IndexOf(x);
                }
            }
            return -1;
        }

        private int GetGenreIndex(string genreName)
        {
            foreach (var x in ComboBoxGenre.Items)
            {
                var row = x as DataRowView;
                if (row.Row["genre_name"].ToString() == genreName)
                {
                    return ComboBoxGenre.Items.IndexOf(x);
                }
            }
            return -1;
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
                if (int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString(), out song_id))
                {
                    Debug.WriteLine("Successfully added song: " + song_id);
                    _mainWindow.GridContent.Children.Clear();
                    _mainWindow.ListBoxCategory.SelectedIndex = 0;
                    _mainWindow.GridContent.Children.Add(new UserControlSongs(_mainWindow, song_id));
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

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 0;
            _mainWindow.GridContent.Children.Add(new UserControlSongs(_mainWindow));
        }
    }
}
