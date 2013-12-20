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
	public class SimpleTests
	{

		[TestMethod]
		public void EmptyConf()
		{
			Configurator.Configurator.AssignConfiguration("", new EmptyConfModel());
			Configurator.Configurator.AssignConfiguration("#", new EmptyConfModel());
			Configurator.Configurator.AssignConfiguration("#sdqfghdf", new EmptyConfModel());
			Configurator.Configurator.AssignConfiguration("\r \n \t\t\r\n\r\r\n\n\t ", new EmptyConfModel());
			Configurator.Configurator.AssignConfiguration("\r \n# \t\t\r\n\r\r\n\n\t# #", new EmptyConfModel());
			Configurator.Configurator.AssignConfiguration("\r \n# xxx\t\t\r\n\r\r\n\n\t# #x", new EmptyConfModel());

			TestHelper.AssertThrows<Exception>(_ => Configurator.Configurator.AssignConfiguration("hello", new EmptyConfModel()));
			TestHelper.AssertThrows<Exception>(_ => Configurator.Configurator.AssignConfiguration("#sdqfghdf\nx", new EmptyConfModel()));
			TestHelper.AssertThrows<Exception>(_ => Configurator.Configurator.AssignConfiguration("\r \n# xxx\t\t\r\n\r\r\n\n\tx# #x", new EmptyConfModel()));
		}

		[TestMethod]
		public void OverwrittenProperties()
		{
			var actualModel = new SimpleTestsModel();
			Configurator.Configurator.AssignConfiguration(
				@"
Overwrite=x
Overwrite=y
", actualModel);
			Assert.AreEqual("y", actualModel.Overwrite);

			actualModel = new SimpleTestsModel();
			Configurator.Configurator.AssignConfiguration(
				@"
Overwrite=x
<@Overwrite>z</@Overwrite>
", actualModel);
			Assert.AreEqual("z", actualModel.Overwrite);

			Configurator.Configurator.AssignConfiguration(
	@"
Overwrite=x
Overwrite=a
", actualModel);
			Assert.AreEqual("a", actualModel.Overwrite);
		}

		[TestMethod]
		public void MergeLists()
		{
			var actualModel = new SimpleTestsModel();
			Configurator.Configurator.AssignConfiguration(
				@"
<*MergeList>1 2 3</*MergeList>
<*MergeList> 4 5 6 </*MergeList>
", actualModel);
			CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5, 6 }, actualModel.MergeList.ToList());
		}

	}
}
