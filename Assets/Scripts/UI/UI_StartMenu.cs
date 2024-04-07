using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StartMenu : MonoBehaviour
{
	[SerializeField] private ValueAnimator _scrollDownAnimator;

	private void Start()
	{
		if (GameManager.Instance.IsReplay) { GameManager.Instance.SpawnGame(); StartGame(); }
	}

	public void StartGame()
	{
		GameManager.Instance.StartGame();
		gameObject.SetActive(false);
	}
}