using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SplineMovementController : SplineMovement
{
	[Tooltip("How fast the boat can rotate to the max rotation. 2 = 0.5 second to max rotation.")]
	[SerializeField] private float _rotationAccelaration = 1f;

	[Tooltip("How fast the boat rotates back to zero rotation. 2 = 0.5 second to zero rotation.")]
	[SerializeField] private float _rotationDecelaration = 0.5f;

	protected override void Update()
	{
		// Reduce CurrentRotationState towards zero.
		if (CurrentRotationState != 0f)
		{ CurrentRotationState = (CurrentRotationState / Mathf.Abs(CurrentRotationState)) * Mathf.Clamp((Mathf.Abs(CurrentRotationState) - (_rotationDecelaration * Time.deltaTime)), 0f, 1f); }

		// Read rotation input.
		if (Input.GetKey(KeyCode.LeftArrow)) { CurrentRotationState += _rotationAccelaration * Time.deltaTime; }
		if (Input.GetKey(KeyCode.RightArrow)) { CurrentRotationState -= _rotationAccelaration * Time.deltaTime; }


		// Keep CurrenTotationState between -1 to 1.
		CurrentRotationState = Mathf.Clamp(CurrentRotationState, -1f, 1f);

		base.Update();

		// Generate new chunk if already past half point of current spline and next chunk isn't generated.
		if (SplineTransform.Position > 0.5f && CurrentSpline.NextChunk == null) { CurrentSpline.GenerateNextChunk(); }
	}
}