using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueTarget_Vector2 : ValueTarget_T<Vector2, ValueEvaluator_Vector2>
{
	protected override void SetValue(Vector2 evaluatedValue, float state)
	{
		OnUpdate.Invoke(evaluatedValue);
	}
}