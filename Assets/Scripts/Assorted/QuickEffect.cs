using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickEffect : MonoBehaviour
{
	[Tooltip("Particle System attached to this object.")]
	[SerializeField] private ParticleSystem _mainParticle;

	[Tooltip("Audio Source attached to this object.")]
	[SerializeField] private AudioSource _audioSource;

	[Tooltip("Poolable attached to this object.")]
	[SerializeField] private Poolable _poolable;

	public void OnSpawn()
	{
		// Play only attached systems.
		if (_mainParticle != null) { _mainParticle.Play(); }
		if (_audioSource != null) { _audioSource.Play(); }
	}


	private void FixedUpdate()
	{
		// Return to pool if all systems have stopped playing.
		if (_mainParticle != null && _mainParticle.isPlaying) { return; }
		if (_audioSource != null && _audioSource.isPlaying) { return; }
		_poolable.ReturnToPool();
	}
}
