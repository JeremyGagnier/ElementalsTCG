using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawEffect : Effect
{
	public int numberOfDraws = 1;

	override public void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.DrawCard (c.player, numberOfDraws);
	}
}
