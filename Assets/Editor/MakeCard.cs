using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MakeCard : EditorWindow
{
	private string cardName = "";
	private string mana = "";
    private CardType type = CardType.CREATURE;
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
        GameObject card = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Card"));
        Card component = card.GetComponent<Card>();
        component.name = cardName;
        component.mana = Convert.ToInt32(mana);
        component.basicType = type;
        component.attack = Convert.ToInt32(attack);
        component.health = Convert.ToInt32(health);
        component.effectString = effect;

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
        PrefabUtility.CreatePrefab("Assets/Resources/Prefabs/Cards/" + cardName + ".prefab", card);
        DestroyImmediate(card);
	}
	
	void OnGUI()
	{
		GUILayout.Space(4f);
		GUILayout.Label("Card Maker");
		GUILayout.Space(4f);

        cardName = EditorGUILayout.TextField("Name", cardName);
        mana = EditorGUILayout.TextField("Mana", mana);
        type = (CardType)EditorGUILayout.EnumPopup("Card Type", type);
		attack = EditorGUILayout.TextField("Attack", attack);
		health = EditorGUILayout.TextField("Health", health);
		
		
		if (GUILayout.Button ("Make Card"))
		{
			CreateCardObject();
		}
	}
}