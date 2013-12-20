using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
	public abstract class Converter
	{
		/// <summary>
		/// Indicates if the converter can convert to the specified type.
		/// </summary>
		public abstract bool CanConvert(Type type);

		/// <summary>
		/// Actually convert the value to the required type.
		/// </summary>
		public abstract object Convert(string value, Type requiredType);
	}

	public class SimpleTypeConverter : Converter
	{
		Dictionary<Type, Func<string, object>> ConvertFunctions = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(bool), x => bool.Parse(x) },
			{ typeof(byte), x => byte.Parse(x) },
			{ typeof(short), x => short.Parse(x) },
			{ typeof(int), x => int.Parse(x) },
			{ typeof(long), x => long.Parse(x) },
			//The number style is explicitly set to disallow thousands separator
			//The decimal separator is a dot
			{ typeof(float), x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture) },
			{ typeof(double), x => double.Parse(x,NumberStyles.Float, CultureInfo.InvariantCulture) },
			//The decimal type disallows the exponent notation
			{ typeof(decimal), x => decimal.Parse(x, NumberStyles.Float & ~NumberStyles.AllowExponent, CultureInfo.InvariantCulture) },
			{ typeof(char), x => char.Parse(x) },
			//the unsigned integers and the signed byte are not implemented...
		};

		public override bool CanConvert(Type type)
		{
			return ConvertFunctions.Keys.Contains(type);
		}

		public override object Convert(string value, Type requiredType)
		{
			return ConvertFunctions[requiredType](value);
		}
	}

}
