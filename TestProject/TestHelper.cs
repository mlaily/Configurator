using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
	public static class TestHelper
	{
		public static int GetSequenceHashCode<T>(IEnumerable<T> collection)
		{
			int result = 0;
			foreach (var item in collection)
			{
				result ^= item == null ? 42 : item.GetHashCode();
			}
			return result;
		}
	}
}
