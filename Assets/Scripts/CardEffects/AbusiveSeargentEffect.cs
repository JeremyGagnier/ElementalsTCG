using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbusiveSeargentEffect : Effect {

    public override void TriggerBattlecry(Game g, Card c, List<Target> targets)
    {
        if (targets.Count > 0)
        {
            targets[0].card.AddModifier(new AttackModifier(targets[0].card, 2, 1));
        }
    }
}
