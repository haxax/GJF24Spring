using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

// Keep component disabled if no need to update position. Enable whenever position/rotation/height has changed to activate LateUpdate. LateUpdate should disable the component.

public class SplineTransform : MonoBehaviour
{
	[Tooltip("Poolable component attached to this object. Leave null if not poolable.")]
	[SerializeField] private Poolable _poolable;

	[Tooltip("Transform used as pivot point on the surface of spline.")]
	[SerializeField] private Transform _surfaceTransform;
	/// <summary> Transform on the surface of spline. </summary>
	public Transform SurfaceTransform => _surfaceTransform;

	/// <summary> Transform in the center of the spline. </summary>
	public Transform CenterTransform => transform;


	[Tooltip("SplineChunk this object is currently moving along.")]
	[SerializeField] private SplineChunk _currentSpline;
	public SplineChunk CurrentSpline
	{
		get => _currentSpline;
		set
		{
			// Don't re-set if trying to set same as currently set.
			if (_currentSpline == value) { return; }

			// If setting non-null spline, Unregisted from CurrentSpline.
			else if (_currentSpline != null) { _currentSpline.Registry.Unregister(this); }

			// Set value.
			_currentSpline = value;

			// If set non-null spline, Register to the new spline. Enable to update position.
			if (_currentSpline != null) { _currentSpline.Registry.Register(this); this.enabled = true; }

			// If set null spline, despawn if set so.
			else if (_despawnIfOutOfBounds) { Despawn(); }
		}
	}


	[Space(20)]
	[Tooltip("If true, object is despawned if CurrentSpline becomes null. Otherwise tries to stay within existing splines.")]
	[SerializeField] private bool _despawnIfOutOfBounds = true;

	[Tooltip("Current position along the CurrentSpline. Value between 0 to 1.")]
	[Range(0.00f, 1.00f)][SerializeField] private float _position = 0.0f;
	public float Position { get => _position; set { _position = value; this.enabled = true; ValidatePosition(); } }


	[Tooltip("Current rotation around the CurrentSpline. Value between 0 to 360.")]
	[Range(0.00f, 360.00f)][SerializeField] private float _rotation = 0.0f;
	public float Rotation { get => _rotation; set { _rotation = value; this.enabled = true; } }


	[Tooltip("Height offset from the surface level of the spline. Adjusted to the SurfaceTransform.")]
	[SerializeField] private float _height = 0.0f;
	public float Height { get => _height; set { _height = value; this.enabled = true; } }




	private void OnValidate()
	{
		// Setup SurfaceTransform if missing. Create child named so.
		if (_surfaceTransform == null)
		{
			_surfaceTransform = transform.Find("SurfaceTransform");
			if (_surfaceTransform == null)
			{
				Transform surfaceTransform = new GameObject("SurfaceTransform").transform;
				surfaceTransform.parent = transform;
				surfaceTransform.position = Vector3.zero;
				surfaceTransform.rotation = Quaternion.identity;
				_surfaceTransform = surfaceTransform;
			}
		}
	}

	// Update position whenever the object is enabled. Disable the object if no need to update position.
	private void Start()
	{ UpdatePosition(); }

	private void LateUpdate()
	{ UpdatePosition(); }


	// Running values to evaluate and calculate new position/rotation.
	private float3 _evaluatedPosition = new float3(0.0f, 0.0f, 0.0f);
	private float3 _evaluatedTangent = new float3(0.0f, 0.0f, 0.0f);
	private float3 _evaluatedUpVector = new float3(0.0f, 0.0f, 0.0f);
	private Vector3 _calculatedAngles = Vector3.zero;

	/// <summary> Updates the position/rotation/height along the CurrentSpline based on set values. </summary>
	public void UpdatePosition()
	{
		// Evaluate values from spline at the current Position.
		if (CurrentSpline.Spline.Evaluate(Position, out _evaluatedPosition, out _evaluatedTangent, out _evaluatedUpVector))
		{
			// Set the root transform to the center of spline.
			CenterTransform.localPosition = _evaluatedPosition;

			// Rotate the object based on spline rotation + current Rotation.
			_calculatedAngles = Quaternion.LookRotation(_evaluatedTangent, _evaluatedUpVector).eulerAngles;
			_calculatedAngles.z += Rotation;
			CenterTransform.localRotation = Quaternion.Euler(_calculatedAngles);

			// Set the SurfaceTransform on the surface of spline.
			SurfaceTransform.localPosition = Vector3.up * (CurrentSpline.Radius + Height);
		}
		else { Debug.Log($"Failed? {transform.root.gameObject.name} {Position}"); }

		// Disable component to improve performance.
		this.enabled = false;
	}

	/// <summary> Makes sure the Position stays within 0-1 range. Moves between different spline chunks. </summary>
	private void ValidatePosition()
	{
		if (Position > 1.0f)
		{
			Position -= 1.0f;
			CurrentSpline = CurrentSpline.NextChunk;
			ValidatePosition();
			return;
		}
		else if (Position < 0.0f)
		{
			Position += 1.0f;
			CurrentSpline = CurrentSpline.PreviousChunk;
			ValidatePosition();
			return;
		}
	}

	/// <summary> Destroys the object or returns to pool. </summary>
	public void Despawn()
	{
		this.enabled = false;
		if (CurrentSpline != null) { CurrentSpline.Registry.Unregister(this); } // TODO move to be invoked from ReturnToPool instead.

		if (_poolable == null) { Destroy(gameObject); }
		else { _poolable.ReturnToPool(); }
	}
}
