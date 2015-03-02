using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	[SerializeField]
	private int numPlayers = 2;

	[SerializeField]
	private List<Hand> hands;
	[SerializeField]
	private List<Deck> decks;
    [SerializeField]
    private List<Field> fields;
    [SerializeField]
    private List<Hero> heroes;
    [SerializeField]
    private List<ManaBar> mana;

    private List<Modifier> gameModifiers = new List<Modifier>();

	private int turn;
	private int turnCount;

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
        this.mana[player].currentMana -= count;
    }

    public void GiveMana(int player, int count)
    {
        this.mana[player].currentMana += count;
    }

    public int GetMana(int player)
    {
        return this.mana[player].currentMana;
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
		foreach (Card card in this.orderOfPlay)
		{
			card.TriggerEndTurn();
		}
        foreach (Modifier mod in this.gameModifiers)
        {
            mod.EndTurn();
        }
		this.turn += 1;
		this.turnCount += 1;
		if (this.turn == this.numPlayers) {
			this.turn = 0;
		}
		this.StartTurn();
	}

	public void StartTurn () {
		this.hands [0].SetHandVisible (turn == 0);
		this.hands [1].SetHandVisible (turn == 1);
		foreach (Card card in this.orderOfPlay)
		{
			card.TriggerStartTurn ();
        }
        this.mana[this.turn].maxMana = Mathf.Min(10, 1 + turnCount / 2);
        this.mana[this.turn].currentMana = this.mana[this.turn].maxMana;
		this.queueDraws += 1;
		this.whoDrawsNext = this.turn;
	}

	public void Damage (Card c, int damage)
	{
		c.AddModifier (new DamageModifier (c, damage));
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

    public bool TargetExists(Card c, TargetInfo t)
    {
        if (t.players)
        {
            return true;
        }
        if (fields[c.player].CardCount() != 0 && t.ally)
        {
            foreach (Card allyCard in fields[c.player].GetCards())
            {
                if (c.IsTargetValid(new Target(true, allyCard, 0)))
                {
                    return true;
                }
            }
        }
        if (fields[(c.player == 0) ? 1 : 0].CardCount() != 0 && t.enemy)
        {
            foreach (Card enemyCard in fields[(c.player == 0) ? 1 : 0].GetCards())
            {
                if (c.IsTargetValid(new Target(true, enemyCard, 0)))
                {
                    return true;
                }
            }
        }
        return false;
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

    public void ModifyMana(int player, int amount, int duration = 0)
    {
        PlayerManaModifier manaMod = new PlayerManaModifier(this.mana[player], amount, duration);
        manaMod.Apply();
        this.gameModifiers.Add(manaMod);
    }
}






