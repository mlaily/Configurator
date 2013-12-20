using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Configurator.Parser;
using System.IO;
using System.Text;
using System.Reflection;

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

	}
}
