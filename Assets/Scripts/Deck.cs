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
		cards = new List<Card>(GetComponentsInChildren<Card> ());

		for(int i = 0; i < cards.Count * 5; ++i)
		{
			int randomIndex1 = Random.Range (0, cards.Count);
			int randomIndex2 = Random.Range (0, cards.Count);

			Card tmp = cards[randomIndex1];
			cards[randomIndex1] = cards[randomIndex2];
			cards[randomIndex2] = tmp;
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
		player = p;
		foreach (Card card in cards)
		{
			card.SetPlayer(player);
		}
	}

	public void SetGameMgr (Game game)
	{
		gameMgr = game;
		foreach (Card card in cards)
		{
			card.SetGameMgr (gameMgr);
		}
	}

	public void DrawCard()
	{
		if (cards.Count > 0)
		{
			cards [0].gameObject.SetActive (true);
			hand.DrawCard (cards [0]);
			cards [0].SetHandVisible (gameMgr.turnPlayer () == cards[0].player);
			cards.RemoveAt (0);
		}
	}

	public Card GetCardAtIndex (int index)
	{
		return cards [index];
	}

	public void DestroyTopCard ()
	{
		cards.RemoveAt (0);
	}
}
