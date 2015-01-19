using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArchangelEffect : Effects
{
	public int numberOfDiscards = 2;
	public int damageAmount = 1;
	public int healAmount = 1;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		g.Discard (c.player, numberOfDiscards);
	}

	public override void TriggerEndTurn (Game g, Card c)
	{
		foreach(Card enemy in g.EnemyField (c.player).GetCards ())
		{
			g.Damage (enemy, damageAmount);
		}
		foreach(Card enemy in g.GetField (c.player).GetCards ())
		{
			g.Damage (enemy, -healAmount);
		}
		g.Damage (g.EnemyField (c.player).player, damageAmount);
		g.Damage (c.player, -healAmount);
	}
}
