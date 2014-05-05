using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniDicEnum<Key, Value> : IEnumerator
{
	public List<Key> _keys;
	public List<Value> _vals;
	
	// Enumerators are positioned before the first element 
	// until the first MoveNext() call. 
	int position = -1;
	
	public UniDicEnum(List<Key> k, List<Value> v)
	{
		_keys = k;
		_vals = v;
	}
	
	public bool MoveNext()
	{
		position++;
		return (position < _keys.Count);
	}
	
	public void Reset()
	{
		position = -1;
	}
	
	object IEnumerator.Current
	{
		get
		{
			return Current;
		}
	}
	
	public KeyValuePair<Key, Value> Current
	{
		get
		{
			try
			{
				return new KeyValuePair<Key,Value>(_keys[position], _vals[position]);
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException();
			}
		}
	}
}

[Serializable]
public class UniDictionary<Key, Value> : IEnumerable
{
	[SerializeField]
	private List<Key> keys = new List<Key>();

	[SerializeField]
	private List<Value> values = new List<Value>();

	public void Add(Key key, Value value)
	{
		if (keys.Contains(key))
			return;
		keys.Add(key);
		values.Add(value);
	}

	public void Remove(Key key)
	{
		if (!keys.Contains(key))
			return;
		int index = keys.IndexOf(key);
		keys.RemoveAt(index);
		values.RemoveAt(index);
	}
	
	public bool TryGetValue(Key key, out Value value)
	{
		if (keys.Count != values.Count)
		{
			keys.Clear();
			values.Clear();
			value = default(Value);
			return false;
		}
		if (!keys.Contains(key))
		{
			value = default(Value);
			return false;
		}

		int index = keys.IndexOf(key);
		value = values[index];

		return true;
	}
	
	public void ChangeValue(Key key, Value value)
	{
		if (!keys.Contains(key))
			return;

		int index = keys.IndexOf(key);
		values[index] = value;
	}

	public Value this[Key key]
	{
		get
		{
			Value val;
			bool exists = TryGetValue(key, out val);
			if (exists)
				return val;
			else
				return default(Value);
		}
		set
		{
			Value temp;
			if (TryGetValue(key, out temp))
				ChangeValue(key,value);
			else
				Add(key,value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return (IEnumerator) GetEnumerator();
	}
	
	public UniDicEnum<Key, Value> GetEnumerator()
	{
		return new UniDicEnum<Key, Value>(keys, values);
	}

}
