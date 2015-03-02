using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TidalWaveEffect : Effect
{
	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		foreach(Card enemy in g.EnemyField (c.player).GetCards ())
		{
			g.Damage (enemy, Random.Range(2,3));
		}
	}
}
