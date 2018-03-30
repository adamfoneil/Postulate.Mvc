using Postulate.Orm.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Postulate.Mvc.Extensions
{
	public static class DynamicExtensions
	{
		public static dynamic ToDynamic(this object @object)
		{
			// thanks to http://blog.jorgef.net/2011/06/converting-any-object-to-dynamic.html

			if (@object == null) return null;

			IDictionary<string, object> expando = new ExpandoObject();
			Type t = @object.GetType();
			PropertyInfo[] props = t.GetProperties();
			foreach (PropertyInfo p in props.Where(property => property.PropertyType.IsSimpleType())) expando.Add(p.Name, p.GetValue(@object, null));
			return expando as ExpandoObject;
		}

		public static bool HasProperty(this object value, string propertyName)
		{
			// thanks to http://stackoverflow.com/questions/8640927/checking-to-see-if-viewbag-has-a-property-or-not-to-conditionally-inject-javasc

			var dyn = value as DynamicObject;
			if (dyn == null) return false;
			return dyn.GetDynamicMemberNames().Contains(propertyName);
		}
	}
}