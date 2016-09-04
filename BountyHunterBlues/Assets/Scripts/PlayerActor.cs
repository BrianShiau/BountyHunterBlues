using UnityEngine;
using System.Collections;
using System;

public class PlayerActor : GameActor
{
    int healthPool = 3;

    public override void loseHealth()
    {
        healthPool--;
    }

    public override void aim()
    {
        // visual indicator
        isAiming = true;
    }

    public override void disableAim()
    {
        // deactivate visual indicator
        isAiming = false;
    }

    public override void attack()
    {
        if (isAiming)
        {
            // shoot
        }
        else;
        // melee
    }

    public override void interact()
    {

    }
}
