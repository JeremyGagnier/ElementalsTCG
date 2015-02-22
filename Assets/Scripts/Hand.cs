using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour {
	public int player;
	private List<Card> cards;
	private Game gameMgr;
	[SerializeField]
	private Deck deck;
	private Card hovering = null;

	void Awake ()
	{
		cards = new List<Card>(GetComponentsInChildren<Card> ());
	}

	void Start () {

	}

	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (hovering != null)
		{
			Vector3 hoveredPos = new Vector3(mousePos.x, mousePos.y, hovering.transform.position.z);
			if (!hovering.GetComponent<BoxCollider2D> ().bounds.Contains(hoveredPos))
			{
				hovering.OnMouseExitHack ();
				hovering = null;
			}
			else
			{
				hovering.OnMouseOverHack ();
			}
		}
		else
		{
			for (int i = cards.Count - 1; i >= 0; --i)
			{
				Vector3 hoveredPos = new Vector3(mousePos.x, mousePos.y, cards[i].transform.position.z);
				if (cards[i].GetComponent<BoxCollider2D> ().bounds.Contains(hoveredPos))
				{
					cards[i].OnMouseEnterHack ();
					cards[i].OnMouseOverHack ();
					hovering = cards[i];
					break;
				}
			}
		}
	}

	public int CardIndex(Card c) {
		return cards.IndexOf (c);
	}

	public int CardCount() {
		return cards.Count;
	}

	public void CardEntered(Card c) {
		int index = cards.IndexOf (c);
		foreach (Card card in cards) {
			card.CardEntered (index);
		}
	}

	public void CardExited(Card c) {
		foreach (Card card in cards) {
			card.ResetCardPositionInHand ();
		}
	}

	public void SetPlayer(int p)
	{
		player = p;
		foreach (Card card in cards)
		{
			card.player = this.player;
		}
	}

	public void SetGameMgr (Game game)
	{
		gameMgr = game;
		foreach (Card card in cards)
		{
			card.gameMgr = this.gameMgr;
		}
	}
	
	public void DrawCard(Card c) {
		cards.Add (c);
		c.DeckToHandTransition(this);
		foreach (Card card in cards) {
			card.ResetCardPositionInHand ();
		}
	}

	public void SetHandVisible (bool visible)
	{
		foreach (Card card in cards) {
			card.SetVisible(visible);
		}
	}

	public void Play(Card c)
	{
		cards.Remove (c);
		foreach (Card card in cards)
		{
			card.ResetCardPositionInHand ();
		}
	}

	public Card GetCardAtIndex (int index)
	{
		return cards [index];
	}

	public void RandomDiscard ()
	{
		if(cards.Count > 0)
		{
			int index = Random.Range (0, cards.Count);
			cards[index].gameObject.SetActive (false);
			cards.RemoveAt (index);
			foreach (Card card in cards)
			{
				card.ResetCardPositionInHand ();
			}
		}
	}

	public void PickUp (Card c)
	{
		cards.Add (c);
		c.FieldToHandTransition(this);
		foreach (Card card in cards)
		{
			card.ResetCardPositionInHand ();
		}
	}
}
