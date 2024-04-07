using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	[Tooltip("Chunk where the player is spawned at the start of the game.")]
	[SerializeField] private SplineChunk _startChunkPrefab;

	[Tooltip("Prefab of the player character.")]
	[SerializeField] private Poolable _playerPrefab;

	public bool IsReplay { get; set; } = false;

	protected override void OnInstantiate()
	{
		DontDestroyOnLoad(gameObject);
		Time.timeScale = 0f;

		SpawnGame();
	}

	public void SpawnGame()
	{
		// Create player.
		SplineTransform player = _playerPrefab.GetFromPool().GetComponent<SplineTransform>();

		// Create first chunks.
		SplineChunk startChunk = (SplineChunk)_startChunkPrefab.GetFromPool();
		startChunk.GenerateChunk();

		// Set player to the first chunk.
		player.CurrentSpline = startChunk;
		player.Height = 0.0f;
		player.Position = 0.1f;
		player.Rotation = 0.0f;

		// Create additional four chunks so that there is preloaded stuff.
		startChunk.GenerateNextChunkWithoutDespawning();
		startChunk.NextChunk.GenerateNextChunkWithoutDespawning();
		startChunk.NextChunk.NextChunk.GenerateNextChunkWithoutDespawning();
		startChunk.NextChunk.NextChunk.NextChunk.GenerateNextChunkWithoutDespawning();
	}

	public void StartGame()
	{
		Time.timeScale = 1f;
	}

	public void GameOver()
	{
		Time.timeScale = 0f;
		UIManager.Instance.GameOver();
	}
}