using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Postulate.Mvc.Extensions
{
	public static class DynamicExtensions
	{
		/// <summary>
		/// Determine whether a type is simple (String, Decimal, DateTime, etc)
		/// or complex (i.e. custom class with public properties and methods).
		/// from https://gist.github.com/jonathanconway/3330614
		/// </summary>
		/// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
		public static bool IsSimpleType(this Type type)
		{
			return
				type.IsValueType ||
				type.IsPrimitive ||
				new Type[] {
				typeof(String),
				typeof(Decimal),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(Guid)
			}.Contains(type) ||
				Convert.GetTypeCode(type) != TypeCode.Object;
		}

		public static dynamic ToDynamic(this object anyObject)
		{
			// thanks to http://blog.jorgef.net/2011/06/converting-any-object-to-dynamic.html

			if (anyObject == null) return null;

			IDictionary<string, object> expando = new ExpandoObject();
			Type t = anyObject.GetType();
			PropertyInfo[] props = t.GetProperties();
			foreach (PropertyInfo p in props.Where(property => property.PropertyType.IsSimpleType())) expando.Add(p.Name, p.GetValue(anyObject, null));
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