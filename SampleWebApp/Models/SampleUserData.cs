using Postulate.Mvc.Abstract;

namespace SampleWebApp.Models
{
	public class SampleUserData : UserData
	{
		public override string Filename => "Sample.json";

		public string Greeting { get; set; } = "Hello";
		public string Farewell { get; set; } = "Whatever";
	}
}