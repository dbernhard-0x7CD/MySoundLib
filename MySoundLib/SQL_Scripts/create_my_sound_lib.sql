CREATE DATABASE my_sound_lib DEFAULT COLLATE utf8_general_ci;
USE my_sound_lib;

CREATE TABLE albums
(
  album_id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  album_name text NOT NULL,
  title_song int(11) DEFAULT NULL,
  album_thumbnail mediumblob,
  FOREIGN KEY (title_song) REFERENCES songs (song_id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE artists
(
  artist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  artist_name text NOT NULL
) ENGINE=InnoDB;


CREATE TABLE genres
(
  genre_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  genre_name text NOT NULL
) ENGINE=InnoDB;


CREATE TABLE songs (
  song_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  song_title text NOT NULL,
  artist int(11) DEFAULT NULL,
  album int(11) DEFAULT NULL,
  genre int(11) DEFAULT NULL,
  track longblob NOT NULL,
  length int(11) DEFAULT NULL,
  release_date date DEFAULT NULL,
  FOREIGN KEY (artist) REFERENCES artists (artist_id) ON DELETE SET NULL,
  FOREIGN KEY (album) REFERENCES albums (album_id) ON DELETE SET NULL,
  FOREIGN KEY (genre) REFERENCES genres (genre_id) ON DELETE SET NULL
) ENGINE=InnoDB;


CREATE TABLE playlists (
  playlist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  name text NOT NULL,
  description text DEFAULT NULL,
  date_created date DEFAULT NULL
) ENGINE=InnoDB;


CREATE TABLE song_playlist (
  song_playlist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  song int(11) NOT NULL,
  playlist int(11) NOT NULL,
  FOREIGN KEY (song) REFERENCES songs (song_id) ON DELETE CASCADE,
  FOREIGN KEY (playlist) REFERENCES playlists (playlist_id) ON DELETE CASCADE
) ENGINE=InnoDB;

GRANT SELECT, SHOW DATABASES ON *.* TO 'msl_user'@'%' WITH
	MAX_QUERIES_PER_HOUR 999
	MAX_CONNECTIONS_PER_HOUR 999
	MAX_USER_CONNECTIONS 30;