using System;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	public static class CommandFactory
	{
		public static MySqlCommand InsertNewArtist(string name)
		{
			return new MySqlCommand($"INSERT INTO artists(artist_name) VALUES('{name}')");
        }

		public static MySqlCommand InsertNewSong(string title, byte[] track, int? artistId, int? albumId, int? genreId, DateTime? dateTimeReleased)
		{
			var command = new MySqlCommand("INSERT INTO Songs(song_title, track, artist, album, genre, release_date) VALUES(@title, @track, @artist, @album, @genre, @release_date)");

			command.Parameters.AddWithValue("@title", title);
			command.Parameters.AddWithValue("@track", track);
			
			command.Parameters.AddWithValue("@artist", artistId);
			command.Parameters.AddWithValue("@album", albumId);
			command.Parameters.AddWithValue("@genre", genreId);
			command.Parameters.AddWithValue("@release_date", dateTimeReleased);

			return command;
		}

		public static MySqlCommand GetLastInsertedId()
		{
			return new MySqlCommand("SELECT LAST_INSERT_ID();");
		}
	}
}