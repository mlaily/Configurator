using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Configurator
{
	class Configuration : IConfiguration
	{
		#region Default configuration

		public const string DefaultConfigurationFileName = "ClearSky.Flickr.conf";
		public const string DefaultConfiguration =
@"#Api key
ApiKey=
ApiSecret=
TokenIdentifier=
TokenSharedSecret=
";
		#endregion

		private static Configuration _current;
		public static Configuration Current
		{
			get
			{
				Initialize();
				return _current;
			}
		}

		public static void Initialize()
		{
			if (_current == null)
			{
				_current = new Configuration();
			}
		}

		private Configuration()
		{
			string configFileContent;
			try
			{
				configFileContent = File.ReadAllText(DefaultConfigurationFileName);
			}
			catch (Exception)
			{
				configFileContent = DefaultConfiguration;
				try
				{
					File.WriteAllText(DefaultConfigurationFileName, DefaultConfiguration);
				}
				catch (Exception) { }
			}
			Parser.AssignConfiguration(configFileContent, this);
		}

		[CorrespondTo("ApiKey")]
		public string ApiKey { get; protected set; }

		[CorrespondTo("ApiSecret")]
		public string ApiSecret { get; protected set; }

		[CorrespondTo("TokenIdentifier")]
		public string TokenIdentifier { get; protected set; }

		[CorrespondTo("TokenSharedSecret")]
		public string TokenSharedSecret { get; protected set; }

		public void HandleDynamicKey(string key, string value) { }
	}

}
