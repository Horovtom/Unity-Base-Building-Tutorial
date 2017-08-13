using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DebuggingMenu))]
public class DebuggingMenuEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if (GUILayout.Button("Create Pathfinding Example.")) {
			((DebuggingMenu)target).CreatePathfindingExample();
		}

		if (GUILayout.Button("Show Pathfinding debugging info.")) {
			((DebuggingMenu)target).ShowPathfindingDebuggingInfo();
		}
	}
}
