using System;
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
				MySqlConnection connection;
				try
				{
					connection = new MySqlConnection($"server={server};uid={userName}");
					if (database != "")
						connection.ConnectionString += $";database={database}";
					if (password != "")
					{
						connection.ConnectionString += $";password={password}";
					}
					connection.Open();
				}
				catch (MySqlException e)
				{
					HandleException(e);
					return database != "" && Connect(server, userName, password);
				}
				catch (Exception e)
				{
					MessageBox.Show("Unknown exception:" + e.Message);
					return false;
				}

				CurrentConnection = connection;
				CurrentConnection.StateChange += CurrentConnectionOnStateChange;
			}
			else
			{
				Debug.WriteLine("Already connected");
			}

			return CurrentConnection != null && CurrentConnection.State == ConnectionState.Open;
		}

		private static void CurrentConnectionOnStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
		{
			if (stateChangeEventArgs.CurrentState != ConnectionState.Open && stateChangeEventArgs.CurrentState != ConnectionState.Connecting)
			{
				Debug.WriteLine("New unallowed state: " + stateChangeEventArgs.CurrentState);
			}
		}

		/// <summary>
		/// Closes the connection to the server
		/// </summary>
		public void Disconnect()
		{
			CurrentConnection.StateChange -= CurrentConnectionOnStateChange;
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
		/// <returns>Amoun of affected lines (data rows); -1 means an error occurred</returns>
		public int ExecuteCommand(string command)
		{
			var mySqlCommand = new MySqlCommand(command) {Connection =  CurrentConnection};

			try
			{
				return mySqlCommand.ExecuteNonQuery();
			}
			catch (MySqlException e)
			{
				HandleException(e);

				Debug.WriteLine("Exception-ExecuteCommand: " + e.Message + "\nCommand:" + command.Substring(0,50) + "...");
			}
			return -1;
		}

		/// <summary>
		/// Execute the command as scalar
		/// </summary>
		/// <param name="command">SQL-Command as string</param>
		/// <returns>Object, null if an error occurred</returns>
		public object ExecuteScalar(string command)
		{
			var mySqlCommand = new MySqlCommand(command) {Connection = CurrentConnection};

			try
			{
				return mySqlCommand.ExecuteScalar();
			}
			catch (MySqlException e)
			{
				HandleException(e);
				
				Debug.WriteLine("Exception-ExecuteCommand: " + e.Message + "\nCommand:" + command.Substring(0, 50) + "...");
			}

			return null;
		}

		private static void HandleException(MySqlException mySqlException)
		{
			MySqlErrorCode errorCode;
			if (!Enum.TryParse(mySqlException.Number.ToString(), false, out errorCode))
			{
				Debug.WriteLine("Unable to parse exception: " + mySqlException.Message);
			}

			string message;

			switch (errorCode)
			{
				case MySqlErrorCode.DatabaseAccessDenied:
					message = "Access denied (Right user?)";
					break;
				case MySqlErrorCode.UnableToConnectToHost:
					message = "Unable to connect to the specified server";
					break;
				default:
					message = "MySqlException: " + errorCode + "\tMessage: " + mySqlException.Message;
					break;
			}
			Debug.WriteLine(message);
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

			var dataTable = new DataTable();
			var mySqlCommand = new MySqlCommand(command) {Connection = CurrentConnection};

			try
			{
				var reader = mySqlCommand.ExecuteReader();
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
			}
			catch (MySqlException e)
			{
				HandleException(e);
			}
			catch (Exception e)
			{
				Debug.WriteLine("ServerConnectionManager.GetDataTable-Exception: " + e.Message);
			}
			
			return dataTable;
		}
	}
}
