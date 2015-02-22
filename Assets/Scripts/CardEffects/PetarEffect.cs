using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PetarEffect : Effects
{
	public override void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
		targets [0].card.player = c.player;
		targets [0].card.field = g.GetField(c.player);
		targets [0].card.Exhausted = true;
		targets [0].card.rigidbody2D.gravityScale = -targets [0].card.rigidbody2D.gravityScale;
		g.EnemyField (c.player).Destroy (targets [0].card);
		g.GetField (c.player).Play (targets [0].card);
	}
}
