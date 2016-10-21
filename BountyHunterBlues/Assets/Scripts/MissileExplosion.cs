using UnityEngine;
using System.Collections;
using System;

public class MissileExplosion : Explosion {

    protected override bool isValidHit(GameActor hitActor)
    {
        return hitActor is PlayerActor;
    }

    protected override void explosionHit(GameActor hitActor)
    {
        hitActor.takeDamage();
    }
}
