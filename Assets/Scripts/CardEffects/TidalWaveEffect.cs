using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TidalWaveEffect : Effects
{
	public int damage = 2;

	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		foreach(Card enemy in g.EnemyField (c.player).GetCards ())
		{
			g.Damage (enemy, damage);
		}
		g.Damage (g.EnemyField (c.player).player, damage);
	}
}
