using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	public class ServerConnectionManager
	{
		private MySqlConnection CurrentConnection { get; set; }

		public bool Connect(string server, string userName, string password, string database)
		{
			if (CurrentConnection == null)
			{
				try
				{
					CurrentConnection = new MySqlConnection($"server={server};uid={userName};pwd={password};database={database}");
					CurrentConnection.Open();
				}
				catch (MySqlException ex)
				{
					MessageBox.Show(ex.Message);
					return false;
				}
				catch (Exception e)
				{
					MessageBox.Show("Unknown exception:" + e.Message);
					return false;
				}
			}
			else
			{
				Debug.WriteLine("Already connected");
			}

			return CurrentConnection != null && CurrentConnection.State == ConnectionState.Open;
		}

		public bool Connect(string server, string userName, string password)
		{
			return Connect(server, userName, password, "master");
		}

		public void Disconnect()
		{
			CurrentConnection.Close();
		}

		public void test()
		{
			if (CurrentConnection == null)
				return;

			var command = new MySqlCommand("show databases") {Connection = CurrentConnection};

			var reader = command.ExecuteReader();

			var rows = new List<string[]>();

			// get column-head
			var columnHead = new string[reader.FieldCount];
			for (var i = 0; i < reader.FieldCount; i++)
			{
				columnHead[i] = reader.GetName(i);
			}
			rows.Add(columnHead);

			while (reader.Read())
			{
				var row = new string[reader.FieldCount];

				for (var i = 0; i < reader.FieldCount; i++)
				{
					row[i] = reader.GetValue(i).ToString();
				}

				rows.Add(row);
			}

			foreach (var row in rows)
			{
				Debug.Write("Row: ");
				foreach (var s in row)
				{
					Debug.Write(s + "\t\t");
				}
				Debug.Write("\n");
			}
		}
	}
}
