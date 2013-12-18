using Configurator.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
	internal class EvaluationCore
	{
		Stack<object> currentObjectStack = new Stack<object>();
		PropertyInfo currentProperty = null;

		public IEnumerable<Converter> Converters { get; protected set; }

		public EvaluationCore(params Converter[] converters)
		{
			var list = new List<Converter>(converters);
			list.Add(new SimpleTypeConverter());
			this.Converters = list;
		}

		/// <summary>
		/// Walk the whole parse tree and assign the configuration.
		/// </summary>
		internal void WalkTree(ParseNode current, object mainConf, params object[] subConfs)
		{
			//skip the children if the evaluation tell us to.
			if (Evaluation(current, mainConf, subConfs)) return;
			//recursively walk the children
			foreach (var child in current.Nodes)
			{
				WalkTree(child, mainConf, subConfs);
			}
		}

		/// <returns>returns true to indicate the children should be skipped</returns>
		private bool Evaluation(ParseNode node, object mainConf, params object[] subConfs)
		{
			if (!currentObjectStack.Any())
			{
				currentObjectStack.Push(mainConf);
			}
			switch (node.Token.Type)
			{
				case TokenType.Namespace:
					//the actual change of scope is done with NamespaceBegin
					GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
					break;
				case TokenType.NamespaceBegin:
					//switch to the sub config 
					currentObjectStack.Clear();
					currentObjectStack.Push(GetNamespace(node.FindChild(TokenType.NAME).Text, subConfs));
					return true;
				case TokenType.NamespaceEnd:
					//return to main config
					currentObjectStack.Clear();
					currentObjectStack.Push(mainConf);
					return true;
				/**************************/
				case TokenType.SimpleDeclaration:
					currentProperty = FindProperty(currentObjectStack.Peek(), node.FindChild(TokenType.NAME).Text);
					break;
				case TokenType.QUOTEDCONTENT:
					ConvertAndSetProperty(currentObjectStack.Peek(), currentProperty, ParseQuotedString(node.Token.Text));
					return true;
				case TokenType.SINGLELINECONTENT:
					ConvertAndSetProperty(currentObjectStack.Peek(), currentProperty, node.Token.Text);
					return true;
				/**************************/
				case TokenType.MultiLineDeclaration:
					currentProperty = FindProperty(currentObjectStack.Peek(), node);
					ConvertAndSetProperty(currentObjectStack.Peek(), currentProperty, node.FindChild(TokenType.MULTILINECONTENT).Text);
					return true;
				/**************************/
				//Complex Declaration
				case TokenType.TagBegin:
					string nextName = node.FindChild(TokenType.NAME).Text;
					PropertyInfo nextProperty;
					//WARNING: if the current object is an IEnumerable and have a property with the same name as the one defined for the items in the enumerable,
					//the property will be set instead of added to the enumerable.
					//NOTE: actually this is not possible, since all the enumerable initalized by the configurator are Lists...
					bool propertyNameFound = TryFindProperty(currentObjectStack.Peek(), nextName, out nextProperty);
					if (propertyNameFound)
					{
						currentProperty = nextProperty;
						var newObject = GetNewCurrentObject(currentObjectStack.Peek(), nextName);
						SetProperty(currentObjectStack.Peek(), currentProperty, newObject);
						currentObjectStack.Push(newObject);
					}
					else
					{
						//the name does not match any property...
						//if the current objet is an enumerable
						if (typeof(IEnumerable).IsAssignableFrom(currentObjectStack.Peek().GetType()))
						{
							// add a new complex item to the current object, and set it as the new current
							var newObject = GetNewCurrentObject(currentObjectStack.Peek());
							IList collection = (IList)currentObjectStack.Peek();
							collection.Add(newObject);
							currentObjectStack.Push(newObject);
						}
						else
						{
							throw new EvaluationException("No matching property found for a complex declaration!", node.Token);
						}
					}

					return true;
				case TokenType.TagEnd:
					currentObjectStack.Pop();
					return true;
				/**************************/
				case TokenType.ListDeclaration:
					currentProperty = FindProperty(currentObjectStack.Peek(), node);
					break;
				case TokenType.MultiLineItem:
					ConvertAndAddToProperty(currentObjectStack.Peek(), currentProperty, node.FindChild(TokenType.MULTILINECONTENT).Text);
					return true;
				case TokenType.SIMPLEITEM:
					ConvertAndAddToProperty(currentObjectStack.Peek(), currentProperty, node.Token.Text);
					return true;
				case TokenType.QUOTEDITEM:
					ConvertAndAddToProperty(currentObjectStack.Peek(), currentProperty, ParseQuotedString(node.Token.Text));
					return true;
				/**************************/
				default:
					//ignored
					break;
			}
			return false; //the children will be evaluated
		}

		private PropertyInfo FindProperty(object currentObject, ParseNode node)
		{
			string name = GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
			return FindProperty(currentObject, name);
		}

		private PropertyInfo FindProperty(object currentObject, string newPropertyName)
		{
			return (from p in currentObject.GetType().GetProperties()
					where StringComparer.Ordinal.Equals(p.Name, newPropertyName)
					select p).Single();
		}

		private bool TryFindProperty(object currentObject, string newPropertyName, out PropertyInfo result)
		{
			var potentialResults = from p in currentObject.GetType().GetProperties()
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

		private IList GetOrCreateCollection(PropertyInfo currentProperty, object currentObject)
		{
			IList collection = (IList)currentProperty.GetValue(currentObject);
			if (collection == null)
			{
				collection = CreateCollection(currentProperty.PropertyType.GenericTypeArguments.Single());
				currentProperty.SetValue(currentObject, collection);
			}
			return collection;
		}

		private IList CreateCollection(Type ofType)
		{
			//even though the property type can be an IEnumerable, an IList or an ICollection,
			//the auto generated type is a List of the specified type.
			Type listType = typeof(List<>).MakeGenericType(ofType);
			return (IList)Activator.CreateInstance(listType);
		}

		private void ConvertAndSetProperty(object currentObject, PropertyInfo currentProperty, string value)
		{
			var convertedValue = ConvertValue(value, currentProperty.PropertyType);
			SetProperty(currentObject, currentProperty, convertedValue);

		}
		private void SetProperty(object currentObject, PropertyInfo currentProperty, object value)
		{
			if (currentProperty.CanWrite)
				currentProperty.SetValue(currentObject, value);
			else
				throw new EvaluationException(string.Format("Cannot set the value of a read-only property! ({0})", currentProperty.ToString()));
		}

		private void ConvertAndAddToProperty(object currentObject, PropertyInfo currentProperty, string value)
		{
			var convertedValue = ConvertValue(value, currentProperty.PropertyType.GenericTypeArguments.Single());
			var collection = GetOrCreateCollection(currentProperty, currentObject);
			collection.Add(convertedValue);
		}
		private void AddToProperty(object currentObject, PropertyInfo currentProperty, object value)
		{
			var collection = GetOrCreateCollection(currentProperty, currentObject);
			collection.Add(value);
		}

		private object ConvertValue(string value, Type targetType)
		{
			object convertedValue;
			if (targetType != typeof(string) && targetType != typeof(object))
			{
				var matchingConverter = Converters.Where(x => x.CanConvert(targetType)).FirstOrDefault();
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

		private object GetNewCurrentObject(object currentObject, string newPropertyName)
		{
			var property = (from p in currentObject.GetType().GetProperties()
							where StringComparer.Ordinal.Equals(p.Name, newPropertyName)
							select p).Single();
			object newObject = property.GetValue(currentObject);
			if (newObject == null)
			{
				if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
				{
					//create a new enumerable
					newObject = GetOrCreateCollection(property, currentObject);
				}
				else
				{
					newObject = Activator.CreateInstance(property.PropertyType);
				}
			}
			return newObject;
		}

		private object GetNewCurrentObject(object currentObject)
		{
			Type targetType = currentObject.GetType().GenericTypeArguments.Single();
			object newObject;
			if (typeof(IEnumerable).IsAssignableFrom(targetType))
			{
				//create a new enumerable
				newObject = CreateCollection(targetType.GetGenericArguments().Single());
			}
			else
			{
				newObject = Activator.CreateInstance(targetType);
			}
			return newObject;
		}

		private object GetNamespace(string newPropertyName, params object[] subConfs)
		{
			return (from subConf in subConfs
					where StringComparer.Ordinal.Equals(subConf.GetType().Name, newPropertyName)
					select subConf).Single();
		}

		private string GetAndValidateTagsNames(ParseNode openTag, ParseNode closeTag)
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

		private string ParseQuotedString(string content)
		{
			//remove enclosing quotes, then replace pairs of quotes by just one quote.
			return content.Substring(1, content.Length - 2).Replace("\"\"", "\"");
		}
	}
}
