using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using MySoundLib.Configuration;

namespace MySoundLib.Windows
{
	/// <summary>
	///     Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow
	{
		private static readonly byte[] Entropy = { 1, 7, 6, 9, 4 };
		private readonly bool _tryAutoConnect;

		public LoginWindow(bool tryAutoconnect = true)
		{
			InitializeComponent();
			_tryAutoConnect = tryAutoconnect;

			Settings.LoadSettings();
			if (Settings.Contains(Property.LastServer))
			{
				TextBoxServer.Text = Settings.GetValue(Property.LastServer);
			}
			if (Settings.Contains(Property.LastUser))
			{
				TextBoxUser.Text = Settings.GetValue(Property.LastUser);
			}
			if (Settings.Contains(Property.LastPassword))
			{
				byte[] ciphertext = StringToByteArray(Settings.GetValue(Property.LastPassword));

				var data = ProtectedData.Unprotect(ciphertext, Entropy, DataProtectionScope.CurrentUser);

				TextBoxPassword.Password = Encoding.UTF8.GetString(data);
			}

			if (TextBoxServer.Text != "" && TextBoxUser.Text != "" && TextBoxPassword.Password.Length == 0)
			{
				TextBoxPassword.SelectAll();
			}
			else if (TextBoxServer.Text.Length == 0)
			{
				TextBoxServer.Select(0,0);
			}
			else if (TextBoxUser.Text.Length == 0)
			{
				TextBoxUser.Select(0,0);
			}
			else
			{
				ButtonConnect.Focus();
			}
		}

		public ServerConnectionManager ResultConnectionManager { get; private set; }

		private void ButtonConnect_Click(object sender, RoutedEventArgs e)
		{
			var connectionManager = new ServerConnectionManager();
			
			var connectionSuccess = connectionManager.Connect(TextBoxServer.Text, TextBoxUser.Text, TextBoxPassword.Password, "");

			ResultConnectionManager = connectionManager;
			Settings.SetProperty(Property.LastServer, TextBoxServer.Text);
			Settings.SetProperty(Property.LastUser, TextBoxUser.Text);

			if (CheckBoxSavePassword.IsChecked != null && (bool) CheckBoxSavePassword.IsChecked)
			{
				byte[] passwordPlain = Encoding.UTF8.GetBytes(TextBoxPassword.Password);

				byte[] ciphertext = ProtectedData.Protect(passwordPlain, Entropy, DataProtectionScope.CurrentUser);

				Settings.SetProperty(Property.LastPassword, ByteArrayToString(ciphertext));
			}
			if (CheckBoxAutoConnect.IsChecked != null && CheckBoxAutoConnect.IsChecked.Value && connectionSuccess)
			{
				Settings.SetProperty(Property.AutoConnect, "true");
			}
			else
			{
				if (Settings.Contains(Property.AutoConnect))
				{
					Settings.RemoveProperty(Property.AutoConnect);
				}
			}

			Settings.SaveConfig();

			if (connectionSuccess)
			{
				Close();
			}
		}

		public static string ByteArrayToString(byte[] ba)
		{
			string hex = BitConverter.ToString(ba);
			return hex.Replace("-", "");
		}

		public static byte[] StringToByteArray(string hex)
		{
			var numberChars = hex.Length;
			byte[] bytes = new byte[numberChars / 2];
			for (int i = 0; i < numberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Settings.Contains(Property.AutoConnect) && _tryAutoConnect)
			{
				ButtonConnect_Click(null, null);
			}
		}
	}
}