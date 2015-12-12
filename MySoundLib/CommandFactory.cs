using System;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
    public static class CommandFactory
    {
        public static MySqlCommand InsertNewArtist(string artistName)
        {
            var command = new MySqlCommand("INSERT INTO artists(artist_name) VALUES(@artist_name)");
            command.Parameters.AddWithValue("@artist_name", artistName);

            return command;
        }

        public static MySqlCommand InsertNewGenre(string genreName)
        {
            var command = new MySqlCommand("INSERT INTO genres(genre_name) VALUES(@genre_name)");
            command.Parameters.AddWithValue("@genre_name", genreName);

            return command;
        }

        public static MySqlCommand InsertNewAlbum(string albumName)
        {
            var command = new MySqlCommand("INSERT INTO albums(album_name) VALUES(@album_name)");
            command.Parameters.AddWithValue("@album_name", albumName);

            return command;
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

        public static MySqlCommand GetAlbums()
        {
            return new MySqlCommand("select album_id, album_name, count(s.song_id) as song_count, artist_name from albums a left join songs s on (s.album = a.album_id) left join artists ar on (s.artist = ar.artist_id) group by a.album_id order by album_name;");
        }

        public static MySqlCommand GetGenres()
        {
            return new MySqlCommand("select genre_id, genre_name, count(s.song_id) as song_count from genres g left join songs s on (s.genre = g.genre_id) group by s.genre order by genre_name;");
        }

        public static MySqlCommand GetGenreNames()
        {
            return new MySqlCommand("select genre_id, genre_name from genres order by genre_name;");
        }

        public static MySqlCommand GetArtistNames()
        {
            return new MySqlCommand("select artist_id, artist_name from artists order by artist_name");
        }

        public static MySqlCommand GetArtists()
        {
            return new MySqlCommand("select artist_id, artist_name, count(s.song_id) as song_count from artists a left join songs s on (a.artist_id = s.artist) group by a.artist_id order by artist_name");
        }

        public static MySqlCommand DeleteSong(int id)
        {
            return new MySqlCommand($"DELETE FROM songs WHERE `song_id`='{id}'");
        }

        public static MySqlCommand DeleteArtist(int id)
        {
            return new MySqlCommand($"DELETE FROM artists WHERE `artist_id`='{id}'");
        }

        public static MySqlCommand DeleteAlbum(int id)
        {
            return new MySqlCommand($"DELETE FROM albums WHERE `album_id`='{id}'");
        }

        public static MySqlCommand DeleteGenre(int id)
        {
            return new MySqlCommand($"DELETE FROM genres WHERE `genre_id`='{id}'");
        }
    }
}