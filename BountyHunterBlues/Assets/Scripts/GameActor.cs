using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    protected bool isAiming; // will need to specify "isAiming with what" later for special items
    protected int healthPool; 

    public abstract void attack(); 
    public abstract void interact();

    public bool isAlive()
    {
        return healthPool > 0;
    }
    
    public void kill()
    {
        healthPool = 0;
    }

    public void loseHealth()
    {
        healthPool--;
    }

    public virtual void aim(Vector2 dir)
    {

    }

    public virtual void disableAim()
    {

    }

    public virtual void move(Vector2 dir)
    {

    }

	
}
