using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSpawner : MonoBehaviour
{
	[Tooltip("Prefab which is spawned.")]
	[SerializeField] private Poolable _prefab;

	[Tooltip("Object is spawned to this local position in relation to this object if spawned using SpawnToPresetPosition().")]
	[SerializeField] private Vector3 _manualLocalPosition = new Vector3();

	[Tooltip("Object is spawned to this local rotation in relation to this object if spawned using SpawnToPresetPosition().")]
	[SerializeField] private Vector3 _manualLocalEulerAngles = new Vector3();

	[Tooltip("Object is always spawned using this scale.")]
	[SerializeField] private Vector3 _manualScale = Vector3.one;




	/// <summary> Spawns the prefab to the same position and rotation as this object. </summary>
	public void Spawn()
	{ Spawn(transform.position, transform.rotation); }

	/// <summary> Spawns the prefab to the given position using Qaternion.identity as rotation. </summary>
	public void Spawn(Vector3 position)
	{ Spawn(position, Quaternion.identity); }

	/// <summary> Spawns the prefab to the given position and rotation. </summary>
	public void Spawn(Vector3 position, Quaternion rotation)
	{
		Poolable instance = _prefab.GetFromPool();
		instance.transform.position = position;
		instance.transform.rotation = rotation;
		instance.transform.localScale = _manualScale;
	}

	/// <summary> Spawns the prefab to the preset position and rotation. </summary>
	public void SpawnToPresetPosition()
	{
		// Add manual position and euler angles to this objects position and rotation. Use the sum as spawn settings.
		Spawn(transform.TransformPoint(_manualLocalPosition), transform.rotation * Quaternion.Euler(_manualLocalEulerAngles));
	}
}
