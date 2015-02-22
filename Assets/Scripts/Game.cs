﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	[SerializeField]
	private int numPlayers = 2;
	[SerializeField]
	private int startingHealth = 20;

	[SerializeField]
	private List<Hand> hands;
	[SerializeField]
	private List<Deck> decks;
    [SerializeField]
    private List<Field> fields;
    [SerializeField]
    private List<Hero> heroes;

	private int turn;
	private int turnCount;
    private List<int> mana = new List<int>();

	private List<Card> orderOfPlay;
	
	private int queueDraws;
	private int whoDrawsNext;
	private float timeSinceDraw;

	private List<GUIText> healthText;

	private int targetsLeft;
	private List<Target> targetsFound;
	private Card targeterCard;

    private int? hovered = null;

	void Awake ()
	{
		for (int i = 0; i < numPlayers; ++i)
		{
            mana.Add(1);
		}

		orderOfPlay = new List<Card> ();

		queueDraws = 7;
		whoDrawsNext = 1;

		turn = 0;
		turnCount = -1;
		targetsLeft = 0;
		targeterCard = null;
		targetsFound = new List<Target> ();
	}

	void Start ()
	{
		for (int i = 0; i < numPlayers; ++i)
		{
			hands[i].SetPlayer(i);
			decks[i].SetPlayer(i);
			fields[i].SetPlayer(i);
            heroes[i].SetPlayer(i);
			hands[i].SetGameMgr(this);
            decks[i].SetGameMgr(this);
            fields[i].SetGameMgr(this);
            heroes[i].SetGameMgr(this);
		}
		
		StartTurn ();
	}

	void Update ()
	{
		// Queue draws handles all draw effects to make sure cards are not drawn too quickly
		if (queueDraws > 0)
		{
			timeSinceDraw += Time.deltaTime;
			if (timeSinceDraw >= 0.125f)
			{
				queueDraws -= 1;
				decks [whoDrawsNext].DrawCard ();
				if (turnCount == -1)
				{
					whoDrawsNext += 1;
					if (whoDrawsNext == numPlayers)
					{
							whoDrawsNext = 0;
					}
				}
				timeSinceDraw -= 0.125f;
			}
		}
		else
		{
			timeSinceDraw = 0;
			if (turnCount == -1)
			{
				turnCount = 0;
			}
		}
	}

	void OnGUI ()
	{
		GUI.skin.label.fontSize = 40;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		GUI.Label (new Rect (20f,Camera.main.pixelHeight/2f, 300f, 100f), "Mana: " + mana[0].ToString ());

		if (GUI.Button (new Rect(Camera.main.pixelWidth - 200f, Camera.main.pixelHeight / 2f - 50f, 200f, 100f), "End Turn"))
		{
			EndTurn ();
		}
	}

    public void HeroHovered(int player)
    {
        if (hovered != null)
        {
            Debug.LogWarning("A hero was already being hovered!");
        }
        hovered = player;
    }

    public void HeroUnhovered(int player)
    {
        if ((int)hovered != player)
        {
            Debug.LogWarning("Player" + (player + 1).ToString() + "'s hero was unhovered but it isn't the hovered hero!");
        }
        hovered = null;
    }

    public Target GetHoverTarget()
    {
        if (hovered != null)
        {
            return new Target(false, null, (int)hovered);
        }
        return null;
    }

	public void Play(Card c, bool spendMana = true)
	{
        if(spendMana)
        {
            SpendMana(c.player, c.currentMana);
        }
        this.hands[c.player].Play(c);
        c.HandToFieldTransition(this.fields[c.player]);
		if (c.IsCreature())
		{
            this.fields[c.player].Play(c);
		}
		c.TriggerBattlecry (targetsFound);
		foreach (Card card in orderOfPlay)
		{
			card.TriggerPlayed(c);
		}
		if (c.IsCreature ())
		{
			this.orderOfPlay.Add (c);
		}
	}

    public void SpendMana(int player, int count)
    {
        this.mana[player] -= count;
        if (this.mana[player] < 0)
        {
            Debug.LogWarning("Player " + (player + 1).ToString() + " has spent " + (-this.mana[player]).ToString() + " mana more than they had.");
            this.mana[player] = 0;
        }
    }

    public void GiveMana(int player, int count)
    {
        this.mana[player] += count;
        if (this.mana[player] > 10)
        {
            this.mana[player] = 10;
        }
    }

    public int GetMana(int player)
    {
        return this.mana[player];
    }
	
	public void Destroy(Card c)
	{
		orderOfPlay.Remove (c);
		fields [c.player].Destroy (c);
	}

	public void DrawCard (int player, int count=1) {
		whoDrawsNext = player;
		queueDraws += count;
	}

	public int turnPlayer ()
	{
		return turn;
	}

	public void EndTurn () {
		foreach (Card card in orderOfPlay)
		{
			card.TriggerEndTurn ();
		}
		turn += 1;
		turnCount += 1;
		if (turn == numPlayers) {
			turn = 0;
		}
		StartTurn ();
	}

	public void StartTurn () {
		hands [0].SetHandVisible (turn == 0);
		hands [1].SetHandVisible (turn == 1);
		foreach (Card card in orderOfPlay)
		{
			card.TriggerStartTurn ();
		}
        this.mana[this.turn] = Mathf.Min(10, 1 + turnCount/2);
		queueDraws += 1;
		whoDrawsNext = turn;
	}

	public void Damage (Card c, int damage)
	{
		c.AddModifier (new Modifier (c, 0, 0, 0, damage));
	}

	public void Damage (int p, int damage)
	{
        heroes[p].OnDamage(damage);
	}

	public Card GetCardAtIndex (int index)
	{
		return orderOfPlay [index];
	}

	public int CardCount ()
	{
		return orderOfPlay.Count;
	}

	public void GetTargets (Card c, int count)
	{
		targetsLeft = count;
		targeterCard = c;
		targetsFound.Clear ();
	}

	public void StopGettingTargets ()
	{
		targetsLeft = 0;
		targeterCard = null;
		targetsFound.Clear ();
	}

	public void CardClicked (Card c)
	{
		if (targetsLeft > 0)
		{
			Target t = new Target(true, c, c.player);
			if (targeterCard.IsTargetValid (t))
			{
				targetsFound.Add (t);
				targetsLeft -= 1;
			}
			if (targetsLeft == 0)
			{
				Play (targeterCard);
				targeterCard = null;
			}
		}
	}

	public void Combat (Card attacker, Card defender)
	{
		attacker.TriggerCombat (true, defender);
		defender.TriggerCombat (false, attacker);
	}

	public void Combat (Card attacker, int player)
	{
		attacker.TriggerDirectAttack (player);
	}

	public Field EnemyField (int player)
	{
		return fields[(player == 0) ? 1 : 0];
	}

	public Card GetTopCard(int player)
	{
		return decks [player].GetCardAtIndex(0);
	}

	public void DestroyTopCard(int player)
	{
		decks [player].DestroyTopCard ();
	}

	public Field GetField (int player)
	{
		return fields[player];
	}

	public void Discard(int player, int number)
	{
		for(int i = 0; i < number; ++i)
		{
			hands[player].RandomDiscard();
		}
	}

	public void PickUp(Card c)
	{
		hands [c.player].PickUp (c);
      	orderOfPlay.Remove (c);
      	fields [c.player].Destroy (c);
	}
}






