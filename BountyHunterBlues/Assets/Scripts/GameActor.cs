using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    protected bool isAiming; // will need to specify "isAiming with what" later for special items

    public abstract void attack(); 
    public abstract void loseHealth();
    public abstract void aim();
    public abstract void disableAim();
    public abstract void interact();

    public void move(Vector2 vel) // speed and direction
    {

    }

	
}
