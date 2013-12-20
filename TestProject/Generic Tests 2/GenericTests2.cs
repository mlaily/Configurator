using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Configurator.Parser;
using System.IO;
using System.Text;
using System.Reflection;
using TestProject.GenericTests.Test2;

namespace TestProject.Test2
{
	[TestClass]
	public class GenericTests2
	{

		MainModel expectedMain = new MainModel()
		{
			SingleLineList = new List<string>() { "true", "true", "false", " hoy# \" ..", "", "false", "</*blah>", "true" },
			List = new List<string>() {"Value1", "Value2",
			@"sdqf
sqdf
	sfd

sdqfsqdf", "bla", "Value3", "Value4", "Value5", "Value6\"", "Value7", "Value8", "Value9#", "Value \" 10! # o/"},
			ItemX = new ItemXComplex()
			{
				a = "sdf",
				b = new BComplex()
				{
					c = new CComplex()
					{
						x = "ba"
					}
				}
			}
		};

		@namespace expectedSub = new @namespace()
		{
			Bla = "true#",
			Bite = "true Blu=bla",
			Property1 = 42,
			Property2 = "bla\"blah#",
			Complex1 = new FirstComplexType()
			{
				Property3 = true,
				Prop4 = "42 \"",
				Expanded1 = @"	# won't work as a comment and will be included
	because of the @ => read all until the corresponding end tag is found
	
	even new lines and spaces...",
			},
			Expanded1 = @"	# won't work as a comment and will be included
	because of the @ => read all until the corresponding end tag is found
	<bla>tfdy</bla>
	even new lines and spaces...",
		};

		MainModel actualMain;
		@namespace actualSub;

		[TestInitialize]
		public void ReadConfig()
		{
			string rawConf = System.IO.File.ReadAllText("GenericTests2Config.conf");
			actualMain = new MainModel();
			actualSub = new @namespace();

			Configurator.Configurator.AssignConfiguration(rawConf, actualMain, actualSub);
		}

		[TestMethod]
		public void Lists()
		{
			CollectionAssert.AreEquivalent(expectedMain.SingleLineList.ToList(), actualMain.SingleLineList.ToList());
			CollectionAssert.AreEquivalent(expectedMain.List.ToList(), actualMain.List.ToList());
		}

		[TestMethod]
		public void ComplexType()
		{
			Assert.AreEqual(expectedMain.ItemX.a, actualMain.ItemX.a);
			Assert.AreEqual(expectedMain.ItemX.b.c.x, actualMain.ItemX.b.c.x);
		}

		[TestMethod]
		public void SubConfSimpleTypes()
		{
			Assert.AreEqual(expectedSub.Bla, actualSub.Bla);
			Assert.AreEqual(expectedSub.Bite, actualSub.Bite);
			Assert.AreEqual(expectedSub.Property1, actualSub.Property1);
			Assert.AreEqual(expectedSub.Property2, actualSub.Property2);
		}

		[TestMethod]
		public void SubConfExpandedStrings()
		{
			Assert.AreEqual(expectedSub.Expanded1, actualSub.Expanded1);
			Assert.AreEqual(expectedSub.Complex1.Expanded1, actualSub.Complex1.Expanded1);
		}

		[TestMethod]
		public void SubConfComplexType()
		{
			Assert.AreEqual(expectedSub.Complex1.Property3, actualSub.Complex1.Property3);
			Assert.AreEqual(expectedSub.Complex1.Prop4, actualSub.Complex1.Prop4);
			Assert.AreEqual(expectedSub.Complex1.Expanded1, actualSub.Complex1.Expanded1);
		}


	}
}
