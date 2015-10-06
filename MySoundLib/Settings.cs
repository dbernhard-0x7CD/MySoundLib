using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Berburger
{
	/// <summary>
	/// Settings manager
	/// </summary>
	public static class Settings
	{
		/// <summary>
		/// Path which contains the config file
		/// </summary>
		static string path_programFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Berburger");
		/// <summary>
		/// Configuration file path
		/// </summary>
		static string path_configFile = Path.Combine(path_programFolder, "config.txt");
		
		/// <summary>
		/// Dictionary of values accessed by properties
		/// </summary>
		public static Dictionary<string, string> config = new Dictionary<string, string>();
		
		/// <summary>
		/// Create paths if they don't exist and load the dictionary
		/// </summary>
		public static void LoadSettings()
		{
			config.Clear();
			createDirectory(path_programFolder);
			createFile(path_configFile);
			
			// load settings from file
			string[] lines = getLines(path_configFile);
			
			foreach (var line in lines) {
				var property = line.Split('=')[0];
				var value = line.Replace(property + "=", "");
				
				config.Add(property, value);
			}
		}
		
		/// <summary>
		/// Makes sure the given property exists, if not it gets created with the defaultValue
		/// </summary>
		/// <param name="p">Property</param>
		/// <param name="defaultValue">The default value from the property</param>
		static void EnsureProperty(string p, object defaultValue)
		{
			if (!Contains(p)) {
				config.Add(p, defaultValue.ToString());
			}
		}
		
		/// <summary>
		/// Makes sure a directory exists. Else it gets created
		/// </summary>
		/// <param name="strPath">Directory path</param>
		static void createDirectory(string strPath)
		{
			if (!Directory.Exists(strPath)) {
				Directory.CreateDirectory(strPath);	
			}
		}
		
		/// <summary>
		///  Makes sure the file at the given path exists
		/// </summary>
		/// <param name="strPath">Path to the file</param>
		static void createFile(string strPath)
		{
			if (!File.Exists(strPath)) {
				File.Create(strPath).Dispose();
			}
		}
		
		/// <summary>
		/// Sets the property to a given value
		/// </summary>
		/// <param name="property">Property</param>
		/// <param name="strValue">Value</param>
		public static void SetProperty(string property, string strValue)
		{
			if (Contains(property)) {
				config[property] = strValue;
			} else {
				config.Add(property, strValue);
			}
		}
		
		/// <summary>
		/// Returns the value from the given property
		/// </summary>
		/// <param name="property">Property</param>
		public static string GetValue(string property)
		{
			return config[property];
		}
		
		/// <summary>
		/// Return true if the property is already in the config
		/// </summary>
		/// <param name="property">Property</param>
		public static bool Contains(string property)
		{
			return config.ContainsKey(property);
		}
		
		
		/// <summary>
		/// Removes the given property from the config
		/// </summary>
		/// <param name="property">Property</param>
		public static void RemoveProperty(string property)
		{
			config.Remove(property);
		}
		
		/// <summary>
		/// Saves the config to the configuration file
		/// </summary>
		/// <returns>true if success, false otherwise</returns>
		public static bool SaveConfig()
		{
			List<string> list = new List<string>();
			
			foreach (var e in config) {
				list.Add(e.Key + "=" + e.Value);
			}			
			
			return writeLines(list.ToArray(), path_configFile);
		}
		
		/// <summary>
		/// Returns the lines from the given file
		/// </summary>
		/// <param name="strPath">Path to the file</param>
		/// <returns>Array of lines</returns>
		static string[] getLines(string strPath)
		{
			string[] lines;
			try {
				lines = File.ReadAllLines(strPath);
			} catch (Exception) {
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
		static bool writeLines(string[] lines, string strPath)
		{
			try {
				File.WriteAllLines(strPath, lines);
			} catch (Exception) {
				Debug.WriteLine("Can't write to " + strPath);
				return false;
			}
			return true;
		}
	}
}
