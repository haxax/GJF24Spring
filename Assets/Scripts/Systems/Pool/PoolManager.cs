using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	/// <summary> Currently pooled objects. Item1 = PoolableId, Item2 = Queue of inactive objects. </summary>
	public List<(string, Queue<Poolable>)> PooledObjects { get; private set; } = new List<(string, Queue<Poolable>)>();


	/// <summary> Use Poolable.GetFromPool() instead to properly setup poolable. Intended for Poolable to use. </summary>
	public Poolable GetFromPool(Poolable prefab)
	{
		if (prefab == null) { Debug.Log($"Trying to get null reference from pool."); return null; }

		Queue<Poolable> queue = FindQueue(prefab);
		Poolable poolable = GetFromQueue(prefab, queue);

		return poolable;
	}

	/// <summary> Finds or creates matching queue for given prefab. </summary>
	private Queue<Poolable> FindQueue(Poolable prefab)
	{
		// Find existing queue based on PoolableId.
		foreach (var poolQueue in PooledObjects)
		{
			if (poolQueue.Item1 != prefab.PoolableId) { continue; }
			return poolQueue.Item2;
		}

		// Create new queue for the PoolableId if one doesn't exist.
		PooledObjects.Add((prefab.PoolableId, new Queue<Poolable>()));
		return PooledObjects[PooledObjects.Count - 1].Item2;
	}

	/// <summary> Finds existing or creates new instance of the given prefab from the given queue. </summary>
	private Poolable GetFromQueue(Poolable prefab, Queue<Poolable> queue)
	{
		if (queue.Count > 0) { return queue.Dequeue(); }
		else { return CreateNewInstance(prefab); }
	}

	/// <summary> Creates and returns new instance of the given prefab. </summary>
	private Poolable CreateNewInstance(Poolable prefab)
	{
		Poolable newInstance = Instantiate(prefab);
		newInstance.gameObject.name = $"Clone: {prefab.PoolableId}";
		newInstance.OnInstantiate();
		return newInstance;
	}

	/// <summary> Use Poolable.ReturnToPool() instead to properly deactiveate poolable. Intended for Poolable to use. </summary>
	public void ReturnToPool(Poolable poolable)
	{
		if (poolable == null) { Debug.Log($"Trying to return null to pool."); return; }

		Queue<Poolable> queue = FindQueue(poolable);
		queue.Enqueue(poolable);
	}
}