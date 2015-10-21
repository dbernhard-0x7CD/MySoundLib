using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using MySql.Data.MySqlClient;

namespace MySoundLib
{
	public class ServerConnectionManager
	{
		/// <summary>
		/// Gets or sets the current connection
		/// </summary>
		private MySqlConnection CurrentConnection { get; set; }

		/// <summary>
		/// Connect to the server
		/// </summary>
		/// <param name="server">IP or domain</param>
		/// <param name="userName">Username</param>
		/// <param name="password">Password</param>
		/// <param name="database">Optional initial database</param>
		/// <returns></returns>
		public bool Connect(string server, string userName, string password, string database = "")
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

		/// <summary>
		/// Closes the connection to the server
		/// </summary>
		public void Disconnect()
		{
			CurrentConnection.Close();
		}

		/// <summary>
		/// Check if the connection is open
		/// </summary>
		/// <returns>True if the state is open, else false</returns>
		public bool IsConnected()
		{
			return CurrentConnection.State == ConnectionState.Open;
		}
		
		/// <summary>
		/// Execute the command and get the amount of affected lines
		/// </summary>
		/// <param name="command">SQL-Command as string</param>
		/// <returns>Amoun of affected lines (data rows)</returns>
		public int ExecuteCommand(string command)
		{
			var mySqlCommand = new MySqlCommand(command) {Connection =  CurrentConnection};

			try
			{
				return mySqlCommand.ExecuteNonQuery();
			}
			catch (MySqlException e)
			{
				Debug.WriteLine(e.Message);
			}
			return -1;
		}

		/// <summary>
		/// Execute the command as scalar
		/// </summary>
		/// <param name="command">SQL-Command as string</param>
		/// <returns>Object</returns>
		public object ExecuteScalar(string command)
		{
			var mySqlCommand = new MySqlCommand(command) {Connection = CurrentConnection};

			try
			{
				return mySqlCommand.ExecuteScalar();
			}
			catch (MySqlException e)
			{
				Debug.WriteLine(e.Message);
			}

			return -1;
		}

		/// <summary>
		/// Get the datatable from a command
		/// </summary>
		/// <param name="command">SQL-Command</param>
		/// <returns>DataTable</returns>
		public DataTable GetDataTable(string command)
		{
			if (CurrentConnection == null)
				throw new Exception("Not connected");

			var mySqlCommand = new MySqlCommand(command) {Connection = CurrentConnection};

			var reader = mySqlCommand.ExecuteReader();

			var dataTable = new DataTable();

			var schemaTable = reader.GetSchemaTable();

			if (schemaTable != null)
			{
				foreach (DataRowView x in schemaTable.DefaultView)
				{
					var columnName = (string) x["ColumnName"];
					var type = (Type) x["DataType"];

					dataTable.Columns.Add(columnName, type);
				}

				dataTable.Load(reader);
			}

			return dataTable;
		}
	}
}
