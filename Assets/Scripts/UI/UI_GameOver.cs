using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
	[SerializeField] private ValueAnimator _scrollDownAnimator;
	public void GameOver()
	{
		gameObject.SetActive(true);
		_scrollDownAnimator.Start();
	}

	public void RestartGame()
	{
		GameManager.Instance.IsReplay = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
