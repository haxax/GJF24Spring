using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[Tooltip("Reference to component handling playtime points.")]
	[SerializeField] private UI_Points _points;

	[Tooltip("Reference to component handling game over.")]
	[SerializeField] private UI_GameOver _gameOver;

	/// <summary> Updates the current points to UI. </summary>
	/// <param name="points"> Total points at the time. </param>
	public void UpdatePoints(float points) => _points.UpdatePoints(points);

	public void GameOver() => _gameOver.GameOver();

	public void StartGame()
	{
		GameManager.Instance.StartGame();
	}
}