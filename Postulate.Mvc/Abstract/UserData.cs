using Newtonsoft.Json;
using System.IO;
using System.Web;

namespace Postulate.Mvc.Abstract
{
	public abstract class UserData
	{
		[JsonIgnore]
		public string UserName { get; protected set; }

		[JsonIgnore]
		public HttpServerUtilityBase Server { get; protected set; }

		protected UserData()
		{
		}

		[JsonIgnore]
		public abstract string Filename { get; }

		public static T Load<T>(string fileName) where T : UserData, new()
		{
			if (File.Exists(fileName))
			{
				return LoadInner<T>(fileName);
			}

			return new T();
		}

		public void Save(string fileName)
		{
			SaveInner(fileName);
		}

		public static T Load<T>(HttpServerUtilityBase server, string userName, T defaultInstance = null) where T : UserData, new()
		{
			var userData = defaultInstance ?? new T();

			string saveFile = SaveFilename(server, userName, userData.Filename);
			if (File.Exists(saveFile))
			{
				userData = LoadInner<T>(saveFile);
			}
			
			userData.Server = server;
			userData.UserName = userName;
			return userData;
		}

		private static T LoadInner<T>(string fileName) where T : UserData, new()
		{			
			using (StreamReader reader = File.OpenText(fileName))
			{
				string json = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<T>(json);
			}			
		}

		public void Save()
		{
			string userDir = SaveFolder(Server, UserName);
			if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);
			SaveInner(SaveFilename(Server, UserName, Filename));
		}

		private void SaveInner(string fileName)
		{
			string json = JsonConvert.SerializeObject(this);
			using (StreamWriter writer = File.CreateText(fileName)) { writer.Write(json); }
		}

		private static string SaveFolder(HttpServerUtilityBase server, string userName)
		{
			return Path.Combine(server.MapPath("~/App_Data/UserData"), userName);
		}

		private static string SaveFilename(HttpServerUtilityBase server, string userName, string fileName)
		{
			return Path.Combine(SaveFolder(server, userName), fileName);
		}
	}
}