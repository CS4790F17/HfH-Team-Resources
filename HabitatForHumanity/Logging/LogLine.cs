using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitatForHumanity.Logging {

	[Table("LogHistory")]
	public class LogLine {
		[Key]
		public int logID { get; set; }

		[Required, DataType(DataType.DateTime)]
		[Display(Name = "Timestamp")]
		public DateTime timestamp { get; set; }

		[Required, StringLength(150)]
		[Display(Name = "File Name")]
		public string fileName { get; set; }

		[Required, StringLength(100)]
		[Display(Name = "Function Name")]
		public string functionName { get; set; }

		[Required, Display(Name = "Line Number")]
		public int lineNumber { get; set; }

		[Display(Name = "Message"), StringLength(255)]
		public string message { get; set; }

		[Required, Display(Name = "Status Code")]
		public int statusCode { get; set; }

		[Display(Name = "Other Info."), StringLength(255)]
		public string extraData { get; set; }

		public override string ToString() {
			string ret = "";
			ret += $"Timestamp: {timestamp}, ";
			ret += $"File Name: {fileName}, ";
			ret += $"Function Name: {functionName}, ";
			ret += $"Line Number: {lineNumber}, ";
			ret += $"Message: {message}, ";
			ret += $"StatusCode: {statusCode}, ";
			ret += $"Other Data: {extraData};";
			return ret;
		}

	}

}
