using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator
{
	/// <summary>
	/// Generic implementation of IDictionary supporting multiple values with the same key.
	/// </summary>
	public class KeyValueCollection<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private List<KeyValuePair<TKey, TValue>> backingList = new List<KeyValuePair<TKey, TValue>>();

		public void Add(TKey key, TValue value)
		{
			backingList.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool ContainsKey(TKey key)
		{
			return backingList.Any(x => Comparer<TKey>.Equals(x.Key, key));
		}

		public ICollection<TKey> Keys
		{
			get
			{
				ICollection<TKey> result = new List<TKey>();
				foreach (var item in backingList)
				{
					result.Add(item.Key);
				}
				return result;
			}
		}

		public bool Remove(TKey key)
		{
			for (int i = 0; i < backingList.Count; i++)
			{
				var item = backingList[i];
				if (Comparer<TKey>.Equals(item.Key, key))
				{
					backingList.Remove(item);
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			foreach (var item in backingList)
			{
				if (Comparer<TKey>.Equals(item.Key, key))
				{
					value = item.Value;
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		public ICollection<TValue> Values
		{
			get
			{
				ICollection<TValue> result = new List<TValue>();
				foreach (var item in backingList)
				{
					result.Add(item.Value);
				}
				return result;
			}
		}

		/// <summary>
		/// As this implementation supports multiple values with the same key, this method always throws NotSupportedException.
		/// </summary>
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<TValue> this[TKey key]
		{
			get
			{
				foreach (var item in backingList)
				{
					if (Comparer<TKey>.Equals(item.Key, key))
					{
						yield return item.Value;
					}
				}
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			backingList.Add(item);
		}

		public void Clear()
		{
			backingList.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return backingList.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			backingList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return backingList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return backingList.Remove(item);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return backingList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return backingList.GetEnumerator();
		}
	}
}
