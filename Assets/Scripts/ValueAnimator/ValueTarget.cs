using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ValueTarget : MonoBehaviour
{
	public abstract Type GetVariableType();
	public abstract Type GetEvaluatableType();
	public abstract void UpdateValue(float state);
}
