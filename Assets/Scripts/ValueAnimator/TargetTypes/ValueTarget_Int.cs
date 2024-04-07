using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTarget_Int : ValueTarget_T<int, ValueEvaluator_Int>
{
	protected override void SetValue(int evaluatedValue, float state)
	{
		OnUpdate.Invoke(evaluatedValue);
	}
}