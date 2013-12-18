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
		/// <param name="value"></param>
		/// <param name="requiredType"></param>
		/// <returns></returns>
		public abstract object Convert(string value, Type requiredType);
	}

	public class SimpleTypeConverter : Converter
	{

		Dictionary<Type, Func<string, object>> ConvertFunctions = new Dictionary<Type, Func<string, object>>()
		{
			{ typeof(short), x => short.Parse(x) },
			{ typeof(int), x => int.Parse(x) },
			{ typeof(long), x => long.Parse(x) },
			{ typeof(float), x => float.Parse(x, CultureInfo.InvariantCulture) },
			{ typeof(double), x => double.Parse(x, CultureInfo.InvariantCulture) },
			{ typeof(decimal), x => decimal.Parse(x, CultureInfo.InvariantCulture) },
			{ typeof(char), x => char.Parse(x) },
			{ typeof(bool), x => bool.Parse(x) },
			{ typeof(byte), x => byte.Parse(x) },
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
