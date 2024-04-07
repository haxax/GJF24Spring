using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueEvaluator_Float : ValueEvaluator<float>
{
	[SerializeField] private AnimationCurve _curve;
	public AnimationCurve Curve { get => _curve; set => _curve = value; }

	public override float Evaluate(float state)
	{ return _curve.Evaluate(state); }
}