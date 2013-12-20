using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Configurator.Parser;
using System.IO;
using System.Text;
using System.Reflection;
using Configurator;

namespace TestProject.SimpleTests
{
	[TestClass]
	public class ConverterTests
	{

		[TestMethod]
		public void SimpleTypes()
		{
			var actualModel = new ConverterTestsModel();
			Configurator.Configurator.AssignConfiguration(
				@"
Bool = false
Bool = False
Bool = FALSE
Bool = trUE
Bool = true #all the preceding values are also attributed (and parsed to bool) but only the last one is kept
Byte = 42
Byte = 042
Short = 42
Short = -42
Short = -042
Short = 042
Int = 42
Int = -42
Int = -042
Int = 042
Long = 42
Long = -42
Long = -042
Long = 042
Float = -10
Float = 0
Float = 1.5E2
Float = -1.5E2
Float = 42.50 # not enough precision for .24
Double = -10
Double = 0
Double = 1.5E2
Double = -1.5E2
Double = 42.24
Decimal = -10
Decimal = 0
Decimal = 42.24
Char = "" ""
Char = """""""" #one quote
Char = ♥
Char = A
", actualModel);
			Assert.AreEqual(true, actualModel.Bool);
			Assert.AreEqual((byte)42, actualModel.Byte);
			Assert.AreEqual((short)42, actualModel.Short);
			Assert.AreEqual(42, actualModel.Int);
			Assert.AreEqual(42L, actualModel.Long);
			Assert.AreEqual(42.50F, actualModel.Float);
			Assert.AreEqual(42.24D, actualModel.Double);
			Assert.AreEqual(42.24M, actualModel.Decimal);
			Assert.AreEqual('A', actualModel.Char);
			//
			actualModel = new ConverterTestsModel();
			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Bool = 1", actualModel));
			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Bool = 0", actualModel));
			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Bool = trrrue", actualModel));

			TestHelper.AssertThrows<OverflowException>(_ => Configurator.Configurator.AssignConfiguration("Byte = -1", actualModel));
			TestHelper.AssertThrows<OverflowException>(_ => Configurator.Configurator.AssignConfiguration("Byte = 256", actualModel));
			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Byte = FF", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Short = FF", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Int = FF", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Long = FF", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Float = 42,50", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Double = 42,24", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Decimal = 42,24", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Char = \"\" #empty", actualModel));

			TestHelper.AssertThrows<FormatException>(_ => Configurator.Configurator.AssignConfiguration("Char = xx", actualModel));
		}

		[TestMethod]
		public void CustomConverter()
		{
			var actualModel = new ConverterTestsModel();
			Configurator.Configurator.AssignConfiguration(
				@"
			<@FloatMatrix>
			{  0.3333333, -0.6666667, -0.6666667,  0.0000000,  0.0000000 }
			{ -0.6666667,  0.3333333, -0.6666667,  0.0000000,  0.0000000 }
			{ -0.6666667, -0.6666667,  0.3333333,  0.0000000,  0.0000000 }
			{  0.0000000,  0.0000000,  0.0000000,  1.0000000,  0.0000000 }
			{  1.0000000,  1.0000000,  1.0000000,  0.0000000,  1.0000000 }
			</@FloatMatrix>
			", actualModel, new MatrixConverter());
			var expected = new float[,] 
			{{  0.3333333f, -0.6666667f, -0.6666667f,  0.0000000f,  0.0000000f },
			 { -0.6666667f,  0.3333333f, -0.6666667f,  0.0000000f,  0.0000000f },
			 { -0.6666667f, -0.6666667f,  0.3333333f,  0.0000000f,  0.0000000f },
			 {  0.0000000f,  0.0000000f,  0.0000000f,  1.0000000f,  0.0000000f },
			 {  1.0000000f,  1.0000000f,  1.0000000f,  0.0000000f,  1.0000000f }};
			CollectionAssert.AreEquivalent(expected, actualModel.FloatMatrix);
		}
	}

	class MatrixConverter : Converter
	{
		public override bool CanConvert(Type type)
		{
			return typeof(float[,]) == type;
		}

		public override object Convert(string value, Type requiredType)
		{
			return StaticParseMatrix(value);
		}

		//Straight from NegativeScreen...
		static float[,] StaticParseMatrix(string rawValue)
		{
			float[,] matrix = new float[5, 5];
			var rows = System.Text.RegularExpressions.Regex.Matches(rawValue, @"{(?<row>.*?)}",
				System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
			if (rows.Count != 5)
			{
				throw new Exception("The matrices must have 5 rows.");
			}
			for (int x = 0; x < rows.Count; x++)
			{
				var row = rows[x];
				var columnSplit = row.Groups["row"].Value.Split(',');
				if (columnSplit.Length != 5)
				{
					throw new Exception("The matrices must have 5 columns.");
				}
				for (int y = 0; y < matrix.GetLength(1); y++)
				{
					float value;
					if (!float.TryParse(columnSplit[y],
						System.Globalization.NumberStyles.Float,
						System.Globalization.NumberFormatInfo.InvariantInfo,
						out value))
					{
						throw new Exception(string.Format("Unable to parse \"{0}\" to a float.", columnSplit[y]));
					}
					matrix[x, y] = value;
				}
			}
			return matrix;
		}
	}
}
