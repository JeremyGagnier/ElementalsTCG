using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawEffect : Effects
{
	public int numberOfDraws = 1;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.DrawCard (c.player, numberOfDraws);
	}
}
