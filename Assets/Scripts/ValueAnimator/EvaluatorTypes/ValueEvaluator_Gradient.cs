using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueEvaluator_Gradient : ValueEvaluator<Color>
{
	[SerializeField] private Gradient _gradient;

	public override Color Evaluate(float state)
	{ return _gradient.Evaluate(state); }
}