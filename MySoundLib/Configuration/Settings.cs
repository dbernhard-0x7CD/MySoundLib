using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MySoundLib.Configuration
{
    /// <summary>
    /// Settings manager
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Path which contains the config file
        /// </summary>
        public static readonly string PathProgramFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MySoundLib");
        /// <summary>
        /// Configuration file path
        /// </summary>
        static readonly string PathConfigFile = Path.Combine(PathProgramFolder, "config.txt");

        /// <summary>
        /// Dictionary of values accessed by properties
        /// </summary>
        static readonly Dictionary<Property, string> Config = new Dictionary<Property, string>();

        static readonly string[] SizeSuffixes =
                  { "bytes", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Create paths if they don't exist and load the dictionary
        /// </summary>
        public static void LoadSettings()
        {
            Config.Clear();
            CreateDirectory(PathProgramFolder);
            CreateFile(PathConfigFile);

            // load settings from file
            string[] lines = GetLines(PathConfigFile);

            foreach (var line in lines)
            {
                var property = line.Split('=')[0];
                var value = line.Replace(property + "=", "");

                Config.Add(ParseEnum<Property>(property), value);
            }
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Makes sure a directory exists. Else it gets created
        /// </summary>
        /// <param name="strPath">Directory path</param>
        static void CreateDirectory(string strPath)
        {
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
        }

        /// <summary>
        ///  Makes sure the file at the given path exists
        /// </summary>
        /// <param name="strPath">Path to the file</param>
        static void CreateFile(string strPath)
        {
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
        }

        /// <summary>
        /// Sets the property to a given value
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="strValue">Value</param>
        public static void SetProperty(Property property, string strValue)
        {
            if (Contains(property))
            {
                Config[property] = strValue;
            }
            else {
                Config.Add(property, strValue);
            }
        }

        /// <summary>
        /// Returns the value from the given property
        /// </summary>
        /// <param name="property">Property</param>
        public static string GetValue(Property property)
        {
            return Config[property];
        }

        /// <summary>
        /// Return true if the property is already in the config
        /// </summary>
        /// <param name="property">Property</param>
        public static bool Contains(Property property)
        {
            return Config.ContainsKey(property);
        }


        /// <summary>
        /// Removes the given property from the config
        /// </summary>
        /// <param name="property">Property</param>
        public static void RemoveProperty(Property property)
        {
            Config.Remove(property);
        }

        /// <summary>
        /// Saves the config to the configuration file
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        public static bool SaveConfig()
        {
            List<string> list = new List<string>();

            foreach (var e in Config)
            {
                list.Add(e.Key + "=" + e.Value);
            }

            return WriteLines(list.ToArray(), PathConfigFile);
        }

        /// <summary>
        /// Returns the lines from the given file
        /// </summary>
        /// <param name="strPath">Path to the file</param>
        /// <returns>Array of lines</returns>
        static string[] GetLines(string strPath)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(strPath);
            }
            catch (Exception)
            {
                return null;
            }
            return lines;
        }

        /// <summary>
        /// Writes the lines to the given file
        /// </summary>
        /// <param name="lines">Lines to write</param>
        /// <param name="strPath">Path to file</param>
        /// <returns>True if success, false otherwise</returns>
        static bool WriteLines(string[] lines, string strPath)
        {
            try
            {
                File.WriteAllLines(strPath, lines);
            }
            catch (Exception)
            {
                Debug.WriteLine("Can't write to " + strPath);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Deletes all data files
        /// </summary>
        public static void DeleteLocalSongs()
        {
            string[] files = GetDataFiles();

            foreach (var x in files)
            {
                if (!x.Contains("config"))
                {
                    try
                    {
                        File.Delete(x);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Unable to delete file: " + x);
                    }
                }
            }
        }

        /// <summary>
        /// Returns all data file paths which are in the program-data folder
        /// </summary>
        /// <returns>String array filled with absolute paths</returns>
        private static string[] GetDataFiles()
        {
            return Directory.GetFiles(PathProgramFolder, "*", SearchOption.AllDirectories);
        }

        public static string GetSizeOfLocalSongs()
        {
            var paths = GetDataFiles();
            long sum = 0;

            foreach (var x in paths)
            {
                sum += new FileInfo(x).Length;
            }

            return GetSizeWithSuffix(sum);
        }

        static string GetSizeWithSuffix(long value)
        {
            if (value < 0) { return "-" + GetSizeWithSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }
    }
}
