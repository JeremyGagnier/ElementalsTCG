using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour
{
    public int player;
    public Game gameMgr;

    [HideInInspector]
    public int currentHealth;
    [HideInInspector]
    public int maxHealth;


    private GameObject healthSprite;
    private TextMesh healthText;

    void Awake()
    {
        maxHealth = 30;
        currentHealth = 30;
    }

    void Start()
    {
        healthSprite = this.transform.FindChild("HealthSprite").gameObject;
        healthText = healthSprite.transform.FindChild("Health").GetComponent<TextMesh>();
        this.healthText.text = this.currentHealth.ToString();
    }

    void Update()
    {
    }

    public void SetPlayer(int p)
    {
        this.player = p;
    }

    public void SetGameMgr(Game game)
    {
        this.gameMgr = game;
    }

    public void OnMouseEnter()
    {
        this.gameMgr.HeroHovered(this.player);
    }

    public void OnMouseExit()
    {
        this.gameMgr.HeroUnhovered(this.player);
    }

    public void OnDamage(int damage)
    {
        this.currentHealth -= damage;
        this.healthText.text = this.currentHealth.ToString();
        if (this.currentHealth <= 0)
        {
            this.OnDeath();
        }
    }

    public void OnDeath()
    {
    }
}











