using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;


[CustomPropertyDrawer(typeof(RandomObjectPicker))]
public class RandomObjectPickerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Convert property to RandomObjectPicker to access values.
		RandomObjectPicker picker = (RandomObjectPicker)GetPropertyInstance(property);

		// Base settings of property.
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Loop through all pickables in the list and create line for each.
		for (int i = 0; i < picker.Pickables.Count; i++)
		{
			// Add object slot and chances to a same line.
			GUILayout.BeginHorizontal();
			EditorGUI.indentLevel = 1;

			// Draw and check pickable.
			picker.Pickables[i].Pickable = (Poolable)EditorGUILayout.ObjectField(picker.Pickables[i].Pickable, typeof(Poolable));

			// Check if chances have changed.
			EditorGUI.BeginChangeCheck();

			// Draw and check chances.
			EditorGUILayout.LabelField("Odds:", GUILayout.MaxWidth(50));
			picker.Pickables[i].Chances = EditorGUILayout.IntField(picker.Pickables[i].Chances, GUILayout.MaxWidth(75));

			// Recalculate total odds if chances have changed.
			if (EditorGUI.EndChangeCheck())
			{ picker.RecalculateTotalOdds(); }

			// Set next pickable to a new line.
			GUILayout.EndHorizontal();
		}

		// Add buttons and odds to the same line.
		GUILayout.BeginHorizontal();
		GUILayout.Space((Screen.width / 2) - 120);

		// Add buttons to add or remove objects from the pickables list.
		if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) { picker.AddPickable(new RandomObjectPicker.PickableStats<Poolable>(null, 0)); }
		if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { picker.RemovePickable(picker.Pickables.Count - 1); }
		GUILayout.Space((Screen.width / 2) - 100);

		// Add total odds.
		EditorGUILayout.LabelField($"Total odds: {picker.TotalOdds}", GUILayout.MaxWidth(150));
		GUILayout.EndHorizontal();

		// End and save property.
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
		property.serializedObject.ApplyModifiedProperties();

		return;
	}


	/// <summary> Gets the given property as object. Code get from online. </summary>
	public System.Object GetPropertyInstance(SerializedProperty property)
	{
		string path = property.propertyPath;

		System.Object obj = property.serializedObject.targetObject;
		var type = obj.GetType();

		var fieldNames = path.Split('.');
		for (int i = 0; i < fieldNames.Length; i++)
		{
			var info = type.GetField(fieldNames[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (info == null)
				break;

			// Recurse down to the next nested object.
			obj = info.GetValue(obj);
			type = info.FieldType;
		}

		return obj;
	}
}