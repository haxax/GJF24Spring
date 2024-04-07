using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DamageBox : MonoBehaviour
{
	[Tooltip("Amount of damage this object deals for each collision with HealthBox.")]
	[SerializeField] private float _damageAmount = 1f;
	public float DamageAmount => _damageAmount;

	[Tooltip("Invoked whenever HealthBox collides with this object. Call ReturnToPool or such if one-time damage object.")]
	[SerializeField] private UnityEvent<float> _onDealDamage = new UnityEvent<float>();
	public UnityEvent<float> OnDealDamage => _onDealDamage;



	private void OnValidate()
	{
		// Make sure both HealthBox and DamageBox are on same layer. HealthBoxes should be non-triggers and DamageBoxes triggers.
		GetComponent<Collider>().isTrigger = true;
		gameObject.OnValidateSetLayer_EditorOnly(10); // Set layer to 10 'HP'
	}


	private void OnTriggerEnter(Collider other)
	{
		HealthBox healthBox = other.GetComponent<HealthBox>();
		if (healthBox == null) { return; }

		healthBox.DealDamage(DamageAmount);
		OnDealDamage.Invoke(DamageAmount);
	}
}