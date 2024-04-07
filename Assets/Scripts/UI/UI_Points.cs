using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UI_Points : MonoBehaviour
{
	[Tooltip("TMP_Text component attached to this object.")]
	[SerializeField] private TMP_Text _pointsText;

	[Tooltip("ValueAnimator controlling the change from one valule to another. This animator should update the pure points amount and update it to this component's UpdatePointsTxt.")]
	[SerializeField] private ValueAnimator _pointsAnimator;

	[Tooltip("ValueTarget used by _pointsAnimator to change the point value")]
	[SerializeField] private ValueTarget_Float _pointsTarget;

	[Tooltip("Duration per point set to PointsAnimator based on change amount.")]
	[SerializeField] private float _durationPerPoint = 0.1f;

	[Tooltip("Maximum duration set to PointsAnimator based on change amount.")]
	[SerializeField] private float _maxDuration = 1f;

	[Tooltip("Invoked whenever the point amount changed. Outputs the current amount of points.")]
	[SerializeField] private UnityEvent<float> _onPointsUpdate = new UnityEvent<float>();

	/// <summary> ValueEvaluator in _pointsTarget. </summary>
	private ValueEvaluator_Float _pointsEvaluator = null;

	private void Awake()
	{
		_pointsEvaluator = _pointsTarget.Evaluator as ValueEvaluator_Float;
	}

	/// <summary> Updates the current points to UI. </summary>
	/// <param name="points"> Total points at the time. </param>
	public void UpdatePoints(float points)
	{
		// Update the animation curve in _pointsEvaluator so that it starts from the current displayed point amount and ends to the given points amount.
		Keyframe[] keys = _pointsEvaluator.Curve.keys;
		keys[0] = new Keyframe(keys[0].time, _pointsTarget.Evaluate(_pointsAnimator.CurrentProgress));
		keys[keys.Length - 1] = new Keyframe(keys[keys.Length - 1].time, points);
		_pointsEvaluator.Curve.keys = keys;

		// Adjust the _pointsAnimator duration based on the points change amount.
		_pointsAnimator.Duration = Mathf.Clamp(_durationPerPoint * Mathf.Abs(keys[keys.Length - 1].value - keys[0].value), 0, _maxDuration);

		_pointsAnimator.Start();
		_onPointsUpdate.Invoke(points);
	}

	/// <summary> Sets the current visual point amount to UI in text format. </summary>
	public void UpdatePointsTxt(float amount)
	{ _pointsText.text = $"Points: {Mathf.RoundToInt(amount)}"; }
}
