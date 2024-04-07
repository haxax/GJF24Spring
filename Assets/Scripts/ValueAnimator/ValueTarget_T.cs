using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class ValueTarget_T<T1, T2> : ValueTarget where T2 : ValueEvaluator<T1>
{
	// Value can be anything that can be multiplied by the evaluation. By default it is expected to be int, float, vector2, vector3, string, color etc.
	// Many use the value as multiplier. But with things like string.length it can be used as the base string value which is modified.
	[SerializeField] private T1 _modifier;

	// By default use AnimationCurve or Gradient as evaluatable. It is possible to create custom class for same purpose.
	[SerializeField] private T2 _evaluator;

	[SerializeField] private UnityEvent<T1> _onUpdate;

	public T1 Modifier { get => _modifier; set => _modifier = value; }
	public T2 Evaluator { get => _evaluator; set => _evaluator = value; }
	protected UnityEvent<T1> OnUpdate { get => _onUpdate; set => _onUpdate = value; }


	public override void UpdateValue(float state)
	{ SetValue(Evaluate(state), state); }

	public T1 Evaluate(float state)
	{ return Evaluator.Evaluate(state); }

	protected abstract void SetValue(T1 evaluatedValue, float state);

	public override Type GetVariableType()
	{ return typeof(T1); }
	public override Type GetEvaluatableType()
	{ return typeof(T2); }
}