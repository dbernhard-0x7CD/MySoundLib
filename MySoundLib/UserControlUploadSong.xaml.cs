using MySoundLib.Windows;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public bool IsEditMode = false;
        private int _songId;

        public UserControlUploadSong(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        public UserControlUploadSong(MainWindow mainWindow, int songId) : this(mainWindow)
        {
            _songId = songId;

            LabelHeaderTitle.Content = "Edit song";
            ButtonAddSong.Content = "Save";
            IsEditMode = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dataTableArtists = _connectionManager.GetDataTable(CommandFactory.GetArtistNames());

            ComboBoxArtist.ItemsSource = _dataTableArtists.DefaultView;

            _dataTableGenres = _connectionManager.GetDataTable(CommandFactory.GetGenreNames());

            ComboBoxGenre.ItemsSource = _dataTableGenres.DefaultView;

            _dataTableAlbums = _connectionManager.GetDataTable(CommandFactory.GetAlbums());

            ComboBoxAlbum.ItemsSource = _dataTableAlbums.DefaultView;

            if (IsEditMode)
            {
                var songInformation = _connectionManager.GetDataTable(CommandFactory.GetSongInformationIds(_songId)).Rows[0];

                TextBoxSongTitle.Text = songInformation["song_title"].ToString();

                ButtonSelectFile.Content = "Select different file";
                ButtonSelectFile.IsEnabled = false;

                ComboBoxArtist.SelectedValue = songInformation["artist"];
                ComboBoxAlbum.SelectedValue = songInformation["album"];
                ComboBoxGenre.SelectedValue = songInformation["genre"];

                string releaseDate = songInformation["release_date"].ToString();

                if (!string.IsNullOrEmpty(releaseDate))
                    DatePickerReleased.SelectedDate = Convert.ToDateTime(releaseDate);

                TextBoxSongTitle.Focus();
                TextBoxSongTitle.Select(TextBoxSongTitle.Text.Length, 0);
            }
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
                if (string.IsNullOrEmpty(TextBoxSongTitle.Text))
                {
                    TextBoxSongTitle.Text = Path.GetFileNameWithoutExtension(_filePath);
                }
            }
        }

        private int GetArtistIndex(string artistName)
        {
            return GetIndex(artistName, ComboBoxArtist, "artist_name");
        }

        private int GetAlbumIndex(string albumName)
        {
            return GetIndex(albumName, ComboBoxAlbum, "album_name");
        }

        private int GetGenreIndex(string genreName)
        {
            return GetIndex(genreName, ComboBoxGenre, "genre_name");
        }

        private int GetIndex(string name, ComboBox comboBox, string header)
        {
            foreach (var x in comboBox.Items)
            {
                var row = x as DataRowView;
                if (row.Row[header].ToString() == name)
                {
                    return comboBox.Items.IndexOf(x);
                }
            }
            return -1;
        }

        private void ButtonAddSong_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxSongTitle.Text == "" || (string.IsNullOrEmpty(_filePath) && !IsEditMode))
            {
                MessageBox.Show("Missing data");
                return;
            }

            byte[] data = null;

            if (!string.IsNullOrEmpty(_filePath))
            {
                var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                data = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            int? artistId = null;
            int? albumId = null;
            int? genreId = null;

            // when selected value of comboBox is null, the value has to be new
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

            int affectedLines = -1;

            if (IsEditMode)
            {
                affectedLines = _connectionManager.ExecuteCommand(CommandFactory.UpdateSong(_songId, TextBoxSongTitle.Text, artistId, albumId, genreId, DatePickerReleased.SelectedDate));

                MoveToSongList(_songId);
            } else
            {
                affectedLines = _connectionManager.ExecuteCommand(CommandFactory.InsertNewSong(TextBoxSongTitle.Text, data, artistId, albumId, genreId, DatePickerReleased.SelectedDate));

                if (affectedLines != 1)
                {
                    Debug.WriteLine("Unable to create song!");
                    return;
                }

                int songId;
                if (int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetLastInsertedId()).ToString(), out songId))
                {
                    Debug.WriteLine("Successfully added song: " + songId);
                    MoveToSongList(songId);
                }
            }

            if (affectedLines != 1)
            {
                Debug.WriteLine("More lines affected: " + affectedLines);
            }
        }

        private void MoveToSongList(int songId)
        {
            _mainWindow.GridContent.Children.Clear();
            _mainWindow.ListBoxCategory.SelectedIndex = 0;
            _mainWindow.GridContent.Children.Add(new UserControlSongs(_mainWindow, songId));
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
            comboBox.UpdateLayout();
            var x = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
            x.Focus();
            x.SelectAll();
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
