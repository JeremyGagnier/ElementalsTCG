using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanicButtonEffect : Effects
{
	public int numberOfDraws = 1;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.DrawCard (c.player, numberOfDraws);
		if (g.GetField (c.player).CardCount() == 0)
		{
			g.DrawCard (c.player, 2);
		}
	}
}
