using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManaAcceleratorEffect : Effects
{
	public int numberOfPlays = 2;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.AddPlay (numberOfPlays);
		Card topCard = g.GetTopCard (c.player);
		if (topCard.IsCreature ())
		{
			g.DestroyTopCard (c.player);
		}
		g.DrawCard (c.player);
	}
}
