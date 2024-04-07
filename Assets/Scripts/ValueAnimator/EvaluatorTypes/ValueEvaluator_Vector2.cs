using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueEvaluator_Vector2 : ValueEvaluator<Vector2>
{
	[SerializeField] private AnimationCurve _curveX = new AnimationCurve();
	[SerializeField] private AnimationCurve _curveY = new AnimationCurve();

	public override Vector2 Evaluate(float state)
	{ return new Vector2(_curveX.Evaluate(state), _curveY.Evaluate(state)); }
}