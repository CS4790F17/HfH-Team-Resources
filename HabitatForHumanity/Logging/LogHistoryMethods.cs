using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using static HabitatForHumanity.Logging.ConnectionStrings;
using static HabitatForHumanity.Logging.LoggingMethods;

namespace HabitatForHumanity.Logging {
	public static class LogHistoryMethods {

		//!Infinite loop warning! Use ONLY Console logging in this class.

		private static string table = "LogHistory";

		//returns RS<string | int>.
		public static ResultStatus insertLogLineWithObject(LogLine line,
				string connectionString = logConnStr) {

			int rows = 0;
			string query = $"INSERT INTO [{table}] " +
				"([timeStamp], fileName, functionName, lineNumber, message, statusCode, extraData) " +
				"VALUES (@now, @file, @function, @line, @eText, @code, @extraData)";

			using (OleDbConnection conn = new OleDbConnection(connectionString)) {
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(query, conn);
				cmd.Parameters.AddWithValue("@now", line.timestamp.ToString());
				cmd.Parameters.AddWithValue("@file", line.fileName);
				cmd.Parameters.AddWithValue("@function", line.functionName);
				cmd.Parameters.AddWithValue("@line", line.lineNumber);
				cmd.Parameters.AddWithValue("@eText", line.message);
				cmd.Parameters.AddWithValue("@code", line.statusCode);
				cmd.Parameters.AddWithValue("@extraData", line.extraData);

				try {
					rows = cmd.ExecuteNonQuery();
				}
				catch (Exception e) {
					logConsoleError(e, -1); //ignore return.
					return new ResultStatus(-1, e.Message);
				}
			}

			if (rows == 1)
				return new ResultStatus(0, rows);
			else {
				logConsoleDiag($"Unexpected number of rows altered: {rows}.", -2); //ignore return.
				return new ResultStatus(-2, $"Unexpected number of rows altered: {rows}.");
			}
		}

		//returns RS<string | int>.
		public static ResultStatus insertLogLinesWithCollection(ICollection<LogLine> coll,
				string connectionString = logConnStr) {
			int sum = 0; bool error = false; ResultStatus stat;

			foreach (LogLine line in coll) {
				stat = insertLogLineWithObject(line, connectionString);
				if (stat.resultCode == 0) sum += 1;
				else error = true;
			}

			if (!error)
				return new ResultStatus(0, sum);
			else {
				logConsoleDiag($"Unexpected number of rows altered: {sum}.", -2); //ignore return.
				return new ResultStatus(-2, $"Unexpected number of rows altered: {sum}.");
			}
		}

		//returns RS<string | LogLine>.
		public static ResultStatus findLogLineWithID(int id,
				string connectionString = logConnStr) {
			LogLine line = new LogLine();

			try {
				ResultStatus stat = getLogLines(connectionString); //RS<ICollection<LogLine>>.
				if (stat.resultCode == 0)
					line = ((ICollection<LogLine>)stat.innerMessage)
							.Where(l => l.logID == id).Single();
				else {
					logConsoleDiag("Couldn't get collection of rows.", -2); //ignore return.
					return new ResultStatus(-2, "Error getting rows.");
				}
			}
			catch (Exception e) {
				logConsoleError(e, -1); //ignore return.
				return new ResultStatus(-2, "Something went wrong with the collection.");
			}

			return new ResultStatus(0, line);
		}

		//returns RS<string | ICollection<LogLine>>.
		public static ResultStatus findLogLinesByDate(DateTime date1, DateTime date2,
				string connectionString = dbConnStr) {
			IEnumerable<LogLine> list = new List<LogLine>();

			try {
				ResultStatus stat = getLogLines(connectionString); //RS<ICollection<LogLine>>.
				if (stat.resultCode == 0)
					list = ((ICollection<LogLine>)stat.innerMessage)
						.Where(l => l.timestamp.CompareTo(date1) >= 0)
						.Where(l => l.timestamp.CompareTo(date2) < 0);
				else throw new Exception("Couldn't get collection of rows.");
			}
			catch (Exception e) {
				logConsoleError(e, -1); //ignore return;
				return new ResultStatus(-2, "Something went wrong with the collection.");
			}

			return new ResultStatus(0, (ICollection<LogLine>)list);
		}

		//returns RS<string | ICollection<LogLine>>
		public static ResultStatus getLogLines(
				string connectionString = logConnStr) {
			ICollection<LogLine> list = new List<LogLine>();
			string query = $"SELECT * FROM [{table}] ORDER BY [timestamp] DESC";

			DataSet ds = new DataSet();
			OleDbConnection conn = new OleDbConnection(connectionString);
			OleDbCommand cmd = new OleDbCommand(query, conn);
			OleDbDataAdapter da = new OleDbDataAdapter(cmd);

			try {
				da.Fill(ds);
				ResultStatus stat = mapLogLines(ds); //RS<ICollection<LogLine>>.
				if (stat.resultCode == 0)
					list = (ICollection<LogLine>)stat.innerMessage;
				else {
					logConsoleDiag("Problem mapping collection of rows.", -2); //ignore return.
					return new ResultStatus(-2, "Error mapping rows.");
				}
			}
			catch (Exception e) {
				logConsoleError(e, -1); //ignore return.
				return new ResultStatus(-2, e.Message);
			}

			return new ResultStatus(0, list);
		}

		//Purposely omitting updateLogLinesWithObject.

		//Purposely omitting updateLogLinesWithCollection.

		//returns RS<string | int>.
		public static ResultStatus deleteLogLineWithID(int id,
				string connectionString = logConnStr) {
			int rows = 0;
			string query = $"DELETE FROM [{table}] WHERE logID = @logID";

			using (OleDbConnection conn = new OleDbConnection(connectionString)) {
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(query, conn);
				cmd.Parameters.AddWithValue("@logID", id);

				try {
					rows = cmd.ExecuteNonQuery();
				}
				catch (Exception e) {
					logConsoleError(e, -1); //ignore return.
					return new ResultStatus(-1, e.Message);
				}
			}

			if (rows == 1)
				return new ResultStatus(0, rows);
			else {
				logConsoleDiag($"Unexpected number of rows altered: {rows}.", -2); //ignore return.
				return new ResultStatus(-2, $"Unexpected number of rows altered: {rows}.");
			}
		}

		//returns RS<string | int>.
		public static ResultStatus deleteLogLinesWithCollection(ICollection<LogLine> coll,
				string connectionString = logConnStr) {
			int sum = 0; bool error = false;

			foreach (LogLine line in coll) {
				ResultStatus stat = deleteLogLineWithID(line.logID, connectionString);
				if (stat.resultCode == 0) sum += 1;
				else error = true;
			}

			if (!error)
				return new ResultStatus(0, sum);
			else {
				logConsoleDiag($"Unexpected number of rows altered: {sum}.", -2); //ignore return.
				return new ResultStatus(-2, $"Unexpected number of rows altered: {sum}.");
			}
		}

		//returns RS<string | int>
		public static ResultStatus deleteLogLinesFromDate(DateTime date,
				string connectionString = logConnStr) {
			int sum = 0; bool error = false; ResultStatus stat;
			IEnumerable<LogLine> coll = new List<LogLine>();

			try {
				stat = getLogLines();
				if (stat.resultCode == 0) {
					coll = ((ICollection<LogLine>)stat.innerMessage)
						.Where(l => l.timestamp.CompareTo(date) < 0);
				}
			}
			catch (Exception e) {
				logConsoleError(e, -1); //ignore return.
				return new ResultStatus(-2, "Couldn't get collection of rows.");
			}

			foreach (LogLine line in coll) {
				stat = deleteLogLineWithID(line.logID, connectionString);
				if (stat.resultCode == 0) sum += 1;
				else error = true;
			}

			if (!error)
				return new ResultStatus(0, sum);
			else {
				logConsoleDiag($"Unexpected number of rows altered: {sum}.", -2); //ignore return.
				return new ResultStatus(-2, $"Unexpected number of rows altered: {sum}.");
			}
		}

		//returns RS<LogLine>.
		private static ResultStatus mapLogLine(DataRow dr) {
			LogLine line = new LogLine();
			int tmp; DateTime tmp2; bool error = false;

			if (!int.TryParse(dr["logID"].ToString(), out tmp)) {
				line.logID = -1; error = true;
				logConsoleDiag("Error parsing logID", -1); //ignore return.
			}
			else line.logID = tmp;

			if (!int.TryParse(dr["lineNumber"].ToString(), out tmp)) {
				line.lineNumber = -1; error = true;
				logConsoleDiag("Error parsing lineNumber", -1); //ignore return.
			}
			else line.lineNumber = tmp;

			if (!int.TryParse(dr["statusCode"].ToString(), out tmp)) {
				line.statusCode = -1; error = true;
				logConsoleDiag("Error parsing statusCode", -1);
			}
			else line.statusCode = tmp;

			if (!DateTime.TryParse(dr["timeStamp"].ToString(), out tmp2)) {
				line.timestamp = new DateTime(); error = true;
				logConsoleDiag("Error parsing timeStamp", -1); //ignore return.
			}
			else line.timestamp = tmp2;

			line.fileName = dr["fileName"].ToString();
			line.functionName = dr["functionName"].ToString();
			line.message = dr["message"].ToString();
			line.extraData = dr["extraData"].ToString();

			if (!error)
				return new ResultStatus(0, line);
			else return new ResultStatus(-1, line);
		}

		//returns RS<string | ICollection<LogLine>>
		private static ResultStatus mapLogLines(DataSet ds) {
			ICollection<LogLine> list = new List<LogLine>();
			LogLine line; bool error = false; ResultStatus stat;

			try {
				foreach (DataRow row in ds.Tables[0].Rows) {
					stat = mapLogLine(row);
					line = (LogLine)stat.innerMessage;
					list.Add(line);
					if (stat.resultCode != 0)
						error = true;
				}
			}
			catch (Exception e) {
				logConsoleError(e, -1); //ignore return.
				return new ResultStatus(-2, e.Message);
			}

			if (!error)
				return new ResultStatus(0, list);
			else {
				logConsoleDiag("Something went wrong with mapping a line.", -2); //ignore return.
				return new ResultStatus(-2, "Something went wrong with mapping a line.");
			}
		}

	}
}
