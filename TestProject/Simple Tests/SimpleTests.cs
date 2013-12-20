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

			TestHelper.AssertThrowsException<Exception>(_ => Configurator.Configurator.AssignConfiguration("hello", new EmptyConfModel()));
			TestHelper.AssertThrowsException<Exception>(_ => Configurator.Configurator.AssignConfiguration("#sdqfghdf\nx", new EmptyConfModel()));
			TestHelper.AssertThrowsException<Exception>(_ => Configurator.Configurator.AssignConfiguration("\r \n# xxx\t\t\r\n\r\r\n\n\tx# #x", new EmptyConfModel()));
		}

		[TestMethod]
		public void Test()
		{
			Configurator.Configurator.AssignConfiguration("", new EmptyConfModel());
		}
	}
}
