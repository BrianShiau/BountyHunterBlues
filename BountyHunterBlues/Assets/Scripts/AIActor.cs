using UnityEngine;
using System.Collections;
using System;

public enum State
{
    NO_ALERT, VISUAL_ALERT, AUDIO_ALERT, FULL_ALERT
}

public class AIActor : GameActor {

    public int fov;

    private Vector2 faceDir; // normalized vector that indiciates the center of the vision cone
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
        alertness = State.NO_ALERT;
    }

    public void updateState(State newAlertState)
    {

    }
}
