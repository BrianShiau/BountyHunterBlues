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

    public override void aim(Vector2 dir)
    {
        base.aim(dir);
        faceDir = dir;

    }

    public override void attack()
    {
        if (isAiming)
        {
            // shoot
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
