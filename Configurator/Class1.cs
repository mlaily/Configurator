using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Configurator
{
	class Class1
	{
		static void Main()
		{

		}
	}

	public interface IConfiguration
	{ }

	public interface ISubConfiguration : IConfiguration
	{
		/// <summary>
		/// Must be a valid non empty name.
		/// </summary>
		string Namespace { get; }
	}

	public static class Configurator
	{
		private class EmptyConfiguration : IConfiguration { }
		//public static void LoadConfigurationFromFiles(params string[] paths) //file or directory paths?
		public static void LoadConfiguration(string content, params ISubConfiguration[] subConfigurations) { LoadConfiguration(content, null, subConfigurations); }
		public static void LoadConfiguration(string content, IConfiguration mainConfiguration, params ISubConfiguration[] subConfigurations)
		{
			mainConfiguration = mainConfiguration ?? new EmptyConfiguration(); //avoid nre
			foreach (var item in subConfigurations)
			{
				if (string.IsNullOrWhiteSpace(item.Namespace) || Regex.IsMatch(item.Namespace, @"[:<>]")) //cannot be null or whitespace, or contain : or < or >
				{
					throw new ArgumentException("A sub configuration namespace is not valid. Cannot continue!");
				}
				foreach (var item2 in subConfigurations)
				{
					if (item.Namespace.ToLowerInvariant() == item2.Namespace.ToLowerInvariant())
					{
						throw new ArgumentException("Found two identical namespaces in the provided sub configurations. Cannot continue!");
					}
				}
			}

			string cleanedContent = content.Replace("\r\n", "\n").Replace('\r', '\n');
			string currentNamespace = null;
			


			//j'ai un flux de texte.
			//lire un token = essayer de matcher le plus possible de texte pour faire un match

		}

	}

	class Token
	{
		public static readonly Dictionary<TokenType, string> MatchExpressions = new Dictionary<TokenType, string>()
		{
			{ TokenType.QuotedString, @"""(?<value>[^""]*)"""}
		};

		public TokenType Type { get; private set; }
		public string Value { get; private set; }

		public static Token Read(string content, int offset)
		{
			throw new NotImplementedException();
			//Regex.
		}
	}

	public enum TokenType
	{
		QuotedString
	}

}
