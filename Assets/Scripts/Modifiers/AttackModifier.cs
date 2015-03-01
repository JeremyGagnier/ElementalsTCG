using UnityEngine;
using System.Collections;

public class AttackModifier : Modifier {
    public int amount = 1;

    public AttackModifier(Card target, int amount, int duration = 0)
    {
        this.target = target;
        this.amount = amount;
        this.duration = duration;
    }

    override public void Apply()
    {
        target.currentAttack += amount;
    }

    override public void Undo()
    {
        target.currentAttack -= amount;
    }

    override public void Invert()
    {
        target.currentAttack -= amount;
    }
}
