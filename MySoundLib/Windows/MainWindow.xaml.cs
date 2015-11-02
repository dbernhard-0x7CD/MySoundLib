using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MySoundLib.Configuration;
using WMPLib;

namespace MySoundLib.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private ServerConnectionManager _connectionManager;
		private int _currentSongId;
		private WindowsMediaPlayer _mediaPlayer;
		private DispatcherTimer _updateProgressTimer;

		public MainWindow()
		{
			Settings.LoadSettings();
			if (Settings.Contains(Property.AutoConnect) && Settings.Contains(Property.LastServer) &&
			    Settings.Contains(Property.LastUser))
			{
				_connectionManager = new ServerConnectionManager();
				_connectionManager.Connect(Settings.GetValue(Property.LastServer), Settings.GetValue(Property.LastUser),
					LoginWindow.GetDecryptedPassword(), "my_sound_lib");
			}
			else
			{
				if (!ShowLoginWindow())
				{
					Close();
					return;
				}
			}

			InitializeComponent();

			int songExists;
			if (int.TryParse(_connectionManager.ExecuteScalar(CommandFactory.GetSongAmount()).ToString(), out songExists))
			{
				if (songExists == 0)
				{
					GridContent.Children.Add(new UserControlUploadSong(_connectionManager));
				}
				else
				{
					ListBoxCategory.SelectedIndex = 0;
				}
			}

			HideCurrentSong();
		}

		private bool ShowLoginWindow()
		{
			var loginWindow = new LoginWindow();
			loginWindow.ShowDialog();

			if (loginWindow.ResultConnectionManager == null) return false;
			_connectionManager = loginWindow.ResultConnectionManager;
			return true;
		}

		private void MenuItemTestingWindow_OnClick(object sender, RoutedEventArgs e)
		{
			var testWindow = new TestWindow(_connectionManager) {Owner =  this};

			testWindow.Show();
		}

		private void MenuItemDisconnect_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_connectionManager.Disconnect();
			if (ShowLoginWindow())
				Show();
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(_connectionManager) {Owner = this};

			aboutWindow.ShowDialog();
		}

		public void HideCurrentSong()
		{
			StackPanelCurrentSongInformation.Visibility = Visibility.Collapsed;
			GridSplitterHor.Visibility = Visibility.Collapsed;
			Grid.SetRowSpan(GridContent, 3);
		}

		public void ShowCurrentSong()
		{
			StackPanelCurrentSongInformation.Visibility = Visibility.Visible;
			GridSplitterHor.Visibility = Visibility.Visible;
			Grid.SetRowSpan(GridContent, 1);
		}

		private void ListBoxItemSongs_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			GridContent.Children.Add(new UserControlSongs(_connectionManager, this));
		}

		private void ListBoxItemAlbums_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			GridContent.Children.Add(new UserControlAlbums(_connectionManager));
		}

		private void ListBoxItemArtists_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			var userControlArtists = new UserControlArtists();
			GridContent.Children.Add(userControlArtists);

			var artists = _connectionManager.GetDataTable("select artist_name from artists");

			foreach (DataRow row in artists.Rows)
			{
				userControlArtists.ListBoxArtists.Items.Add(row[0]);
			}
		}

		private void ListBoxItemGenres_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			var userControlGenres = new UserControlGenres();
			GridContent.Children.Add(userControlGenres);

			var genres = _connectionManager.GetDataTable("select genre_name from genres");

			foreach (DataRow row in genres.Rows)
			{
				userControlGenres.ListBoxGenres.Items.Add(row[0]);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_connectionManager.Disconnect();
		}

		public void PlaySong(int id)
		{
			ShowCurrentSong();
			_currentSongId = id;
			var song = _connectionManager.GetDataTable("select song_title, artist_name, album_name, genre_name, length, release_date from songs s left join artists a on (s.artist = a.artist_id) left join genres g on (s.genre = g.genre_id) left join albums al on (s.album = al.album_id) where song_id = " + id);

			var title = song.Rows[0]["song_title"];

			LabelSongTitle.Content = "Title: " + title;
			ButtonPlay.Content = "Start";
		}

		private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
		{
			switch (ButtonPlay.Content.ToString())
			{
				case "Pause":
					_mediaPlayer.controls.pause();
					ButtonPlay.Content = "Continue";
					return;
				case "Continue":
					_mediaPlayer.controls.play();
					ButtonPlay.Content = "Pause";
					return;
			}
			_mediaPlayer?.controls.stop();

			Debug.WriteLine("Loading track");
			var track = _connectionManager.GetDataTable("SELECT track FROM songs WHERE song_id = " + _currentSongId);

			var byteTrack = (byte[])track.Rows[0]["track"];

			var pathFile = Path.Combine(Settings.PathProgramFolder, Path.GetRandomFileName()) + ".mp3";

			try
			{
				var fileStream = new FileStream(pathFile, FileMode.Create, FileAccess.Write);

				fileStream.Write(byteTrack, 0, byteTrack.Length);

				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("unable to save file: ", ex.ToString());
				return;
			}
			ButtonPlay.Content = "Pause";

			_mediaPlayer = new WindowsMediaPlayer {URL = pathFile};
			
			_mediaPlayer.PlayStateChange += MediaPlayerOnPlayStateChange;

			_mediaPlayer.controls.play();

			_updateProgressTimer = new DispatcherTimer();
			_updateProgressTimer.Tick += UpdateProgressTimerOnTick;
			_updateProgressTimer.Interval = new TimeSpan(0,0,0,0,500);
			_updateProgressTimer.Start();
			
			Debug.WriteLine("Playing song");
		}

		private void UpdateProgressTimerOnTick(object sender, EventArgs eventArgs)
		{
			ProgressBarTrack.Maximum = _mediaPlayer.currentMedia.duration;
			ProgressBarTrack.Value = _mediaPlayer.controls.currentPosition;
		}

		private void MediaPlayerOnPlayStateChange(int newState)
		{
			if (newState == (int)WMPPlayState.wmppsStopped)
			{
				ButtonPlay.Content = "Play";
			}
		}

		private void ButtonMute_OnClick(object sender, RoutedEventArgs e)
		{
			if (ButtonMute.Content.ToString() == "Unmute")
			{
				_mediaPlayer.settings.mute = false;
				ButtonMute.Content = "Mute";
				return;
			}
			_mediaPlayer.settings.mute = true;
			ButtonMute.Content = "Unmute";
		}

		private void ButtonRestart_OnClick(object sender, RoutedEventArgs e)
		{
			_mediaPlayer.controls.currentPosition = 0;
		}
	}
}
