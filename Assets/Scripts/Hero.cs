using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth = 30;
    [HideInInspector]
    public int maxHealth = 30;


    private GameObject healthSprite;
    private TextMesh healthText;

    void Awake()
    {
    }

    void Start()
    {
        healthSprite = this.transform.FindChild("HealthSprite").gameObject;
        healthText = healthSprite.transform.FindChild("Health").GetComponent<TextMesh>();
    }

    void Update()
    {
    }
}











