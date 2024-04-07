using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomObjectPicker
{
	[Tooltip("All possible poolables that can be get from random.")]
	[SerializeField] private List<PickableStats<Poolable>> _pickables = new List<PickableStats<Poolable>>();
	public List<PickableStats<Poolable>> Pickables { get => _pickables; set { _pickables = value; RecalculateTotalOdds(); } }


	[Tooltip("Automatically calculated total odds of all pickables.")]
	[HideInInspector][SerializeField] private int _totalOdds = -1;
	public int TotalOdds => _totalOdds;



	/// <summary> Adds new pickable to the list of possible picks. </summary>
	public void AddPickable(PickableStats<Poolable> spawnStats)
	{
		_pickables.Add(spawnStats);
		RecalculateTotalOdds();
	}

	/// <summary> Removes a pickable from the list of possible picks at given index. </summary>
	public void RemovePickable(int index)
	{
		_pickables.RemoveAt(index);
		RecalculateTotalOdds();
	}

	/// <summary> Returns new pooled instance of a random poolable from the list of possible pickables. Can return null as 'nothing'.</summary>
	public Poolable GetRandom()
	{
		// Tries to verify the odds are calculated, at least at some point.
		if (_totalOdds < 0) { RecalculateTotalOdds(); }

		// Get random number and check which pickable has that number in it's 'range'.
		int random = Random.Range(0, _totalOdds);

		for (int i = 0; i < _pickables.Count; i++)
		{
			// Reduce chances from random until random is less than 0. If less than 0, choose that pickable as result.
			random -= _pickables[i].Chances;
			if (random < 0)
			{
				// Check if pickable is null. Can be null to be 'miss' roll which returns nothing.
				if (_pickables[i].Pickable == null) { return null; }

				// Return new pooled instance of the pickable.
				return _pickables[i].Pickable.GetFromPool();
			}
		}

		if (_pickables.Count == 0) { Debug.LogWarning($"Trying to get random pickable from an empty list! {this.GetHashCode()}"); }
		else { Debug.LogWarning($"Incorrect total odds, forgot to recalculate or set chances? {this.GetHashCode()}"); }

		// Return nothing in case of error.
		return null;
	}

	/// <summary> Calculates the total odds of all pickables in the list. </summary>
	public void RecalculateTotalOdds()
	{
		_totalOdds = 0;
		_pickables.ForEach(x => { _totalOdds += x.Chances; });
	}





	[System.Serializable]
	public class PickableStats<T>
	{
		public PickableStats(T pickable, int chances)
		{ _pickable = pickable; _chances = chances; }


		[Tooltip("Value or object which is picked.")]
		[SerializeField] private T _pickable;
		public T Pickable { get => _pickable; set { _pickable = value; } }


		[Tooltip("Chances of being picked, out of the total chances.")]
		[Min(0)][SerializeField] private int _chances = 1;
		public int Chances { get => _chances; set { _chances = value; if (_chances < 0) { _chances = 0; } } }
	}
}