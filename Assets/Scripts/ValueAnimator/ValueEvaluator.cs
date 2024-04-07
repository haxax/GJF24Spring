using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ValueEvaluator<T>
{
	public abstract T Evaluate(float state);
}
