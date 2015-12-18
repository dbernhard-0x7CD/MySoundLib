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
            if (dateTimeReleased.HasValue)
                command.Parameters.AddWithValue("@release_date", dateTimeReleased.Value.ToString("yyyy-MM-dd"));
            else
                command.Parameters.AddWithValue("@release_date", null);

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

        public static MySqlCommand GetSongs()
        {
            return new MySqlCommand("select song_id, song_title, artist_name, album_name, genre_name, length from songs s left join artists a on (a.artist_id = s.artist) left join genres g on (s.genre = g.genre_id) left join albums al on (al.album_id = s.album) order by song_title");
        }

        public static MySqlCommand GetAlbums()
        {
            return new MySqlCommand("select album_id, album_name, count(s.song_id) as song_count, artist_name from albums a left join songs s on (s.album = a.album_id) left join artists ar on (s.artist = ar.artist_id) group by a.album_id order by album_name;");
        }

        public static MySqlCommand GetArtists()
        {
            return new MySqlCommand("select artist_id, artist_name, count(s.song_id) as song_count from artists a left join songs s on (a.artist_id = s.artist) group by a.artist_id order by artist_name");
        }

        public static MySqlCommand GetGenres()
        {
            return new MySqlCommand("select genre_id, genre_name, count(s.song_id) as song_count from genres g left join songs s on (g.genre_id = s.genre) group by g.genre_id order by genre_name;");
        }

        public static MySqlCommand GetGenreNames()
        {
            return new MySqlCommand("select genre_id, genre_name from genres order by genre_name;");
        }

        public static MySqlCommand GetArtistNames()
        {
            return new MySqlCommand("select artist_id, artist_name from artists order by artist_name");
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

        public static MySqlCommand GetSongInformation(int id)
        {
            return new MySqlCommand("select song_title, artist_name, album_name, genre_name, length, release_date from songs s left join artists a on (s.artist = a.artist_id) left join genres g on (s.genre = g.genre_id) left join albums al on (s.album = al.album_id) where song_id = " + id);
        }

        public static MySqlCommand GetSongInformationIds(int id)
        {
            return new MySqlCommand("select song_title, artist, album, genre, release_date from songs where song_id = " + id);
        }

        public static MySqlCommand GetGenreInformation(int id)
        {
            return new MySqlCommand($"select genre_name from genres where genre_id = {id}");
        }

        public static MySqlCommand UpdateSong(int id, string title, int? artistId, int? albumId, int? genreId, DateTime? dateTimeReleased)
        {
            var command = new MySqlCommand($"update songs set song_title=@title, release_date=@release_date, artist=@artist, genre=@genre, album=@album where song_id = {id}");

            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@artist", artistId);
            command.Parameters.AddWithValue("@album", albumId);
            command.Parameters.AddWithValue("@genre", genreId);
            if (dateTimeReleased.HasValue)
                command.Parameters.AddWithValue("@release_date", dateTimeReleased.Value.ToString("yyyy-MM-dd"));
            else
                command.Parameters.AddWithValue("@release_date", null);

            return command;
        }

        public static MySqlCommand UpdateGenre(int id, string name)
        {
            var command = new MySqlCommand($"update genres set genre_name=@genre_name where genre_id={id}");

            command.Parameters.AddWithValue("@genre_name", name);

            return command;
        }

        public static MySqlCommand ExistGenre(string name)
        {
            return new MySqlCommand($"select 1 from genres where genre_name='{name}'");
        }
    }
}