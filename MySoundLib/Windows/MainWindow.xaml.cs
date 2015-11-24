using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MySoundLib.Configuration;
using WMPLib;
using System.Threading.Tasks;

namespace MySoundLib.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public ServerConnectionManager ConnectionManager { get; private set; }
		private int _currentSongId;
		private WindowsMediaPlayer _mediaPlayer;
		private DispatcherTimer _updateProgressTimer;

		public MainWindow()
		{
			// connect  to database if autoconnect is active
			Settings.LoadSettings();
			if (Settings.Contains(Property.AutoConnect) && Settings.Contains(Property.LastServer) &&
			    Settings.Contains(Property.LastUser))
			{
				ConnectionManager = new ServerConnectionManager();
				ConnectionManager.Connect(Settings.GetValue(Property.LastServer), Settings.GetValue(Property.LastUser),
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
			InitialiseFirstView();
		}

		private void InitialiseFirstView() {
			// show upload-song control when no songs exist
			var songExists = ConnectionManager.ExecuteScalar(CommandFactory.GetSongAmount());
			int amountSongs;

			if (songExists == null || !int.TryParse(songExists.ToString(), out amountSongs))
			{
				Debug.WriteLine("unable to retrieve if there are songs");
				return;
			}

			if (amountSongs == 0)
			{
				GridContent.Children.Clear();
				GridContent.Children.Add(new UserControlUploadSong(this));
			}
			else
			{
				ListBoxCategory.SelectedIndex = 0;
			}

			// decide visibility of menu
			if (Settings.Contains(Property.CollapseMenu)) {
				HideMenu();
			} // is shown by default

			HideCurrentSong();
		}

		private bool ShowLoginWindow()
		{
			var loginWindow = new LoginWindow();
			loginWindow.ShowDialog();

			if (loginWindow.ResultConnectionManager == null) return false;
			ConnectionManager = loginWindow.ResultConnectionManager;

			return true;
		}

		private void MenuItemTestingWindow_OnClick(object sender, RoutedEventArgs e)
		{
			var testWindow = new TestWindow(ConnectionManager) {Owner =  this};

			testWindow.Show();
		}

		private void MenuItemDisconnect_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			ConnectionManager.Disconnect();
			if (ShowLoginWindow()) {
				InitialiseFirstView();
				Show();
			}
			else
				Close();
		}

		private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
		{
			var aboutWindow = new AboutWindow(ConnectionManager) {Owner = this};

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
			GridContent.Children.Add(new UserControlSongs(ConnectionManager, this));
		}

		private void ListBoxItemAlbums_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			GridContent.Children.Add(new UserControlAlbums(ConnectionManager, this));
		}

		private void ListBoxItemArtists_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			GridContent.Children.Add(new UserControlArtists(ConnectionManager, this));
		}

		private void ListBoxItemGenres_OnSelected(object sender, RoutedEventArgs e)
		{
			GridContent.Children.Clear();
			var userControlGenres = new UserControlGenres();
			GridContent.Children.Add(userControlGenres);

			var genres = ConnectionManager.GetDataTable("select genre_name from genres");

			foreach (DataRow row in genres.Rows)
			{
				userControlGenres.ListBoxGenres.Items.Add(row[0]);
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ConnectionManager.Disconnect();
			Settings.SaveConfig();
		}

		public void PlaySong(int id)
		{
			_mediaPlayer?.close();

			ShowCurrentSong();
			_currentSongId = id;
			var song = ConnectionManager.GetDataTable("select song_title, artist_name, album_name, genre_name, length, release_date from songs s left join artists a on (s.artist = a.artist_id) left join genres g on (s.genre = g.genre_id) left join albums al on (s.album = al.album_id) where song_id = " + id);

			var title = song.Rows[0]["song_title"];

			LabelSongTitle.Content = "Title: " + title;
			ButtonPlay.Content = "Start";

			Debug.WriteLine("moving to play track");
			Task task = new Task(PlayTrack);

			task.Start(TaskScheduler.FromCurrentSynchronizationContext());
			Debug.WriteLine("end play");
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
				case "Start":
					PlayTrack();
					return;
				default:
					Debug.WriteLine("Undefined action");
					break;
			}
		}

		private void PlayTrack()
		{
			Debug.WriteLine("Loading track from song_id " + _currentSongId);
			var track = ConnectionManager.GetDataTable("SELECT track FROM songs WHERE song_id = " + _currentSongId);

			var byteTrack = (byte[]) track.Rows[0]["track"];

			var pathFile = Path.Combine(Settings.PathProgramFolder, byteTrack.GetHashCode().ToString()) + ".mp3"; // TODO: save as hash

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
			_updateProgressTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_updateProgressTimer.Start();

			Debug.WriteLine("Playing song");
		}

		private void UpdateProgressTimerOnTick(object sender, EventArgs eventArgs)
		{
			var duration = _mediaPlayer?.currentMedia?.duration;

			if (duration != null && (int)duration != 0)
			{
				ProgressBarTrack.Maximum = (double) duration;
				ProgressBarTrack.Value = _mediaPlayer.controls.currentPosition;
			}
		}

		private void MediaPlayerOnPlayStateChange(int newState)
		{
			if (newState == (int)WMPPlayState.wmppsStopped)
			{
				ButtonPlay.Content = "Play";
				_currentSongId = 0;
				_mediaPlayer = null;
				ProgressBarTrack.Value = 0;
				HideCurrentSong();

				var userControlSongs = GridContent.Children[0] as UserControlSongs;

				userControlSongs?.ResetBackgroundFromRecentSong();
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

		private void ProgressBarTrack_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			double value = GetProgressBarValue(e.GetPosition(ProgressBarTrack).X);

			ProgressBarTrack.Value = value;
			_mediaPlayer.controls.currentPosition = value;
		}

		private double GetProgressBarValue(double MousePosition)
		{
			double ratio = MousePosition / ProgressBarTrack.ActualWidth;
			double ProgressBarValue = ratio * ProgressBarTrack.Maximum;
			return ProgressBarValue;
		}

		private void MenuItemHideMenu_Click(object sender, RoutedEventArgs e)
		{
			HideMenu();
			Settings.SetProperty(Property.CollapseMenu, "yes");
		}

		private void ButtonShowMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowMenu();
			Settings.RemoveProperty(Property.CollapseMenu);
		}

		private void HideMenu() {
			AppMenu.Visibility = Visibility.Collapsed;
			ButtonShowMenuItem.Visibility = Visibility.Visible;
		}

		private void ShowMenu() {
			AppMenu.Visibility = Visibility.Visible;
			ButtonShowMenuItem.Visibility = Visibility.Collapsed;
		}
	}
}
