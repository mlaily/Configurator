using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.SubConfTests
{

	class MainModel
	{
		public int SimpleInt { get; set; }
		public int ExtendedInt { get; set; }
		public IEnumerable<float> FloatCollection { get; set; }
		public Complex Complex { get; set; }
	}

	class Complex
	{
		public string SimpleString { get; set; }
	}

	interface ISubConf
	{
		int SimpleInt { get; set; }
		int ExtendedInt { get; set; }
		IEnumerable<float> FloatCollection { get; set; }
		Complex Complex { get; set; }
	}

	class SubConf1 : ISubConf
	{
		public int SimpleInt { get; set; }
		public int ExtendedInt { get; set; }
		public IEnumerable<float> FloatCollection { get; set; }
		public Complex Complex { get; set; }
	}

	class SubConf2 : ISubConf
	{
		public int SimpleInt { get; set; }
		public int ExtendedInt { get; set; }
		public IEnumerable<float> FloatCollection { get; set; }
		public Complex Complex { get; set; }
	}

	class SubConf3 : ISubConf
	{
		public int SimpleInt { get; set; }
		public int ExtendedInt { get; set; }
		public IEnumerable<float> FloatCollection { get; set; }
		public Complex Complex { get; set; }
	}

}