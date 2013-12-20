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
	internal class Core
	{
		Stack<object> currentObjectStack = new Stack<object>();
		PropertyInfo currentProperty = null;
		object mainConf;
		object[] subConfs;

		/// <summary>
		/// Shortcut for currentObjectStack.Peek()
		/// </summary>
		private object currentObject { get { return currentObjectStack.Peek(); } }

		public IEnumerable<Converter> Converters { get; protected set; }

		public Core(params Converter[] converters)
		{
			var list = new List<Converter>(converters);
			list.Add(new SimpleTypeConverter());
			this.Converters = list;
		}

		/// <summary>
		/// Walk the whole parse tree and assign the configuration.
		/// </summary>
		public void WalkTree(ParseTree tree, object mainConf, params object[] subConfs)
		{
			this.mainConf = mainConf;
			this.subConfs = subConfs;
			if (!currentObjectStack.Any())
			{
				currentObjectStack.Push(mainConf);
			}
			WalkTree(tree);
		}

		/// <summary>
		/// Actual recursive function.
		/// Walk the whole parse tree and assign the configuration.
		/// </summary>
		private void WalkTree(ParseNode current)
		{
			//skip the children if the evaluation tell us to.
			if (Evaluation(current)) return;
			//recursively walk the children
			foreach (var child in current.Nodes)
			{
				WalkTree(child);
			}
		}

		/// <returns>returns true to indicate the children should be skipped</returns>
		private bool Evaluation(ParseNode node)
		{
			switch (node.Token.Type)
			{
				case TokenType.Namespace:
					//the actual change of scope is done with NamespaceBegin
					Helpers.GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
					break;
				case TokenType.NamespaceBegin:
					//switch to the sub config 
					currentObjectStack.Clear();
					currentObjectStack.Push(GetCorrespondingSubConf(Helpers.GetTextForNode(node, TokenType.NAME)));
					return true;
				case TokenType.NamespaceEnd:
					//return to main config
					currentObjectStack.Clear();
					currentObjectStack.Push(mainConf);
					return true;
				/**************************/
				case TokenType.SimpleDeclaration:
					currentProperty = Helpers.FindProperty(currentObject, Helpers.GetTextForNode(node, TokenType.NAME));
					break;
				case TokenType.QUOTEDCONTENT:
					Helpers.ConvertAndSetProperty(currentObject, currentProperty, Helpers.ParseQuotedString(node.Token.Text.Trim()), Converters);
					return true;
				case TokenType.SINGLELINECONTENT:
					Helpers.ConvertAndSetProperty(currentObject, currentProperty, node.Token.Text.Trim(), Converters);
					return true;
				/**************************/
				case TokenType.MultiLineDeclaration:
					currentProperty = Helpers.FindProperty(currentObject, node);
					string parsedMultiLineString = Helpers.ParseMultiLinesRawString(Helpers.GetTextForNode(node, TokenType.MULTILINECONTENT));
					Helpers.ConvertAndSetProperty(currentObject, currentProperty, parsedMultiLineString, Converters);
					return true;
				/**************************/
				//Complex Declaration
				case TokenType.TagBegin:
					string nextName = Helpers.GetTextForNode(node, TokenType.NAME);
					PropertyInfo nextProperty;
					//WARNING: if the current object is an IEnumerable and have a property with the same name as the one defined for the items in the enumerable,
					//the property will be set instead of added to the enumerable.
					//NOTE: actually this is not possible, since all the enumerable initalized by the configurator are Lists...
					bool propertyNameFound = Helpers.TryFindProperty(currentObject, nextName, out nextProperty);
					if (propertyNameFound)
					{
						currentProperty = nextProperty;
						currentObjectStack.Push(Helpers.GetOrCreateObject(currentObject, currentProperty));
					}
					else
					{
						//the name does not match any property...
						if (Helpers.IsEnumerable(currentObject.GetType()))
						{
							// add a new complex item to the current object, and set it as the new current
							var newObject = Helpers.CreateObject(currentObject.GetType().GenericTypeArguments.Single());
							IList collection = (IList)currentObject;
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
					currentProperty = Helpers.FindProperty(currentObject, node);
					break;
				case TokenType.MultiLineItem:
					string parsedMultiLineItem = Helpers.ParseMultiLinesRawString(Helpers.GetTextForNode(node, TokenType.MULTILINECONTENT));
					Helpers.ConvertAndAddToProperty(currentObject, currentProperty, parsedMultiLineItem, Converters);
					return true;
				case TokenType.SIMPLEITEM:
					Helpers.ConvertAndAddToProperty(currentObject, currentProperty, node.Token.Text.Trim(), Converters);
					return true;
				case TokenType.QUOTEDITEM:
					Helpers.ConvertAndAddToProperty(currentObject, currentProperty, Helpers.ParseQuotedString(node.Token.Text.Trim()), Converters);
					return true;
				/**************************/
				default:
					//ignored
					break;
			}
			return false; //the children will be evaluated
		}

		private object GetCorrespondingSubConf(string newPropertyName)
		{
			return (from subConf in this.subConfs
					where StringComparer.Ordinal.Equals(subConf.GetType().Name, newPropertyName)
					select subConf)
					.Single();
		}
	}
}
