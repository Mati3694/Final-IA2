using System.Collections.Generic;
using System.Linq;

public class MinHeap<T>
{
	public int Count { get => _count; }

	private List<T> _items;
	private List<float> _values;
	private Dictionary<T, int> _map;

	private int _count;

	private int _containerSize => _values.Count;

	public MinHeap(int reservedCapacity)
	{
		if (reservedCapacity < 1)
			reservedCapacity = 1;

		_items = new List<T>(reservedCapacity);
		_values = new List<float>(reservedCapacity);
		_map = new Dictionary<T, int>(reservedCapacity);

		Resize(reservedCapacity);
	}

	private void Resize(int size)
	{
		_items.AddRange(Enumerable.Repeat(default(T), size - _containerSize));
		_values.AddRange(Enumerable.Repeat(float.NaN, size - _containerSize));
	}

	private void Swap(int i, int j)
	{
		if (i == j) //Would be strange if this happens. Remove?
			return;

		var tt = _items[i];
		_items[i] = _items[j];
		_items[j] = tt;

		var tv = _values[i];
		_values[i] = _values[j];
		_values[j] = tv;

		_map[_items[i]] = i;
		_map[_items[j]] = j;

	}

	// Returns true if it found the item.
	private bool DecreaseKey(T item, float newValue)
	{
		int current;
		if (!_map.TryGetValue(item, out current))
			return false;


		var oldValue = _values[current];
		if (oldValue > newValue)
		{
			_values[current] = newValue;

			// Bubble up
			while (current != 0)
			{
				var parent = (current - 1) / 2;   //Integer division (round down)
				if (newValue < _values[parent])
				{
					Swap(current, parent);
					current = parent;
				}
				else
					break;
			}
		}

		return true;
	}

	public void Push(T item, float value)
	{
		// If the item already existed, update and exit
		if (DecreaseKey(item, value))
			return;

		// ...Otherwise do a normal push operation

		// Check size
		if (_count == _containerSize)
			Resize(_count * 2);

		// Add last (a leaf)
		int current = _count++;
		_items[current] = item;
		_values[current] = value;
		//if(current == 0)
		//	UnityEngine.Debug.Log($"DUMH {item} <-- {current}");
		_map[item] = current;

		// Keep climbing to top until parent or unable to improve heap
		while (current != 0)
		{
			var parent = (current - 1) / 2;   //Integer division (round down)
			if (value < _values[parent])
			{
				Swap(current, parent);
				current = parent;
			}
			else
				break;
		}
	}

	public T Peek()
	{
		return _items[0];
	}

	public T Pop()
	{
		return Pop(out var v);
	}

	public T Pop(out float value)
	{
		var result = _items[0];
		value = _values[0];

		//Remove first, swap with last and decrease count
		_count--;
		_items[0] = _items[_count];
		_values[0] = _values[_count];
		_items[_count] = default(T);    //Let GC clean up
		_values[_count] = float.NaN;

		if (_count > 0)
		{
			//UnityEngine.Debug.Log($"DUMH 0 {_items[0]} <-- 0");
			_map[_items[0]] = 0;

			// Trickle first down
			int current = 0;
			while (current < _count)
			{
				int child0 = current * 2 + 1;
				int child1 = current * 2 + 2;
				int best = current;
				if (child0 < _count && _values[child0] < _values[best])
					best = child0;
				if (child1 < _count && _values[child1] < _values[best])
					best = child1;

				if (best == current)
					break;              // Can't improve
				else
				{
					Swap(current, best);
					current = best;     // Go down that way
				}
			}
		}

		_map.Remove(result);
		return result;
	}
}
