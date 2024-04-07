using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class HealthBox : MonoBehaviour
{
	[Tooltip("Amount of the HP at the start of the game.")]
	[SerializeField] private float _startHp = 10f;
	public float StartHp { get => _startHp; private set { _startHp = value; } }


	[Tooltip("Amount of the HP that can't be exceeded.")]
	[SerializeField] private float _maxHp = 10f;
	public float MaxHp { get => _maxHp; private set { _maxHp = value; } }


	[Tooltip("Invoked whenever damage is taken.")]
	[SerializeField] private UnityEvent<float> _onTakeDamage = new UnityEvent<float>();
	public UnityEvent<float> OnTakeDamage => _onTakeDamage;

	[Tooltip("Invoked whenever HP changes.")]
	[SerializeField] private UnityEvent<float> _onHpUpdate = new UnityEvent<float>();
	public UnityEvent<float> OnHpUpdate => _onHpUpdate;

	[Tooltip("Invoked whenever CurrentHp goes to 0.")]
	[SerializeField] private UnityEvent _onDeath = new UnityEvent();
	public UnityEvent OnDeath => _onDeath;


	private float _currenHp = 0f;
	/// <summary> Amount of the HP at the moment. Between 0 and MaxHp. </summary>
	public float CurrentHp { get => _currenHp; private set { _currenHp = Mathf.Clamp(value, 0.0f, MaxHp); OnHpUpdate.Invoke(_currenHp); } }

	public bool IsDead { get; private set; } = true;

	/// <summary> Time when this object collided with something last time. </summary>
	private double _previousCollisionTimeStamp = 0f;



	private void OnValidate()
	{
		// Make sure both HealthBox and DamageBox are on same layer. HealthBoxes should be non-triggers and DamageBoxes triggers.
		GetComponent<Collider>().isTrigger = false;
		gameObject.OnValidateSetLayer_EditorOnly(10); // Set layer to 10 'HP'
	}

	/// <summary> Resets HP back to start state. </summary>
	public void ResetHp()
	{
		CurrentHp = StartHp;
		IsDead = false;
		_previousCollisionTimeStamp = 0;
	}

	/// <summary> Add health without invoking damage effects. </summary>
	public void AddHealth(float amount)
	{ CurrentHp += amount; }

	/// <summary> Deals damage to this HealthBox. </summary>
	/// <param name="damage"> Amount of damage dealt. </param>
	public void DealDamage(float damage)
	{
		// Don't deal damage if already dead.
		if (IsDead) { return; }

		CurrentHp -= damage;
		OnTakeDamage.Invoke(damage);

		if (CurrentHp <= 0f)
		{
			IsDead = true;
			OnDeath.Invoke();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		// Ignore collisions if dead or already collided during this frame.
		if (IsDead || HasCollidedDuringThisFrame()) { return; }

		// DamageBox handles the collision with them.
		// Check collision with other HealthBoxes.
		HealthBox healthBox = collision.collider.GetComponent<HealthBox>();
		if (healthBox == null) { return; }

		TimeStampCollision();
		healthBox.TimeStampCollision();

		// Exchange damage based on the remaining HP each has. Weaker should die.
		float ownDamage = CurrentHp;
		float otherDamage = healthBox.CurrentHp;

		healthBox.DealDamage(ownDamage);
		DealDamage(otherDamage);
	}

	/// <summary> Capture the current TimeStamp to check when the object has collided last time. </summary>
	public void TimeStampCollision() { _previousCollisionTimeStamp = Time.timeSinceLevelLoadAsDouble; }

	/// <summary> Checks if the object has already collided during this frame. </summary>
	public bool HasCollidedDuringThisFrame()
	{
		if (_previousCollisionTimeStamp >= Time.timeSinceLevelLoadAsDouble)
		{ return true; }
		return false;
	}
}