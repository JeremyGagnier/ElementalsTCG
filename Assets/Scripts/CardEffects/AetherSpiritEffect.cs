using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AetherSpiritEffect : Effects
{
	public int healAmount = 8;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.Damage (c.player, -healAmount);
	}
}
