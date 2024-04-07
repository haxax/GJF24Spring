using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class SplineChunk : Poolable
{
	[Tooltip("Prefab of the chunk which will be spawned after this.")]
	[SerializeField] private SplineChunk _nextChunkPrefab;

	[Tooltip("Length of tangents in spline knots. Currently randomized in code.")]
	[HideInInspector][SerializeField] private float _tangentLength = 5.0f;

	[Tooltip("Each knot is given a random distance form the 'center'. Distance is randomized withing this range.")]
	[SerializeField] private Vector2 _knotRadiusRange = new Vector2(1.0f, 10f);

	[Tooltip("Each knot is given a random distance to the previous knot. Distance is randomized withing this range.")]
	[SerializeField] private Vector2 _knotHeightIncreaseRange = new Vector2(1.0f, 10f);

	[Tooltip("Radius of the spline model. Used for creation of the mesh only.")]
	[SerializeField] private float _splineRadius = 10f;
	/// <summary> Current radius of the spline. </summary>
	public float Radius => _extrude.Radius;

	[Tooltip("Amount of edges the spline model has. Increases quality.")]
	[SerializeField] private int _splineEdges = 32;

	[Tooltip("Materials applied to the model.")]
	[SerializeField] private List<Material> _splineMaterials = new List<Material>();

	[Tooltip("Attached SplineContainer component. Generated in runtime if missing.")]
	[HideInInspector][SerializeField] private SplineContainer _spline;
	public SplineContainer Spline => _spline;

	[Tooltip("Attached SplineExtrude component. Generated in runtime if missing. Radius and edges are only applied if the component is added runtime.")]
	[HideInInspector][SerializeField] private CustomizedSplineExtrude _extrude;

	[Tooltip("Objects currently attached to this spline. Mainly added runtime. If manually added to prefab, add to this registry also.")]
	[SerializeField] private SplineObjectRegistry _objectRegistry;
	public SplineObjectRegistry Registry => _objectRegistry;

	[Tooltip("Number of the chunk.")]
	public int ChunkId { get; private set; } = 0;

	[Tooltip("Reference to the previous chunk.")]
	public SplineChunk PreviousChunk { get; private set; }

	[Tooltip("Reference to the next chunk.")]
	public SplineChunk NextChunk { get; private set; }

	[Tooltip("Values of the last knot of the previous chunk. Used to align chunks.")]
	public BezierKnot EndKnot { get; private set; } = new BezierKnot();



	/// <summary> Sets the next chunk prefab to this object. Can be used to avoid a prefab referencing itself as an instance and not prefab.</summary>
	public void SetPrefab(SplineChunk prefab) { _nextChunkPrefab = prefab; }

	/// <summary> Generates next chunk based on _nextChunkPrefab. Despawns PreviousChunk if exists. </summary>
	public void GenerateNextChunk()
	{
		GenerateNextChunkWithoutDespawning();

		if (PreviousChunk != null)
		{ PreviousChunk.DestroyChunk(); }
	}

	/// <summary> Generates next chunk based on _nextChunkPrefab without despawning previous chunks. </summary>
	public void GenerateNextChunkWithoutDespawning()
	{
		// Don't generate chunk if NextChunk already exists.
		if (NextChunk != null) { return; }

		// Setup and generate.
		NextChunk = (SplineChunk)_nextChunkPrefab.GetFromPool();
		NextChunk.gameObject.name += "" + UnityEngine.Random.Range(1000, 9999);
		//NextChunk.SetPrefab(_nextChunkPrefab); // Pass the prefab reference to the next one to avoid self referencing to an instance. Change if outer system randomizes between different chunks.
		NextChunk.GenerateChunk(this);
	}

	/// <summary> Use to generate chunk content if previous chunk is unknown. </summary>
	public void GenerateChunk() { GenerateChunk(null); }

	/// <summary> Use to generate chunk content if previous chunk is known. </summary>
	public void GenerateChunk(SplineChunk previousChunk)
	{
		// Use existense of PreviousChunk to check if content is already generated.
		if (PreviousChunk != null) { return; }

		// Set PreviousChunk and id.
		if (previousChunk == null) { ChunkId = 0; }
		else
		{
			PreviousChunk = previousChunk;
			ChunkId = PreviousChunk.ChunkId + 1;
		}

		// Add SplineContainer and SplineExtrude components.
		if (_spline == null)
		{ _spline = gameObject.AddComponent<SplineContainer>(); }

		if (_extrude == null)
		{
			// Setup SplineExtrude.
			_extrude = gameObject.AddComponent<CustomizedSplineExtrude>();
			_extrude.Radius = _splineRadius;
			_extrude.Sides = _splineEdges;
			_extrude.Container = _spline;
			_extrude.Capped = false;

			// SplineExtrude automatically adds MeshRenderer.
			MeshRenderer mesh = GetComponent<MeshRenderer>();
			mesh.SetMaterials(_splineMaterials);
		}

		// Keep chunk objects at the center of world but use knot positions to manage the spline positions.
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		ResetKnots();
	}

	/// <summary> Randomizes and sets new positions and rotations to knots. </summary>
	private void ResetKnots()
	{
		// Each spline should have 5 knots.
		_spline.Spline = new Spline(5, false);

		// Tangent lengths for evert knot.
		_tangentLength = RandomRadius() / 2f; // <--- makes cool wavy effect
											  // For 'correct' smooth increase, calculate vector2 angle between the heights and insert to upvector

		// Calculates the world space height of knots.
		float heightCounter = 0f;

		// First knot should match the last knot of previous spline.
		if (PreviousChunk != null)
		{
			_spline.Spline.Add(PreviousChunk.EndKnot);
			heightCounter = PreviousChunk.EndKnot.Position.y;
		}
		else
		{
			// Knot A
			_spline.Spline.Add(new BezierKnot(
				new float3(RandomRadius(), 0f, 0f),
				new float3(-_tangentLength, 0f, 0f),
				new float3(_tangentLength, 0f, 0f),
				quaternion.LookRotation(new float3(-1f, 0f, 0f), new float3(0f, 0.5f, -0.5f))), TangentMode.Continuous);
		}

		// Rest of the knots should form a deformed spiral shape.

		// Knot B
		heightCounter += RandomHeightIncrease();
		_spline.Spline.Add(new BezierKnot(
			new float3(0f, heightCounter, RandomRadius()),
			new float3(-_tangentLength, 0f, 0f),
			new float3(_tangentLength, 0f, 0f),
			quaternion.LookRotation(new float3(0f, 0f, -1f), new float3(0.5f, 0.5f, 0f))), TangentMode.Continuous);

		// Knot C
		heightCounter += RandomHeightIncrease();
		_spline.Spline.Add(new BezierKnot(
			new float3(-RandomRadius(), heightCounter, 0f),
			new float3(-_tangentLength, 0f, 0f),
			new float3(_tangentLength, 0f, 0f),
			quaternion.LookRotation(new float3(1f, 0f, 0f), new float3(0f, 0.5f, 0.5f))), TangentMode.Continuous);

		// Knot D
		heightCounter += RandomHeightIncrease();
		_spline.Spline.Add(new BezierKnot(
			new float3(0f, heightCounter, -RandomRadius()),
			new float3(-_tangentLength, 0f, 0f),
			new float3(_tangentLength, 0f, 0f),
			quaternion.LookRotation(new float3(0f, 0f, 1f), new float3(-0.5f, 0.5f, 0f))), TangentMode.Continuous);

		// Knot E
		heightCounter += RandomHeightIncrease();
		_spline.Spline.Add(new BezierKnot(
			new float3(RandomRadius(), heightCounter, 0f),
			new float3(-_tangentLength, 0f, 0f),
			new float3(_tangentLength, 0f, 0f),
			quaternion.LookRotation(new float3(-1f, 0f, 0f), new float3(0f, 0.5f, -0.5f))), TangentMode.Continuous);

		// Save last knot for next chunk to check how first knot should be set.
		EndKnot = _spline.Spline[_spline.Spline.Count - 1];

		// Generate the mesh.
		_extrude.Reset();
	}

	/// <summary> Randomizes height increse intended for knots. </summary>
	private float RandomHeightIncrease() { return UnityEngine.Random.Range(_knotHeightIncreaseRange.x, _knotHeightIncreaseRange.y); }

	/// <summary> Randomizes radius intended for knots. </summary>
	private float RandomRadius() { return UnityEngine.Random.Range(_knotRadiusRange.x, _knotRadiusRange.y); }

	/// <summary> Despawns the chunk and all objects on it. </summary>
	public void DestroyChunk()
	{
		// Despawn possible previous chunks too.
		if (PreviousChunk != null) { PreviousChunk.DestroyChunk(); }

		// Clear Registry list before nulling surrounding chunks so that the Objects on the spline can adjust themselves to the surrounding splines.
		Registry.ClearList();

		// Null surrounding splines so that the chunk can be reset properly when get from pool.
		PreviousChunk = null;
		NextChunk = null;
		ReturnToPool();
	}
}