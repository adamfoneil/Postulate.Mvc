using Newtonsoft.Json;
using System;
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

        public static T Load<T>(HttpServerUtilityBase server, string userName, T defaultInstance = null) where T : UserData, new()
        {
			var userData = defaultInstance ?? new T();

			string saveFile = SaveFilename(server, userName, userData.Filename);

            if (File.Exists(saveFile))
            {				
				using (StreamReader reader = File.OpenText(saveFile))
                {
                    string json = reader.ReadToEnd();
                    userData = JsonConvert.DeserializeObject<T>(json);
                }
            }			

            userData.Server = server;
            userData.UserName = userName;
            return userData;
        }

        public void Save()
        {
            string userDir = SaveFolder(Server, UserName);
            if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);

            string json = JsonConvert.SerializeObject(this);
            using (StreamWriter writer = File.CreateText(SaveFilename(Server, UserName, Filename))) { writer.Write(json); }
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