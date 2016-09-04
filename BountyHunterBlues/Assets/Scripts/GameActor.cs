﻿using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    protected bool isAiming; // will need to specify "isAiming with what" later for special items

    public abstract void attack(); 
    public abstract void loseHealth();
    public abstract void disableAim();
    public abstract void interact();

    public void aim(Vector2 dir)
    {

    }

    public void move(Vector2 dir)
    {

    }

	
}
