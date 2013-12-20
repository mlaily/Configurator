using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.GenericTests.Test2
{

	class MainModel
	{
		public IEnumerable<string> SingleLineList { get; set; }
		public IEnumerable<string> List { get; set; }
		public ItemXComplex ItemX { get; set; }
	}

	class @namespace
	{
		public string Bla { get; set; }
		public string Bite { get; set; }
		public int Property1 { get; set; }
		public string Property2 { get; set; }
		public FirstComplexType Complex1 { get; set; }
		public string Expanded1 { get; set; }
	}

	class FirstComplexType
	{
		public bool Property3 { get; set; }
		public string Prop4 { get; set; }
		public string Expanded1 { get; set; }
	}

	class ItemXComplex
	{
		public string a { get; set; }
		public BComplex b { get; set; }
	}

	class BComplex
	{
		public CComplex c { get; set; }
	}

	class CComplex
	{
		public string x { get; set; }
	}
}