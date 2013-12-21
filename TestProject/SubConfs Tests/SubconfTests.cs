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

		SubConf1 actualSubConf1_1;
		SubConf2 actualSubConf2_1;
		SubConf3 actualSubConf3_1;

		SubConf1 actualSubConf1_2;
		SubConf2 actualSubConf2_2;
		SubConf3 actualSubConf3_2;


		[TestInitialize]
		public void ReadConfig()
		{
			string rawConf = System.IO.File.ReadAllText("SubConfsTestsConfig_1.conf");
			actualMain = new MainModel();
			actualSubConf1_1 = new SubConf1();
			actualSubConf2_1 = new SubConf2();
			actualSubConf3_1 = new SubConf3();

			Configurator.Configurator.AssignConfiguration(rawConf, actualMain, actualSubConf1_1, actualSubConf2_1, actualSubConf3_1);


			//same config file, but with the sub confs in multiple parts
			rawConf = System.IO.File.ReadAllText("SubConfsTestsConfig_2.conf");
			actualMain = new MainModel();
			actualSubConf1_2 = new SubConf1();
			actualSubConf2_2 = new SubConf2();
			actualSubConf3_2 = new SubConf3();

			Configurator.Configurator.AssignConfiguration(rawConf, actualMain, actualSubConf1_2, actualSubConf2_2, actualSubConf3_2);
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
		public void TestSubConf1_1()
		{
			TestSubConf(expectedSubConf1, actualSubConf1_1);
		}

		[TestMethod]
		public void TestSubConf2_1()
		{
			TestSubConf(expectedSubConf2, actualSubConf2_1);
		}

		[TestMethod]
		public void TestSubConf3_1()
		{
			TestSubConf(expectedSubConf3, actualSubConf3_1);
		}

		[TestMethod]
		public void TestSubConf1_2()
		{
			TestSubConf(expectedSubConf1, actualSubConf1_2);
		}

		[TestMethod]
		public void TestSubConf2_2()
		{
			TestSubConf(expectedSubConf2, actualSubConf2_2);
		}

		[TestMethod]
		public void TestSubConf3_2()
		{
			TestSubConf(expectedSubConf3, actualSubConf3_2);
		}

		private static void TestSubConf(ISubConf expectedSubConf, ISubConf actualSubConf)
		{
			CollectionAssert.AreEquivalent(expectedSubConf.FloatCollection.ToList(), actualSubConf.FloatCollection.ToList());
			Assert.AreEqual(expectedSubConf.Complex.SimpleString, actualSubConf.Complex.SimpleString);
			Assert.AreEqual(expectedSubConf.ExtendedInt, actualSubConf.ExtendedInt);
			Assert.AreEqual(expectedSubConf.SimpleInt, actualSubConf.SimpleInt);
		}
	}
}
