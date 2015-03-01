using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Modifier
{
    public Card target;
	public int duration;

    public Modifier()
    {
    }

    public Modifier(Card target, int duration = 0)
    {
        this.target = target;
		this.duration = duration;	// Zero duration means effect is perpetual.
	}

    virtual public void Apply()
    {
	}

    virtual public void Undo()
    {
    }

    virtual public void Invert()
    {
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
            Invert();
		}
	}
}
