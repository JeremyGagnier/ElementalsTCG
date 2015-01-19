using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KronosEffect : Effects
{
	public int numberOfDiscards = 2;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.Discard (c.player, numberOfDiscards);
		g.AddPlay (g.EnemyField (c.player).CardCount ());
	}
}
