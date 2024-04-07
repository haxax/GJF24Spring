using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTarget_Float : ValueTarget_T<float, ValueEvaluator_Float>
{
	protected override void SetValue(float evaluatedValue, float state)
	{
		OnUpdate.Invoke(evaluatedValue);
	}
}