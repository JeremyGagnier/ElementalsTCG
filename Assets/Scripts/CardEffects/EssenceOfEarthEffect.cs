using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EssenceOfEarthEffect : Effects
{
	public int numberOfPlays = 1;

	public override void TriggerPlayed(Game g, Card parent, Card trigger)
	{
		if (trigger.IsCreature () && trigger.player == parent.player)
		{
				parent.AddModifier (new Modifier (parent, 1, 0, 1));
		}
	}

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.AddPlay (numberOfPlays);
	}
}
