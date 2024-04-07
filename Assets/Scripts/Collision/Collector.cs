using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary> Component to collect Collectables. Add to separate child from other collider faetures. </summary>
[RequireComponent(typeof(Collider))]
public class Collector : MonoBehaviour
{
	[Tooltip("Setup what happens when something is collected. Each CollectableType should have only one CollectEvent defined.")]
	[SerializeField] private List<CollectEvent> OnCollect = new List<CollectEvent>();

	private void OnValidate()
	{
		// Make sure both Collector and Collectable are on same layer. Collectors should be non-triggers and Collectables triggers.
		GetComponent<Collider>().isTrigger = false;
		gameObject.OnValidateSetLayer_EditorOnly(11); // Set layer to 11 'Collect'
	}

	private void OnTriggerEnter(Collider other)
	{
		Collectable collectable = other.GetComponent<Collectable>();
		if (collectable == null) { return; }

		// Check if Collectable is set to type this Collector can use.
		for (int i = 0; i < OnCollect.Count; i++)
		{
			if (OnCollect[i].CollectableType == collectable.Type)
			{
				// Collect if matching types.
				OnCollect[i].OnCollect.Invoke(collectable.Amount);
				collectable.Collect(this);
				break;
			}
		}
	}
}

[System.Serializable]
public class CollectEvent
{
	[SerializeField] private CollectableType _collectableType = CollectableType.none;
	[SerializeField] private UnityEvent<float> _onCollect = new UnityEvent<float>();

	public CollectableType CollectableType => _collectableType;
	public UnityEvent<float> OnCollect => _onCollect;
}