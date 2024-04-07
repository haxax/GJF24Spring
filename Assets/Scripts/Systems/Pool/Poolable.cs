using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Poolable : MonoBehaviour
{
	[Tooltip("Unique id to find matching instance from the pool.")]
	[SerializeField] private string _poolableId = "";
	public string PoolableId { get { return _poolableId; } private set { _poolableId = value; } }

	[Tooltip("Invoked whenever new instance of the poolable is created.")]
	[SerializeField] private UnityEvent OnInstantiateE = new UnityEvent();

	[Tooltip("Invoked whenever instance of the poolable is get from pool.")]
	[SerializeField] private UnityEvent OnGetFromPoolE = new UnityEvent();

	[Tooltip("Invoked whenever instance of the poolable is returned to pool.")]
	[SerializeField] private UnityEvent OnReturnToPoolE = new UnityEvent();


	virtual protected void OnValidate()
	{
		// Generate unique id if not defined.
		if (PoolableId == null || PoolableId == "")
		{ PoolableId = $"{gameObject.name}_{Random.Range(1000, 9999)}"; }
	}

	/// <summary> Returns this instance to the PoolManager and calls OnReturnToPool. </summary>
	public void ReturnToPool()
	{
		OnReturnToPool();
		gameObject.SetActive(false);
		PoolManager.Instance.ReturnToPool(this);
	}

	/// <summary> Returns new pooled instance of this Poolable and calls OnGetFromPool.</summary>
	public Poolable GetFromPool()
	{
		Poolable pooledInstance = PoolManager.Instance.GetFromPool(this);
		pooledInstance.gameObject.SetActive(true);
		pooledInstance.OnGetFromPool();
		return pooledInstance;
	}

	protected virtual void OnReturnToPool() { OnReturnToPoolE?.Invoke(); }
	protected virtual void OnGetFromPool() { OnGetFromPoolE?.Invoke(); }
	public virtual void OnInstantiate() { OnInstantiateE?.Invoke(); }
}