using MySoundLib.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MySoundLib
{
    /// <summary>
    /// Interaction logic for WindowSettings.xaml
    /// </summary>
    public partial class WindowSettings : Window
    {
        public WindowSettings()
        {
            InitializeComponent();

            var theme = Settings.GetValue(Property.Theme);

            foreach (ComboBoxItem x in ComboBoxTheme.Items)
            {
                if (x.Content.ToString() == theme)
                {
                    ComboBoxTheme.SelectedIndex = ComboBoxTheme.Items.IndexOf(x);
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            ((App)Application.Current).LoadTheme(Settings.GetValue(Property.Theme));
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ComboBoxTheme.SelectedValue as ComboBoxItem;

            Settings.SetProperty(Property.Theme, selectedItem.Content.ToString());
            ((App)Application.Current).LoadTheme(selectedItem.Content.ToString());
            Close();
        }

        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ComboBoxTheme.SelectedValue as ComboBoxItem;

            ((App)Application.Current).LoadTheme(selectedItem.Content.ToString());
        }
    }
}
