using UnityEngine;
using System.Collections;
using System;

public class MissileExplosion : Explosion {


    public override void Start()
    {
        base.Start();
    }

    public static MissileExplosion Create(GameObject prefab, Vector3 position)
    {
        GameObject explosionObj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        return explosionObj.GetComponent<MissileExplosion>();
    }

    protected override bool isValidHit(GameActor hitActor)
    {
        return hitActor is PlayerActor;
    }

    protected override void explosionHit(GameActor hitActor)
    {
        hitActor.takeDamage();
    }
}