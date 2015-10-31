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
			try
			{
				var connection = new MySqlConnection($"server={server};uid={userName}");
				if (password != "")
				{
					connection.ConnectionString += $";password={password}";
				}
				Debug.WriteLine($"Connecting to {server} as {userName}");
				connection.Open();

				CurrentConnection = connection;
			}
			catch (Exception exception)
			{
				Debug.WriteLine("Unable to connect...");
				var myException = exception as MySqlException;
				if (myException != null)
					HandleException(myException);
				else
					MessageBox.Show("Unknown exception:" + exception.Message);
				return false;
			}

			if (database != "")
			{
				var databaseExists = new MySqlCommand($"SHOW DATABASES LIKE '{database}'") {Connection = CurrentConnection};

				if (databaseExists.ExecuteScalar() != null)
				{
					CurrentConnection.ChangeDatabase(database);
					Debug.WriteLine($"Database {database} already exists");
				}
				else
				{
					Debug.WriteLine($"Trying to create database {database}");

					try
					{
						ExecuteCommand(Properties.Resources.create_my_sound_lib);
					}
					catch (DatabaseAccessDeniedExcpetion)
					{
						MessageBox.Show($"{userName} is not allowed to create databases");
						return false;
					}
					
					Debug.WriteLine($"Successfully created {database}");
				}
			}

			CurrentConnection.StateChange += CurrentConnectionOnStateChange;

			return CurrentConnection.State == ConnectionState.Open;
		}

		private static void CurrentConnectionOnStateChange(object sender, StateChangeEventArgs stateChangeEventArgs)
		{
			if (stateChangeEventArgs.CurrentState != ConnectionState.Open)
			{
				Debug.WriteLine("New unallowed state: " + stateChangeEventArgs.CurrentState);
				if (stateChangeEventArgs.CurrentState == ConnectionState.Closed)
				{
					MessageBox.Show("You are now disconnected from the server");
				}
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
			return CurrentConnection != null && CurrentConnection.State == ConnectionState.Open;
		}
		
		/// <summary>
		/// Execute the command and get the amount of affected lines
		/// </summary>
		/// <param name="command">SQL-Command as string</param>
		/// <returns>Amoun of affected lines (data rows); -1 means an error occurred</returns>
		public int ExecuteCommand(string command)
		{
			return ExecuteCommand(new MySqlCommand(command));
		}

		/// <summary>
		/// Execute the command and get the amount of affected lines
		/// </summary>
		/// <param name="command">SQL-Command as MySqlCommand</param>
		/// <returns>Amoun of affected lines (data rows); -1 means an error occurred</returns>
		public int ExecuteCommand(MySqlCommand command)
		{
			if (!IsConnected())
				return -1;
			command.Connection = CurrentConnection;
			try
			{
				return command.ExecuteNonQuery();
			}
			catch (MySqlException e)
			{
				HandleException(e);
			}
			return -1;
		}

		/// <summary>
		/// Execute the command as scalar
		/// </summary>
		/// <param name="command">SQL-Command as string</param>
		/// <returns>Object, null if an error occurred</returns>
		public object ExecuteScalar(MySqlCommand command)
		{
			if (!IsConnected())
				return null;
			command.Connection = CurrentConnection;

			try
			{
				return command.ExecuteScalar();
			}
			catch (MySqlException e)
			{
				HandleException(e);
			}

			return null;
		}

		public object ExecuteScalar(string command)
		{
			return ExecuteCommand(new MySqlCommand(command));
		}

		/// <summary>
		/// Get the datatable from a command
		/// </summary>
		/// <param name="command">SQL-Command</param>
		/// <returns>DataTable</returns>
		public DataTable GetDataTable(string command)
		{
			if (!IsConnected())
				return new DataTable();

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
			
			return dataTable;
		}

		private static void HandleException(MySqlException mySqlException)
		{
			MySqlErrorCode errorCode;
			if (!Enum.TryParse(mySqlException.Number.ToString(), false, out errorCode))
			{
				Debug.WriteLine("Unable to parse exception: " + mySqlException.Message);
			}

			if (errorCode == MySqlErrorCode.DatabaseAccessDenied)
			{
				throw new DatabaseAccessDeniedExcpetion("Error-Code: " + errorCode);
			}
			if (errorCode == MySqlErrorCode.UnableToConnectToHost)
			{
				MessageBox.Show("Unable to reach server");
				return;
			}

			Debug.WriteLine("MySqlException: " + errorCode + "\tMessage: " + mySqlException.Message + "\tInner-Exception " + mySqlException.InnerException?.Message);
		}
	}
}
