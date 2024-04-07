using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
	/// <summary> Sets GameObject's layer without unnecessary warning messages. </summary>
	public static void OnValidateSetLayer_EditorOnly(this GameObject gameObject, int layer)
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.delayCall += () =>
		{ if (gameObject != null) { gameObject.layer = layer; } };
#endif
	}
}