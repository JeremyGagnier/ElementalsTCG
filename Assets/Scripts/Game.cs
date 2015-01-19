using UnityEngine;
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
	private int[] health;

	private int turn;
	private int turnCount;
	private int playsLeft;

	private List<Card> orderOfPlay;
	
	private int queueDraws;
	private int whoDrawsNext;
	private float timeSinceDraw;

	private List<GUIText> healthText;

	private int targetsLeft;
	private List<Target> targetsFound;
	private Card targeterCard;

	void Awake ()
	{
		health = new int[numPlayers];
		for (int i = 0; i < numPlayers; ++i)
		{
			health[i] = startingHealth;
		}

		orderOfPlay = new List<Card> ();

		queueDraws = 7;
		whoDrawsNext = 1;

		turn = 0;
		turnCount = -1;
		playsLeft = 1;
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
			hands[i].SetGameMgr(this);
			decks[i].SetGameMgr(this);
			fields[i].SetGameMgr(this);
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

		if (GUI.Button (new Rect (50f,Camera.main.pixelHeight - 150f, 250f, 100f), "Health: " + health [0].ToString ()))
		{
			if (targetsLeft > 0)
			{
				Target t = new Target(false, null, 0);
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

		if (GUI.Button (new Rect (50f,50f,250f,100f), "Health: " + health [1].ToString ()))
		{
			if (targetsLeft > 0)
			{
				Target t = new Target(false, null, 1);
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

		GUI.Label (new Rect (20f,Camera.main.pixelHeight/2f, 300f, 100f), "Plays: " + playsLeft.ToString ());

		if (GUI.Button (new Rect(Camera.main.pixelWidth - 200f, Camera.main.pixelHeight / 2f - 50f, 200f, 100f), "End Turn"))
		{
			EndTurn ();
		}
	}

	public void Play(Card c)
	{
		playsLeft -= 1;
		int player = c.player;
		hands [player].Play (c);
		c.Play (fields [player]);
		if (c.IsCreature())
		{
			fields [player].Play (c);
		}
		c.TriggerBattlecry (targetsFound);
		foreach (Card card in orderOfPlay)
		{
			card.TriggerPlayed(c);
		}
		if (c.IsCreature ())
		{
			orderOfPlay.Add (c);
		}
	}

	public void AddPlay (int count)
	{
		playsLeft += count;
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

	public bool hasMorePlays ()
	{
		return (playsLeft > 0);
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
		playsLeft = 1;
		queueDraws += 1;
		whoDrawsNext = turn;
	}

	public void Damage (Card c, int damage)
	{
		c.AddModifier (new Modifier (c, 0, 0, 0, damage));
	}

	public void Damage (int p, int damage)
	{
		health [p] -= damage;
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






