using System;
using System.Diagnostics; //.StackFrame, .Debug
using System.IO; //.FileInfo
using static HabitatForHumanity.Logging.ConnectionStrings;
using static HabitatForHumanity.Logging.LogHistoryMethods;

namespace HabitatForHumanity.Logging {
	public static class LoggingMethods {

		//returns RS<string | int>.
		public static ResultStatus logError(Exception ex, int statusCode = 1,
				string additionalInformation = "", string connectionString = logConnStr) {
			StackFrame stackFrame = new StackFrame(1, true);
			FileInfo temp = new FileInfo(stackFrame.GetFileName());
			string functionName = stackFrame.GetMethod().Name.ToString(),
					fileName = temp.Name, exStr = ex.ToString(), sea = ":line ",
					loc = exStr.Substring(exStr.IndexOf(sea)).Replace(sea, "");
			//see also String.LastIndexOf() if needed..
			int line = -1;
			if (!int.TryParse(loc, out line)) {
				line = stackFrame.GetFileLineNumber();
			}

			LogLine thing = new LogLine();
			thing.timestamp = DateTime.Now;
			thing.fileName = fileName;
			thing.functionName = functionName;
			thing.lineNumber = line;
			thing.message = ex.Message;
			thing.statusCode = statusCode;
			thing.extraData = additionalInformation;

			return insertLogLineWithObject(thing);
		}

		//returns RS<string | int>.
		public static ResultStatus logDiagnostic(string message, int statusCode = -1,
				string additionalInformation = "", string connectionString = logConnStr) {
			StackFrame stackFrame = new StackFrame(1, true);
			string functionName = stackFrame.GetMethod().Name.ToString();
			int line = stackFrame.GetFileLineNumber();

			LogLine thing = new LogLine();
			thing.timestamp = DateTime.Now;
			thing.fileName = "Diagnostic";
			thing.functionName = functionName;
			thing.lineNumber = line;
			thing.message = message;
			thing.statusCode = statusCode;
			thing.extraData = additionalInformation;

			return insertLogLineWithObject(thing);
		}

		//returns RS<string>.
		public static ResultStatus logConsoleError(Exception ex, int statusCode = 1, string additionalInformation = "") {
			StackFrame frame = new StackFrame(1, true);
			FileInfo temp = new FileInfo(frame.GetFileName());

			string functionName = frame.GetMethod().Name.ToString(),
				fileName = temp.Name, exStr = ex.ToString(), sea = ":line ",
				loc = exStr.Substring(exStr.IndexOf(sea)).Replace(sea, "");
			//see also String.LastIndexOf() if needed.
			int line = -1;
			if (!int.TryParse(loc, out line)) {
				line = frame.GetFileLineNumber();
			}

			string debug = $"{DateTime.Now.ToString()}, {fileName}:{functionName}:{line}; " +
						$"{ex.Message}; Code: {statusCode}; {additionalInformation}.";

			Debug.WriteLine(debug);
			return new ResultStatus(0, "All's well.");
		}

		//returns RS<string>.
		public static ResultStatus logConsoleDiag(string message, int statusCode = -1, string additionalInformation = "") {
			StackFrame frame = new StackFrame(1, true);
			int line = frame.GetFileLineNumber();
			string functionName = frame.GetMethod().Name.ToString(),
				debug = $"{DateTime.Now.ToString()}, Diagnostic:{functionName}:{line}; " +
						$"{message}; Code: {statusCode}; {additionalInformation}.";
			Debug.WriteLine(debug);
			return new ResultStatus(0, "All's well.");
		}

	}
}
