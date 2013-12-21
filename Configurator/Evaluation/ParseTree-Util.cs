using Configurator.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Parser
{
	public partial class ParseNode
	{
		/// <summary>
		/// Look for a token of one of the specified types among the children nodes.
		/// </summary>
		public Token FindChild(params TokenType[] validMatches)
		{
			foreach (var item in this.Nodes)
			{
				if (validMatches.Contains(item.Token.Type))
				{
					return item.Token;
				}
			}
			StringBuilder sbError = new StringBuilder("No match found trying to match one of");
			sbError.Append(" (");
			foreach (var item in validMatches)
			{
				if (item != validMatches.First())
				{
					sbError.Append(", ");
				}
				sbError.Append(item);
			}
			sbError.Append(")");

			throw new EvaluationException(sbError.ToString(), this.Token);
		}
	}
}
