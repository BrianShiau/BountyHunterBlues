using UnityEngine;
using System.Collections;
using System;

public enum State
{
    GREEN, YELLOW, RED
}

public class AIActor : GameActor {

    public GameObject playerObject;

    
    private State alertness;

    /*
     * AI could be attached to AIActor like so
     * AIManager AI;
     */

    public override void Start()
    {
        base.Start();
        healthPool = 1;
    }

    public override void attack()
    {
        if (isAiming)
        {
            Debug.Log("Enemy Shoots");
            if(aimTarget!=null && Vector2.Distance(aimTarget.transform.position, transform.position) <= sightDistance)
            {
                if(aimTarget is PlayerActor)
                {
                    aimTarget.takeDamage();
                    if (!aimTarget.isAlive())
                        aimTarget = null;
                }

                // else for friendly fire with AI
            }
        }
        // no else since AI can't melee
    }

    public override void interact()
    {
        throw new NotImplementedException();
    }

    public void resetState()
    {
        alertness = State.GREEN;
    }

    public void updateState(State newAlertState)
    {

    }

	public override void die()
	{
		Destroy (gameObject);
	}
}
