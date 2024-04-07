using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SplineMovement : MonoBehaviour
{
	[Tooltip("Movement amount along spline per second. 1 = full spline, 0.1 = 10% of spline.")]
	[SerializeField] private float _movementSpeed = 0.1f;
	public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }


	[Tooltip("Movement amount round spline in angles per second.")]
	[SerializeField] private float _rotationSpeed = 45f;
	public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }


	[Space(20)]
	[Tooltip("SplineTransform attached to this object.")]
	[SerializeField] private SplineTransform _splineTransform;
	public SplineTransform SplineTransform { get => _splineTransform; private set => _splineTransform = value; }


	[Tooltip("Invoked each frame, outputs CurrentRotationState which is value between -1 to 1.")]
	[SerializeField] private UnityEvent<float> _onRotate = new UnityEvent<float>();


	// SplineChunk this object is currently moving along.
	protected SplineChunk CurrentSpline => SplineTransform.CurrentSpline;

	/// <summary> Value between -1 to 1. 1 = full speed forward. </summary>
	public float CurrentMovementState { get; set; } = 1f;

	/// <summary> Value between -1 to 1. 1 = full rotation to left. </summary>
	public float CurrentRotationState { get; set; } = 0f;


	protected virtual void Update()
	{
		// Update position.
		SplineTransform.Position += Time.deltaTime * MovementSpeed * CurrentMovementState;
		SplineTransform.Rotation += (Time.deltaTime * RotationSpeed * CurrentRotationState) / CurrentSpline.Radius;
		_onRotate.Invoke(CurrentRotationState);
	}
}
