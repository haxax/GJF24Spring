using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Used to track objects on spline chunk. </summary>
[System.Serializable]
public class SplineObjectRegistry
{
	[Tooltip("Objects currently on the spline.")]
	[SerializeField] private List<SplineTransform> _attachedObjects = new List<SplineTransform>();

	/// <summary> Add object to this spline chunk. </summary>
	public void Register(SplineTransform obj)
	{ if (!_attachedObjects.Contains(obj)) { _attachedObjects.Add(obj); } }

	/// <summary> Remove object to this spline chunk. </summary>
	public void Unregister(SplineTransform obj)
	{ _attachedObjects.Remove(obj); }

	/// <summary> Remove all objects from this spline chunk. </summary>
	public void ClearList()
	{
		for (int i = _attachedObjects.Count - 1; i >= 0; i--)
		{
			_attachedObjects[i].CurrentSpline = null;
		}
	}
}