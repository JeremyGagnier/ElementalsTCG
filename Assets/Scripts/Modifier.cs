using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Modifier
{
	public Card target;
	public int attackModifier;
	public int retaliationModifier;
	public int maxHealthModifier;
	public int damageAmount;
	public int healAmount;
	public int duration;

	public Modifier(Card c, int aMod, int rMod, int hMod, int damage=0, int duration=0)
	{
		target = c;
		attackModifier = aMod;
		retaliationModifier = rMod;
		maxHealthModifier = hMod;
		damageAmount = damage;		// Negative damage is a heal.
		this.duration = duration;	// Zero duration means effect is perpetual.
	}

	public void Apply ()
	{
		target.currentAttack += attackModifier;
		target.currentRetaliation += retaliationModifier;
		target.maxHealth += maxHealthModifier;
		target.currentHealth += maxHealthModifier;

		int trueDamage = damageAmount;
		if (damageAmount > 0)
		{
			if (damageAmount - target.toughness < 1)
			{
				trueDamage = 1;
			}
			else
			{
				trueDamage = damageAmount - target.toughness;
			}
		}
		if (damageAmount < 0 || target.glance < damageAmount)
		{
			target.currentHealth -= trueDamage;
		}
		if (target.currentHealth > target.maxHealth)
		{
			target.currentHealth = target.maxHealth;
		}
	}

	public void Remove ()
	{
		target.currentAttack -= attackModifier;
		target.currentRetaliation -= retaliationModifier;
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
