using Configurator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
	public static class Configurator
	{

		public static void AssignConfiguration(string rawConfContent, IEnumerable<Converter> converters, object mainConf, params object[] subConfs)
		{
			Scanner scanner = new Scanner();
			Parser.Parser parser = new Parser.Parser(scanner);
			Evaluation.Core evaluator = new Evaluation.Core(converters.ToArray());
			ParseTree tree = parser.Parse(rawConfContent);
			evaluator.WalkTree(tree, mainConf, subConfs);
		}

		/// <summary>
		/// The first conf is interpreted as the main conf.
		/// </summary>
		public static void AssignConfiguration(string rawConfContent, params object[] allConfs)
		{
			if (allConfs == null || !allConfs.Any()) throw new ArgumentException("You must provide at least one conf!", "allConfs");
			AssignConfiguration(rawConfContent, Enumerable.Empty<Converter>(), allConfs[0], allConfs.Skip(1).ToArray());
		}

		/// <summary>
		/// No subconfs
		/// </summary>
		public static void AssignConfiguration(string rawConfContent, object mainConf, params Converter[] converters)
		{
			AssignConfiguration(rawConfContent, converters, mainConf);
		}
	}
}
