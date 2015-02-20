using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;


public class MakeCard : EditorWindow
{
	private string scriptableObjectName = "New Effect";
	private string scriptableObjectClass = "Effects";
	private string name = "";
	private string mana = "";
	private string attack = "";
	private string health = "";
	private string effects = "";
	private string attributes = "";
	
	[MenuItem("Card Manager/Make Card")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MakeCard));
	}
	
	private void CreateCardObject()
	{
		GameObject card = new GameObject(name);
		card.AddComponent<Card>();
        Card component = card.GetComponent<Card>();
        component.name = name;
        component.mana = Convert.ToInt32(mana);
        component.attack = Convert.ToInt32(attack);
        component.health = Convert.ToInt32(health);
        component.effectScript = Resources.Load<Effects>("Assets/Resources/ScriptableObjects/" + name + ".asset");

		AssetDatabase.CreateAsset(card, "Assets/Resources/Prefabs/Cards/" + name + ".prefab");
	}
	
	void OnGUI()
	{
		GUILayout.Space(4f);
		GUILayout.Label("Card Maker");
		GUILayout.Space(4f);

        scriptableObjectName = EditorGUILayout.TextField("Name", name);
        scriptableObjectClass = EditorGUILayout.TextField("Mana", mana);
		scriptableObjectClass = EditorGUILayout.TextField("Attack", attack);
		scriptableObjectClass = EditorGUILayout.TextField("Health", health);
		
		
		if (GUILayout.Button ("Make Card"))
		{
			CreateCardObject();
		}
	}
}