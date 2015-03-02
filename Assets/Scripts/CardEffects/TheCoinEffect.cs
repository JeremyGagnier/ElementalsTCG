using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheCoinEffect : Effect
{
    public override void TriggerBattlecry(Game g, Card c, List<Target> targets)
    {
        g.ModifyMana(c.player, 1);
    }
}
