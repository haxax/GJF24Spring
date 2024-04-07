using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueEvaluator_Int : ValueEvaluator<int>
{
	[SerializeField] private AnimationCurve _curve;

	public override int Evaluate(float state)
	{ return Mathf.RoundToInt(_curve.Evaluate(state)); }
}