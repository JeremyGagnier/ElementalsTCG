using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Field : MonoBehaviour
{
	public int player;
	private List<Card> cards;
	private Game gameMgr;

	private Card hovered;

	void Awake ()
	{
		cards = new List<Card>(GetComponentsInChildren<Card> ());
	}

	void Update ()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		if (hovered != null)
		{
			Vector3 hoveredPos = new Vector3(mousePos.x, mousePos.y, hovered.transform.position.z);
			if (!hovered.GetComponent<BoxCollider2D> ().bounds.Contains(hoveredPos))
			{
				hovered.OnMouseExitHack ();
				hovered = null;
			}
			else
			{
				hovered.OnMouseOverHack ();
			}
		}
		else
		{
			for (int i = cards.Count - 1; i >= 0; --i)
			{
				Vector3 hoveredPos = new Vector3(mousePos.x, mousePos.y, cards[i].transform.position.z);
				if (cards[i].GetComponent<BoxCollider2D> ().bounds.Contains(hoveredPos))
				{
					hovered = cards[i];
					cards[i].OnMouseEnterHack ();
					break;
				}
			}
		}
	}
	
	public int CardIndex(Card c)
	{
        return this.cards.IndexOf(c);
	}
	
	public int CardCount()
	{
        return this.cards.Count;
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
        this.gameMgr = game;
		foreach (Card card in cards)
		{
            card.gameMgr = this.gameMgr;
		}
	}

	public void Play(Card c)
	{
        this.cards.Add(c);
        foreach (Card card in this.cards)
		{
			card.ResetCardPositionOnField ();
		}
	}

	public void Destroy(Card c)
	{
        this.cards.Remove(c);
        foreach (Card card in this.cards)
		{
			card.ResetCardPositionOnField ();
		}
	}

	public void CardEntered (Card c)
	{
        this.hovered = c;
	}

	public void CardExited (Card c)
	{
        this.hovered = null;
	}

	public Target GetHoverTarget ()
	{
		if (hovered != null)
		{
			return new Target(true, hovered, 0);
		}
		else if (Input.mousePosition.x < 300 && Input.mousePosition.x > 50 && Input.mousePosition.y > Camera.main.pixelHeight - 150 && Input.mousePosition.y < Camera.main.pixelHeight - 50)
		{
			return new Target(false, null, 1);
		}
		else if (Input.mousePosition.x < 300 && Input.mousePosition.x > 50 && Input.mousePosition.y < 150 && Input.mousePosition.y > 50)
		{
			return new Target(false, null, 0);
		}
		else
		{
			return null;
		}
	}

	public List<Card> GetCards ()
	{
		return cards;
	}

	public void ResetCardPosition ()
	{
		foreach(Card card in cards)
		{
			card.ResetCardPositionOnField ();
		}
	}
}

















