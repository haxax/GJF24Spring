using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	private float _points = 0f;
	/// <summary> Amount of points collected during the game. </summary>
	public float Points { get => _points; set { _points = value; UIManager.Instance.UpdatePoints(_points); } }



	/// <summary> Resets stats to starting state. </summary>
	public void ResetStats()
	{
		Points = 0;
	}

	/// <summary> Adds given amount of points to current Points. </summary>
	public void AddPoints(float amount) { Points += amount; }

	public void PlayerDied() { GameManager.Instance.GameOver(); }
}