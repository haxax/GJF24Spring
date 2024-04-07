using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpecialActionsBehaviour : MonoBehaviour
{
	[SerializeField] private SplineTransform _splineTransform;
	[SerializeField] private SplineMovement _splineMovement;

	[Space(20)]
	[Tooltip("Time required between pressing down arrow.")]
	[SerializeField] private float _downArrowCooldown = 1f;
	[SerializeField] private ValueAnimator _diveAnimator;
	[SerializeField] private ValueTarget_Float _rotationTarget;
	[SerializeField] private ValueTarget_Float _heighTarget;

	[Space(20)]
	[Tooltip("Time required between pressing up arrow.")]
	[SerializeField] private float _upArrowCooldown = 1f;

	[Space(20)]
	[SerializeField] private UnityEvent _onDownArrow = new UnityEvent();
	[SerializeField] private UnityEvent _onUpArrow = new UnityEvent();

	// Calculates the remaining cooldown time.
	private float _downArrowCooldownTimer = 0f;
	private float _upArrowCooldownTimer = 0f;

	private void Update()
	{

		// Update cooldown timers.
		if (_downArrowCooldownTimer > 0f)
		{
			_downArrowCooldownTimer -= Time.deltaTime;
			if (_downArrowCooldownTimer < 0f)
			{ _downArrowCooldownTimer = 0f; }
		}

		if (_upArrowCooldownTimer > 0f)
		{
			_upArrowCooldownTimer -= Time.deltaTime;
			if (_upArrowCooldownTimer < 0f)
			{ _upArrowCooldownTimer = 0f; }
		}

		// Read bonus action inputs.
		if (_downArrowCooldownTimer <= 0f && Input.GetKeyDown(KeyCode.DownArrow))
		{ DownArrow(); }

		if (_upArrowCooldownTimer <= 0f && Input.GetKeyDown(KeyCode.UpArrow))
		{ UpArrow(); }
	}

	private void DownArrow()
	{
		_downArrowCooldownTimer = _downArrowCooldown;

		// Setup rotation keys to flip the boat 180 degrees.
		Keyframe[] rotationKeys = _rotationTarget.Evaluator.Curve.keys;
		rotationKeys[0].value = _splineTransform.Rotation;

		if (_splineMovement.CurrentRotationState > 0f)
		{ rotationKeys[rotationKeys.Length - 1].value = _splineTransform.Rotation + 180f; }
		else { rotationKeys[rotationKeys.Length - 1].value = _splineTransform.Rotation - 180f; }
		_rotationTarget.Evaluator.Curve.keys = rotationKeys;

		// Setup height keys to dive to the center and back.
		Keyframe[] heightKeys = _heighTarget.Evaluator.Curve.keys;
		heightKeys[0].value = _splineTransform.Height;
		heightKeys[1].value = -_splineTransform.CurrentSpline.Radius;
		heightKeys[heightKeys.Length - 1].value = _splineTransform.Height;
		_heighTarget.Evaluator.Curve.keys = heightKeys;

		_diveAnimator.Start();

		_onDownArrow.Invoke();
	}
	private void UpArrow()
	{
		_upArrowCooldownTimer = _upArrowCooldown;


		_onUpArrow.Invoke();
	}
}
