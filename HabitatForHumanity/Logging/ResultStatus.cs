namespace HabitatForHumanity.Logging {
	public class ResultStatus {
		//this is supposed to act like a struct.

		public int resultCode { get; set; }

		public object innerMessage { get; set; }

		public ResultStatus() { }

		public ResultStatus(int code, object data) {
			resultCode = code;
			innerMessage = data;
		}

	}
}
