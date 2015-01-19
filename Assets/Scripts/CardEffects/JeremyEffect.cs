using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JeremyEffect : Effects
{
	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		int enemyCardCount = g.EnemyField (c.player).CardCount ();
		for(int i = 0; i < enemyCardCount; ++i)
		{
			g.PickUp (g.EnemyField (c.player).GetCards ()[0]);
		}
		int allyCardCount = g.GetField (c.player).CardCount ();
		for(int i = 0; i < allyCardCount; ++i)
		{
			if (g.GetField (c.player).GetCards ()[0] != c)
			{
				g.PickUp (g.GetField (c.player).GetCards ()[0]);
			}
		}
	}
}
