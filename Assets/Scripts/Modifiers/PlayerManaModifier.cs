using UnityEngine;
using System.Collections;

public class PlayerManaModifier : Modifier
{
    public ManaBar playerTarget = null;

    public int amount = 1;

    private int overflow = 0;
    private int overdrawn = 0;

    public PlayerManaModifier(ManaBar target, int amount, int duration = 0)
    {
        this.target = null;
        this.playerTarget = target;
        this.amount = amount;
        this.duration = duration;
    }

    override public void Apply()
    {
        this.playerTarget.currentMana += this.amount;
        if (this.playerTarget.currentMana > 10)
        {
            this.overflow = playerTarget.currentMana - 10;
            this.playerTarget.currentMana = 10;
        }
        else if (this.playerTarget.currentMana < 0)
        {
            this.overdrawn = -playerTarget.currentMana;
        }
    }

    override public void Undo()
    {
        this.playerTarget.currentMana -= (amount - this.overflow + this.overdrawn);
    }
}
