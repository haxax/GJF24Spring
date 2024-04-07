using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
	[Tooltip("SplineChunk attached to this object.")]
	[SerializeField] private SplineChunk _chunk;

	[Tooltip("How many sections are around the spline for each vertical section.")]
	[SerializeField] private int _angularSections = 12;

	[Tooltip("How many sections are along the spline for each angular section.")]
	[SerializeField] private int _verticalSections = 10;

	[Tooltip("Only select poolables with SplineTransform. These objects might be spawned along the spline when the spline is created.")]
	[SerializeField] private RandomObjectPicker _spawnPool;


	/// <summary> Spawns _angularSections * _vericalSections amount of random objects from _spawnPool along the _chunk spline.</summary>
	public void SpawnObjects()
	{
		// Calculate section sizes.
		float sectionAngleSize = 360f / _angularSections;
		float sectionLength = 1.0f / _verticalSections;

		// Running values to calculate spawn points.
		float spawnAngle = 0f;
		float spawnPosition = 0f;

		for (int i = 0; i < _verticalSections; i++)
		{
			for (int j = 0; j < _angularSections; j++)
			{
				// Calculate spawn point for each section and spawn random object.
				spawnPosition = Random.Range(i * sectionLength, (i + 1) * sectionLength);
				spawnAngle = Random.Range(j * sectionAngleSize, (j + 1) * sectionAngleSize);
				SpawnRandomObject(spawnPosition, spawnAngle);
			}
		}
	}

	/// <summary> Tries to get random object from pool and spawn it at the given location. </summary>
	/// <param name="position"> Position along the spline (0 to 1) where object will be spawned. </param>
	/// <param name="rotation"> Angle around the spline where object will be spawned. </param>
	private void SpawnRandomObject(float position, float rotation)
	{
		// Try get a random pooled instance from spawn pool.
		Poolable newObj = _spawnPool.GetRandom();
		if (newObj == null) { return; }

		// Make sure the object has SplineTransform.
		SplineTransform splineTransform = newObj.GetComponent<SplineTransform>();
		if (splineTransform == null)
		{
			Debug.Log($"Trying to spawn {newObj.name} but is missing SplineTransform.");
			newObj.ReturnToPool();
			return;
		}

		// Set object to the given location.
		splineTransform.CurrentSpline = _chunk;
		splineTransform.Rotation = rotation;
		splineTransform.Position = position;
	}
}