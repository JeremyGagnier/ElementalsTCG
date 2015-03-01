using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssassinateEffect : Effect
{
	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		if (targets[0].isCard)
		{
			targets[0].card.currentHealth = 0;
		}
	}
}
