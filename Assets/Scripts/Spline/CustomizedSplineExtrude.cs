using System.Collections.Generic;

// THIS IS CUSTOMIZED VERSION OF BUILT IN SPLINEEXTRUDE TO ALLOW REAL TIME MESH GENERATING
// Unnecessary stuff removed to increase performance. Compare to original SplineExtrude if needed.

namespace UnityEngine.Splines
{
	/// <summary>
	/// A component for creating a tube mesh from a Spline at runtime.
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	[AddComponentMenu("Splines/Customized Spline Extrude")]
	public class CustomizedSplineExtrude : MonoBehaviour
	{
		[SerializeField, Tooltip("The Spline to extrude.")]
		SplineContainer m_Container;

		[SerializeField, Tooltip("The number of sides that comprise the radius of the mesh.")]
		int m_Sides = 8;

		[SerializeField, Tooltip("The number of edge loops that comprise the length of one unit of the mesh. The " +
			 "total number of sections is equal to \"Spline.GetLength() * segmentsPerUnit\".")]
		float m_SegmentsPerUnit = 4;

		[SerializeField, Tooltip("Indicates if the start and end of the mesh are filled. When the Spline is closed this setting is ignored.")]
		bool m_Capped = true;

		[SerializeField, Tooltip("The radius of the extruded mesh.")]
		float m_Radius = .25f;

		[SerializeField, Tooltip("The section of the Spline to extrude.")]
		Vector2 m_Range = new Vector2(0f, 1f);

		Mesh m_Mesh;

		/// <summary>The SplineContainer of the <see cref="Spline"/> to extrude.</summary>
		public SplineContainer Container { get => m_Container; set => m_Container = value; }

		/// <summary>How many sides make up the radius of the mesh.</summary>
		public int Sides { get => m_Sides; set => m_Sides = Mathf.Max(value, 3); }

		/// <summary>How many edge loops comprise the one unit length of the mesh.</summary>
		public float SegmentsPerUnit { get => m_SegmentsPerUnit; set => m_SegmentsPerUnit = Mathf.Max(value, .0001f); }

		/// <summary>Whether the start and end of the mesh is filled. This setting is ignored when spline is closed.</summary>
		public bool Capped { get => m_Capped; set => m_Capped = value; }

		/// <summary>The radius of the extruded mesh.</summary>
		public float Radius { get => m_Radius; set => m_Radius = Mathf.Max(value, .00001f); }

		/// <summary> The section of the Spline to extrude. </summary>
		public Vector2 Range { get => m_Range; set => m_Range = new Vector2(Mathf.Min(value.x, value.y), Mathf.Max(value.x, value.y)); }

		/// <summary>The main Spline to extrude.</summary>
		public Spline Spline { get => m_Container.Spline; }

		/// <summary>The Splines to extrude.</summary>
		public IReadOnlyList<Spline> Splines { get => m_Container.Splines; }


		/// <summary> Use to reset and rebuild spline mesh. </summary>
		public void Reset()
		{
			TryGetComponent(out m_Container);

			if (TryGetComponent<MeshFilter>(out var filter))
				filter.sharedMesh = m_Mesh = CreateMeshAsset();

			if (TryGetComponent<MeshRenderer>(out var renderer) && renderer.sharedMaterial == null)
			{
				var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				var mat = cube.GetComponent<MeshRenderer>().sharedMaterial;
				DestroyImmediate(cube);
				renderer.sharedMaterial = mat;
			}

			Rebuild();
		}

		private void Rebuild()
		{
			if ((m_Mesh = GetComponent<MeshFilter>().sharedMesh) == null) { return; }

			SplineMesh.Extrude(Splines, m_Mesh, m_Radius, m_Sides, m_SegmentsPerUnit, m_Capped, m_Range);
		}

		internal Mesh CreateMeshAsset()
		{
			var mesh = new Mesh();
			mesh.name = name;

			return mesh;
		}
	}
}
