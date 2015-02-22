using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Modifier
{
    public Card target;
    public int manaModifier;
	public int attackModifier;
	public int maxHealthModifier;
	public int damageAmount;
	public int healAmount;
	public int duration;

    public Modifier(Card c, int mMod, int aMod, int hMod, int damage = 0, int duration = 0)
    {
        target = c;
        manaModifier = mMod;
		attackModifier = aMod;
		maxHealthModifier = hMod;
		damageAmount = damage;		// Negative damage is a heal.
		this.duration = duration;	// Zero duration means effect is perpetual.
	}

	public void Apply ()
    {
        target.currentMana += manaModifier;
		target.currentAttack += attackModifier;
		target.maxHealth += maxHealthModifier;
		target.currentHealth += maxHealthModifier;
        target.currentHealth -= damageAmount;

		if (target.currentHealth > target.maxHealth)
		{
			target.currentHealth = target.maxHealth;
		}
	}

	public void Remove ()
    {
        target.currentMana -= manaModifier;
		target.currentAttack -= attackModifier;
		target.maxHealth -= maxHealthModifier;
		target.currentHealth += damageAmount;
		if (target.currentHealth > target.maxHealth)
		{
			target.currentHealth = target.maxHealth;
		}
	}

	public void EndTurn ()
	{
		if (duration == 0)
		{
			return;
		}

		duration -= 1;
		if (duration == 0)
		{
			Remove ();
		}
	}
}
