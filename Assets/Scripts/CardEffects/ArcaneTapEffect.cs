using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcaneTapEffect : Effects
{
	public int numberOfPlays = 1;
	public int numberOfDraws = 1;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.AddPlay (numberOfPlays);
		g.DrawCard (c.player, numberOfDraws);
		if (g.CardCount () > 0)
		{
			g.Damage (g.GetCardAtIndex (Random.Range (0, g.CardCount ())), 1);
		}
	}
}
