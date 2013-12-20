using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		public static void AssertThrowsException<TException>(Action<object> action, object userDefined = null) where TException : Exception
		{
			const string specialMarker = "@!SHOULD NOT BE HERE BUT WAS ALL THE SAME!@";
			bool failWithWrongException = false;
			try
			{
				action(userDefined);
				//should not be here
				Assert.Fail(specialMarker);
			}
			catch (AssertFailedException ex)
			{
				if (ex.Message.Contains(specialMarker))
					Assert.Fail("The action performed without throwing any exception");
				else //exception comes from the user code!
				{
					if (typeof(TException) == typeof(AssertFailedException))
					{ /*OK!*/}
					else
						failWithWrongException = true;
				}
			}
			catch (TException)
			{
				//OK!
			}
			catch (Exception ex)
			{
				var y = ex; //supress ex not used warning...
				failWithWrongException = true;
			}
			if (failWithWrongException) Assert.Fail("The action threw an exception, but not of the expected type...");
		}
	}
}
