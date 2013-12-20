using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.SimpleTests
{
	class EmptyConfModel { }

	class SimpleTestsModel
	{
		public string Overwrite { get; set; }
		public IEnumerable<int> MergeList { get; set; }
	}

	class ConverterTestsModel
	{
		public bool Bool { get; set; }
		public byte Byte { get; set; }
		public short Short { get; set; }
		public int Int { get; set; }
		public long Long { get; set; }
		public float Float { get; set; }
		public double Double { get; set; }
		public decimal Decimal { get; set; }
		public char Char { get; set; }
	}

}