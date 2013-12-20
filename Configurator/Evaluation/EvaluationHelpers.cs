using Configurator.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Configurator.Evaluation
{
	internal static class Helpers
	{

		public static bool IsEnumerable(Type type)
		{
			return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
		}

		#region Find Property

		public static PropertyInfo FindProperty(object obj, ParseNode node)
		{
			string name = GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
			return FindProperty(obj, name);
		}

		public static PropertyInfo FindProperty(object obj, string newPropertyName)
		{
			PropertyInfo propertyInfo;
			if (TryFindProperty(obj, newPropertyName, out propertyInfo))
				return propertyInfo;
			else throw new EvaluationException("The specified property name does not match any property in the given object!");
		}

		public static bool TryFindProperty(object obj, string newPropertyName, out PropertyInfo result)
		{
			var potentialResults = from p in obj.GetType().GetProperties()
								   where StringComparer.Ordinal.Equals(p.Name, newPropertyName)
								   select p;
			if (potentialResults.Any())
			{
				result = potentialResults.Single();
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		#endregion

		#region Convert and Set Property

		public static void ConvertAndSetProperty(object currentObject, PropertyInfo currentProperty, string value, IEnumerable<Converter> converters)
		{
			var convertedValue = ConvertValue(value, currentProperty.PropertyType, converters);
			if (currentProperty.CanWrite)
				currentProperty.SetValue(currentObject, convertedValue);
			else
				throw new EvaluationException(string.Format("Cannot set the value of a read-only property! ({0})", currentProperty.ToString()));
		}

		public static void ConvertAndAddToProperty(object currentObject, PropertyInfo currentProperty, string value, IEnumerable<Converter> converters)
		{
			var convertedValue = ConvertValue(value, currentProperty.PropertyType.GenericTypeArguments.Single(), converters);
			var collection = (IList)GetOrCreateObject(currentObject, currentProperty);
			collection.Add(convertedValue);
		}

		private static object ConvertValue(string value, Type targetType, IEnumerable<Converter> converters)
		{
			object convertedValue;
			if (targetType != typeof(string) && targetType != typeof(object))
			{
				var matchingConverter = converters.Where(x => x.CanConvert(targetType)).FirstOrDefault();
				if (matchingConverter == null)
					throw new NotImplementedException(string.Format("No Converter for the type {0}", targetType));
				convertedValue = matchingConverter.Convert(value, targetType);
			}
			else
			{
				convertedValue = value;
			}
			return convertedValue;
		}

		#endregion

		#region Create new objects and collections

		/// <summary>
		/// Get the value of the property defined on obj,
		/// or initialize the property to a new instance of the type of the property if its value is null.
		/// Collection properties are correctly initialized to new empty collections.
		/// Collection properties of the newly created object are also recursively initialized to empty collections.
		/// </summary>
		public static object GetOrCreateObject(object obj, PropertyInfo property)
		{
			object newObject = property.GetValue(obj);
			if (newObject == null)
			{
				if (IsEnumerable(property.PropertyType))
				{
					//create a new enumerable
					newObject = CreateCollection(property.PropertyType.GenericTypeArguments.Single());
				}
				else
				{
					newObject = CreateObject(property.PropertyType);
				}
				property.SetValue(obj, newObject);
			}
			return newObject;
		}

		/// <summary>
		/// Create a generic IList for the given type.
		/// </summary>
		public static IList CreateCollection(Type ofType)
		{
			//even though the property type can be an IEnumerable, an IList or an ICollection,
			//the auto generated type is a List of the specified type.
			Type listType = typeof(List<>).MakeGenericType(ofType);
			return (IList)Activator.CreateInstance(listType);
		}

		/// <summary>
		/// Create a new instance of the specified type, using its default ctor.
		/// The collection properties of the new object are initialized to empty instances of collections.
		/// </summary>
		public static object CreateObject(Type targetType)
		{
			object newObject = Activator.CreateInstance(targetType);
			InitializeProperties(newObject);
			return newObject;
		}

		/// <summary>
		/// Initialize all the null collection properties of the object to new collections of the correct type.
		/// </summary>
		private static void InitializeProperties(object obj)
		{
			var allProperties = from p in obj.GetType().GetProperties()
								where p.CanRead && p.CanWrite
								select p;
			var collections = from p in allProperties
							  where IsEnumerable(p.PropertyType)
							  select p;
			var others = allProperties.Except(collections);
			foreach (var item in collections)
			{
				if (item.GetValue(obj) == null)
				{
					GetOrCreateObject(obj, item);
				}
			}
			//does not currently work if a type does not have a parameterless ctor
			//foreach (var item in others)
			//{
			//	if (item.GetValue(obj) == null)
			//	{
			//		item.SetValue(obj, Activator.CreateInstance(item.PropertyType));
			//	}
			//}
		}

		#endregion

		#region Parsing And Validation

		public static string GetTextForNode(ParseNode node, params TokenType[] validMatches)
		{
			return node.FindChild(validMatches).Text;
		}

		/// <summary>
		/// Check whether the NAME tokens of the open and close tags match, and return the name on success.
		/// Throws an exception otherwise.
		/// </summary>
		public static string GetAndValidateTagsNames(ParseNode openTag, ParseNode closeTag)
		{
			var openTagName = openTag.FindChild(TokenType.NAME);
			var closeTagName = closeTag.FindChild(TokenType.NAME);
			if (StringComparer.Ordinal.Equals(openTagName.Text, closeTagName.Text))
			{
				return openTagName.Text;
			}
			else
			{
				throw new EvaluationException("Tag Names don't match!", openTag.Token);
			}
		}

		/// <summary>
		/// Remove enclosing quotes, then replace pairs of quotes by just one quote.
		/// </summary>
		public static string ParseQuotedString(string content)
		{
			return content.Substring(1, content.Length - 2).Replace("\"\"", "\"");
		}

		/// <summary>
		/// Remove first and last new lines if present.
		/// Tabs and spaces before and after the first and last new lines are ignored.
		/// </summary>
		public static string ParseMultiLinesRawString(string content)
		{
			if (Regex.IsMatch(content, @"^\s*$"))
			{
				return content;
			}
			else
			{
				var m = Regex.Match(content, @"([\t\x20]*\r?\n)?(?<text>.+?)(\r?\n[\t\x20]*)?$", RegexOptions.Singleline);
				if (m.Success)
					return m.Groups["text"].Value;
				else
					return content;
			}
		}

		#endregion

	}
}
