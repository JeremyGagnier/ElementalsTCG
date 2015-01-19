using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;


public class CreateScriptableObjects : EditorWindow {
	
	private string scriptableObjectName = "New Effect";
	private string scriptableObjectClass = "Effects";
	
	[MenuItem("ScriptableObjects/Create Scriptable Object")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(CreateScriptableObjects));
	}
	
	private void CreateScriptableObject() {
		UnityEngine.Object instance = ScriptableObject.CreateInstance(scriptableObjectClass);
		AssetDatabase.CreateAsset(instance, "Assets/Resources/ScriptableObjects/"+scriptableObjectName+".asset");
	}
	
	void OnGUI() {
		GUILayout.Space(4f);
		GUILayout.Label("Create Scritable Object");
		GUILayout.Space(4f);
		
		scriptableObjectName = EditorGUILayout.TextField("Name", scriptableObjectName);
		scriptableObjectClass = EditorGUILayout.TextField("Class", scriptableObjectClass);
		
		
		if (GUILayout.Button ("Create Scriptable Object by Class Name")) {
			CreateScriptableObject();
		}
	}
}