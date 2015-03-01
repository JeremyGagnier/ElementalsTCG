using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameTosserEffect : Effect
{
	public int damage = 2;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.Damage (targets[0].card, damage);
	}
}
