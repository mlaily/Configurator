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
		object currentObject = null;
		PropertyInfo currentProperty = null;

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
			currentObject = mainConf;
			switch (node.Token.Type)
			{
				case TokenType.Namespace:
					//the actual change of scope is done with NamespaceBegin
					GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
					break;
				case TokenType.NamespaceBegin:
					//switch to the sub config 
					currentObject = GetNamespace(node.FindChild(TokenType.NAME).Text, subConfs);
					return true;
				case TokenType.NamespaceEnd:
					//return to main config
					currentObject = mainConf;
					return true;
				case TokenType.SimpleDeclaration:
					currentProperty = FindProperty(currentObject, node.FindChild(TokenType.NAME).Text);
					break;
				case TokenType.QUOTEDCONTENT:
					SetProperty(currentObject, currentProperty, ParseQuotedString(node.Token.Text));
					return true;
				case TokenType.SINGLELINECONTENT:
					SetProperty(currentObject, currentProperty, node.Token.Text);
					return true;
				case TokenType.MultiLineDeclaration:
					currentProperty = FindProperty(currentObject, node);
					SetProperty(currentObject, currentProperty, node.FindChild(TokenType.MULTILINECONTENT));
					return true;
				case TokenType.ComplexDeclaration:
					ChangeCurrentObject(ref currentObject, node);
					break;
				case TokenType.ListDeclaration:
					currentProperty = FindProperty(currentObject, node);
					break;
				case TokenType.MultiLineItem:
					SetProperty(currentObject, currentProperty, node.FindChild(TokenType.MULTILINECONTENT), addToList: true);
					return true;
				case TokenType.SIMPLEITEM:
					SetProperty(currentObject, currentProperty, node.Token.Text, addToList: true);
					return true;
				case TokenType.QUOTEDITEM:
					SetProperty(currentObject, currentProperty, ParseQuotedString(node.Token.Text), addToList: true);
					return true;
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

		private void SetProperty(object currentObject, PropertyInfo currentProperty, object value, bool addToList = false)
		{
			if (addToList)
			{
				IList collection = (IList)currentProperty.GetValue(currentObject);
				if (collection == null)
				{
					//even though the property type can be an IEnumerable, an IList or an ICollection,
					//the auto generated type is a List of the specified type.
					Type listType = typeof(List<>).MakeGenericType(currentProperty.PropertyType.GenericTypeArguments);
					collection = (IList)Activator.CreateInstance(listType);
				}
				collection.Add(value);
			}
			else
			{
				if (currentProperty.CanWrite)
					currentProperty.SetValue(currentObject, value);
				else
					throw new EvaluationException(string.Format("Cannot set the value of a read-only property! ({0})", currentProperty.ToString()));
			}
		}

		private void ChangeCurrentObject(ref object currentObject, ParseNode node)
		{
			string name = GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
			ChangeCurrentObject(ref currentObject, name);
		}

		private void ChangeCurrentObject(ref object currentObject, string newPropertyName)
		{
			var property = (from p in currentObject.GetType().GetProperties()
							where StringComparer.Ordinal.Equals(p.Name, newPropertyName)
							select p).Single();
			object newObject = property.GetValue(currentObject);
			if (newObject == null)
			{
				newObject = Activator.CreateInstance(property.PropertyType);
			}
			currentObject = newObject;
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
