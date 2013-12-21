using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Configurator.Parser;
using System.IO;
using System.Text;
using System.Reflection;

namespace TestProject.SubConfTests
{
	[TestClass]
	public class SubConfTests
	{

		MainModel expectedMain = new MainModel()
		{
			FloatCollection = new List<float>() { 1, 2, 3 },
			SimpleInt = 42,
			ExtendedInt = -42,
			Complex = new Complex() { SimpleString = "blah" },
		};
		SubConf1 expectedSubConf1 = new SubConf1()
		{
			FloatCollection = new List<float>() { 1, 2, 3, 1 },
			SimpleInt = 421,
			ExtendedInt = -421,
			Complex = new Complex() { SimpleString = "blah1" },
		};
		SubConf2 expectedSubConf2 = new SubConf2()
		{
			FloatCollection = new List<float>() { 1, 2, 3, 2 },
			SimpleInt = 422,
			ExtendedInt = -422,
			Complex = new Complex() { SimpleString = "blah2" },
		};
		SubConf3 expectedSubConf3 = new SubConf3()
		{
			FloatCollection = new List<float>() { 1, 2, 3, 3 },
			SimpleInt = 423,
			ExtendedInt = -423,
			Complex = new Complex() { SimpleString = "blah3" },
		};

		MainModel actualMain;
		SubConf1 actualSubConf1;
		SubConf2 actualSubConf2;
		SubConf3 actualSubConf3;

		[TestInitialize]
		public void ReadConfig()
		{
			string rawConf = System.IO.File.ReadAllText("SubConfsTestsConfig.conf");
			actualMain = new MainModel();
			actualSubConf1 = new SubConf1();
			actualSubConf2 = new SubConf2();
			actualSubConf3 = new SubConf3();

			Configurator.Configurator.AssignConfiguration(rawConf, actualMain, actualSubConf1, actualSubConf2, actualSubConf3);
		}

		[TestMethod]
		public void TestMain()
		{
			CollectionAssert.AreEquivalent(expectedMain.FloatCollection.ToList(), actualMain.FloatCollection.ToList());
			Assert.AreEqual(expectedMain.Complex.SimpleString, actualMain.Complex.SimpleString);
			Assert.AreEqual(expectedMain.ExtendedInt, actualMain.ExtendedInt);
			Assert.AreEqual(expectedMain.SimpleInt, actualMain.SimpleInt);
		}

		[TestMethod]
		public void TestSubConf1()
		{
			CollectionAssert.AreEquivalent(expectedSubConf1.FloatCollection.ToList(), actualSubConf1.FloatCollection.ToList());
			Assert.AreEqual(expectedSubConf1.Complex.SimpleString, actualSubConf1.Complex.SimpleString);
			Assert.AreEqual(expectedSubConf1.ExtendedInt, actualSubConf1.ExtendedInt);
			Assert.AreEqual(expectedSubConf1.SimpleInt, actualSubConf1.SimpleInt);
		}

		[TestMethod]
		public void TestSubConf2()
		{
			CollectionAssert.AreEquivalent(expectedSubConf2.FloatCollection.ToList(), actualSubConf2.FloatCollection.ToList());
			Assert.AreEqual(expectedSubConf2.Complex.SimpleString, actualSubConf2.Complex.SimpleString);
			Assert.AreEqual(expectedSubConf2.ExtendedInt, actualSubConf2.ExtendedInt);
			Assert.AreEqual(expectedSubConf2.SimpleInt, actualSubConf2.SimpleInt);
		}

		[TestMethod]
		public void TestSubConf3()
		{
			CollectionAssert.AreEquivalent(expectedSubConf3.FloatCollection.ToList(), actualSubConf3.FloatCollection.ToList());
			Assert.AreEqual(expectedSubConf3.Complex.SimpleString, actualSubConf3.Complex.SimpleString);
			Assert.AreEqual(expectedSubConf3.ExtendedInt, actualSubConf3.ExtendedInt);
			Assert.AreEqual(expectedSubConf3.SimpleInt, actualSubConf3.SimpleInt);
		}
	}
}
