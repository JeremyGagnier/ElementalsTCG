using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
	public int player;
	private List<Card> cards;
	private Game gameMgr;
	[SerializeField]
	private Hand hand;

	void Awake ()
	{
		this.cards = new List<Card>(GetComponentsInChildren<Card> ());

		for(int i = 0; i < this.cards.Count * 5; ++i)
		{
			int randomIndex1 = Random.Range (0, this.cards.Count);
			int randomIndex2 = Random.Range (0, this.cards.Count);

			Card tmp = this.cards[randomIndex1];
			this.cards[randomIndex1] = this.cards[randomIndex2];
			this.cards[randomIndex2] = tmp;
		}
	}

	void Start ()
	{
	}

	void Update ()
	{
	}

	public void SetPlayer(int p)
	{
		this.player = p;
		foreach (Card card in this.cards)
		{
			card.player = this.player;
		}
	}

	public void SetGameMgr (Game game)
	{
		this.gameMgr = game;
		foreach (Card card in cards)
		{
			card.gameMgr = this.gameMgr;
		}
	}

	public void DrawCard()
	{
		if(cards.Count > 0)
		{
            this.cards[0].gameObject.SetActive(true);
            this.hand.DrawCard(this.cards[0]);
            this.cards[0].SetVisible(this.gameMgr.turnPlayer() == this.cards[0].player);
			this.cards.RemoveAt(0);
		}
	}

	public Card GetCardAtIndex(int index)
	{
		return cards[index];
	}

	public void DestroyTopCard ()
	{
		cards.RemoveAt (0);
	}
}
