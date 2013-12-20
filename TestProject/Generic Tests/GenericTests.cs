using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Configurator.Parser;
using System.IO;
using System.Text;
using System.Reflection;
using TestProject.GenericTests.Expected;

namespace TestProject.GenericTests
{
	[TestClass]
	public class GenericTests
	{

		MainModel expectedMain = new MainModel()
		{
			SimpleString = "blah bleh bluh",
			SimpleString2 = @"helloooo "" ",
			SimpleInt = 42,
			SimpleString3 =
			@"Hello how are you
bite ""entre guillemets""
bite
BLah blah blaah!!! #sera inclu.
parce que ca fait plus de sens de 
considérer que c'est comme des quotes
	=>c'est fait pour coller des pavés de texte...
hahaha! hohoho!",
			SimpleBool = true,
			StringCollection = new List<string>()
			{
				"first",
				"second",
				"cest un peu de la merde\r\nquand meme",
				"blah",
			},
			IntCollection = new List<int>() { 1, 2, 3 },
			BoolCollection = new List<bool>() { true, false, true },
			ComplexTypeItem = new ComplexType()
			{
				Name = "Helllooo",
				Count = 12,
				Collection = new List<bool>() { true, true },
			},
			SecondComplexType = new ComplexType2()
			{
				Blah = true,
				SimpleComplexType = new ComplexType()
				{
					Name = "biteuh # yo",
					Count = 2,
					Collection = new List<bool>() { false, true },
				},
				ComplexTypeCollection = new List<ComplexType>()
				{
					new ComplexType()
					{
						Name = "mabite",
						Count = 1,
						Collection = new List<bool>() {false}
					},
					new ComplexType()
					{
						//no name
						Count = -1,
						Collection = new List<bool>() //empty
					},
				}
			},
			Complex2ItemCollection = new List<ComplexType2>()
			{
				new ComplexType2()
				{
					Blah = true,
					SimpleComplexType = new ComplexType()
					{
					Name = "biteuh # yo",
					Count = 2,
					Collection = new List<bool>() {false, true},
					},
					ComplexTypeCollection = new List<ComplexType>()
					{
						new ComplexType()
						{
							Name = "mabite",
							Count = 1,
							Collection = new List<bool>() {false}
						},
						new ComplexType()
						{
							//no name
							Count = -1,
							Collection = new List<bool>() //empty
						},
					}
				},
				new ComplexType2()
				{
					Blah = true,
					SimpleComplexType = new ComplexType()
					{
					Name = "biteuh # yo",
					Count = 2,
					Collection = new List<bool>() {false, true},
					},
					ComplexTypeCollection = new List<ComplexType>()
					{
						new ComplexType()
						{
							Name = "mabite",
							Count = 1,
							Collection = new List<bool>() {false}
						},
						new ComplexType()
						{
							//no name
							Count = -1,
							Collection = new List<bool>() //empty
						},
					}
				}
			}
		};

		SubModel expectedSub = new SubModel()
		{
			JeSersARien = "c'est vrai!",
			Vrai = true,
			Idem = new List<string>()
			  {
				  "bla",
				  "bite",
				  "second try"
			  }
		};

		MainModel actualMain;
		SubModel actualSub;

		[TestInitialize]
		public void ReadConfig()
		{
			string rawConf = System.IO.File.ReadAllText("GenericTestsConfig.conf");
			actualMain = new MainModel();
			actualSub = new SubModel();

			Configurator.Configurator.AssignConfiguration(rawConf, actualMain, actualSub);
		}

		[TestMethod]
		public void SimpleStrings()
		{
			Assert.AreEqual(expectedMain.SimpleString, actualMain.SimpleString);
			Assert.AreEqual(expectedMain.SimpleString2, actualMain.SimpleString2);
			Assert.AreEqual(expectedMain.SimpleString3, actualMain.SimpleString3);
		}

		[TestMethod]
		public void SimpleInt()
		{
			Assert.AreEqual(expectedMain.SimpleInt, actualMain.SimpleInt);
		}

		[TestMethod]
		public void SimpleBool()
		{
			Assert.AreEqual(expectedMain.SimpleBool, actualMain.SimpleBool);
		}

		[TestMethod]
		public void ComplexType()
		{
			Assert.AreEqual(expectedMain.ComplexTypeItem, actualMain.ComplexTypeItem);
		}

		[TestMethod]
		public void SecondComplexType()
		{
			Assert.AreEqual(expectedMain.SecondComplexType, actualMain.SecondComplexType);
		}

		[TestMethod]
		public void SimpleCollections()
		{
			CollectionAssert.AreEquivalent(expectedMain.BoolCollection.ToList(), actualMain.BoolCollection.ToList());
			CollectionAssert.AreEquivalent(expectedMain.IntCollection.ToList(), actualMain.IntCollection.ToList());
			CollectionAssert.AreEquivalent(expectedMain.StringCollection.ToList(), actualMain.StringCollection.ToList());
		}

		[TestMethod]
		public void ComplexCollection()
		{
			CollectionAssert.AreEquivalent(expectedMain.Complex2ItemCollection.ToList(), actualMain.Complex2ItemCollection.ToList());
		}

		[TestMethod]
		public void TestSubModel()
		{
			Assert.AreEqual(expectedSub.Vrai, actualSub.Vrai);
			Assert.AreEqual(expectedSub.JeSersARien, actualSub.JeSersARien);
			CollectionAssert.AreEquivalent(expectedSub.Idem.ToList(), actualSub.Idem.ToList());
		}
	}
}
