using Configurator.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Evaluation
{
	[Serializable]
	public class EvaluationException : Exception
	{
		public Token Token { get; protected set; }
		public EvaluationException(string message, Token token = null, Exception inner = null)
			: base(message, inner)
		{
			this.Token = token;
		}
	}
}
