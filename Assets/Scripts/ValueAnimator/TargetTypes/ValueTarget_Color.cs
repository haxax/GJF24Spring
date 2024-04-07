using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTarget_Color : ValueTarget_T<Color, ValueEvaluator_Gradient>
{
	protected override void SetValue(Color evaluatedValue, float state)
	{
		OnUpdate.Invoke(evaluatedValue);
	}
}