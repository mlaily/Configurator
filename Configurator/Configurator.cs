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

		public static void AssignConfiguration(string rawConfContent, object mainConf, params object[] subConfs)
		{
			Scanner scanner = new Scanner();
			Parser.Parser parser = new Parser.Parser(scanner);
			EvaluationCore evaluator = new EvaluationCore();
			ParseTree tree = parser.Parse(rawConfContent);
			evaluator.WalkTree(tree, mainConf, subConfs);
		}
	}
}
