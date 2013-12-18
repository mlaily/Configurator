using Configurator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
	internal class EvaluationCore
	{
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
			object currentObject = mainConf;
			PropertyInfo currentProperty = null;
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
					SetProperty(currentProperty, ParseQuotedString(node.Token.Text));
					return true;
				case TokenType.SINGLELINECONTENT:
					SetProperty(currentProperty, node.Token.Text);
					return true;
				case TokenType.MultiLineDeclaration:
					currentProperty = FindProperty(currentObject, node);
					SetProperty(currentProperty, node.FindChild(TokenType.MULTILINECONTENT));
					return true;
				case TokenType.ComplexDeclaration:
					ChangeCurrentObject(ref currentObject, node);
					break;
				case TokenType.ListDeclaration:
					currentProperty = FindProperty(currentObject, node);
					break;
				case TokenType.MultiLineItem:
					SetProperty(currentProperty, node.FindChild(TokenType.MULTILINECONTENT), addToList: true);
					return true;
				case TokenType.SIMPLEITEM:
					SetProperty(currentProperty, node.Token.Text, addToList: true);
					return true;
				case TokenType.QUOTEDITEM:
					SetProperty(currentProperty, ParseQuotedString(node.Token.Text), addToList: true);
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
			throw new NotImplementedException();
			//todo: look for the new property in the current object and returns it.
		}

		private void SetProperty(PropertyInfo currentProperty, object value, bool addToList = false)
		{
			throw new NotImplementedException();
		}

		private void ChangeCurrentObject(ref object currentObject, ParseNode node)
		{
			string name = GetAndValidateTagsNames(node.Nodes.First(), node.Nodes.Last());
			ChangeCurrentObject(ref currentObject, name);
		}

		private void ChangeCurrentObject(ref object currentObject, string newPropertyName)
		{
			throw new NotImplementedException();
			//todo: look for the property with the matching name in the current object and returns its value.
			//(also create a new instance if null ?)
		}

		private object GetNamespace(string newPropertyName, params object[] subConfs)
		{
			throw new NotImplementedException();
			//todo: look for the object with the type matching the given name among the sub configs.
			//(also create a new instance if null ?)
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
