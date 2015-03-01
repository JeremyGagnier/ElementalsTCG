using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZanderEffect : Effect
{
	public int damage = 2;

	public override void TriggerPlayed (Game g, Card parent, Card trigger)
	{
		if (parent != trigger && trigger.IsCreature ())
		{
			g.Damage (trigger, damage);
		}
	}
}
