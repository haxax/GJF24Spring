using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTarget_Vector3 : ValueTarget_T<Vector3, ValueEvaluator_Vector3>
{
	protected override void SetValue(Vector3 evaluatedValue, float state)
	{
		OnUpdate.Invoke(evaluatedValue);
	}
}