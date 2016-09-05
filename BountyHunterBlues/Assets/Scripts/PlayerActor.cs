using UnityEngine;
using System.Collections;
using System;

public class PlayerActor : GameActor
{

    public override void Start()
    {
        base.Start();
        healthPool = 3;
    }
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
            Debug.Log("attack happening on left click");
            if (lookTarget != null && Vector2.Distance(lookTarget.transform.position, transform.position) <= meleeDistance)
                lookTarget.takeDamage();

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
