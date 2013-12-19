using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Expected
{
	public class Helper
	{
		public static int GetSequenceHashCode<T>(IEnumerable<T> collection)
		{
			int result = 0;
			foreach (var item in collection)
			{
				result ^= item == null ? 42 : item.GetHashCode();
			}
			return result;
		}
	}

	class MainModel
	{
		public string SimpleString { get; set; }
		public string SimpleString2 { get; set; }
		public string SimpleString3 { get; set; }
		public int SimpleInt { get; set; }
		public bool SimpleBool { get; set; }

		public IEnumerable<string> StringCollection { get; set; }
		public IEnumerable<int> IntCollection { get; set; }
		public IEnumerable<bool> BoolCollection { get; set; }

		public ComplexType ComplexTypeItem { get; set; }
		public ComplexType2 SecondComplexType { get; set; }

		public IEnumerable<ComplexType2> Complex2ItemCollection { get; set; }
	}

	class SubModel
	{
		public string JeSersARien { get; set; }
		public bool Vrai { get; set; }
		public IEnumerable<string> Idem { get; set; }
	}

	class ComplexType
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public IEnumerable<bool> Collection { get; set; }

		public override bool Equals(object obj)
		{
			ComplexType cast;
			if ((cast = obj as ComplexType) == null)
			{
				return false;
			}
			return this.Name == cast.Name && this.Count == cast.Count && this.Collection.SequenceEqual(cast.Collection);
		}

		public override int GetHashCode()
		{
			return (this.Name ?? "").GetHashCode() ^
				this.Count.GetHashCode() ^
				(this.Collection == null ? 0 : Helper.GetSequenceHashCode(this.Collection));
		}

		public static bool operator ==(ComplexType a, ComplexType b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}
			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			// Return true if the fields match:
			return a.Equals(b);
		}

		public static bool operator !=(ComplexType a, ComplexType b)
		{
			return !(a == b);
		}
	}

	class ComplexType2
	{
		public bool Blah { get; set; }
		public ComplexType SimpleComplexType { get; set; }
		public IEnumerable<ComplexType> ComplexTypeCollection { get; set; }

		public override bool Equals(object obj)
		{
			ComplexType2 cast;
			if ((cast = obj as ComplexType2) == null)
			{
				return false;
			}
			return this.Blah == cast.Blah && this.SimpleComplexType == cast.SimpleComplexType && this.ComplexTypeCollection.SequenceEqual(cast.ComplexTypeCollection);
		}

		public override int GetHashCode()
		{
			return this.Blah.GetHashCode() ^
				(this.SimpleComplexType == null ? 0 : this.SimpleComplexType.GetHashCode()) ^
				(this.ComplexTypeCollection == null ? 0 : Helper.GetSequenceHashCode(this.ComplexTypeCollection));
		}

		public static bool operator ==(ComplexType2 a, ComplexType2 b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}
			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			// Return true if the fields match:
			return a.Equals(b);
		}

		public static bool operator !=(ComplexType2 a, ComplexType2 b)
		{
			return !(a == b);
		}
	}

}
