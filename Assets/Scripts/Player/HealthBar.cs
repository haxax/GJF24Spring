using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private ValueAnimator _barScaleAnimator;
	[SerializeField] private HealthBox _healthBox;

	public void UpdateHealthBar(float currenHP)
	{
		// Calculate the bar progress from 1 to 0 based on remaining and start hp.
		_barScaleAnimator.Simulate = currenHP / _healthBox.StartHp;
	}
}
