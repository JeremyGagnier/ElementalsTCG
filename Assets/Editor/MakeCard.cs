using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MakeCard : EditorWindow
{
	private string cardName = "";
	private string mana = "";
	private string attack = "";
	private string health = "";
	private string effect = "";
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
        component.name = cardName;
        component.mana = Convert.ToInt32(mana);
        component.attack = Convert.ToInt32(attack);
        component.health = Convert.ToInt32(health);
        component.effectString = effect;
        try
        {
            // This might always fail because "Effects" is the name of the script on the prefab.
            component.effectScript = Resources.Load<Effects>("Assets/Resources/ScriptableObjects/" + name + ".asset");
        }
        catch
        {
            component.effectScript = Resources.Load<Effects>("Assets/Resources/ScriptableObjects/Null.asset");
        }
        
        foreach (String attribute in attributes.Split(' '))
        {
            switch (attribute.ToLower())
            {
                case "murloc":
                    component.attributes.Add(Attribute.MURLOC);
                    break;
                case "pirate":
                    component.attributes.Add(Attribute.PIRATE);
                    break;
                case "demon":
                    component.attributes.Add(Attribute.DEMON);
                    break;
                case "mech":
                    component.attributes.Add(Attribute.MECH);
                    break;
            }
        }

		AssetDatabase.CreateAsset(card, "Assets/Resources/Prefabs/Cards/" + name + ".prefab");
	}
	
	void OnGUI()
	{
		GUILayout.Space(4f);
		GUILayout.Label("Card Maker");
		GUILayout.Space(4f);

        cardName = EditorGUILayout.TextField("Name", cardName);
        mana = EditorGUILayout.TextField("Mana", mana);
		attack = EditorGUILayout.TextField("Attack", attack);
		health = EditorGUILayout.TextField("Health", health);
		
		
		if (GUILayout.Button ("Make Card"))
		{
			CreateCardObject();
		}
	}
}