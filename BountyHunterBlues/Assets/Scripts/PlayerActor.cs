using UnityEngine;
using System.Collections;
using System;

public class PlayerActor : GameActor
{
    public override void aim(Vector2 dir)
    {
        base.aim(dir);
        
    }

    public override void attack()
    {
        if (isAiming)
        {
            // shoot
        }
        else
        {
            // melee
        }
        
    }

    public override void interact()
    {

    }

	public override void die()
	{
		// reset the game here for now
	}
}
