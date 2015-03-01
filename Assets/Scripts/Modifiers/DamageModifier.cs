using UnityEngine;
using System.Collections;

public class DamageModifier : Modifier {
    public int amount = 1;

    private int overkill = 0;

    public DamageModifier(Card target, int amount, int duration = 0)
    {
        this.target = target;
        this.amount = amount;
        this.duration = duration;
    }

    override public void Apply()
    {
        if (amount < 0)
        {
            Debug.LogError("Use HealModifier for healing effects. Damage amount was set to: " + amount.ToString());
        }
        target.currentHealth -= amount;
        if (target.currentHealth < 0)
        {
            overkill = -target.currentHealth;
        }
    }

    override public void Undo()
    {
        target.currentHealth += amount - overkill;
    }

    override public void Invert()
    {
        target.currentHealth += amount;
    }
}
