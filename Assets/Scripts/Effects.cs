using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TargetInfo
{
	public bool ally;
	public bool enemy;
	public bool creatures;
	public bool players;

	public TargetInfo ()
	{
		ally = true;
		enemy = true;
		creatures = true;
		players = true;
	}
	public TargetInfo (bool ally, bool enemy, bool creatures, bool players)
	{
		this.ally = ally;
		this.enemy = enemy;
		this.creatures = creatures;
		this.players = players;
	}
}

public class Target
{
	public bool isCard;
	public Card card;
	public int player;

	public Target ()
	{
		isCard = false;
		card = null;
		player = 0;
	}

	public Target (bool isCard, Card card, int player)
	{
		this.isCard = isCard;
		this.card = card;
		this.player = player;
	}
}

public class Effects : ScriptableObject
{
	public List<TargetInfo> targetInfo;

	virtual public void TriggerPlayed(Game g, Card parent, Card trigger)
	{
	}

	virtual public void TriggerBattlecry (Game g, Card c, List<Target> targets)
	{
	}

	virtual public void TriggerEndTurn (Game g, Card c)
	{
	}

	virtual public void TriggerStartTurn (Game g, Card c)
	{
	}

	virtual public void TriggerCombat (Game g, Card c, bool isAttacker, Card other)
	{
		other.AddModifier (new Modifier (other, 0, 0, 0, isAttacker ? c.currentAttack : c.currentRetaliation));
		c.Exhaust();
	}

	public void TriggerDirectAttack (Game g, Card c, int player)
	{
		g.Damage (player, c.currentAttack);
		c.Exhaust();
	}
	
	public bool IsTargetValid (bool ally, bool isCreature)
	{
		foreach (TargetInfo tInfo in targetInfo)
		{
			if (((ally && tInfo.ally) || (!ally && tInfo.enemy)) &&
			    (( isCreature && tInfo.creatures) || (!isCreature && tInfo.players)))
		    {
				return true;
			}
		}
		return false;
	}
}









