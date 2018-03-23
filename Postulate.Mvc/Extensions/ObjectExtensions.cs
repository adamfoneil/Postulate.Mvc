using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Postulate.Mvc.Extensions
{
	//thanks to http://stackoverflow.com/questions/4943817/mapping-object-to-dictionary-and-vice-versa

	public static class ObjectExtensions
	{
		public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
		{
			T @object = new T();
			Type someObjectType = @object.GetType();

			foreach (KeyValuePair<string, object> item in source)
			{
				someObjectType.GetProperty(item.Key).SetValue(@object, item.Value, null);
			}

			return @object;
		}

		public static IDictionary<string, object> ConvertToDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
		{
			return source.GetType().GetProperties(bindingAttr).Where(p => DynamicExtensions.IsSimpleType(p.PropertyType)).ToDictionary
			(
				propInfo => propInfo.Name,
				propInfo => propInfo.GetValue(source, null)
			);
		}
	}
}