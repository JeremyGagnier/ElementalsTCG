using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiscardEffect : Effects
{
	public int numberOfDiscards = 1;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.Discard (c.player, numberOfDiscards);
	}
}
