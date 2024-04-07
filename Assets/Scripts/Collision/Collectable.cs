using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
	[Tooltip("Type of this collectable. Can be collected only by collectors who have behaviour defined for this type.")]
	[SerializeField] private CollectableType _type = CollectableType.none;
	public CollectableType Type { get => _type; private set => _type = value; }


	[Tooltip("Score, amount etc. given by this collectable.")]
	[SerializeField] private float _amount = 0f;
	public float Amount { get => _amount; private set => _amount = value; }


	[Tooltip("Invoked whenever this object is collected. Call ReturnToPool or such if one-time collectable.")]
	[SerializeField] private UnityEvent _onCollect = new UnityEvent();
	private UnityEvent OnCollect => _onCollect;



	private void OnValidate()
	{
		// Make sure both Collector and Collectable are on same layer. Collectors should be non-triggers and Collectables triggers.
		GetComponent<Collider>().isTrigger = true;
		gameObject.OnValidateSetLayer_EditorOnly(11); // Set layer to 11 'Collect'
	}

	public void Collect(Collector collector) { OnCollect.Invoke(); }
}


public enum CollectableType
{
	none = 0,
	point = 1,
	health = 2,
	ammo = 3,
}