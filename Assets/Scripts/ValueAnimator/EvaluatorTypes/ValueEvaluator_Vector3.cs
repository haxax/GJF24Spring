using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueEvaluator_Vector3 : ValueEvaluator<Vector3>
{
	[SerializeField] private AnimationCurve _curveX = new AnimationCurve();
	[SerializeField] private AnimationCurve _curveY = new AnimationCurve();
	[SerializeField] private AnimationCurve _curveZ = new AnimationCurve();

	public override Vector3 Evaluate(float state)
	{ return new Vector3(_curveX.Evaluate(state), _curveY.Evaluate(state), _curveZ.Evaluate(state)); }
}