using System;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	public static class CommandFactory
	{
		public static MySqlCommand InsertNewArtist(string artistName)
		{
			return new MySqlCommand($"INSERT INTO artists(artist_name) VALUES('{artistName}')");
        }

		public static MySqlCommand InsertNewGenre(string genreName)
		{
			return new MySqlCommand($"INSERT INTO genres(genre_name) VALUES('{genreName}')");
		}

		public static MySqlCommand InsertNewAlbum(string albumName)
		{
			return new MySqlCommand($"INSERT INTO albums(album_name) VALUES('{albumName}')");
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

		public static MySqlCommand GetSongAmount()
		{
			return new MySqlCommand("select count(*) from songs");
		}
    }
}