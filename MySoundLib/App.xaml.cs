using MySoundLib.Configuration;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace MySoundLib
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        public void LoadTheme(string theme)
        {
            string fileName = Environment.CurrentDirectory + @"\Themes\" + theme + ".xaml";

            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    ResourceDictionary dic = (ResourceDictionary)XamlReader.Load(fs);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(dic);
                }
            }
            else
                MessageBox.Show("Unable to find theme: " + fileName);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings.LoadSettings();
            LoadTheme(Settings.GetValue(Property.Theme));
        }
    }
}
