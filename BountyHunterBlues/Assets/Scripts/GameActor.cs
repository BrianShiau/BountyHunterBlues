using UnityEngine;
using System.Collections;

public abstract class GameActor : MonoBehaviour {

    public float moveSpeed; // subject to change based on testing

    protected bool isAiming; // will need to specify "isAiming with what" later for special items
    protected int healthPool;
    protected GameActor target; // null unless isAiming is true and aim ray collides with an unobstructed valid GameActor

    public abstract void attack(); 
    public abstract void interact();
	public abstract void die();

    public bool isAlive()
    {
        return healthPool > 0;
    }
    
    public void kill()
    {
        healthPool = 0;
		die ();
    }

    public void takeDamage()
    {
        healthPool--;
		if (healthPool == 0) {
			die ();
		}
    }

    public virtual void aim(Vector2 dir)
    {
        isAiming = true;
        
    }

    public virtual void disableAim()
    {
        isAiming = false;
        target = null;
    }

    public virtual void move(Vector2 dir)
    {
        dir.Normalize();
        Vector2 newPos = moveSpeed * dir * Time.deltaTime;
        gameObject.transform.Translate(new Vector3(newPos.x, 0, newPos.y));
    }

	
}
