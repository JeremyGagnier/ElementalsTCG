using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireballEffect : Effect
{
	public int damage = 5;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		if (targets[0].isCard)
		{
			g.Damage (targets[0].card, damage);
		}
		else
		{
			g.Damage (targets[0].player, damage);
		}
	}
}
