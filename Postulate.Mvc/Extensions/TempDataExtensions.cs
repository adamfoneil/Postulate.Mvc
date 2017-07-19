using System.Web.Mvc;

namespace Postulate.Mvc.Extensions
{
    public static class TempDataExtensions
    {
        public static void RemoveAndAdd(this TempDataDictionary tempData, string key, object value)
        {
            if (tempData.ContainsKey(key)) tempData.Remove(key);
            tempData.Add(key, value);
        }
    }
}
