using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueAnimator : MonoBehaviour
{
	[Tooltip("If true, TimeScale doesn't affect the behaviour of this component.")]
	[SerializeField] private bool _unscaledTime = false;
	public bool UnscaledTime { get => _unscaledTime; set => _unscaledTime = value; }


	[Tooltip("If true, playing is freezed until visible to a camera again.")]
	[SerializeField] private bool _freezeIfInvisible = true;
	public bool FreezeIfInvisible { get => _freezeIfInvisible; set => _freezeIfInvisible = value; }


	[Tooltip("If true, LoopReplay() is automatically called when end is reached.")]
	[SerializeField] private bool _loop = true;
	public bool Loop { get => _loop; set => _loop = value; }


	[Tooltip("Time in seconds it takes to reach the end from the beginning.")]
	[Min(0.000f)][SerializeField] private float _duration = 1f;
	public float Duration { get => _duration; set => _duration = value; }


	[Tooltip("Events that can occur during the playing.")]
	[SerializeField] private List<PlayerEvent> _events = new List<PlayerEvent>();
	public List<PlayerEvent> Events { get => _events; set => _events = value; }


	[Tooltip("Linked components which will be affected by this player.")]
	[SerializeField] private List<ValueTarget> _targets = new List<ValueTarget>();
	public List<ValueTarget> Targets { get => _targets; set => _targets = value; }



	[Tooltip("Value between 0 to 1, indicates the progress of the player.")]
	[Range(0, 1)][SerializeField] private float _currentProgress = 0f;
	public float CurrentProgress { get => _currentProgress; private set => _currentProgress = value; }


	private PlayerState _currentState = PlayerState.Stopped;
	public PlayerState CurrentState { get => _currentState; private set => _currentState = value; }


	private float _simulate = 0f;
	/// <summary> Updates the animator to a state without playing. </summary>
	public float Simulate { get => _simulate; set { _simulate = value; CurrentProgress = _simulate; UpdateState(); } }



	/*	----- States and what can be done to them -----
	Playing	> Cen be stopped, paused, freezed, started.
	Stopped	> Can be started, played.
	Paused	> Can be stopped, started, played.
	Frozen	> Can be stopped, paused, unfreezed, started, played.
	*/

	/// <summary> Starts playing from the beginning. Can be called at any state. </summary>
	public void Start()
	{
		this.enabled = true;
		CurrentState = PlayerState.Playing;
		CurrentProgress = 0f;
		UpdateState();
		TryEvents(PlayerEventState.Start);
	}

	/// <summary> Stops playing and sets the progress to beginning. Can't be called if already stopped. </summary>
	public void Stop()
	{
		if (_currentState == PlayerState.Stopped) { return; }
		this.enabled = false;
		CurrentState = PlayerState.Stopped;
		CurrentProgress = 0f;
		UpdateState();
		TryEvents(PlayerEventState.Stop);
	}



	/// <summary> Continues playing from the previous point. If Stopped, invokes AbortedRestart and then Start instead. Can't be called if already playing. </summary>
	public void Play()
	{
		if (CurrentState == PlayerState.Playing) { return; }
		if (CurrentState == PlayerState.Stopped)
		{
			TryEvents(PlayerEventState.AbortedRestart);
			Start();
		}
		else
		{
			this.enabled = true;
			CurrentState = PlayerState.Playing;
			TryEvents(PlayerEventState.Play);
		}
	}

	/// <summary> Pauses playing and keeps the current progress. Can't be called if already stopped or paused. </summary>
	public void Pause()
	{
		if (CurrentState == PlayerState.Stopped || CurrentState == PlayerState.Paused) { return; }
		this.enabled = false;
		CurrentState = PlayerState.Paused;
		TryEvents(PlayerEventState.Pause);
	}



	/// <summary> Continues playing from the previous point if frozen. Can't be called unless frozen. </summary>
	public void Unfreeze()
	{
		if (CurrentState != PlayerState.Frozen) { return; }
		this.enabled = true;
		CurrentState = PlayerState.Playing;
		TryEvents(PlayerEventState.Unfreeze);
	}

	/// <summary> Freezes the playing and keeps the current progress. Can't be called unless playing. </summary>
	public void Freeze()
	{
		if (CurrentState != PlayerState.Playing) { return; }
		this.enabled = false;
		CurrentState = PlayerState.Frozen;
		TryEvents(PlayerEventState.Freeze);
	}



	/// <summary> Called when player progress reaches the end and is looping. </summary>
	private void LoopReplay()
	{
		CurrentState = PlayerState.Playing;
		CurrentProgress -= 1f;
		UpdateState();
		TryEvents(PlayerEventState.LoopReplay);
	}

	/// <summary> Called when player progress reaches the end and isn't looping. </summary>
	private void End()
	{
		this.enabled = false;
		CurrentState = PlayerState.Stopped;
		CurrentProgress = 1f;
		UpdateState();
		TryEvents(PlayerEventState.End);
	}


	/// <summary> Called when object becomes invisible to all cameras. </summary>
	private void OnBecameInvisible()
	{ if (FreezeIfInvisible) { Freeze(); } }

	/// <summary> Called when object becomes visible to any camera if not visible. </summary>
	private void OnBecameVisible()
	{ if (FreezeIfInvisible) { Unfreeze(); } }


	public void Update()
	{
		if (_unscaledTime) { CurrentProgress += Time.unscaledDeltaTime / Duration; }
		else { CurrentProgress += Time.deltaTime / Duration; }

		if (CurrentProgress >= 1f)
		{
			if (Loop) { LoopReplay(); }
			else { End(); }
		}
		else { UpdateState(); }
	}

	/// <summary> Updates all linked components' state to match the CurrentProgress. </summary>
	public void UpdateState()
	{ Targets.ForEach(x => x.UpdateValue(CurrentProgress)); }

	/// <summary> Invokes all the events that has the 'state' flag included in OccurStates. </summary>
	/// <param name="state"> Each event with this state flag will be invoked.</param>
	public void TryEvents(PlayerEventState state)
	{
		foreach (PlayerEvent e in Events)
		{ e.TryInvoke(state); }
	}

	[Flags]
	public enum PlayerEventState
	{
		Start = 1,
		Stop = 2,
		Play = 4,
		Pause = 8,
		Freeze = 16,
		Unfreeze = 32,
		End = 64,
		LoopReplay = 128,
		AbortedRestart = 256,
	}

	public enum PlayerState
	{
		Playing = 1,
		Stopped = 2,
		Paused = 3,
		Frozen = 4,
	}

	[System.Serializable]
	public class PlayerEvent
	{
		[SerializeField] private PlayerEventState _occurStates = 0;
		[SerializeField] private UnityEvent _onEvent = new UnityEvent();

		public UnityEvent OnEvent { get => _onEvent; set => _onEvent = value; }
		public PlayerEventState OccurStates { get => _occurStates; set => _occurStates = value; }

		public float LastInvokeTime { get; set; } = -100000f;

		public bool TryInvoke(PlayerEventState state)
		{
			// To avoid double invoking during a same frame. For example if Stop and End invokes the same event.
			if (LastInvokeTime == Time.unscaledTime)
			{ return false; }

			if (!OccurStates.HasFlag(state))
			{ return false; }

			LastInvokeTime = Time.unscaledTime;
			OnEvent.Invoke();
			return true;
		}
	}
}