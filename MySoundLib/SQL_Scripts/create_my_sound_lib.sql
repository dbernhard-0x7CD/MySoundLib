CREATE DATABASE my_sound_lib DEFAULT COLLATE utf8_general_ci;
USE my_sound_lib;

CREATE TABLE albums
(
  album_id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
  album_name text NOT NULL,
  title_song int(11) DEFAULT NULL,
  album_thumbnail mediumblob
) ENGINE=InnoDB;

CREATE TABLE artists
(
  artist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  artist_name int(11) NOT NULL
) ENGINE=InnoDB;


CREATE TABLE genres
(
  genre_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  genre_name text NOT NULL
) ENGINE=InnoDB;


CREATE TABLE songs (
  song_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  song_name text NOT NULL,
  artist int(11) DEFAULT NULL,
  album int(11) DEFAULT NULL,
  genre int(11) DEFAULT NULL,
  track longblob NOT NULL,
  length int(11) DEFAULT NULL,
  release_date date DEFAULT NULL
) ENGINE=InnoDB;


CREATE TABLE playlists (
  playlist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  name text NOT NULL,
  description text DEFAULT NULL,
  date_created date DEFAULT NULL
) ENGINE=InnoDB;


CREATE TABLE song_playlist (
  song_playlist_id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY,
  song_id int(11) NOT NULL,
  playlist_id int(11) NOT NULL
) ENGINE=InnoDB;

 -- testing create users
 /*
 GRANT SELECT, SHOW DATABASES ON *.* TO 'msl_user'@'%';

GRANT ALL PRIVILEGES ON my_sound_lib.* TO 'msl_user'@'%';

*/