using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockbiterWeaponEffect : Effect
{
    public override void TriggerBattlecry(Game g, Card c, List<Target> targets)
    {
        targets[0].card.AddModifier(new AttackModifier(targets[0].card, 3, 1));
    }
}
